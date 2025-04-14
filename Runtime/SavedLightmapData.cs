using System;
using System.Collections.Generic;
using UnityEngine;

namespace TOJGAMES.LightmapSwitcher
{
    [CreateAssetMenu(fileName = "SavedLightmapData", menuName = "Lighting/Saved Lightmap Data")]
    public class SavedLightmapData : ScriptableObject
    {
        [Serializable]
        public class LightmapTextureData
        {
            public Texture2D lightmapColor;
            public Texture2D lightmapDir;

            public LightmapTextureData() { }

            public LightmapTextureData(Texture2D color, Texture2D dir)
            {
                lightmapColor = color;
                lightmapDir = dir;
            }
        }


        [Serializable]
        public class RendererLightmapInfo
        {
            public string path;
            public int lightmapIndex;
            public Vector4 lightmapScaleOffset;

            public RendererLightmapInfo() { }

            public RendererLightmapInfo(string path, int index, Vector4 offset)
            {
                this.path = path;
                this.lightmapIndex = index;
                this.lightmapScaleOffset = offset;
            }
        }

        public List<LightmapTextureData> lightmaps = new();
        public List<RendererLightmapInfo> renderers = new();
    }
}