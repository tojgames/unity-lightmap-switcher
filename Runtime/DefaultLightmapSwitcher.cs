using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TOJGAMES.LightmapSwitcher
{
    public class DefaultLightmapSwitcher : MonoBehaviour
    {
        [Tooltip("Reference to the LightmapSceneCollection asset for this scene.")]
        public LightmapSceneCollection sceneCollection;

        [ContextMenu("Apply Latest Lightmap Version")]
        public void ApplyLatestVersion()
        {
            string sceneName = SceneManager.GetActiveScene().name;

            if (sceneCollection == null || sceneCollection.sceneName != sceneName)
            {
                Debug.LogError($"❌ LightmapSceneCollection not assigned or doesn't match active scene: {sceneName}");
                return;
            }

            if (sceneCollection.versions.Count == 0)
            {
                Debug.LogWarning($"⚠️ No lightmap versions available in collection for scene '{sceneName}'.");
                return;
            }

            ApplyLightmapData(sceneCollection.versions[^1]); // ^1 = last element
        }

        public void ApplyLightmapByIndex(int index)
        {
            string sceneName = SceneManager.GetActiveScene().name;

            if (sceneCollection == null || sceneCollection.sceneName != sceneName)
            {
                Debug.LogError($"❌ LightmapSceneCollection not assigned or doesn't match active scene: {sceneName}");
                return;
            }

            if (index < 0 || index >= sceneCollection.versions.Count)
            {
                Debug.LogWarning($"⚠️ Invalid lightmap version index: {index} for scene '{sceneName}'.");
                return;
            }

            ApplyLightmapData(sceneCollection.versions[index]);
        }

        private void ApplyLightmapData(SavedLightmapData data)
        {
            if (data == null || data.lightmaps.Count == 0)
            {
                Debug.LogWarning("No lightmap textures in the selected version.");
                return;
            }

            LightmapData[] lightmaps = new LightmapData[data.lightmaps.Count];
            for (int i = 0; i < data.lightmaps.Count; i++)
            {
                lightmaps[i] = new LightmapData
                {
                    lightmapColor = data.lightmaps[i].lightmapColor,
                    lightmapDir = data.lightmaps[i].lightmapDir
                };
            }

            LightmapSettings.lightmapsMode = LightmapsMode.CombinedDirectional;
            LightmapSettings.lightmaps = lightmaps;

            foreach (var info in data.renderers)
            {
                GameObject obj = GameObject.Find(info.path);
                if (obj == null) continue;

                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.lightmapIndex = info.lightmapIndex;
                    renderer.lightmapScaleOffset = info.lightmapScaleOffset;
                }
            }

            DynamicGI.UpdateEnvironment();
            Debug.Log("✅ Lightmap version applied successfully.");
        }
    }
}