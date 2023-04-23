// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

namespace Fusion.Sample.DedicatedServer
{
    public class PlayerListLayout : MonoBehaviour
    {
        public LayoutGroup listRoot;
        public List<PlayerListEntry> listEntries = new();
        public PlayerListEntry entryPrefab;

        private void Awake()
        {
        }

        public void Initialize()
        {
            var runner = ClientManager.Runner;
            var players = runner.ActivePlayers.Where(p => p.IsValid).Select(p =>
            {
                // if (runner.TryGetPlayerObject(p, out var plObject) && plObject.TryGetComponent<SpaceshipBehaviour>(out var spaceship))
                if (SpaceshipBehaviour.Instances.TryGetValue(p, out var spaceship))
                {
                    return (id: p.PlayerId, name: spaceship.Nickname, ready: spaceship.IsReady, color: spaceship.Color);
                }
                return (id: p.PlayerId, name: "P_" + p.PlayerId, ready: false, color: Color.gray);
            });

            foreach (Transform item in listRoot.transform)
            {
                Destroy(item.gameObject);
            }
            listEntries.Clear();

            foreach (var item in players)
            {
                var entry = Instantiate(entryPrefab, listRoot.transform);
                entry.Initialize(item.id, item.name, item.color);
                entry.SetPlayerReady(item.ready);
            }
        }
    }
}