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
                    if(!neighbors.ContainsKey(direction) || neighbors[direction] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Block(Vector3 position)
        {
            this.position = position;
        }
    }
}
