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
using Sirenix.Utilities;
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

        private readonly Dictionary<PlayerRef, SpaceshipBehaviour> _playerMap = new();
        private readonly List<NetworkBehaviour> _allAstereoids = new();

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

            var character = runner.Spawn(SpaceshipPrefab, pos, Quaternion.identity, player, (runner, obj) =>
            {
                var ss = obj.GetComponent<SpaceshipBehaviour>();
            });

            _playerMap[player] = character;

            Log.Info($"Spawn for Player: {player} " + pos);
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
                SpaceshipBehaviour.Instances.ForEach(i => i.Value.RPC_SetGamePhase(GamePhase.PreStart));
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
            SpaceshipBehaviour.Instances.ForEach(i => i.Value.RPC_SetGamePhase(GamePhase.PreStart));
            Debug.Log("RepositionSpaceships!");
            RepositionSpaceships();

            await UniTask.Delay(TimeSpan.FromSeconds(2));

            CurrentPhase = GamePhase.Playing;
            SpaceshipBehaviour.Instances.ForEach(i => i.Value.RPC_SetGamePhase(GamePhase.Playing));
            Debug.Log("SpawnAsteroidLoop!");
            var spawnAstroidTask = SpawnAsteroidLoop();
            await spawnAstroidTask;
            ClearAsteroids();

            CurrentPhase = GamePhase.Finished;
            if (Runner.ActivePlayers.Count() > 0)
            {
                SpaceshipBehaviour.Instances.ForEach(i => i.Value.RPC_SetGamePhase(GamePhase.Finished));
            }
            Debug.Log("EndOfGame!");
            await EndOfGame();
        }

        void RepositionSpaceships()
        {
            ClearAsteroids();

            // foreach (var p in Runner.ActivePlayers)
            // {
            //     float angularStart = (360.0f / Runner.ActivePlayers.Count()) * p.PlayerId;
            //     float x = 20.0f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
            //     float z = 20.0f * Mathf.Cos(angularStart * Mathf.Deg2Rad);
            //     Vector3 position = new Vector3(x, 0.0f, z);
            //     Quaternion rotation = Quaternion.Euler(0.0f, angularStart, 0.0f);

            //     _playerMap[p].transform.position = position;
            //     _playerMap[p].transform.rotation = rotation;
            // }
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

            Vector3 force = UnityEngine.Random.insideUnitCircle.X_Z() * 200.0f;
            Vector3 torque = Random.insideUnitSphere * Random.Range(200.0f, 500.0f);

            var ast = Runner.Spawn(
                AsteroidLargePrefabs[0],
                position,
                Quaternion.Euler(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f),
                 null,
                (runner, obj) =>
                {
                    obj.GetComponent<AsteroidBehaviour>().Initialize(force, torque, true);
                });
            _allAstereoids.Add(ast);
            // Debug.Log("New asteroid");
            return ast;
        }

        public AsteroidBehaviour[] SpawnSmallAsteroids(Vector3 position, int amount)
        {
            AsteroidBehaviour[] asteroids = new AsteroidBehaviour[amount];
            for (int i = 0; i < amount; i++)
            {
                Vector3 force = Quaternion.Euler(0, i * 360.0f / amount, 0) * Vector3.forward * Random.Range(0.5f, 1.5f) * 100.0f;
                Vector3 torque = Random.insideUnitSphere * Random.Range(200.0f, 500.0f);

                var a = Runner.Spawn<AsteroidBehaviour>(
                    AsteroidsGameManager.Instance.AsteroidSmallPrefabs[0],
                    position + force.normalized * .5f,
                    Quaternion.Euler(0, Random.value * 180.0f, 0),
                    null,
                    (runner, obj) =>
                    {
                        obj.GetComponent<AsteroidBehaviour>().Initialize(force, torque, false);
                    });
                asteroids[i] = a;
                _allAstereoids.Add(a);
            }
            return asteroids;
        }


        public async UniTask SplitAsteroid(AsteroidBehaviour asteroid)
        {
            var position = asteroid.transform.position;
            DestroyAsteroid(asteroid);

            if (asteroid.isLargeAsteroid)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

                int numberToSpawn = Random.Range(3, 6);
                AsteroidsGameManager.Instance.SpawnSmallAsteroids(position, numberToSpawn);
            }
        }

        public void DestroyAsteroid(AsteroidBehaviour asteroid)
        {
            _allAstereoids.Remove(asteroid);
            Runner.Despawn(asteroid.Object);
        }

        public void ClearAsteroids()
        {
            foreach (var item in _allAstereoids.ToArray())
            {
                DestroyAsteroid(item as AsteroidBehaviour);
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

            ClearAsteroids();

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