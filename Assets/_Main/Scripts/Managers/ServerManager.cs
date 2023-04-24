using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion.Sample.DedicatedServer
{

    public class ServerManager : MonoBehaviour
    {

        /// <summary>
        /// Network Runner Prefab used to Spawn a new Runner used by the Server
        /// </summary>
        [SerializeField] private NetworkRunner _runnerPrefab;

        public static NetworkRunner Runner { get; private set; }


        private string backfillTicketId;
        private float acceptBackfillTicketTimer = 5f;
        private MatchmakingResults payloadAllocations;

        private const ushort k_DefaultMaxPlayers = 3;
        private const string k_DefaultServerName = "MyServerExample";
        private const string k_DefaultGameType = "MyGameType";
        private const string k_DefaultBuildId = "MyBuildId";
        private const string k_DefaultMap = "MyMap";

        async void Start()
        {
            // Load Menu Scene if not Running in Headless Mode
            // This can be replaced with a check to UNITY_SERVER if running on Unity 2021.2+
            if (CommandLineUtils.IsHeadlessMode() == false)
            {

                SceneManager.LoadScene((int)SceneDefs.MENU, LoadSceneMode.Single);
                return;
            }

            // Continue with start the Dedicated Server
            Application.targetFrameRate = 30;

            var config = DedicatedServerConfig.Resolve();
            Debug.Log(config);

            // Start a new Runner instance
            var runner = Instantiate(_runnerPrefab);

            // Start the Server
            var result = await StartSimulation(
              runner,
              config.SessionName,
              config.SessionProperties,
              config.Port,
              config.Lobby,
              config.Region,
              config.PublicIP,
              config.PublicPort
            );

            // Check if all went fine
            if (result.Ok)
            {
                Log.Debug($"Runner Start DONE");
            }
            else
            {
                // Quit the application if startup fails
                Log.Debug($"Error while starting Server: {result.ShutdownReason}");

                // it can be used any error code that can be read by an external application
                // using 0 means all went fine
                Application.Quit(1);
            }

            // await Initialize();
        }

        private void Update()
        {
            if (backfillTicketId != null)
            {
                if (acceptBackfillTicketTimer < 0)
                {
                    acceptBackfillTicketTimer = 5f;
                    HandleBackfillTickets();
                }
                acceptBackfillTicketTimer -= Time.unscaledDeltaTime;
            }
        }

        public async UniTask Initialize()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions options = new();
                await UnityServices.InitializeAsync(options);

                MultiplayEventCallbacks multiplayCallbacks = new();
                multiplayCallbacks.Allocate += MultiplayCallbacks_Allocate;
                multiplayCallbacks.Deallocate += MultiplayCallbacks_Deallocate;
                multiplayCallbacks.SubscriptionStateChanged += MultiplayCallbacks_SubscriptionStateChanged;
                multiplayCallbacks.Error += MultiplayCallbacks_Error;
                IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayCallbacks);

                var serverQuery = await MultiplayService.Instance.StartServerQueryHandlerAsync(
                    k_DefaultMaxPlayers,
                    k_DefaultServerName,
                    k_DefaultGameType,
                    k_DefaultBuildId,
                    k_DefaultMap
                );

                Debug.Log("SERVER initialized");
            }
            else
            {
                Debug.Log("SERVER already initialized");

                var serverConfig = MultiplayService.Instance.ServerConfig;
                if (!string.IsNullOrEmpty(serverConfig.AllocationId))
                {
                    MultiplayCallbacks_Allocate(new("", serverConfig.ServerId, serverConfig.AllocationId));
                }
            }
        }

        private void MultiplayCallbacks_Error(MultiplayError obj)
        {
            Debug.LogError(obj.Reason);
            Debug.LogError(obj.Detail);
        }

        private void MultiplayCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj)
        {
        }

        private void MultiplayCallbacks_Deallocate(MultiplayDeallocation obj)
        {
        }

        private void MultiplayCallbacks_Allocate(MultiplayAllocation obj)
        {
            var serverConfig = MultiplayService.Instance.ServerConfig;
            Debug.Log($"{serverConfig.IpAddress} {serverConfig.Port} {serverConfig.QueryPort} {serverConfig.ServerId}");

            // StartServer
            var result = StartSimulation(serverConfig);

            SetupBackfillTickets();
        }

        private async void SetupBackfillTickets()
        {
            payloadAllocations = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();

            backfillTicketId = payloadAllocations.BackfillTicketId;
            acceptBackfillTicketTimer = 5f;

        }

        private async void HandleBackfillTickets()
        {
            // if enough player slot available
            if (Runner != null && Runner.ActivePlayers.Count() < AsteroidsGameManager.PLAYER_COUNT_TO_START)
            {
                var ticket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(backfillTicketId);
                backfillTicketId = ticket.Id;
            }
        }

        public async void HandleUpdateBackfillTickets()
        {
            if (backfillTicketId != null && payloadAllocations != null && Runner != null && Runner.ActivePlayers.Count() < AsteroidsGameManager.PLAYER_COUNT_TO_START)
            {
                var playerList = new List<Player>();

                // TODO
                // Get player list

                MatchProperties matchProperties = new(
                    payloadAllocations.MatchProperties.Teams,
                    playerList,
                    payloadAllocations.MatchProperties.Region,
                    payloadAllocations.MatchProperties.BackfillTicketId
                );

                try
                {
                    await MatchmakerService.Instance.UpdateBackfillTicketAsync(payloadAllocations.BackfillTicketId, new BackfillTicket(
                        backfillTicketId, properties: new(matchProperties)
                    ));
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }


        private async Task<StartGameResult> StartSimulation(ServerConfig config)
        {
            if (Runner == null)
                Runner = Instantiate(_runnerPrefab);

            // Start the Server
            var result = await StartSimulation(
              Runner,
              "MySession",
              new()
              {
              },
              config.Port,
              "MyLobby",
              "",
              config.IpAddress,
              config.Port
            );

            // Check if all went fine
            if (result.Ok)
            {
                Log.Debug($"Runner Start DONE");
            }
            else
            {
                // Quit the application if startup fails
                Log.Debug($"Error while starting Server: {result.ShutdownReason}");

                // it can be used any error code that can be read by an external application
                // using 0 means all went fine
                Application.Quit(1);
            }
            return result;
        }


        private Task<StartGameResult> StartSimulation(
          NetworkRunner runner,
          string SessionName,
          Dictionary<string, SessionProperty> customProps,
          ushort port,
          string customLobby,
          string customRegion,
          string customPublicIP = null,
          ushort customPublicPort = 0
        )
        {

            // Build Custom Photon Config
            var photonSettings = PhotonAppSettings.Instance.AppSettings.GetCopy();

            if (string.IsNullOrEmpty(customRegion) == false)
            {
                photonSettings.FixedRegion = customRegion.ToLower();
            }

            // Build Custom External Addr
            NetAddress? externalAddr = null;

            if (string.IsNullOrEmpty(customPublicIP) == false && customPublicPort > 0)
            {
                if (IPAddress.TryParse(customPublicIP, out var _))
                {
                    externalAddr = NetAddress.CreateFromIpPort(customPublicIP, customPublicPort);
                }
                else
                {
                    Log.Warn("Unable to parse 'Custom Public IP'");
                }
            }

            // runner.AddCallbacks(GetComponent<AsteroidsGameManager>());

            // Start Runner
            return runner.StartGame(new StartGameArgs()
            {
                SessionName = SessionName,
                GameMode = GameMode.Server,
                SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                Scene = (int)SceneDefs.PREGAME,
                SessionProperties = customProps,
                Address = NetAddress.Any(port),
                CustomPublicAddress = externalAddr,
                CustomLobbyName = customLobby,
                CustomPhotonAppSettings = photonSettings,
                PlayerCount = AsteroidsGameManager.PLAYER_COUNT_TO_START,
            });
        }
    }
}