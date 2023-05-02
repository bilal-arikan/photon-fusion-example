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
        public InputField LobbyNameInput;
        public Button JoinRandomGameButton;
        public Button JoinLobbyButton;
        public Button FindMatchButton;
        public GameObject SearchingAMatch;

        private void Start()
        {
            ShowRoomList(false);
            JoinRandomGameButton.onClick.AddListener(async () =>
            {
                JoinRandomGameButton.interactable = false;
                await AsteroidsClientManager.Instance.StartClient("");
                JoinRandomGameButton.interactable = true;
            });
            JoinLobbyButton.onClick.AddListener(async () =>
            {
                JoinLobbyButton.interactable = false;
                var result = await AsteroidsClientManager.Instance.JoinLobby(LobbyNameInput.text);
                JoinLobbyButton.interactable = true;
                ShowRoomList(result);
            });
            FindMatchButton.onClick.AddListener(async () =>
            {
                SearchingAMatch.SetActive(true);
                FindMatchButton.interactable = false;
                var result = await AsteroidsClientManager.Instance.FindMatch().Catch(Debug.LogException);
                FindMatchButton.interactable = true;
                SearchingAMatch.SetActive(false);
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

        void ShowRoomList(bool show)
        {
            roomListLayout.gameObject.SetActive(show);
            JoinLobbyButton.interactable = !show;
        }
    }
}