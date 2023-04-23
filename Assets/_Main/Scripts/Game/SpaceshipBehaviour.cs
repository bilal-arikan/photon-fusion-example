// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Spaceship.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Spaceship
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Fusion.Sample.DedicatedServer
{
    public class SpaceshipBehaviour : NetworkBehaviour
    {
        [ShowInInspector] public static readonly Dictionary<PlayerRef, SpaceshipBehaviour> Instances = new();
        [ShowInInspector] public static SpaceshipBehaviour Local { get; private set; }
        [ShowInInspector] public GamePhase CurrentPhase { get; private set; }

        public string Nickname { get; private set; }
        public Color Color { get; private set; } = Color.white;
        public bool IsReady { get; private set; }

        public Bullet BulletPrefab;
        public float RotationSpeed = 90.0f;
        public float MovementSpeed = 2.0f;
        public float MaxSpeed = 0.2f;

#pragma warning disable 0109
        private new Rigidbody rigidbody;
#pragma warning restore 0109

        private float rotation = 0.0f;
        private float acceleration = 0.0f;
        private bool isShooting = false;

        private float shootingTimer = 0;


        public void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public void Start()
        {
        }


        public override void Spawned()
        {
            Instances.Add(Object.InputAuthority, this);
            if (Runner.LocalPlayer == Object.InputAuthority || Runner.LocalPlayer == Object.StateAuthority)
            {
                Local = this;
                Debug.Log("Local " + name, this);
            }
            if (Runner.IsClient)
            {
                if (Object.InputAuthority)
                {
                    // UniTask.Delay(500).ContinueWith(() =>
                    // {
                    //     Nickname = Nickname ?? PlayerPrefs.GetString("Nickname", "Unknown");
                    //     RPC_SetReady(IsReady);
                    //     RPC_SetColor(Color);
                    //     RPC_SetNickname(Nickname);
                    //     RPC_Reposition(transform.position, transform.eulerAngles);
                    // });
                }

                if (PreGamePanel.Instance != null)
                    PreGamePanel.Instance.Initialize();
                if (GamePlayPanel.Instance != null)
                    GamePlayPanel.Instance.Initialize();
                if (PostGamePanel.Instance != null)
                    PostGamePanel.Instance.Initialize();
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Instances.Remove(Object.InputAuthority);
            if (Runner.IsClient)
            {
                if (PreGamePanel.Instance != null)
                    PreGamePanel.Instance.Initialize();
                if (GamePlayPanel.Instance != null)
                    GamePlayPanel.Instance.Initialize();
                if (PostGamePanel.Instance != null)
                    PostGamePanel.Instance.Initialize();
            }
        }


        public SpaceShipNetworkInput GetLocalInput()
        {
            rotation = Input.GetAxis("Horizontal");
            acceleration = Input.GetAxis("Vertical");
            isShooting = Input.GetKey(KeyCode.Space);

            return new()
            {
                isShooting = isShooting,
                rotation = rotation,
                acceleration = acceleration,
            };
        }

        public override void FixedUpdateNetwork()
        {
            if (CurrentPhase == GamePhase.Playing)
            {
                // if (Object.HasInputAuthority)
                {
                    if (GetInput(out SpaceShipNetworkInput input))
                    {
                        rotation = input.rotation;
                        acceleration = input.acceleration;
                        isShooting = Input.GetKey(KeyCode.Space);
                    }

                    if (isShooting && shootingTimer <= 0.0)
                    {
                        shootingTimer = 0.3f;
                        RPC_Fire(transform.position, transform.rotation);
                    }
                    shootingTimer -= Time.deltaTime;
                }
            }
        }

        public void FixedUpdate()
        {
            if (CurrentPhase != GamePhase.Playing)
            {
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.velocity = Vector3.zero;
                return;
            }
            Quaternion rot = rigidbody.rotation * Quaternion.Euler(0, rotation * RotationSpeed * Time.fixedDeltaTime, 0);
            rigidbody.MoveRotation(rot);

            Vector3 force = (rot * Vector3.forward) * acceleration * 1000.0f * MovementSpeed * Time.fixedDeltaTime;
            rigidbody.AddForce(force);

            if (rigidbody.velocity.magnitude > (MaxSpeed * 1000.0f))
            {
                rigidbody.velocity = rigidbody.velocity.normalized * MaxSpeed * 1000.0f;
            }

            CheckExitScreen();
        }
        private void CheckExitScreen()
        {
            if (Camera.main == null)
            {
                return;
            }

            Rect area = new Rect(0, 0, 50, 50);

            if (Mathf.Abs(rigidbody.position.x) > area.width / 2)
            {
                rigidbody.position = new Vector3(-Mathf.Sign(rigidbody.position.x) * area.width / 2, 0, rigidbody.position.z);
                rigidbody.position -= rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
            }

            if (Mathf.Abs(rigidbody.position.z) > area.height / 2)
            {
                rigidbody.position = new Vector3(rigidbody.position.x, rigidbody.position.y, -Mathf.Sign(rigidbody.position.z) * area.height / 2);
                rigidbody.position -= rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_Fire(Vector3 position, Quaternion rotation, RpcInfo info = default)
        {
            // Debug.Log("RPC_Fire " + info.Source.PlayerId);
            if (AsteroidsGameManager.Instance)
            {
                var bullet = Runner.Spawn(BulletPrefab, position, rotation, info.Source);
                bullet.Initialize(info.Source, rotation * Vector3.forward);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_SetReady(bool ready, RpcInfo info = default)
        {
            Debug.Log("IsReady_Changed " + ready + " " + name, this);
            IsReady = ready;
            if (ClientManager.Instance)
            {
                PreGamePanel.Instance.Initialize();
            }

            if (AsteroidsGameManager.Instance)
            {
                AsteroidsGameManager.Instance.TryStartGame();
            }
        }



        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_SetColor(Color color, RpcInfo info = default)
        {
            Color = color;
            GetComponent<MeshRenderer>().material.color = Color;

            if (ClientManager.Instance)
            {
                PreGamePanel.Instance.Initialize();
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_SetNickname(string nickname, RpcInfo info = default)
        {
            Debug.Log("Nickname_Changed " + name, this);
            Nickname = nickname;

            if (ClientManager.Instance)
            {
                PreGamePanel.Instance.Initialize();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_SetGamePhase(GamePhase phase, RpcInfo info = default)
        {
            Debug.Log("RPC_SetGamePhase " + phase + " " + name, this);
            try
            {
                // if (ClientManager.Instance)
                {
                    PreGamePanel.Instance.gameObject.SetActive(phase == GamePhase.PreStart);
                    GamePlayPanel.Instance.gameObject.SetActive(phase == GamePhase.Playing);
                    PostGamePanel.Instance.gameObject.SetActive(phase == GamePhase.Finished);

                    switch (phase)
                    {
                        case GamePhase.PreStart:
                            PreGamePanel.Instance.Initialize();
                            break;
                        case GamePhase.Playing:
                            GamePlayPanel.Instance.Initialize();
                            break;
                        case GamePhase.Finished:
                            PostGamePanel.Instance.Initialize();
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            CurrentPhase = phase;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_Reposition(Vector3 position, Vector3 rotation, RpcInfo info = default)
        {
            transform.position = position;
            transform.eulerAngles = rotation;
        }
    }
}