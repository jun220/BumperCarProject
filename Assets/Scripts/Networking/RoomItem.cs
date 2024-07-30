using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text host;
    [SerializeField] private TMP_Text capacity;

    public RoomInfo roomInfo { get; private set; }

    public void SetRoom(SessionInfo session) {
        this.roomInfo = new RoomInfo(
            session.Name,
            (string)session.Properties["RoomName"], 
            session.MaxPlayers, 
            (string) session.Properties["HostNickname"]
        );

        title.text = roomInfo.Name;
        host.text = roomInfo.HostNickname;
        capacity.text = string.Format("{0} / {1}", session.PlayerCount, roomInfo.MaxPlayer);
    }

    public void OnClickRoomItem() {
        GameLauncher.Instance.JoinRoom(roomInfo);
    }
}
