﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PerlinNoise;
using System.Threading;

namespace RPGCraft
{
    /// <summary>
    /// Handles world generation and chunks.
    /// </summary>
    [ExecuteInEditMode]
    public class World : MonoBehaviour
    {
        Dictionary<ChunkCoords, Chunk> chunks = new Dictionary<ChunkCoords, Chunk>();
        List<ChunkCoords> loadedChunks = new List<ChunkCoords>();

        public Settings noiseSettings;

        public int seed;

        public int heightMultiplier;
        public int waterHeight;

        public int startingChunks;

        Queue<Chunk> threadedChunks = new Queue<Chunk>();

        Queue<ChunkCoords> generationQueue = new Queue<ChunkCoords>();

        /// <summary>
        /// Reset current world.
        /// </summary>
        public void ClearWorld()
        {
            while(transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            chunks.Clear();
            loadedChunks.Clear();
        }

        /// <summary>
        /// Generates a new world.
        /// </summary>
        public void GenerateWorld()
        {
            ClearWorld();
            seed = UnityEngine.Random.Range(-999999, 999999);
            for(int x = 1 - startingChunks; x < startingChunks; x++)
            {
                for(int y = 1 - startingChunks; y < startingChunks; y++)
                {
                    for(int z = 1 - startingChunks; z < startingChunks; z++)
                    {
                        GenerateChunkImmediate(new ChunkCoords(x, y, z));
                    }
                }
            }

        }

        /// <summary>
        /// Creates a new chunk at the given position.
        /// </summary>
        /// <param name="coords"></param>
        Chunk GenerateChunk(ChunkCoords coords)
        {
            if (chunks.ContainsKey(coords))
                return null;
            int chunkSize = Reference.Instance.chunkSize;
            BlockType[,,] blocks = new BlockType[chunkSize, chunkSize, chunkSize];
            float[,] heightMap = Generator.GenerateHeightmap(chunkSize, chunkSize, seed, noiseSettings, new Vector2(coords.GetStartPos().x, coords.GetStartPos().z));
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        BlockType type;
                        int height = coords.GetStartPos().y + y;
                        int groundHeight = (int)(heightMultiplier * heightMap[x, z]);
                        if (height > Mathf.Max(groundHeight, waterHeight))
                            type = Blocks.Instance.empty;
                        else if(groundHeight > waterHeight)
                        {
                            if (height == groundHeight)
                                type = Blocks.Instance.grass;
                            else if (height > groundHeight - 4)
                                type = Blocks.Instance.dirt;
                            else
                                type = Blocks.Instance.stone;
                        }
                        else if(groundHeight == waterHeight)
                        {
                            if (height > groundHeight - 4)
                                type = Blocks.Instance.sand;
                            else
                                type = Blocks.Instance.stone;
                        }
                        else
                        {
                            if (height > groundHeight)
                                type = Blocks.Instance.water;
                            else if (height > groundHeight - 4)
                                type = Blocks.Instance.sand;
                            else
                                type = Blocks.Instance.stone;
                        }
                        blocks[x, y, z] = type;
                    }
                }
            }
            Chunk chunk = new Chunk(coords, this);
            chunks.Add(coords, chunk);
            chunk.CreateBlocks();
            chunk.SetBlocks(blocks);
            return chunk;
        }

        public void GenerateChunkImmediate(ChunkCoords coords)
        {
            Chunk chunk = GenerateChunk(coords);
            chunk.CreateGO();
            chunk.GenerateMeshImmediate();
            chunk.Load();
        } 

        public void GenerateChunkThreaded(ChunkCoords coords)
        {
            generationQueue.Enqueue(coords);
        }

        public Block GetBlock(Coords coords)
        {
            ChunkCoords chunk = coords.GetChunk();
            if (!chunks.ContainsKey(chunk))
                return null;
            return chunks[chunk].GetBlock(coords);
        }

        public Chunk GetChunk(ChunkCoords coords)
        {
            if (!chunks.ContainsKey(coords))
                return null;
            return chunks[coords];
        }

        public int GetGroundLevel(int x, int z)
        {
            ChunkCoords coords = new Coords(x, 0, z).GetChunk();
            if (!chunks.ContainsKey(coords))
                return -1;
            return chunks[coords].GetGroundLevel(x, z);
        }

        public void LoadChunk(ChunkCoords coords)
        {
            if (loadedChunks.Contains(coords))
                return;
            if (chunks.ContainsKey(coords))
            {
                chunks[coords].Load();
            } else
            {
                GenerateChunkThreaded(coords);
            }
            loadedChunks.Add(coords);
        }

        void UnloadChunk(ChunkCoords coords)
        {
            loadedChunks.Remove(coords);
            if (chunks.ContainsKey(coords))
                chunks[coords].Unload();
        }

        void OnPlayerSpawned(Player player)
        {
            player.onPlayerChunkChange += OnPlayerMoved;
        }

        void OnPlayerMoved(ChunkCoords chunk)
        {
            int renderDistance = Reference.Instance.renderDistance;
            List<ChunkCoords> unloadChunks = new List<ChunkCoords>();
            foreach(ChunkCoords coords in loadedChunks)
            {
                if (coords.Distance(chunk) > renderDistance)
                    unloadChunks.Add(coords);
            }
            for(int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int y = -renderDistance; y <= renderDistance; y++)
                {
                    for (int z = -renderDistance; z <= renderDistance; z++)
                    {
                        LoadChunk(chunk + new ChunkCoords(x, y, z));
                    }
                }
            }
            foreach (ChunkCoords coords in unloadChunks)
                UnloadChunk(coords);
        }

        private void Awake()
        {
            GetComponent<GameController>().onPlayerSpawned += OnPlayerSpawned;
            ThreadStart generationThreadStart = new ThreadStart(delegate
            {
                while (true)
                {
                    if (generationQueue.Count > 0)
                    {
                        threadedChunks.Enqueue(GenerateChunk(generationQueue.Dequeue()));
                    }
                }
            });
            new Thread(generationThreadStart).Start();
            Chunk.StartBuildThread();

        }

        private void Update()
        {
            if(threadedChunks.Count > 0)
            {
                Chunk chunk = threadedChunks.Dequeue();
                chunk.CreateGO();
                chunk.GenerateMeshThreaded((c) => c.Load());
            }
        }
    }

    public class Direction
    {
        public Coords offset;
        Func<Direction> opposite;

        public Direction Opposite => opposite.Invoke();

        public Direction(Coords offset, Func<Direction> opposite)
        {
            this.offset = offset;
            this.opposite = opposite;
        }

        public static Direction UP = new Direction(new Coords(0, 1, 0), () => DOWN);
        public static Direction DOWN = new Direction(new Coords(0, -1, 0), () => UP);
        public static Direction NORTH = new Direction(new Coords(0, 0, 1), () => SOUTH);
        public static Direction EAST = new Direction(new Coords(1, 0, 0), () => WEST);
        public static Direction SOUTH = new Direction(new Coords(0, 0, -1), () => NORTH);
        public static Direction WEST = new Direction(new Coords(-1, 0, 0), () => EAST);

        public static Direction[] directions = new Direction[]
        {
            UP,
            DOWN,
            NORTH,
            EAST,
            SOUTH,
            WEST
        };
    }

    public struct Coords
    {
        public int x;
        public int y;
        public int z;

        public Coords(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public ChunkCoords GetChunk()
        {
            return new ChunkCoords(Mathf.FloorToInt(x * 1f / Reference.Instance.chunkSize), Mathf.FloorToInt(y * 1f / Reference.Instance.chunkSize), Mathf.FloorToInt(z * 1f / Reference.Instance.chunkSize));
        }

        public Coords Neighbor(Direction direction)
        {
            return this + direction.offset;
        }

        public static Coords operator +(Coords a, Coords b)
        {
            return new Coords(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Coords operator -(Coords a, Coords b)
        {
            return new Coords(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static implicit operator Coords(Vector3 v3)
        {
            return new Coords(Mathf.FloorToInt(v3.x), Mathf.FloorToInt(v3.y), Mathf.FloorToInt(v3.z));
        }

        public static implicit operator Vector3(Coords coords)
        {
            return new Vector3(coords.x, coords.y, coords.z);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ", " + z + ")";
        }

        public static bool operator ==(Coords a, Coords b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Coords a, Coords b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coords))
                return false;
            Coords coords = (Coords)obj;
            return coords.x == x && coords.y == y && coords.z == z;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    public struct ChunkCoords
    {
        public int x;
        public int y;
        public int z;

        public ChunkCoords(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Coords GetStartPos()
        {
            return new Coords(x * Reference.Instance.chunkSize, y * Reference.Instance.chunkSize, z * Reference.Instance.chunkSize);
        }

        public override string ToString()
        {
            return "(" + x + ", " + z + ")";
        }

        public static ChunkCoords operator +(ChunkCoords a, ChunkCoords b)
        {
            return new ChunkCoords(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static ChunkCoords operator -(ChunkCoords a, ChunkCoords b)
        {
            return new ChunkCoords(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static bool operator ==(ChunkCoords a, ChunkCoords b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ChunkCoords a, ChunkCoords b)
        {
            return !a.Equals(b);
        }

        public int Distance(ChunkCoords other)
        {
            return Mathf.Max(Mathf.Abs(other.x - x), Mathf.Abs(other.y - y), Mathf.Abs(other.z - z));
        }

        public int DistanceFromCenter()
        {
            ChunkCoords center = (GameController.player != null ? ((Coords)GameController.player.transform.position).GetChunk() : new ChunkCoords(0, 0, 0));
            return Distance(center);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkCoords))
                return false;
            ChunkCoords other = (ChunkCoords)obj;
            return other.x == x && other.y == y && other.z == z;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
