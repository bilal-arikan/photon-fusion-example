using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion.Sockets;
using Photon.Pun.Demo.Asteroids;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion.Sample.DedicatedServer
{

    public class AsteroidsClientManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static AsteroidsClientManager Instance { get; private set; }

        [SerializeField] private NetworkRunner _runnerPrefab;

        public static NetworkRunner Runner { get; private set; }
        private SpaceShipNetworkInput input;
        private CreateTicketResponse mmTicketResponse;
        private float pollTicketTimer;
        private UniTaskCompletionSource<bool> pollMatchMaker;

        void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            // if (AsteroidsGameManager.Instance == null)
            //     StartClient("");

            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions options = new();
                options.SetProfile(UnityEngine.Random.Range(0, 99999).ToString());
                await UnityServices.InitializeAsync(options);

                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                Debug.Log("UnityServices initialized");
            }
            else
            {
                Debug.Log("UnityServices already initialized");
                var serverConfig = MultiplayService.Instance.ServerConfig;
            }
        }

        private void Update()
        {
            input = new()
            {
                isShooting = Input.GetKey(KeyCode.Space),
                rotation = Input.GetAxis("Horizontal"),
                acceleration = Input.GetAxis("Vertical"),
            };

            if (mmTicketResponse != null)
            {
                pollTicketTimer -= Time.deltaTime;
                if (pollTicketTimer < 0)
                {
                    pollTicketTimer = 2f;
                    PollMatchMakerTicket();
                }
            }
        }

        public async UniTask<bool> StartClient(string sessionName)
        {
            if (Runner == null)
                Runner = CreateRunner("Client");

            var result = await Runner.StartGame(new StartGameArgs()
            {
                SessionName = sessionName,
                GameMode = GameMode.Client,
                SceneManager = Runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                Scene = SceneManager.GetActiveScene().buildIndex,
                DisableClientSessionCreation = true,
            });

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

        public async UniTask<UniTask<bool>> FindMatch()
        {
            mmTicketResponse = await MatchmakerService.Instance.CreateTicketAsync(new List<Player>(){
                new Player(
                    AuthenticationService.Instance.PlayerId,
                    new Dictionary<string,string>(){
                        {"skill", "100"}
                    }
                )
            }, new CreateTicketOptions()
            {
                QueueName = "QueA"
            });

            if (pollMatchMaker != null)
            {
                pollMatchMaker.TrySetException(new Exception("new find match started"));
            }
            pollMatchMaker = new();
            return pollMatchMaker.Task;
        }

        private async void PollMatchMakerTicket()
        {
            var statusResponse = await MatchmakerService.Instance.GetTicketAsync(mmTicketResponse.Id);

            if (statusResponse == null)
            {
                Debug.LogError("statusResponse null");
                return;
            }

            if (statusResponse.Value is MultiplayAssignment multiplayAssignment)
            {
                switch (multiplayAssignment.Status)
                {
                    case MultiplayAssignment.StatusOptions.Found:
                        mmTicketResponse = null;

                        Debug.Log(multiplayAssignment.Ip + " " + multiplayAssignment.Port);
                        pollMatchMaker.TrySetResult(true);
                        pollMatchMaker = null;

                        var result = await Runner.StartGame(new StartGameArgs()
                        {
                            SessionName = "MySession",
                            GameMode = GameMode.Client,
                            SceneManager = Runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                            Scene = SceneManager.GetActiveScene().buildIndex,
                            DisableClientSessionCreation = true,
                            Address = NetAddress.CreateFromIpPort(multiplayAssignment.Ip, (ushort)multiplayAssignment.Port),
                        });

                        break;
                    case MultiplayAssignment.StatusOptions.Failed:
                        mmTicketResponse = null;
                        Debug.LogError("Matchmaking failed. " + multiplayAssignment.Message);
                        pollMatchMaker.TrySetResult(false);
                        pollMatchMaker = null;
                        break;
                    case MultiplayAssignment.StatusOptions.Timeout:
                        mmTicketResponse = null;
                        Debug.LogError("Matchmaking timed out");
                        pollMatchMaker.TrySetResult(false);
                        pollMatchMaker = null;
                        break;
                    case MultiplayAssignment.StatusOptions.InProgress:
                        Debug.Log("Matchmaking InProgress");
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            else
                Debug.Log(statusResponse.Type.Name + " " + statusResponse.Value);
        }

        public async UniTask<bool> JoinLobby(string lobbyName)
        {
            if (Runner == null)
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

        public Task<StartGameResult> JoinLobby(NetworkRunner runner, string lobbyName)
        {
            return runner.JoinSessionLobby(
                string.IsNullOrEmpty(lobbyName) ? SessionLobby.ClientServer : SessionLobby.Custom,
                lobbyName);
        }

        // ------------ RUNNER CALLBACKS ------------------------------------------------------------------------------------

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            // Reload scene after shutdown
            if (Application.isPlaying)
            {
                SceneManager.LoadScene((byte)SceneDefs.MENU);
            }
            Destroy(gameObject);
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
            if (SpaceshipBehaviour.Local)
            {
                SpaceshipBehaviour.Local.RPC_SetReady(SpaceshipBehaviour.Local.IsReady);
                SpaceshipBehaviour.Local.RPC_SetColor(SpaceshipBehaviour.Local.Color);
                SpaceshipBehaviour.Local.RPC_SetNickname(SpaceshipBehaviour.Local.Nickname);
            }

            // if (PreGamePanel.Instance != null)
            //     PreGamePanel.Instance.Initialize();
            // if (GamePlayPanel.Instance != null)
            //     GamePlayPanel.Instance.Initialize();
            // if (PostGamePanel.Instance != null)
            //     PostGamePanel.Instance.Initialize();
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
            input.Set(this.input);
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