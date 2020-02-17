using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft {
    public class ChunkBuilder : MonoBehaviour
    {
        public static List<MeshThreadData> buildQueue = new List<MeshThreadData>();

        private void Update()
        {
            if(buildQueue.Count > 0)
            {
                int minDist = int.MaxValue;
                int minId = -1;
                for(int i = 0; i < buildQueue.Count; i++)
                {
                    if(minId == -1 || buildQueue[i].chunk.position.DistanceFromCenter() < minDist)
                    {
                        minDist = buildQueue[i].chunk.position.DistanceFromCenter();
                        minId = i;
                    }
                }
                MeshThreadData data = buildQueue[minId];
                buildQueue.RemoveAt(minId);
                data.chunk.obj.ApplyMesh(data);
            }
        }
    }
}
