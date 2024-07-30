using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchmakingPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text PlayerText;

    private void Start() {
        GameLauncher.AddPlayerJoinedEventListener(OnPlayerJoined);
    }

    private void OnPlayerJoined(PlayerRef player) {
        PlayerText.text = string.Format("{0} / {1}", FusionSocket.Runner.SessionInfo.PlayerCount, FusionSocket.Runner.SessionInfo.MaxPlayers);
    }
}
