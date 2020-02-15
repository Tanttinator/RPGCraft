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
        public Coords position { get; protected set; }
        public BlockType type { get; protected set; }

        public Dictionary<Direction, Block> neighbors = new Dictionary<Direction, Block>();

        public Chunk chunk { get; protected set; }

        public Block(Coords position, Chunk chunk)
        {
            this.position = position;
            this.chunk = chunk;
            foreach(Direction direction in Direction.directions)
            {
                Block neighbor = chunk.world.GetBlock(position + direction.offset);
                SetNeighbor(direction, neighbor);
                if (neighbor != null)
                    neighbor.SetNeighbor(direction.Opposite, this);

            }
        }

        public void SetNeighbor(Direction direction, Block block)
        {
            if (neighbors.ContainsKey(direction))
                neighbors[direction] = block;
            else
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
            if (!neighbors.ContainsKey(direction))
                return true;
            Block neighbor = neighbors[direction];
            return neighbor == null || neighbor.type.transparent || (neighbor.chunk != chunk && !neighbor.chunk.loaded);
        }

        public void SetType(BlockType type)
        {
            this.type = type;
            if(chunk.loaded)
                BlockUpdate();
        }

        public void BlockUpdate(bool updateNeighbors = true)
        {
            chunk.UpdateChunk();
            if(updateNeighbors)
            {
                foreach (Block block in neighbors.Values)
                    block?.BlockUpdate(false);
            }
        }
    }

    public class BlockHitInfo
    {
        public Block block;
        public Direction face;

        public BlockHitInfo(Block block, Direction face)
        {
            this.block = block;
            this.face = face;
        }
    }
}
