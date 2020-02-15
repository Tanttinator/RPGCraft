using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft
{
    [CreateAssetMenu(menuName = "Block", fileName = "Block")]
    public class BlockType : ScriptableObject
    {
        new public string name;
        public bool solid = true;
        public bool transparent = false;
        public BlockModel model;
    }

    /// <summary>
    /// Used to get correct texture for each face.
    /// </summary>
    [System.Serializable]
    public class BlockModel
    {
        public BlockFace[] faces;

        public BlockFace GetFace(Direction direction)
        {
            foreach(BlockFace face in faces)
            {
                if (GetDirectionFromFace(face.face) == direction)
                    return face;
            }
            return null;
        }

        Direction GetDirectionFromFace(BlockFace.Face face)
        {
            switch(face)
            {
                case BlockFace.Face.TOP: return Direction.UP;
                case BlockFace.Face.BOTTOM: return Direction.DOWN;
                case BlockFace.Face.FRONT: return Direction.NORTH;
                case BlockFace.Face.LEFT: return Direction.EAST;
                case BlockFace.Face.BACK: return Direction.SOUTH;
                case BlockFace.Face.RIGHT: return Direction.WEST;
                default: return Direction.UP;
            }
        }
    }

    [System.Serializable]
    public class BlockFace
    {
        public Vector2 textureCoord;
        public Face face;
        public bool adjacent = true;

        public enum Face
        {
            TOP,
            BOTTOM,
            FRONT,
            BACK,
            LEFT,
            RIGHT
        }
    }
}
