using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerlinNoise
{
    /// <summary>
    /// Helper class for Perlin noise generation.
    /// </summary>
    public class Generator
    {
        /// <summary>
        /// Generate a 2D array of Perlin noise values.
        /// </summary>
        /// <param name="width">X-size of the array.</param>
        /// <param name="height">Y-size of the array.</param>
        /// <param name="seed">Seed for RNG.</param>
        /// <param name="octaves">How many layers of detail is added to the noise. Must be greater than 0. Recommended value: 4.</param>
        /// <param name="scale">How "zoomed in" the noise is. The larger the smoother. Must be greater than 0. Recommended value: 20.</param>
        /// <param name="persistence">How much the amplitude changes between octaves. The smaller the smoother. Recommended range: [0f, 1f], Recommended value: 0.5f.</param>
        /// <param name="lacunarity">How much the frequency changes between octaves. The smaller the smoother. Recommended range: [1f, 16f], Recommended value: 2f.</param>
        /// <param name="offset">Manual offset for the noise.</param>
        /// <returns>2D array of noise values.</returns>
        public static float[,] GenerateHeightmap(int width, int height, int seed, int octaves, float scale, float persistence, float lacunarity, Vector2 offset)
        {
            if (octaves <= 0)
            {
                Debug.LogError("PerlinNoise.Generator.GenerateHeightmap: Must have more than 0 octaves!");
                return null;
            }

            //Make sure scale is greater than 0
            if (scale <= 0)
                scale = 0.00001f;

            //Initialize the heightmap
            float[,] heightmap = new float[width, height];

            //Initialize RNG
            System.Random prng = new System.Random(seed);

            //Random offsets for each octave
            Vector2[] octaveOffsets = new Vector2[octaves];

            //Keep track of the lowest and highest values
            float minHeight = 0;
            float maxHeight = 0;

            for (int i = 0; i < octaves; i++)
            {
                //Offset the coordinates by a random value
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                float maxValue = Mathf.Pow(persistence, i);
                minHeight -= maxValue;
                maxHeight += maxValue;
            }

            //Zoom to the center when changing width and height
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            //Loop through our heightmap and generate noise values for each element
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //The strength of the layer -- the larger amplitude, the more it will affect the height value
                    float amplitude = 1f;
                    //The scale of the layer -- the larger frequency, the grainier the noise
                    float frequency = 1f;
                    //The height at these coordinates
                    float heightValue = 0f;

                    //Generate noise layers with differing scales and strengths to add more/less detail
                    for (int i = 0; i < octaves; i++)
                    {
                        //Sample coordinates
                        float sampleX = (x + halfWidth + octaveOffsets[i].x) / scale * frequency;
                        float sampleY = (y + halfHeight + octaveOffsets[i].y) / scale * frequency;

                        //Get noise sample and fit it between -1 and 1
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        //Add noise layer to current value
                        heightValue += perlinValue * amplitude;

                        //Change our amplitude and frequency
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    heightmap[x, y] = heightValue;
                }
            }

            //Normalize values between 0 and 1
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heightmap[x, y] = (heightmap[x, y] - minHeight) / (maxHeight - minHeight);
                }
            }

            return heightmap;
        }

        /// <summary>
        /// Generate a 2D array of Perlin noise values.
        /// </summary>
        /// <param name="width">X-size of the array.</param>
        /// <param name="height">Y-size of the array.</param>
        /// <param name="seed">Seed for RNG</param>
        /// <param name="settings">Settings for the noise.</param>
        /// <param name="offset">Manual offset for the noise.</param>
        /// <returns>2D array of noise values.</returns>
        public static float[,] GenerateHeightmap(int width, int height, int seed, Settings settings, Vector2 offset)
        {
            return GenerateHeightmap(width, height, seed, settings.octaves, settings.scale, settings.persistence, settings.lacunarity, offset);
        }
    }
}
