using UnityEngine;

namespace Fusion.Sample.DedicatedServer
{
    public class Bullet : NetworkRigidbody
    {
        public float AliveSeconds { get; private set; }

        private void Update()
        {
            if (HasStateAuthority)
            {
                AliveSeconds += Time.deltaTime;

                if (AliveSeconds > 3)
                {
                    AliveSeconds = float.MinValue;
                    Object.Runner.Despawn(Object);
                }
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }

        public void Initialize(PlayerRef owner, Vector3 originalDirection)
        {
            transform.forward = originalDirection;

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = originalDirection * 10.0f;
            rigidbody.position += originalDirection * 2;
        }
    }
}