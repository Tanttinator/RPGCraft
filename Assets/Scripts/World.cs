using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPGCraft
{
    /// <summary>
    /// Handles world generation and chunks.
    /// </summary>
    public class World : MonoBehaviour
    {
        public int chunkSize;
        public GameObject chunkGo;
        private void Start()
        {
            Block[,,] blocks = new Block[chunkSize, chunkSize, chunkSize];
            for(int x = 0; x < chunkSize; x++)
            {
                for(int y = 0; y < chunkSize; y++)
                {
                    for(int z = 0; z < chunkSize; z++)
                    {
                        Block block = new Block(new Vector3(x, y, z));
                        block.SetType(Blocks.Instance.stone);
                        blocks[x, y, z] = block;
                    }
                }
            }
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        foreach (Direction direction in Direction.directions)
                        {
                            Block block = blocks[x, y, z];
                            Vector3 pos = block.position + direction.offset;
                            if(pos.x >= 0 && pos.x < chunkSize && pos.y >= 0 && pos.y < chunkSize &&  pos.z >= 0 && pos.z < chunkSize)
                                blocks[x, y, z].SetNeighbor(direction, blocks[(int)pos.x, (int)pos.y, (int)pos.z]);
                        }
                    }
                }
            }
            Chunk chunk = Instantiate(chunkGo, new Vector3(0, 0, 0), Quaternion.identity, transform).GetComponent<Chunk>();
            chunk.SetBlocks(blocks);
            chunk.GenerateMesh();
        }
    }

    public class Direction
    {
        public Vector3 offset;
        Func<Direction> opposite;

        public Direction Opposite => opposite.Invoke();

        public Direction(Vector3 offset, Func<Direction> opposite)
        {
            this.offset = offset;
            this.opposite = opposite;
        }

        public static Direction UP = new Direction(new Vector3(0, 1, 0), () => DOWN);
        public static Direction DOWN = new Direction(new Vector3(0, -1, 0), () => UP);
        public static Direction NORTH = new Direction(new Vector3(0, 0, 1), () => SOUTH);
        public static Direction EAST = new Direction(new Vector3(1, 0, 0), () => WEST);
        public static Direction SOUTH = new Direction(new Vector3(0, 0, -1), () => NORTH);
        public static Direction WEST = new Direction(new Vector3(-1, 0, 0), () => EAST);

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
}
