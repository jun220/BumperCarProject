using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelUI : MonoBehaviour
{
    #region UNITY BASIC METHOD

    private void Awake() {
        // -- [ Initialize Panel ]
        FocusPanel(Panel.INTRO);

        // -- [ Add EventListener Method ]
        FusionSocket.AddNetworkStateChangedEventListener(OnNetworkStateChanged);
    }

    #endregion

    #region PANEL UI

    [Header("Panel")]
    [SerializeField] private GameObject IntroPanel;
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject RoomPanel;

    private GameObject ActivePanel = null;

    private void FocusPanel(Panel type) {
        if (ActivePanel != null) ActivePanel.SetActive(false);

        GameObject panel = MatchPanel(type);
        panel.SetActive(true);
        ActivePanel = panel;
    }

    public void FocusIntro() => FocusPanel(Panel.INTRO);
    public void FocusLobby() => FocusPanel(Panel.LOBBY);
    public void FocusRoom() => FocusPanel(Panel.ROOM);

    #endregion

    #region UNITY EVENT METHOD

    private void OnNetworkStateChanged(FusionSocket.NetworkState state) {
        switch (state) {
            case FusionSocket.NetworkState.ROOM:
                FocusPanel(Panel.ROOM);
                break;
        }
    }

    #endregion

    #region OTHERS

    public enum Panel { INTRO, LOBBY, ROOM }

    private GameObject MatchPanel(Panel type) {
        switch(type) {
            case Panel.INTRO:
                return IntroPanel;

            case Panel.LOBBY:
                return LobbyPanel;

            case Panel.ROOM:
                return RoomPanel;
        }

        return null;
    }

    #endregion
}
