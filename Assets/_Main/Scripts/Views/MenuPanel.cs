using System;
using System.Collections.Generic;
using Arikan;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sample.DedicatedServer;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    public class MenuPanel : SingletonBehaviour<MenuPanel>
    {
        public RoomListLayout roomListLayout;
        public InputField PlayerNameInput;
        public InputField SessionNameInput;
        public InputField LobbyNameInput;
        public Button JoinRandomGameButton;
        public Button JoinLobbyButton;

        private void Start()
        {

            JoinRandomGameButton.onClick.AddListener(() =>
            {
                ClientManager.Instance.StartClient(SessionNameInput.text);
            });
            JoinLobbyButton.onClick.AddListener(() =>
            {
                ClientManager.Instance.JoinLobby(LobbyNameInput.text);
            });

            PlayerNameInput.text = PlayerPrefs.GetString("Nickname", "Player " + UnityEngine.Random.Range(1000, 10000));
            PlayerNameInput.onValueChanged.AddListener(v =>
            {
                PlayerPrefs.SetString("Nickname", v);
            });
        }

        public void Initialize(IEnumerable<SessionInfo> sessions)
        {
            if (sessions != null)
                roomListLayout.Initialize(sessions);
            else
            {

            }
        }
    }
}