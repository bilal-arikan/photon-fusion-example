using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    [SimulationBehaviour(Modes = SimulationModes.Server)]
    public class LobbyTopPanel : SimulationBehaviour, INetworkRunnerCallbacks
    {
        private readonly string connectionStatusMessage = "    Connection Status: ";

        [Header("UI References")]
        public Text ConnectionStatusText;

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner)
        {
            ConnectionStatusText.text = "OnConnectedToServer";
        }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner)
        {
            ConnectionStatusText.text = "OnSceneLoadDone";
        }
        public void OnSceneLoadStart(NetworkRunner runner)
        {
            ConnectionStatusText.text = "OnSceneLoadStart";

        }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            ConnectionStatusText.text = "OnShutdown";
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            ConnectionStatusText.text = "OnDisconnectedFromServer";
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            ConnectionStatusText.text = "OnConnectFailed";
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            ConnectionStatusText.text = "OnSessionListUpdated";
        }
    }
}