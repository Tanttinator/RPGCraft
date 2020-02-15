using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerlinNoise
{
    /// <summary>
    /// Object to store settings for Perlin noise.
    /// </summary>
    [CreateAssetMenu(menuName = "Perlin Noise/Settings")]
    public class Settings : ScriptableObject
    {
        public int octaves = 4;
        public float scale = 20f;
        [Range(0f, 1f)]
        public float persistence = 0.5f;
        [Range(1f, 16f)]
        public float lacunarity = 2f;

        private void OnValidate()
        {
            if (scale <= 0)
                scale = 0.0001f;
        }
    }
}
