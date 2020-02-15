using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft {
    /// <summary>
    /// Helper class to build block meshes.
    /// </summary>
    public static class BlockMetrics
    {
        public static Vector3 wds = new Vector3(0f, 0f, 0f);
        public static Vector3 eds = new Vector3(1f, 0f, 0f);
        public static Vector3 wus = new Vector3(0f, 1f, 0f);
        public static Vector3 eus = new Vector3(1f, 1f, 0f);
        public static Vector3 wdn = new Vector3(0f, 0f, 1f);
        public static Vector3 edn = new Vector3(1f, 0f, 1f);
        public static Vector3 wun = new Vector3(0f, 1f, 1f);
        public static Vector3 eun = new Vector3(1f, 1f, 1f);

        public static Vector3[] up = new Vector3[]
                {
                    eun, //bottom left
                    wun, //bottom right
                    eus, //top left
                    wus, //top right
                };

        public static Vector3[] down = new Vector3[]
                {
                    eds, //bottom left
                    wds, //bottom right
                    edn, //top left
                    wdn, //top right
                };

        public static Vector3[] north = new Vector3[]
                {
                    edn, //bottom left
                    wdn, //bottom right
                    eun, //top left
                    wun, //top right
                };

        public static Vector3[] east = new Vector3[]
                {
                    eds, //bottom left
                    edn, //bottom right
                    eus, //top left
                    eun, //top right
                };

        public static Vector3[] south = new Vector3[]
                {
                    wds, //bottom left
                    eds, //bottom right
                    wus, //top left
                    eus, //top right
                };

        public static Vector3[] west = new Vector3[]
                {
                    wdn, //bottom left
                    wds, //bottom right
                    wun, //top left
                    wus, //top right
                };

        /// <summary>
        /// Get local vertex coords for a face.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3[] GetFaceVertices(Direction direction)
        {
            if(direction == Direction.UP)
            {
                return up;
            }
            if (direction == Direction.DOWN)
            {
                return down;
            }
            if (direction == Direction.NORTH)
            {
                return north;
            }
            if (direction == Direction.EAST)
            {
                return east;
            }
            if (direction == Direction.SOUTH)
            {
                return south;
            }
            if (direction == Direction.WEST)
            {
                return west;
            }
            else
            {
                Debug.LogError("Unknown direction: " + direction.offset);
                return up;
            }
        }
    }
}
