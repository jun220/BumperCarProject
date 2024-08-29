using Fusion;
using TMPro;
using UnityEngine;
using UltimateCartFights.Network;
using WebSocketSharp;
using UltimateCartFights.Utility;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public class RoomItem : MonoBehaviour {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text host;
        [SerializeField] private TMP_Text capacity;
        [SerializeField] private Button entrance;

        public RoomInfo roomInfo { get; private set; }

        public void SetRoom(SessionInfo session) {
            this.roomInfo = new RoomInfo(
                session.Name,
                (string) session.Properties["RoomName"],
                session.MaxPlayers,
                (string) session.Properties["HostNickname"],
                false
            );

            title.text = roomInfo.RoomName;
            host.text = roomInfo.HostNickname;
            capacity.text = string.Format("{0} / {1}", session.PlayerCount, roomInfo.MaxPlayer);

            entrance.interactable = (session.PlayerCount != roomInfo.MaxPlayer);
        }

        public void SetParent(Transform parent) {
            transform.SetParent(parent);
            GetComponent<RectTransform>().localScale = Vector3.one;
        }

        public void OnClickRoomItem() {
            if (!FusionSocket.IsNetworked) return;
            if (ClientInfo.Nickname.IsNullOrEmpty()) return;

            entrance.interactable = false;
            FusionSocket.JoinRoom(roomInfo);
        }
    }
}
