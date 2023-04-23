// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Client.Photon;
using Fusion.Sample.DedicatedServer;
using UnityEngine;
using UnityEngine.UI;

namespace Fusion.Sample.DedicatedServer
{
    public class PlayerListEntry : MonoBehaviour
    {
        [Header("UI References")]
        public Text PlayerNameText;
        public Image PlayerColorImage;
        public Image PlayerReadyImage;

        private int ownerId;

        private void Awake()
        {
        }

        public void Initialize(int playerId, string playerName, Color color)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;

            PlayerColorImage.color = color;
        }

        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyImage.color = playerReady ? Color.green : Color.gray;
            PlayerReadyImage.enabled = playerReady;
        }
    }
}