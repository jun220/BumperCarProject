using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PlayerRoomState { STANBY, READY, MASTER }

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text Nickname;
    [SerializeField] private TMP_Text Ready;
    [SerializeField] private GameObject KartRender;
    [SerializeField] private GameObject Blocked;

    #region UI SETTING METHOD

    public void SetEmpty() {
        Nickname.text = string.Empty;

        Ready.gameObject.SetActive(false);
        KartRender.gameObject.SetActive(false);
        Blocked.gameObject.SetActive(false);
    }

    public void SetBlocked() {
        Nickname.text = string.Empty;

        Ready.gameObject.SetActive(false);
        KartRender.gameObject.SetActive(false);
        Blocked.gameObject.SetActive(true);
    }

    public void SetPlayerInfo(RoomPlayer player) {
        Nickname.text = (string) player.Nickname;
        Ready.text = player.IsHost ? "Host" : "Ready";


        Ready.gameObject.SetActive(player.IsReady);
        Blocked.gameObject.SetActive(false);
    }

    public void SetReady(bool state) => Ready.gameObject.SetActive(state);

    #endregion
}
