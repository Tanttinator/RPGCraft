using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPGCraft
{
    /// <summary>
    /// Represents a player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        Rigidbody rb;

        public float speed;
        public float rotSpeed;
        public float jumpForce;

        public Transform head;
        public Transform crosshair;

        public float interactionRange;

        int blockLayer = 9;

        public World world;

        public event Action<Vector3> onPlayerMove;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            rb.MovePosition(transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))) * speed * Time.deltaTime + transform.position);
            if (Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")) > 0)
                onPlayerMove?.Invoke(transform.position);

            //transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * speed * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, Space.World);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * jumpForce);
            }

            if (Input.GetMouseButtonDown(0))
            {
                BlockHitInfo hit = GetBlockUnderCrosshair();
                if (hit != null)
                {
                    hit.block.SetType(Blocks.Instance.empty);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                BlockHitInfo hit = GetBlockUnderCrosshair();
                if (hit != null)
                {
                    world.GetBlock(hit.block.position.Neighbor(hit.face))?.SetType(Blocks.Instance.stone);
                }
            }

            float headRot = -Input.GetAxis("Mouse Y");
            float angle = Vector3.SignedAngle(Vector3.up, head.up, head.right);
            if ((headRot < 0 && angle <= -90) || (headRot > 0 && angle >= 90))
                return;
            head.Rotate(Vector3.right * headRot * Time.deltaTime * rotSpeed, Space.Self);
        }

        /// <summary>
        /// Fires a ray towards the players look direction and returns the block + face hit.
        /// </summary>
        /// <returns></returns>
        BlockHitInfo GetBlockUnderCrosshair()
        {
            RaycastHit hit;
            Ray ray = new Ray(crosshair.position, crosshair.forward);
            if (Physics.Raycast(ray, out hit, interactionRange, (1 << blockLayer)))
            {
                Block block = world.GetBlock(hit.point - hit.normal * 0.5f);
                if (block == null)
                {
                    Debug.LogError("Block raycast hit something that's not a block! " + hit.transform.name);
                    return null;
                }

                Direction face = null;
                foreach (Direction dir in Direction.directions)
                {
                    if (dir.offset == (Coords)hit.normal)
                        face = dir;
                }
                if (face == null)
                {
                    Debug.Log("Hit face wasn't straight");
                    return null;
                }
                return new BlockHitInfo(block, face);
            }
            return null;
        }

        private void LateUpdate()
        {
            transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
        }
    }
}
