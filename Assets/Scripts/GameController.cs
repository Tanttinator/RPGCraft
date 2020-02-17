using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPGCraft
{
    /// <summary>
    /// Controls the state of the game.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        World world;

        public GameObject playerObj;

        public event Action<Player> onPlayerSpawned;

        public static Player player;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            world = GetComponent<World>();
            world.GenerateWorld();
            player = Instantiate(playerObj, new Vector3(8f, world.GetGroundLevel(8, 8) + 1, 8f), Quaternion.identity).GetComponent<Player>();
            Camera.main.eventMask = 0;
            player.world = world;
            onPlayerSpawned?.Invoke(player);
        }
    }
}
