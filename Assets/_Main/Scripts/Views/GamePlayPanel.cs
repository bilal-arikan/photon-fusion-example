using System;
using System.Collections.Generic;
using System.Linq;
using Arikan;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sample.DedicatedServer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class GamePlayPanel : SingletonBehaviour<GamePlayPanel>
{
    public Text LeftTimeText;
    public Button LeaveGameButton;
    public PlayerListLayout playerListLayout;


    private void Start()
    {
        LeaveGameButton.onClick.AddListener(() =>
        {
            AsteroidsClientManager.Runner.Shutdown();
        });
    }

    private void Update()
    {
        if (AsteroidsClientManager.Instance && SpaceshipBehaviour.Local)
        {
            if (SpaceshipBehaviour.Local.CurrentPhase == GamePhase.Playing)
            {
                LeftTimeText.text = (AsteroidsGameManager.TOTAL_GAME_TIME - (DateTime.Now - SpaceshipBehaviour.Local.PhaseChangeTime)).ToString(@"hh\:mm\:ss");
            }
            else
            {
                LeftTimeText.text = "- - -";
            }
        }
    }

    public void Initialize()
    {
        playerListLayout.Initialize();
    }
}