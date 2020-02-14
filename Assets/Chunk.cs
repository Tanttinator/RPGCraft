using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft
{
    /// <summary>
    /// Handles chunk mesh generation.
    /// </summary>
    public class Chunk : MonoBehaviour
    {
        Block[,,] blocks;

        public int cellsX;
        public int cellsY;

        float StepX => 1f / cellsX;
        float StepY => 1f / cellsY;

        /// <summary>
        /// Set the blocks in the chunk.
        /// </summary>
        /// <param name="blocks"></param>
        public void SetBlocks(Block[,,] blocks)
        {
            this.blocks = blocks;
        }

        /// <summary>
        /// Builds a mesh from the blocks in the chunk.
        /// </summary>
        public void GenerateMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();
            Mesh mesh = new Mesh();

            int i = 0;
            foreach(Block block in blocks)
            {
                if (!block.IsVisible)
                    continue;
                foreach (Direction direction in Direction.directions)
                {
                    FaceData data = new FaceData(block.position, direction, new Vector2(0, 0), StepX, StepY, 1f / (cellsX * 16), 1f / (cellsY * 16), i);
                    vertices.AddRange(data.vertices);
                    uvs.AddRange(data.uvs);
                    mesh.subMeshCount++;
                    tris.AddRange(data.tris);
                    i++;
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            MeshFilter filter = GetComponent<MeshFilter>();

            filter.mesh = mesh;
        }

        struct FaceData
        {
            public Vector3[] vertices;
            public Vector2[] uvs;
            public int[] tris;

            public FaceData(Vector3 blockPos, Direction direction, Vector2 textureCell, float stepX, float stepY, float offsetX, float offsetY, int index)
            {
                int maxId = index * 4 + 3;
                Vector2 uvStart = new Vector2(textureCell.x * stepX, textureCell.y * stepY);
                uvs = new Vector2[]
                {
                    new Vector2(offsetX, offsetY) + uvStart,
                    new Vector2(stepX - offsetX, offsetY) + uvStart,
                    new Vector2(offsetX, stepY - offsetY) + uvStart,
                    new Vector2(stepX - offsetX, stepY - offsetY) + uvStart
                };
                vertices = new Vector3[4];
                for(int i = 0; i < 4; i++)
                {
                    vertices[i] = BlockMetrics.GetFaceVertices(direction)[i] + blockPos;
                }
                tris = new int[]
                {
                    maxId - 3, maxId, maxId - 2, maxId - 3, maxId - 1, maxId
                };
            }
        }
    }
}
