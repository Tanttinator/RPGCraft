using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCraft
{
    /// <summary>
    /// Controls the state of the game.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        World world;

        private void Start()
        {
            world = GetComponent<World>();
            world.GenerateWorld();
        }
    }
}
