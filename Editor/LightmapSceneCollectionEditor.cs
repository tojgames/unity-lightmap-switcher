using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace TOJGAMES.LightmapSwitcher
{

    [CustomEditor(typeof(LightmapSceneCollection))]
    public class LightmapSceneCollectionEditor : Editor
    {

        private LightmapSceneCollection lightmapSceneCollection;

        public override void OnInspectorGUI()
        {

            if ( !lightmapSceneCollection )
                lightmapSceneCollection = (LightmapSceneCollection) target;

            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.PropertyField( serializedObject.FindProperty("savedLightmapsDatas"), new GUIContent("Saved Lightmaps Datas", ""), true );
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        
            if (GUILayout.Button("💾 Save Lightmaps from Scene to Folder"))
            {
                SaveLightmapsToNewFolder();
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField( serializedObject.FindProperty("settings"), new GUIContent("Settings", ""), true );

            if ( string.IsNullOrEmpty(lightmapSceneCollection.settings.baseSaveFolder) )
            {
                EditorGUILayout.HelpBox(
                    "Base Folder for saving files is not selected!", 
                    MessageType.Error
                );
            }
            

            // Saving changes
            serializedObject.ApplyModifiedProperties();
            if ( GUI.changed ) {
                EditorUtility.SetDirty(lightmapSceneCollection);
            }
        }

        private void SaveLightmapsToNewFolder()
        {
            var collection = (LightmapSceneCollection)target;
            string sceneName = SceneManager.GetActiveScene().name;
            var lightmaps = LightmapSettings.lightmaps;

            if (lightmaps == null || lightmaps.Length == 0)
            {
                Debug.LogWarning("No baked lightmaps found.");
                return;
            }

            if ( string.IsNullOrEmpty(lightmapSceneCollection.settings.baseSaveFolder) )
            {
                Debug.LogWarning("Base Folder for saving files is not selected!");
                return;
            }

            // Ensure base save folder exists
            string baseFolder = $"Assets/{collection.settings.baseSaveFolder}".TrimEnd('/');
            if (!AssetDatabase.IsValidFolder(baseFolder))
            {
                string[] parts = baseFolder.Split('/');
                string current = "Assets";
                for (int i = 1; i < parts.Length; i++)
                {
                    string next = $"{current}/{parts[i]}";
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
            }

            // Create version subfolder
            string versionFolderName = $"{sceneName}_v{System.DateTime.Now.Ticks}";
            AssetDatabase.CreateFolder(baseFolder, versionFolderName);
            string fullFolderPath = $"{baseFolder}/{versionFolderName}";

            var savedData = ScriptableObject.CreateInstance<SavedLightmapData>();

            for (int i = 0; i < lightmaps.Length; i++)
            {
                var colorTex = lightmaps[i].lightmapColor;
                var dirTex = lightmaps[i].lightmapDir;

                if (colorTex == null) continue;

                // Handle color texture
                string colorOriginalPath = AssetDatabase.GetAssetPath(colorTex);
                string colorExt = Path.GetExtension(colorOriginalPath);
                string colorCopyPath = $"{fullFolderPath}/Lightmap_{i}_color{colorExt}";

                AssetDatabase.CopyAsset(colorOriginalPath, colorCopyPath);
                CopyImportSettings(colorOriginalPath, colorCopyPath);
                Texture2D colorOut = AssetDatabase.LoadAssetAtPath<Texture2D>(colorCopyPath);

                // Handle directional texture
                Texture2D dirOut = null;
                if (dirTex != null)
                {
                    string dirOriginalPath = AssetDatabase.GetAssetPath(dirTex);
                    string dirExt = Path.GetExtension(dirOriginalPath);
                    string dirCopyPath = $"{fullFolderPath}/Lightmap_{i}_dir{dirExt}";

                    AssetDatabase.CopyAsset(dirOriginalPath, dirCopyPath);
                    CopyImportSettings(dirOriginalPath, dirCopyPath);
                    dirOut = AssetDatabase.LoadAssetAtPath<Texture2D>(dirCopyPath);
                }

                savedData.lightmaps.Add(new SavedLightmapData.LightmapTextureData(colorOut, dirOut));
            }

            // Save renderer lightmap info
            foreach (var rend in GameObject.FindObjectsOfType<Renderer>())
            {
                if (rend.lightmapIndex < 0 || rend.lightmapIndex >= lightmaps.Length) continue;

                string path = GetGameObjectPath(rend.gameObject);
                savedData.renderers.Add(new SavedLightmapData.RendererLightmapInfo(
                    path,
                    rend.lightmapIndex,
                    rend.lightmapScaleOffset
                ));
            }

            // Save the asset
            string dataPath = $"{fullFolderPath}/SavedLightmapData.asset";
            AssetDatabase.CreateAsset(savedData, dataPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            collection.savedLightmapsDatas.Add(savedData);
            EditorUtility.SetDirty(collection);
            AssetDatabase.SaveAssets();

            Debug.Log($"✅ Lightmap version saved to: {fullFolderPath}");
        }

        private void CopyImportSettings(string sourcePath, string targetPath)
        {
            var sourceImporter = AssetImporter.GetAtPath(sourcePath) as TextureImporter;
            var targetImporter = AssetImporter.GetAtPath(targetPath) as TextureImporter;

            if (sourceImporter != null && targetImporter != null)
            {
                targetImporter.textureType = sourceImporter.textureType;
                targetImporter.sRGBTexture = sourceImporter.sRGBTexture;
                targetImporter.mipmapEnabled = sourceImporter.mipmapEnabled;
                targetImporter.alphaSource = sourceImporter.alphaSource;
                targetImporter.wrapMode = sourceImporter.wrapMode;
                targetImporter.filterMode = sourceImporter.filterMode;
                targetImporter.anisoLevel = sourceImporter.anisoLevel;
                targetImporter.textureCompression = TextureImporterCompression.Uncompressed; // Always force uncompressed
                targetImporter.SaveAndReimport();
            }
        }

        private string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
    }
}