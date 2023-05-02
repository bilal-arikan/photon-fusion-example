// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Spaceship.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Spaceship
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;

using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Fusion.Sample.DedicatedServer
{
    public class SpaceshipBehaviour : NetworkRigidbody, IAfterSpawned
    {
        public static readonly Dictionary<PlayerRef, SpaceshipBehaviour> Instances = new();
        public static SpaceshipBehaviour Local { get; private set; }

        public GamePhase CurrentPhase { get; private set; }
        public DateTime PhaseChangeTime { get; private set; }
        public string Nickname { get; private set; }
        public Color Color { get; private set; } = Color.white;
        public bool IsReady { get; private set; }

        private bool isDirty;

        public Bullet BulletPrefab;
        public float RotationSpeed = 90.0f;
        public float MovementSpeed = 2.0f;
        public float MaxSpeed = 0.2f;


        private float rotation = 0.0f;
        private float acceleration = 0.0f;
        private bool isShooting = false;
        private float shootingTimer = 0;

        protected override void Awake()
        {
            base.Awake();
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
                // Debug.Log("Local " + name, this);
            }
            // if (Runner.IsClient)
            // {
            //     isDirty = true;
            // }
        }

        public void AfterSpawned()
        {
            Nickname = Nickname ?? PlayerPrefs.GetString("Nickname", "Unknown");
            RPC_SetReady(IsReady);
            RPC_SetColor(Color);
            RPC_SetNickname(Nickname);
            RPC_Reposition(transform.position, transform.eulerAngles);
        }
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Instances.Remove(Object.InputAuthority);
            if (Runner.IsClient)
            {
                isDirty = true;
            }
        }



        public override void FixedUpdateNetwork()
        {
            if (CurrentPhase == GamePhase.Playing)
            {
                // if (Object.HasInputAuthority)
                // {
                //     var input = GetLocalInput();
                //     rotation = input.rotation;
                //     acceleration = input.acceleration;
                //     isShooting = input.isShooting;
                // }
                // else 
                if (GetInput(out SpaceShipNetworkInput input))
                {
                    rotation = input.rotation;
                    acceleration = input.acceleration;
                    isShooting = input.isShooting;
                }

                if (isShooting && shootingTimer <= 0.0)
                {
                    shootingTimer = 0.8f;
                    RPC_Fire(transform.position, transform.rotation);
                }
                shootingTimer -= Time.deltaTime;
            }
        }

        private void Update()
        {
            if (HasStateAuthority)
                return;
            if (isDirty)
            {
                isDirty = false;
                if (PreGamePanel.Instance != null)
                    PreGamePanel.Instance.Initialize();
                if (GamePlayPanel.Instance != null)
                    GamePlayPanel.Instance.Initialize();
                if (PostGamePanel.Instance != null)
                    PostGamePanel.Instance.Initialize();
            }
        }

        public void FixedUpdate()
        {
            // if (CurrentPhase != GamePhase.Playing)
            // {
            //     Rigidbody.angularVelocity = Vector3.zero;
            //     Rigidbody.velocity = Vector3.zero;
            //     return;
            // }
            Quaternion rot = Rigidbody.rotation * Quaternion.Euler(0, rotation * RotationSpeed * Time.fixedDeltaTime, 0);
            Rigidbody.MoveRotation(rot);

            Vector3 force = (rot * Vector3.forward) * acceleration * 1000.0f * MovementSpeed * Time.fixedDeltaTime;
            Rigidbody.AddForce(force);

            if (Rigidbody.velocity.magnitude > (MaxSpeed * 1000.0f))
            {
                Rigidbody.velocity = Rigidbody.velocity.normalized * MaxSpeed * 1000.0f;
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

            if (Mathf.Abs(Rigidbody.position.x) > area.width / 2)
            {
                Rigidbody.position = new Vector3(-Mathf.Sign(Rigidbody.position.x) * area.width / 2, 0, Rigidbody.position.z);
                Rigidbody.position -= Rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
            }

            if (Mathf.Abs(Rigidbody.position.z) > area.height / 2)
            {
                Rigidbody.position = new Vector3(Rigidbody.position.x, Rigidbody.position.y, -Mathf.Sign(Rigidbody.position.z) * area.height / 2);
                Rigidbody.position -= Rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_Fire(Vector3 position, Quaternion rotation, RpcInfo info = default)
        {
            // Debug.Log("RPC_Fire " + info.Source.PlayerId);
            if (AsteroidsGameManager.Instance)
            {
                var bullet = Runner.Spawn(BulletPrefab, position, rotation, info.Source, (runner, obj) =>
                {
                    obj.GetComponent<Bullet>().Initialize(info.Source, rotation * Vector3.forward);
                });
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SetReady(bool ready, RpcInfo info = default)
        {
            Debug.Log("IsReady_Changed " + ready + " " + name, this);
            IsReady = ready;
            if (AsteroidsClientManager.Instance)
            {
                PreGamePanel.Instance.Initialize();
            }

            if (AsteroidsGameManager.Instance)
            {
                AsteroidsGameManager.Instance.TryStartGame();
            }
            isDirty = true;
        }



        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_SetColor(Color color, RpcInfo info = default)
        {
            Color = color;
            GetComponentInChildren<MeshRenderer>().material.color = Color;

            isDirty = true;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_SetNickname(string nickname, RpcInfo info = default)
        {
            Debug.Log("Nickname_Changed " + name, this);
            Nickname = nickname;

            isDirty = true;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_SetGamePhase(GamePhase phase, long startDateTime, RpcInfo info = default)
        {

            Debug.Log("RPC_SetGamePhase " + phase + " " + name, this);

            // if (HasInputAuthority)
            {

            }
            CurrentPhase = phase;
            PhaseChangeTime = DateTime.FromBinary(startDateTime);
            PreGamePanel.Instance.gameObject.SetActive(phase == GamePhase.PreStart);
            GamePlayPanel.Instance.gameObject.SetActive(phase == GamePhase.Playing);
            PostGamePanel.Instance.gameObject.SetActive(phase == GamePhase.Finished);
            isDirty = true;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_Reposition(Vector3 position, Vector3 rotation, RpcInfo info = default)
        {
            transform.position = position;
            transform.eulerAngles = rotation;
            isDirty = true;
        }

    }
}