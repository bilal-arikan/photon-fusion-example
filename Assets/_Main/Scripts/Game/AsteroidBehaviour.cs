using System.Collections.Generic;
using Fusion;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Fusion.Sample.DedicatedServer
{
    public class AsteroidBehaviour : NetworkRigidbody
    {
        private Rigidbody rigid;
        public bool isLargeAsteroid { get; set; }

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Spawned()
        {
        }

        public void Initialize(Vector3 force, Vector3 torque, bool isLargeAsteroid)
        {
            this.isLargeAsteroid = isLargeAsteroid;
            rigid = GetComponent<Rigidbody>();
            rigid.AddForce(force);
            rigid.AddTorque(torque);
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.IsServer)
            {
                Rect area = new Rect(0, 0, 55, 55);

                // Out of the screen
                if (Mathf.Abs(transform.position.x) > area.width / 2)
                {
                    AsteroidsGameManager.Instance.DestroyAsteroid(this);
                }
                // Out of the screen
                if (Mathf.Abs(transform.position.z) > area.height / 2)
                {
                    AsteroidsGameManager.Instance.DestroyAsteroid(this);
                }
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (!Runner.IsServer)
            {
                // Debug.Log("OnCollisionEnter false ", this);
                return;
            }
            if (collision.gameObject.CompareTag("Bullet") && collision.gameObject.TryGetComponent<Bullet>(out var bullet))
            {
                // Debug.Log("OnCollisionEnter bullet ", this);
                _ = AsteroidsGameManager.Instance.SplitAsteroid(this);
            }
            else if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent<SpaceshipBehaviour>(out var spaceship))
            {
                // Debug.Log("OnCollisionEnter player " + spaceship.name, this);
                _ = AsteroidsGameManager.Instance.SplitAsteroid(this);
            }
        }
    }
}