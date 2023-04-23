using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion.Sockets;
using Photon.Pun.Demo.Asteroids;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion.Sample.DedicatedServer
{

    public class ClientManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static ClientManager Instance { get; private set; }

        [SerializeField] private NetworkRunner _runnerPrefab;

        public static NetworkRunner Runner { get; private set; }

        void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        }

        private void Start()
        {
            if (AsteroidsGameManager.Instance == null)
                StartClient("");
        }

        public async UniTask<bool> StartClient(string sessionName)
        {
            Runner = CreateRunner("Client");

            var result = await StartSimulation(Runner, GameMode.Client, sessionName);

            if (result.Ok == false)
            {
                Debug.LogWarning(result.ShutdownReason);
                return false;
            }
            else
            {
                Debug.Log("Done");
                return true;
            }
        }

        public async UniTask<bool> JoinLobby(string lobbyName)
        {
            Runner = CreateRunner("Client");

            var result = await JoinLobby(Runner, lobbyName);

            if (result.Ok == false)
            {
                Debug.LogWarning(result.ShutdownReason);
                return false;
            }
            else
            {
                Debug.Log("Done");
                return true;
            }
        }

        private NetworkRunner CreateRunner(string name)
        {
            var runner = Instantiate(_runnerPrefab);
            runner.name = name;
            runner.ProvideInput = true;
            runner.AddCallbacks(this);

            return runner;
        }

        public Task<StartGameResult> StartSimulation(
            NetworkRunner runner,
            GameMode gameMode,
            string sessionName
          )
        {

            return runner.StartGame(new StartGameArgs()
            {
                SessionName = sessionName,
                GameMode = gameMode,
                SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                Scene = SceneManager.GetActiveScene().buildIndex,
                DisableClientSessionCreation = true,
            });
        }

        public Task<StartGameResult> JoinLobby(NetworkRunner runner, string lobbyName)
        {
            return runner.JoinSessionLobby(string.IsNullOrEmpty(lobbyName) ? SessionLobby.ClientServer : SessionLobby.Custom, lobbyName);
        }

        // ------------ RUNNER CALLBACKS ------------------------------------------------------------------------------------

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            // Reload scene after shutdown
            if (Application.isPlaying)
            {
                SceneManager.LoadScene((byte)SceneDefs.MENU);
            }
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            runner.Shutdown();
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            runner.Shutdown();
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Log.Debug($"Received: {sessionList.Count}");
            MenuPanel.Instance.Initialize(sessionList.ToArray());
        }

        // Other callbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (PreGamePanel.Instance != null)
                PreGamePanel.Instance.Initialize();
            if (GamePlayPanel.Instance != null)
                GamePlayPanel.Instance.Initialize();
            if (PostGamePanel.Instance != null)
                PostGamePanel.Instance.Initialize();
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (PreGamePanel.Instance != null)
                PreGamePanel.Instance.Initialize();
            if (GamePlayPanel.Instance != null)
                GamePlayPanel.Instance.Initialize();
            if (PostGamePanel.Instance != null)
                PostGamePanel.Instance.Initialize();
        }
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (SpaceshipBehaviour.Local)
            {
                input.Set(SpaceshipBehaviour.Local.GetLocalInput());
            }
        }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner)
        {

        }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (PreGamePanel.Instance != null)
                PreGamePanel.Instance.Initialize();
            if (GamePlayPanel.Instance != null)
                GamePlayPanel.Instance.Initialize();
            if (PostGamePanel.Instance != null)
                PostGamePanel.Instance.Initialize();
        }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    }
}