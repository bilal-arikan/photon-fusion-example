using System.Collections.Generic;
using System.Linq;
using Arikan;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sample.DedicatedServer;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class PreGamePanel : SingletonBehaviour<PreGamePanel>
{
    public Text PlayerCountText;
    public InputField NicknameField;
    public Button PlayerReadyButton;
    public Button LeaveGameButton;
    public PlayerListLayout playerListLayout;


    private void Start()
    {
        PlayerReadyButton.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            SpaceshipBehaviour.Local.RPC_SetReady(!SpaceshipBehaviour.Local.IsReady);
            Debug.Log("IsReady " + SpaceshipBehaviour.Local.IsReady, SpaceshipBehaviour.Local);
        }));
        LeaveGameButton.onClick.AddListener(() =>
        {
            AsteroidsClientManager.Runner.Shutdown();
        });
        NicknameField.text = PlayerPrefs.GetString("Nickname", "Player " + UnityEngine.Random.Range(1000, 10000));
        NicknameField.onValueChanged.AddListener(v =>
        {
            PlayerPrefs.SetString("Nickname", v);
            SpaceshipBehaviour.Local.RPC_SetNickname(v);
        });
    }

    void OnEnable()
    {
        PlayerReadyButton.interactable = true;
    }

    public void Initialize()
    {
        PlayerReadyButton.interactable = SpaceshipBehaviour.Local && !SpaceshipBehaviour.Local.IsReady;

        playerListLayout.Initialize();

        PlayerCountText.text = AsteroidsClientManager.Runner.ActivePlayers.Count() + " / " + AsteroidsGameManager.PLAYER_COUNT_TO_START;
    }
}