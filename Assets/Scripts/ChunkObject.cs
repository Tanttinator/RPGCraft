using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft
{
    [ExecuteInEditMode]
    public class ChunkObject : MonoBehaviour
    {
        MeshFilter filter;
        new MeshCollider collider;

        public Queue<MeshThreadData> meshQueue = new Queue<MeshThreadData>();

        private void Awake()
        {
            filter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>().material = Reference.Instance.chunkMat;
            collider = gameObject.AddComponent<MeshCollider>();
        }

        public void ApplyMesh(MeshThreadData data)
        {
            filter.sharedMesh = data.mesh.Mesh;
            collider.sharedMesh = data.mesh.ColliderMesh;
            data.callback?.Invoke();
        }

        private void Update()
        {
            if (meshQueue.Count > 0)
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

        public struct MeshThreadData
        {
            public MeshData mesh;
            public Action callback;

            public MeshThreadData(MeshData data, Action callback)
            {
                mesh = data;
                this.callback = callback;
            }
        }
    }
}
