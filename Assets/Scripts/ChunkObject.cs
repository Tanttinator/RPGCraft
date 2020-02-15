using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft
{
    [ExecuteInEditMode]
    public class ChunkObject : MonoBehaviour
    {
        MeshFilter filter;
        MeshCollider collider;

        public Queue<MeshData> meshQueue = new Queue<MeshData>();

        private void Awake()
        {
            filter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>().material = Reference.Instance.chunkMat;
            collider = gameObject.AddComponent<MeshCollider>();
        }

        public void ApplyMesh(MeshData data)
        {
            filter.sharedMesh = data.Mesh;
            collider.sharedMesh = data.ColliderMesh;
        }

        private void Update()
        {
            if (meshQueue.Count > 1)
                ApplyMesh(meshQueue.Dequeue());
        }

        public struct MeshData
        {
            Vector3[] vertices;
            Vector2[] uvs;
            int[] tris;

            Vector3[] colliderVertices;
            int[] colliderTris;

            public Mesh Mesh
            {
                get
                {
                    Mesh mesh = new Mesh();
                    mesh.vertices = vertices;
                    mesh.uv = uvs;
                    mesh.triangles = tris;
                    mesh.RecalculateNormals();
                    return mesh;
                }
            }

            public Mesh ColliderMesh
            {
                get
                {
                    Mesh mesh = new Mesh();
                    mesh.vertices = colliderVertices;
                    mesh.triangles = colliderTris;
                    mesh.RecalculateNormals();
                    return mesh;
                }
            }

            public MeshData(Vector3[] vertices, Vector2[] uvs, int[] tris, Vector3[] colliderVertices, int[] colliderTris)
            {
                this.vertices = vertices;
                this.uvs = uvs;
                this.tris = tris;
                this.colliderVertices = colliderVertices;
                this.colliderTris = colliderTris;
            }
        }
    }
}
