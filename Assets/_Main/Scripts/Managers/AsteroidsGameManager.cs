// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsteroidsGameManager.cs" company="Exit Games GmbH">
//   Part of: Asteroid demo
// </copyright>
// <summary>
//  Game Manager for the Asteroid Demo
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public enum GamePhase : byte
{
    PreStart,
    Playing,
    Finished
}

namespace Fusion.Sample.DedicatedServer
{
    [SimulationBehaviour(Modes = SimulationModes.Server)]
    public class AsteroidsGameManager : SimulationBehaviour, INetworkRunnerCallbacks
    {
        const float ASTEROIDS_MIN_SPAWN_TIME = 3;
        const float ASTEROIDS_MAX_SPAWN_TIME = 6;

        public static AsteroidsGameManager Instance = null;
        public GamePhase CurrentPhase { get; private set; }

        [SerializeField] private SpaceshipBehaviour SpaceshipPrefab;
        [SerializeField] public AsteroidBehaviour[] AsteroidLargePrefabs;
        [SerializeField] public AsteroidBehaviour[] AsteroidSmallPrefabs;

        private readonly Dictionary<PlayerRef, SpaceshipBehaviour> _playerMap = new Dictionary<PlayerRef, SpaceshipBehaviour>();

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("Another AsteroidsGameManager");
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public void Start()
        {
            Debug.Log("AsteroidsGameManager Start", this);
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!player.IsValid)
                return;
            if (player.IsNone)
                return;
            var pos = UnityEngine.Random.insideUnitSphere * 20;
            pos.y = 1;

            var character = runner.Spawn(SpaceshipPrefab, pos, Quaternion.identity, inputAuthority: player);
            character.RPC_Reposition(pos, Quaternion.identity.eulerAngles);

            _playerMap[player] = character;

            Log.Info($"Spawn for Player: {player}");
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_playerMap.TryGetValue(player, out var controller))
            {
                // Despawn Player
                runner.Despawn(controller.Object);

                // Remove player from mapping
                _playerMap.Remove(player);

                Log.Info($"Despawn for Player: {player}");
            }

            // if (_playerMap.Count == 0)
            // {
            //     Log.Info("Last player left, shutdown...");

            //     // Shutdown Server after the last player leaves
            //     runner.Shutdown();
            // }
        }

        public void TryStartGame()
        {
            if (CurrentPhase == GamePhase.Playing)
            {
                Debug.Log("Already playing");
                return;
            }
            if (CheckAllPlayersReady())
            {
                Debug.Log("CheckAllPlayersReady true");
                var gameLoop = GameLoop().Catch(e => Debug.LogException(e));
            }
            else
            {
                Debug.Log("CheckAllPlayersReady false");
                SpaceshipBehaviour.Local.RPC_SetGamePhase(GamePhase.PreStart);
            }
        }

        bool CheckAllPlayersReady()
        {
            Debug.Assert(Runner != null, "Runner null");
            Debug.Assert(Runner.ActivePlayers != null, "Runner.ActivePlayers null");
            return Runner.ActivePlayers.Where(p => p.IsValid).All(p =>
            {
                return _playerMap[p].IsReady;
            });
        }

        // called by OnCountdownTimerIsExpired() when the timer ended
        async UniTask GameLoop()
        {
            CurrentPhase = GamePhase.PreStart;
            SpaceshipBehaviour.Local.RPC_SetGamePhase(GamePhase.PreStart);
            Debug.Log("RepositionSpaceships!");
            RepositionSpaceships();

            await UniTask.Delay(TimeSpan.FromSeconds(2));

            CurrentPhase = GamePhase.Playing;
            SpaceshipBehaviour.Local.RPC_SetGamePhase(GamePhase.Playing);
            Debug.Log("SpawnAsteroidLoop!");
            var spawnAstroidTask = SpawnAsteroidLoop();
            await spawnAstroidTask;

            CurrentPhase = GamePhase.Finished;
            SpaceshipBehaviour.Local.RPC_SetGamePhase(GamePhase.Finished);
            Debug.Log("EndOfGame!");
            await EndOfGame();

            _ = Runner.Shutdown();
        }

        void RepositionSpaceships()
        {
            foreach (var p in Runner.ActivePlayers)
            {
                float angularStart = (360.0f / Runner.ActivePlayers.Count()) * p.PlayerId;
                float x = 20.0f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
                float z = 20.0f * Mathf.Cos(angularStart * Mathf.Deg2Rad);
                Vector3 position = new Vector3(x, 0.0f, z);
                Quaternion rotation = Quaternion.Euler(0.0f, angularStart, 0.0f);

                _playerMap[p].transform.position = position;
                _playerMap[p].transform.rotation = rotation;
            }
        }

        async UniTask SpawnAsteroidLoop()
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(ASTEROIDS_MIN_SPAWN_TIME, ASTEROIDS_MAX_SPAWN_TIME)));

                if (IsGameEnded())
                {
                    Debug.Log("Game Ended");
                    break;
                }

                SpawnBigAsteroid();
            }
        }

        public AsteroidBehaviour SpawnBigAsteroid()
        {
            Vector2 direction = Random.insideUnitCircle;

            var position = UnityEngine.Random.insideUnitSphere * 20;
            position.y = 1;

            // Offset slightly so we are not out of screen at creation time (as it would destroy the asteroid right away)
            position -= position.normalized * 0.1f;

            Vector3 force = -position.normalized * 1000.0f;
            Vector3 torque = Random.insideUnitSphere * Random.Range(500.0f, 1500.0f);

            var ast = Runner.Spawn(
                AsteroidLargePrefabs[0],
                position,
                Quaternion.Euler(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f),
                inputAuthority: null);

            ast.Initialize(force, torque, true);
            // Debug.Log("New asteroid");
            return ast;
        }

        public AsteroidBehaviour[] SpawnSmallAsteroids(Vector3 position, int amount)
        {
            AsteroidBehaviour[] asteroids = new AsteroidBehaviour[amount];
            for (int i = 0; i < amount; i++)
            {
                Vector3 force = Quaternion.Euler(0, i * 360.0f / amount, 0) * Vector3.forward * Random.Range(0.5f, 1.5f) * 300.0f;
                Vector3 torque = Random.insideUnitSphere * Random.Range(500.0f, 1500.0f);

                asteroids[i] = Runner.Spawn<AsteroidBehaviour>(
                    AsteroidsGameManager.Instance.AsteroidSmallPrefabs[0],
                    transform.position + force.normalized * 10.0f,
                    Quaternion.Euler(0, Random.value * 180.0f, 0),
                    null);
                asteroids[i].Initialize(force, torque, false);
            }
            return asteroids;
        }


        public async UniTask SplitAsteroid(AsteroidBehaviour asteroid)
        {
            var position = asteroid.transform.position;
            Runner.Despawn(asteroid.Object);
            if (asteroid.isLargeAsteroid)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

                int numberToSpawn = Random.Range(3, 6);
                AsteroidsGameManager.Instance.SpawnSmallAsteroids(position, numberToSpawn);
            }
        }


        bool IsGameEnded()
        {
            if (Runner.ActivePlayers.Count() == 0)
                return true;
            return false;
        }

        private async UniTask EndOfGame()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5f));

            // bool allPlayersDestroyed = true;

            // foreach (var p in Runner.ActivePlayers)
            // {

            //     object lives;
            //     if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LIVES, out lives))
            //     {
            //         if ((int)lives > 0)
            //         {
            //             allPlayersDestroyed = false;
            //             break;
            //         }
            //     }
            // }

            // if (allPlayersDestroyed)
            // {
            //     if (PhotonNetwork.IsMasterClient)
            //     {
            //         StopAllCoroutines();
            //     }

            //     string winner = "";
            //     int score = -1;

            //     foreach (Player p in PhotonNetwork.PlayerList)
            //     {
            //         if (p.GetScore() > score)
            //         {
            //             winner = p.NickName;
            //             score = p.GetScore();
            //         }
            //     }

            //     StartCoroutine(EndOfGame(winner, score));
            // }
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    }
}