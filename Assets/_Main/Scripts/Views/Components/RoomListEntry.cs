using Fusion;
using Fusion.Sample.DedicatedServer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Fusion.Sample.DedicatedServer
{
    public class RoomListEntry : MonoBehaviour
    {
        public Text RoomNameText;
        public Text RoomPlayersText;
        public Text RoomInfoText;
        public Button JoinRoomButton;

        [ShowInInspector] SessionInfo sess;

        public void Start()
        {
            JoinRoomButton.onClick.AddListener(async () =>
            {
                var result = await ClientManager.Instance.StartClient(sess.Name);
            });
        }

        public void Initialize(SessionInfo sess)
        {
            this.sess = sess;
            RoomNameText.text = sess.Name;
            RoomPlayersText.text = sess.PlayerCount + " / " + sess.MaxPlayers;

            var props = "";
            foreach (var item in sess.Properties)
            {
                props += $"{item.Key}={item.Value.PropertyValue}, ";
            }
            RoomInfoText.text += props;
        }
    }
}