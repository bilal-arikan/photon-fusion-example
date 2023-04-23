using System.Collections.Generic;
using System.Linq;
using Arikan;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sample.DedicatedServer;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class GamePlayPanel : SingletonBehaviour<GamePlayPanel>
{
    public Button LeaveGameButton;
    public PlayerListLayout playerListLayout;


    private void Start()
    {
        LeaveGameButton.onClick.AddListener(() =>
        {
            ClientManager.Runner.Shutdown();
        });
    }

    public void Initialize()
    {
        playerListLayout.Initialize();
    }
}