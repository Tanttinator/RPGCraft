using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPGCraft
{
    [CustomEditor(typeof(World))]
    public class WorldEditor : Editor
    {
        int chunkX = 0;
        int chunkY = 0;
        int chunkZ = 0;
        public override void OnInspectorGUI()
        {
            World world = (World)target;
            DrawDefaultInspector();
            chunkX = EditorGUILayout.IntField("Chunk X", chunkX);
            chunkY = EditorGUILayout.IntField("Chunk Y", chunkY);
            chunkZ = EditorGUILayout.IntField("Chunk Z", chunkZ);
            if (GUILayout.Button("Generate Chunk"))
                world.LoadChunk(new ChunkCoords(chunkX, chunkY, chunkZ));
            if (GUILayout.Button("Generate World"))
                world.GenerateWorld();
            if (GUILayout.Button("Clear World"))
                world.ClearWorld();
        }
    }
}
