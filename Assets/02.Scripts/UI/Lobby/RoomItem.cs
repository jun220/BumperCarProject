using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UltimateCartFights.Network;
using WebSocketSharp;

namespace UltimateCartFights.UI {
    public class RoomItem : MonoBehaviour {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text host;
        [SerializeField] private TMP_Text capacity;

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
        }

        public void SetParent(Transform parent) {
            transform.SetParent(parent);
            GetComponent<RectTransform>().localScale = Vector3.one;
        }

        public async void OnClickRoomItem() {
            if (!FusionSocket.IsNetworked) return;
            if (ClientInfo.Nickname.IsNullOrEmpty()) return;

            await FusionSocket.JoinRoom(roomInfo);
        }
    }
}
