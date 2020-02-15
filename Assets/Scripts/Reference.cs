using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft
{
    /// <summary>
    /// Global values accessible anywhere.
    /// </summary>
    [ExecuteInEditMode]
    public class Reference : MonoBehaviour
    {
        static Reference instance;
        public static Reference Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Reference>();
                    if (instance == null)
                    {
                        Debug.LogError("Couldn't find an instance of Reference!");
                        return null;
                    }
                }
                return instance;
            }
        }

        public int chunkSize;

        public int atlasWidth;
        public int atlasHeight;

        public Material chunkMat;

        public int renderDistance;
    }
}
