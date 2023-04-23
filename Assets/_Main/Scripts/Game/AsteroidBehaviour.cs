using System.Collections.Generic;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fusion.Sample.DedicatedServer
{
    public class AsteroidBehaviour : NetworkBehaviour
    {
        private Rigidbody rigid;
        [ShowInInspector] public Vector3 addForce { get; set; }
        [ShowInInspector] public Vector3 addTorque { get; set; }
        [ShowInInspector] public bool isLargeAsteroid { get; set; }

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
        }

        public override void Spawned()
        {
            rigid.AddForce(addForce);
            rigid.AddTorque(addTorque);
        }

        public void Initialize(Vector3 force, Vector3 torque, bool isLargeAsteroid)
        {
            this.isLargeAsteroid = isLargeAsteroid;
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigid.AddForce(addForce);
            rigid.AddTorque(addTorque);
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.IsServer)
            {
                Rect area = new Rect(0, 0, 55, 55);

                // Out of the screen
                if (Mathf.Abs(transform.position.x) > area.width / 2)
                {
                    Runner.Despawn(Object);
                }
                // Out of the screen
                if (Mathf.Abs(transform.position.z) > area.height / 2)
                {
                    Runner.Despawn(Object);
                }
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            // Debug.Log("Despawned", this);
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
                // bullet.Id
                Debug.Log("OnCollisionEnter bullet ", this);
                _ = AsteroidsGameManager.Instance.SplitAsteroid(this);
            }
            else if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent<SpaceshipBehaviour>(out var spaceship))
            {
                // collision.gameObject.GetComponent<PhotonView>().RPC("DestroySpaceship", RpcTarget.All);
                Debug.Log("OnCollisionEnter player " + spaceship.name, this);
                _ = AsteroidsGameManager.Instance.SplitAsteroid(this);
            }
        }
    }
}