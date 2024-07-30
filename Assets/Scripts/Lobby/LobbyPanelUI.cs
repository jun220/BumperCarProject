using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanelUI : MonoBehaviour
{
    #region UNITY BASIC METHOD

    private void Awake() {
        // -- [ Initialize Nickname Section ]
        Nickname.text = ClientInfo.Nickname;

        // -- [ Initialize Mode Section ]
        SetActiveModeButton(false);

        // -- [ Initialize RoomList Section ]
        FocusScreen(LoadingScreen);

        // -- [ Add EventListener Method ]
        FusionSocket.AddNetworkStateChangedEventListener(OnNetworkStateChanged);
        GameLauncher.AddSessionListUpdatedEventListener(OnSessionListUpdated);
    }

    #endregion

    #region LOBBY SCREEN METHOD

    

    #endregion

    #region NICKNAME SECTION

    [Header("Nickname Section")]
    [SerializeField] private TMP_InputField Nickname;

    public void OnModifyNickname() => ClientInfo.Nickname = Nickname.text;

    #endregion

    #region MODE SECTION

    [Header("Mode Section")]
    [SerializeField] private GameObject ModeSection;

    public void OnQuickMathcing() {
        GameLauncher.Instance.MatchRandom();
    }

    public async void OnBackToIntro() {
        await GameLauncher.Instance.OnClose();
        IsRefresh = true;
    }

    private void SetActiveModeButton(bool active) {
        Button[] buttons = ModeSection.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons) {
            button.interactable = active;
        }
    }

    #endregion

    #region ROOMLIST SECTION
    [Header("RoomList Section")]

    [Header("RoomList Panel Screens")]
    [SerializeField] private GameObject RoomScreen;
    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private GameObject EmptyScreen;

    [Header("Room UI")]
    [SerializeField] private TMP_InputField RoomSearching;
    [SerializeField] private GameObject RoomContainer;
    [SerializeField] private RoomItem RoomItem;

    private GameObject ActiveScreen = null;
    private bool IsRefresh = true;

    public void OnClickRefresh() => IsRefresh = true;

    public void OnClickSearch() {
        if (RoomSearching.text == string.Empty) return;
        string query = RoomSearching.text;

        ClearRoomList();

        List<SessionInfo> result = new List<SessionInfo>();
        foreach(SessionInfo session in GameLauncher.SessionList) {
            string name = session.Properties["RoomName"];

            if (name.Contains(query))
                result.Add(session);
        }

        if (result.Count == 0) {
            FocusScreen(EmptyScreen);
        } else {
            CreateRoomList(result);
            FocusScreen(RoomScreen);
        }
    }

    private void FocusScreen(GameObject screen) {
        if (ActiveScreen != null) ActiveScreen.SetActive(false);

        screen.SetActive(true);
        ActiveScreen = screen;
    }

    private void ClearRoomList() {
        int roomListSize = RoomContainer.transform.childCount;
        while (roomListSize > 0) {
            Destroy(RoomContainer.transform.GetChild(0).gameObject);
            roomListSize--;
        }
    }

    private void CreateRoomList(List<SessionInfo> sessionList) {
        foreach (SessionInfo session in sessionList) {
            RoomItem room = Instantiate(RoomItem);
            room.SetRoom(session);
            room.gameObject.transform.SetParent(RoomContainer.transform);
        }
    }

    #endregion

    #region UNITY EVENT METHOD

    private void OnNetworkStateChanged(FusionSocket.NetworkState state) {
        switch (state) {
            case FusionSocket.NetworkState.LOBBY:
                OnJoinLobby();
                break;

            case FusionSocket.NetworkState.CLOSED:
                OnCloseInLobby();
                break;
        }
    }

    public void OnSessionListUpdated() {
        if (!IsRefresh) return;
        IsRefresh = false;

        ClearRoomList();

        if (GameLauncher.SessionList.Count == 0) {
            FocusScreen(EmptyScreen);
        } else {
            CreateRoomList(GameLauncher.SessionList);
            FocusScreen(RoomScreen);
        }
    }

    private void OnJoinLobby() {
        SetActiveModeButton(true);
    }

    private void OnCloseInLobby() {
        SetActiveModeButton(false);
        FocusScreen(LoadingScreen);
    }

    #endregion
}
