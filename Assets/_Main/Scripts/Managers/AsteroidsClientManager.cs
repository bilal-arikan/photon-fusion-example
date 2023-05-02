using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion.Sockets;
using Photon.Pun.Demo.Asteroids;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Services.Matchmaker.Models.MultiplayAssignment;

namespace Fusion.Sample.DedicatedServer
{

    public class AsteroidsClientManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static AsteroidsClientManager Instance { get; private set; }

        [SerializeField] private NetworkRunner _runnerPrefab;

        public static NetworkRunner Runner { get; private set; }
        private SpaceShipNetworkInput input;
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
                options.SetEnvironmentName("production");
                options.SetProfile("main_profile");
                // options.SetProfile(UnityEngine.Random.Range(0, 99999).ToString());
                await UnityServices.InitializeAsync(options);

                await SignIn();

                Debug.Log("UnityServices initialized");
            }
            else
            {
                Debug.Log("UnityServices already initialized");
                var serverConfig = MultiplayService.Instance.ServerConfig;
            }
        }


        async Task SignIn()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId} Player token: {AuthenticationService.Instance.AccessToken}");
        }

        private void Update()
        {
            input = new()
            {
                isShooting = Input.GetKey(KeyCode.Space),
                rotation = Input.GetAxis("Horizontal"),
                acceleration = Input.GetAxis("Vertical"),
            };
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

        public async UniTask<bool> FindMatch()
        {
            var attributes = new Dictionary<string, object>
            {
                // { "platform", "ps4" },
                // { "totalSkill", 9441 }
            };

            var mmTicketResponse = await MatchmakerService.Instance.CreateTicketAsync(new List<Player>(){
                new Player(
                    AuthenticationService.Instance.PlayerId
                )
            }, new CreateTicketOptions("QueA", attributes)
            {
            });

            return await GotTicketResponse(mmTicketResponse).Catch(Debug.LogException);
        }

        async UniTask<bool> GotTicketResponse(CreateTicketResponse createTicketResponse)
        {
            MultiplayAssignment assignment = null;
            bool gotAssignment = false;
            do
            {
                //Rate limit delay
                await Task.Delay(TimeSpan.FromSeconds(1f));

                // Poll ticket
                var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(createTicketResponse.Id);
                if (ticketStatus == null)
                {
                    continue;
                }

                //Convert to platform assignment data (IOneOf conversion)
                if (ticketStatus.Type == typeof(MultiplayAssignment))
                {
                    assignment = ticketStatus.Value as MultiplayAssignment;
                }

                switch (assignment.Status)
                {
                    case StatusOptions.Found:
                        gotAssignment = true;

                        var MatchmakingResults = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();

                        Debug.Log($"Game produced by matchmaker generator {MatchmakingResults.GeneratorName}, Queue {MatchmakingResults.QueueName}, Pool {MatchmakingResults.PoolName}, BackfillTicketId {MatchmakingResults.BackfillTicketId}");

                        Debug.Log(assignment.Ip + " " + assignment.Port);

                        await Runner.StartGame(new StartGameArgs()
                        {
                            SessionName = "mm-" + assignment.MatchId,
                            GameMode = GameMode.Client,
                            SceneManager = Runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                            Scene = SceneManager.GetActiveScene().buildIndex,
                            DisableClientSessionCreation = true,
                            Address = NetAddress.CreateFromIpPort(assignment.Ip, (ushort)assignment.Port),
                        });

                        break;
                    case StatusOptions.InProgress:
                        Debug.Log("GotTicketResponse InProgress");
                        //...
                        break;
                    case StatusOptions.Failed:
                        gotAssignment = true;
                        Debug.LogError("Failed to get ticket status. Error: " + assignment.Message);
                        break;
                    case StatusOptions.Timeout:
                        gotAssignment = true;
                        Debug.LogError("Failed to get ticket status. Ticket timed out.");
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                // To Cancel matchmaking
                // await MatchmakerService.Instance.DeleteTicketAsync("<ticket id here>");

            } while (!gotAssignment);
            return assignment.Status == StatusOptions.Found;
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