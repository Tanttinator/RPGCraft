using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft {
    public class Blocks : MonoBehaviour
    {
        static Blocks instance;
        public static Blocks Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Blocks>();
                    if (instance == null)
                    {
                        Debug.LogError("Couldn't find an instance of Blocks!");
                        return null;
                    }
                }
                return instance;
            }
        }

        public BlockType dirt, grass, stone, sand, water;
    }
}
