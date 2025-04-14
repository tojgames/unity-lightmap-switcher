using System.Collections.Generic;
using UnityEngine;

namespace TOJGAMES.LightmapSwitcher
{
    [CreateAssetMenu(fileName = "LightmapSceneCollection", menuName = "Lighting/Scene Lightmap Collection")]
    public class LightmapSceneCollection : ScriptableObject
    {

        public List<SavedLightmapData> savedLightmapsDatas = new List<SavedLightmapData>();
        public Settings settings;

        [System.Serializable]
        public class Settings
        {
            [Tooltip("Where all the baked lightmaps will be stored")]
            public string baseSaveFolder = "";
        }
    }
}