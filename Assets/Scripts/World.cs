using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPGCraft
{
    /// <summary>
    /// Handles world generation and chunks.
    /// </summary>
    [ExecuteInEditMode]
    public class World : MonoBehaviour
    {
        Dictionary<ChunkCoords, Chunk> chunks = new Dictionary<ChunkCoords, Chunk>();

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
        }

        /// <summary>
        /// Generates a new world.
        /// </summary>
        public void GenerateWorld()
        {
            ClearWorld();
            GenerateChunk(new ChunkCoords(0, 0));
        }

        /// <summary>
        /// Creates a new chunk at the given position.
        /// </summary>
        /// <param name="coords"></param>
        public void GenerateChunk(ChunkCoords coords)
        {
            if (chunks.ContainsKey(coords))
                return;
            int chunkSize = Reference.Instance.chunkSize;
            BlockType[,,] blocks = new BlockType[chunkSize, chunkSize, chunkSize];
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        if (y == chunkSize - 1)
                            blocks[x, y, z] = Blocks.Instance.grass;
                        else if (y > chunkSize - 4)
                            blocks[x, y, z] = Blocks.Instance.dirt;
                        else
                            blocks[x, y, z] = Blocks.Instance.stone;
                    }
                }
            }
            Chunk chunk = new Chunk(coords, this);
            chunks.Add(coords, chunk);
            chunk.CreateBlocks();
            chunk.SetBlocks(blocks);
            chunk.GenerateMesh();
            chunk.Load();
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
            return new ChunkCoords(Mathf.FloorToInt(x * 1f / Reference.Instance.chunkSize), Mathf.FloorToInt(z * 1f / Reference.Instance.chunkSize));
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
        public int z;

        public ChunkCoords(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public Coords GetStartPos()
        {
            return new Coords(x * Reference.Instance.chunkSize, 0, z * Reference.Instance.chunkSize);
        }

        public override string ToString()
        {
            return "(" + x + ", " + z + ")";
        }

        public static ChunkCoords operator +(ChunkCoords a, ChunkCoords b)
        {
            return new ChunkCoords(a.x + b.x, a.z + b.z);
        }

        public static ChunkCoords operator -(ChunkCoords a, ChunkCoords b)
        {
            return new ChunkCoords(a.x - b.x, a.z - b.z);
        }
    }
}
