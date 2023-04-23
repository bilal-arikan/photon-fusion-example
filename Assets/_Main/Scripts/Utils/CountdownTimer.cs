// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="CountdownTimer.cs" company="Exit Games GmbH">
// //   Part of: Photon Unity Utilities,
// // </copyright>
// // <summary>
// // This is a basic CountdownTimer. In order to start the timer, the MasterClient can add a certain entry to the Custom Room Properties,
// // which contains the property's name 'StartTime' and the actual start time describing the moment, the timer has been started.
// // To have a synchronized timer, the best practice is to use PhotonNetwork.Time.
// // In order to subscribe to the CountdownTimerHasExpired event you can call CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
// // from Unity's OnEnable function for example. For unsubscribing simply call CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;.
// // You can do this from Unity's OnDisable function for example.
// // </summary>
// // <author>developer@exitgames.com</author>
// // --------------------------------------------------------------------------------------------------------------------

// using System;
// using System.Collections.Generic;
// using ExitGames.Client.Photon;
// using Fusion;
// using Fusion.Sample.DedicatedServer;
// using Fusion.Sockets;
// using UnityEngine;
// using UnityEngine.UI;

// namespace Fusion.Sample.DedicatedServer
// {
//     /// <summary>This is a basic, network-synced CountdownTimer based on properties.</summary>
//     /// <remarks>
//     /// In order to start the timer, the MasterClient can call SetStartTime() to set the timestamp for the start.
//     /// The property 'StartTime' then contains the server timestamp when the timer has been started.
//     /// 
//     /// In order to subscribe to the CountdownTimerHasExpired event you can call CountdownTimer.OnCountdownTimerHasExpired
//     /// += OnCountdownTimerIsExpired;
//     /// from Unity's OnEnable function for example. For unsubscribing simply call CountdownTimer.OnCountdownTimerHasExpired
//     /// -= OnCountdownTimerIsExpired;.
//     /// 
//     /// You can do this from Unity's OnEnable and OnDisable functions.
//     /// </remarks>
//     public class CountdownTimer : NetworkObject, INetworkRunnerCallbacks
//     {

//         /// <summary>
//         ///     OnCountdownTimerHasExpired delegate.
//         /// </summary>
//         public delegate void CountdownTimerHasExpired();

//         public const string CountdownStartTime = "StartTime";

//         [Header("Countdown time in seconds")]
//         public float Countdown = 5.0f;

//         private bool isTimerRunning;

//         private int startTime;

//         [Header("Reference to a Text component for visualizing the countdown")]
//         public Text Text;


//         /// <summary>
//         ///     Called when the timer has expired.
//         /// </summary>
//         public static event CountdownTimerHasExpired OnCountdownTimerHasExpired;


//         public void Start()
//         {
//             if (this.Text == null) Debug.LogError("Reference to 'Text' is not set. Please set a valid reference.", this);
//         }

//         public void OnEnable()
//         {
//             Debug.Log("OnEnable CountdownTimer");

//             // the starttime may already be in the props. look it up.
//             Initialize();
//         }

//         public void OnDisable()
//         {
//             Debug.Log("OnDisable CountdownTimer");
//         }


//         public void Update()
//         {
//             if (!this.isTimerRunning) return;

//             float countdown = TimeRemaining();
//             this.Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n0"));

//             if (countdown > 0.0f) return;

//             OnTimerEnds();
//         }


//         private void OnTimerRuns()
//         {
//             this.isTimerRunning = true;
//             this.enabled = true;
//         }

//         private void OnTimerEnds()
//         {
//             this.isTimerRunning = false;
//             this.enabled = false;

//             Debug.Log("Emptying info text.", this.Text);
//             this.Text.text = string.Empty;

//             if (OnCountdownTimerHasExpired != null) OnCountdownTimerHasExpired();
//         }


//         public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
//         {
//             Debug.Log("CountdownTimer.OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());
//             Initialize();
//         }


//         private void Initialize()
//         {
//             int propStartTime;
//             if (TryGetStartTime(out propStartTime))
//             {
//                 this.startTime = propStartTime;
//                 Debug.Log("Initialize sets StartTime " + this.startTime + " server time now: " + PhotonNetwork.ServerTimestamp + " remain: " + TimeRemaining());


//                 this.isTimerRunning = TimeRemaining() > 0;

//                 if (this.isTimerRunning)
//                     OnTimerRuns();
//                 else
//                     OnTimerEnds();
//             }
//         }


//         private float TimeRemaining()
//         {
//             int timer = PhotonNetwork.ServerTimestamp - this.startTime;
//             return this.Countdown - timer / 1000f;
//         }


//         public static bool TryGetStartTime(out int startTimestamp)
//         {
//             startTimestamp = PhotonNetwork.ServerTimestamp;

//             object startTimeFromProps;
//             if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTime, out startTimeFromProps))
//             {
//                 startTimestamp = (int)startTimeFromProps;
//                 return true;
//             }

//             return false;
//         }


//         public static void SetStartTime()
//         {
//             int startTime = 0;
//             bool wasSet = TryGetStartTime(out startTime);

//             Hashtable props = new Hashtable
//             {
//                 {CountdownTimer.CountdownStartTime, (int)PhotonNetwork.ServerTimestamp}
//             };
//             PhotonNetwork.CurrentRoom.SetCustomProperties(props);


//             Debug.Log("Set Custom Props for Time: " + props.ToStringFull() + " wasSet: " + wasSet);
//         }

//         void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
//         void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
//         void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
//         void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
//         void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner) { }
//         void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
//         void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
//         void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
//         void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
//         void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
//         void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
//         void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
//         void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
//         void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

//         void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
//         {
//         }
//         void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
//         {
//         }
//     }
// }