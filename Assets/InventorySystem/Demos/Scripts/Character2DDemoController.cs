using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Demo
{
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class Character2DDemoController : MonoBehaviour
    {


        public float speed = 3.0f;
        public float jumpSpeed = 10.0f;

        public float maxSpeed = 20.0f;



        private Rigidbody2D rigid { get; set; }



        public void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        private void AddVelocity(Vector2 velocity)
        {
            rigid.velocity = rigid.velocity + velocity;
            rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSpeed);
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.D))
            {
                // Move right
                AddVelocity(Vector2.right * speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                // Move left
                AddVelocity(-Vector2.right * speed * Time.deltaTime);
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Jump
                AddVelocity(Vector2.up * jumpSpeed);

            }

        }

    }
}
