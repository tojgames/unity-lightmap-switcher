using System.Collections.Generic;
using UnityEngine;

namespace TOJGAMES.LightmapSwitcher
{
    [CreateAssetMenu(fileName = "LightmapSceneCollection", menuName = "Lighting/Scene Lightmap Collection")]
    public class LightmapSceneCollection : ScriptableObject
    {
        public Settings settings;

        public List<SavedLightmapData> savedLightmapsDatas = new List<SavedLightmapData>();

        [System.Serializable]
        public class Settings
        {
            public string baseSaveFolder = "Saved Lightmaps";
        }
    }
}