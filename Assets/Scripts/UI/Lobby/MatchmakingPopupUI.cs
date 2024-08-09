using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchmakingPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text PlayerText;

    private void Start() {
        FusionSocket.AddPlayerJoinedEventListener(OnPlayerJoined);
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        PlayerText.text = string.Format("{0} / {1}", runner.SessionInfo.PlayerCount, runner.SessionInfo.MaxPlayers);
    }

    public async void OnClickCancle() {
        await GameLauncher.Instance.Reconnect();
    }
}
