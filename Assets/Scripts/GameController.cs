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

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            world = GetComponent<World>();
            world.GenerateWorld();
            Player player = Instantiate(playerObj, new Vector3(8f, 16f, 8f), Quaternion.identity).GetComponent<Player>();
            player.world = world;
            onPlayerSpawned?.Invoke(player);
        }
    }
}
