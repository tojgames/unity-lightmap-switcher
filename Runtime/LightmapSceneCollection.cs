using System.Collections.Generic;
using UnityEngine;

namespace TOJGAMES.LightmapSwitcher
{
    [CreateAssetMenu(fileName = "LightmapSceneCollection", menuName = "Lighting/Scene Lightmap Collection")]
    public class LightmapSceneCollection : ScriptableObject
    {
        [Tooltip("Set the folder path (relative to Assets/) where lightmap EXR files and assets will be saved.")]
        public string baseSaveFolder = "SavedLightmaps/MyScene"; // e.g. Assets/SavedLightmaps/MyScene

        [Tooltip("Scene this collection belongs to (auto-filled or set manually).")]
        public string sceneName;

        public List<SavedLightmapData> versions = new List<SavedLightmapData>();
    }
}