using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft
{
    /// <summary>
    /// Handles chunk mesh generation.
    /// </summary>
    public class Chunk
    {
        public ChunkCoords position;

        Block[,,] blocks;

        Mesh mesh;
        GameObject go;

        public World world { get; protected set; }

        public bool loaded { get; protected set; } = false;

        public Chunk(ChunkCoords coords, World world)
        {
            position = coords;
            this.world = world;
            go = new GameObject("Chunk " + coords);
            go.layer = 9;
            go.transform.SetParent(world.transform);

            MeshFilter filter = go.AddComponent<MeshFilter>();
            Mesh copy = Mesh.Instantiate(filter.sharedMesh? filter.sharedMesh : new Mesh()) as Mesh;
            mesh = filter.mesh = copy;

            go.AddComponent<MeshCollider>();
            go.AddComponent<MeshRenderer>().material = Reference.Instance.chunkMat;
        }

        /// <summary>
        /// Try to get a block with the given coordinates in this chunk.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public Block GetBlock(Coords coords)
        {
            Coords relative = coords - position.GetStartPos();
            if (relative.x < 0 || relative.x >= Reference.Instance.chunkSize || relative.y < 0 || relative.y >= Reference.Instance.chunkSize || relative.z < 0 || relative.z >= Reference.Instance.chunkSize)
                return null;
            return blocks[relative.x, relative.y, relative.z];
        }

        /// <summary>
        /// Initialize all blocks.
        /// </summary>
        public void CreateBlocks()
        {
            int chunkSize = Reference.Instance.chunkSize;
            blocks = new Block[chunkSize, chunkSize, chunkSize];
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        Block block = new Block(new Coords(x, y, z) + position.GetStartPos(), this);
                        blocks[x, y, z] = block;
                    }
                }
            }
        }

        /// <summary>
        /// Set the blocks in the chunk.
        /// </summary>
        /// <param name="blocks"></param>
        public void SetBlocks(BlockType[,,] blocks)
        {
            int chunkSize = Reference.Instance.chunkSize;
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        this.blocks[x, y, z].SetType(blocks[x, y, z]);
                    }
                }
            }
        }

        public void UpdateChunk()
        {
            if (!loaded)
                return;
            GenerateMesh();
        }

        /// <summary>
        /// Builds a mesh from the blocks in the chunk.
        /// </summary>
        public void GenerateMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            List<Vector3> colliderVertices = new List<Vector3>();
            List<int> colliderTris = new List<int>();

            mesh.Clear();
            Mesh colliderMesh = new Mesh();

            int i = 0;
            foreach(Block block in blocks)
            {
                if (!block.IsVisible)
                    continue;
                foreach (Direction direction in Direction.directions)
                {
                    if (!block.IsVisibleFrom(direction) || block.type.model.GetFace(direction) == null)
                        continue;
                    FaceData data = new FaceData(block.position, direction, block.type.model.GetFace(direction).textureCoord, i);
                    vertices.AddRange(data.vertices);
                    uvs.AddRange(data.uvs);
                    tris.AddRange(data.tris);
                    if(block.type.solid)
                    {
                        colliderVertices.AddRange(data.vertices);
                        colliderTris.AddRange(data.tris);
                    }
                    i++;
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            colliderMesh.vertices = colliderVertices.ToArray();
            colliderMesh.triangles = colliderTris.ToArray();
            colliderMesh.RecalculateNormals();
            go.GetComponent<MeshCollider>().sharedMesh = colliderMesh;
        }

        public void Load()
        {
            go.SetActive(true);
            loaded = true;
            world.GetChunk(position + new ChunkCoords(1, 0))?.UpdateChunk();
            world.GetChunk(position + new ChunkCoords(-1, 0))?.UpdateChunk();
            world.GetChunk(position + new ChunkCoords(0, 1))?.UpdateChunk();
            world.GetChunk(position + new ChunkCoords(0, -1))?.UpdateChunk();
        }

        public void Unload()
        {
            loaded = false;
            go.SetActive(false);
            world.GetChunk(position + new ChunkCoords(1, 0))?.UpdateChunk();
            world.GetChunk(position + new ChunkCoords(-1, 0))?.UpdateChunk();
            world.GetChunk(position + new ChunkCoords(0, 1))?.UpdateChunk();
            world.GetChunk(position + new ChunkCoords(0, -1))?.UpdateChunk();
        }

        struct FaceData
        {
            public Vector3[] vertices;
            public Vector2[] uvs;
            public int[] tris;

            public FaceData(Vector3 blockPos, Direction direction, Vector2 textureCell, int index)
            {
                int maxId = index * 4 + 3;
                float stepX = 1f / Reference.Instance.atlasWidth;
                float stepY = 1f / Reference.Instance.atlasHeight;
                float offsetX = stepX / 16f;
                float offsetY = stepY / 16f;
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
