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
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

namespace Fusion.Sample.DedicatedServer
{
    public class RoomListLayout : MonoBehaviour
    {
        public LayoutGroup listRoot;
        public List<RoomListEntry> listEntries = new();
        public RoomListEntry entryPrefab;

        private void Awake()
        {
        }

        public void Initialize(IEnumerable<SessionInfo> sessions)
        {
            foreach (Transform item in listRoot.transform)
            {
                Destroy(item.gameObject);
            }
            listEntries.Clear();

            foreach (var item in sessions)
            {
                var entry = Instantiate(entryPrefab, listRoot.transform);
                entry.Initialize(item);
            }
        }
    }
}