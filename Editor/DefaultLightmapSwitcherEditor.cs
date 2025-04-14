using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TOJGAMES.LightmapSwitcher
{
    [CustomEditor(typeof(DefaultLightmapSwitcher))]
    public class DefaultLightmapSwitcherEditor : Editor
    {
        private int versionIndex = 0;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            DefaultLightmapSwitcher switcher = (DefaultLightmapSwitcher)target;
            var collection = switcher.sceneCollection;

            if (collection == null)
            {
                EditorGUILayout.HelpBox("⚠️ Assign a LightmapSceneCollection to this switcher.", MessageType.Warning);
                return;
            }

            string activeScene = SceneManager.GetActiveScene().name;

            int versionCount = collection.savedLightmapsDatas.Count;
            EditorGUILayout.LabelField("Scene:", activeScene);
            EditorGUILayout.LabelField("Saved Lightmap Versions:", versionCount.ToString());

            if (versionCount == 0)
            {
                EditorGUILayout.HelpBox("This collection has no saved lightmap versions.", MessageType.Info);
                return;
            }

            versionIndex = EditorGUILayout.IntSlider("Version Index", versionIndex, 0, versionCount - 1);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            
            if ( GUILayout.Button("<- Previos Lightmaps") )
            {
                versionIndex --;
                
                if (versionIndex < 0)
                    versionIndex = versionCount - 1;

                switcher.ApplyLightmapByIndex(versionIndex);
            }

            if ( GUILayout.Button("-> Next Lightmaps") )
            {
                versionIndex = (versionIndex + 1) % versionCount;
                switcher.ApplyLightmapByIndex(versionIndex);
            }

            EditorGUILayout.EndHorizontal();


            // if (GUILayout.Button("📥 Apply Lightmap Version by Index"))
            // {
            //     switcher.ApplyLightmapByIndex(versionIndex);
            // }

            // if (GUILayout.Button("📥 Apply Latest Lightmap Version"))
            // {
            //     switcher.ApplyLatestVersion();
            // }

            // EditorGUILayout.Space();

            // if (GUILayout.Button("🔄 Refresh Version Info"))
            // {
            //     Repaint();
            // }
        }
    }
}