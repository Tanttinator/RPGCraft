using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft
{
    /// <summary>
    /// Stores data for a block position in the world.
    /// </summary>
    public class Block
    {
        public Vector3 position;
        public BlockType type;

        public Dictionary<Direction, Block> neighbors = new Dictionary<Direction, Block>();

        public void SetNeighbor(Direction direction, Block block)
        {
            neighbors.Add(direction, block);
        }

        public bool IsVisible
        {
            get
            {
                foreach(Direction direction in Direction.directions)
                {
                    if(IsVisibleFrom(direction))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsVisibleFrom(Direction direction)
        {
            return !neighbors.ContainsKey(direction) || neighbors[direction] == null;
        }

        public Block(Vector3 position)
        {
            this.position = position;
        }

        public void SetType(BlockType type)
        {
            this.type = type;
        }
    }
}
