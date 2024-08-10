using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.CoreUtils;

public class LobbyPanelUI : MonoBehaviour
{
    #region UNITY BASIC METHOD

    private void Awake() {
        // -- [ Initialize Nickname Section ]
        Nickname.text = ClientInfo.Nickname;

        // -- [ Initialize Mode Section ]
        buttons = ModeSection.GetComponentsInChildren<Button>(true);
        SetActiveModeButton();

        // -- [ Initialize RoomList Section ]
        FocusScreen(LoadingScreen);

        // -- [ Add EventListener Method ]
        FusionSocket.AddNetworkStateChangedEventListener(OnNetworkStateChanged);
        FusionSocket.AddSessionListUpdatedEventListener(OnSessionListUpdated);
    }

    #endregion

    #region NICKNAME SECTION

    [Header("Nickname Section")]
    [SerializeField] private TMP_InputField Nickname;

    public void OnModifyNickname() {
        ClientInfo.Nickname = Nickname.text;
        SetActiveModeButton();
    }

    #endregion

    #region MODE SECTION

    [Header("Mode Section")]
    [SerializeField] private GameObject ModeSection;

    private Button[] buttons;

    public async void OnQuickMathcing() {
        await GameLauncher.Instance.MatchRandom();
    }

    public async void OnCloseNetwork() {
        await GameLauncher.Instance.OnClose();
    }

    private bool CanActiveMode() {
        if (FusionSocket.State != FusionSocket.NetworkState.LOBBY) return false;
        if (ClientInfo.Nickname == string.Empty) return false;
        return true;
    }

    private void SetActiveModeButton() {
        bool active = CanActiveMode();

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
    private List<SessionInfo> Sessions = new List<SessionInfo>();

    public void OnClickRefresh() {
        RoomSearching.text = string.Empty;
        ShowRoomList(Sessions);
    }

    public void OnClickSearch() {
        if (RoomSearching.text == string.Empty) return;
        string query = RoomSearching.text;

        List<SessionInfo> result = new List<SessionInfo>();
        foreach(SessionInfo session in Sessions) {
            string name = session.Properties["RoomName"];

            if (name.Contains(query))
                result.Add(session);
        }

        ShowRoomList(result);
    }

    private void FocusScreen(GameObject screen) {
        if (ActiveScreen != null) ActiveScreen.SetActive(false);

        screen.SetActive(true);
        ActiveScreen = screen;
    }

    private void ShowRoomList(List<SessionInfo> sessions) {
        ClearRoomList();

        if (sessions.Count == 0) {
            FocusScreen(EmptyScreen);
        } else {
            CreateRoomList(sessions);
            FocusScreen(RoomScreen);
        }
    }

    private void ClearRoomList() {
        int roomListSize = RoomContainer.transform.childCount;
        while (roomListSize > 0) {
            Destroy(RoomContainer.transform.GetChild(0).gameObject);
            roomListSize--;
        }
    }

    private List<SessionInfo> GetValidSessionInfo(List<SessionInfo> sessions) {
        List<SessionInfo> result = new List<SessionInfo> ();

        foreach(SessionInfo session in sessions) {
            if(IsValidSession(session)) 
                result.Add(session);
        }

        return result;
    }

    private void CreateRoomList(List<SessionInfo> sessionList) {
        foreach (SessionInfo session in sessionList) {
            RoomItem room = Instantiate(RoomItem);
            room.SetRoom(session);
            room.gameObject.transform.SetParent(RoomContainer.transform);
        }
    }

    private bool IsValidSession(SessionInfo session) {
        if (session.Properties.ContainsKey("RoomName"))     return false;
        if (session.Properties.ContainsKey("HostNickname")) return false;
        return true;
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

    public void OnSessionListUpdated(List<SessionInfo> sessions) {
        Sessions = GetValidSessionInfo(sessions);

        if (ActiveScreen == LoadingScreen)
            ShowRoomList(sessions);
    }

    private void OnJoinLobby() {
        SetActiveModeButton();
    }

    private void OnCloseInLobby() {
        SetActiveModeButton();
        FocusScreen(LoadingScreen);
    }

    #endregion
}
