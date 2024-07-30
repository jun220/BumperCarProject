using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreationPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField RoomName;
    [SerializeField] private TMP_Text MaxPlayerText;
    [SerializeField] private Slider MaxPlayer;

    public void OnCreateRoom() {
        RoomInfo room = new RoomInfo(
            string.Empty,
            RoomName.text,
            (int) MaxPlayer.value,
            ClientInfo.Nickname
        );

        GameLauncher.Instance.CreateRoom(room);
    }

    public void OnChangedMaxPlayer() {
        MaxPlayerText.text = string.Format("{0}", MaxPlayer.value);
    }
}
