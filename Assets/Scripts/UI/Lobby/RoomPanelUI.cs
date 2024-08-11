using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanelUI : MonoBehaviour
{
    #region UNITY BASIC METHOD

    private void Awake() {
        RoomPlayer.PlayerJoined += AddPlayer;
        RoomPlayer.PlayerLeft += RemovePlayer;
        RoomPlayer.PlayerChanged += UpdatePlayer;

        ChatClientNetwork.GetMessage += OnGetMessage;
        ChatInput.onSubmit.AddListener(OnSendMessage);
    }

    private void OnEnable() {
        InitializePlayerUI();
        InitializeSelectionUI();
    }

    #endregion

    #region Player Section

    [Header("Player Section")]
    [SerializeField] private PlayerUI[] PlayerUIs;

    private void InitializePlayerUI() {
        Debug.Log(string.Format("[ * Debug * ] Max Player : {0}", FusionSocket.Runner.SessionInfo.MaxPlayers));

        for(int i=0; i < PlayerUIs.Length; i++) {
            if (i < FusionSocket.Runner.SessionInfo.MaxPlayers)
                PlayerUIs[i].SetEmpty();
            else
                PlayerUIs[i].SetBlocked();
        }
    }

    private void AddPlayer(RoomPlayer player) {
        PlayerUIs[player.PlayerID].SetPlayerInfo(player);
        UpdateKartTypeUI(player);
        UpdateKartColorUI(player);
    }

    private void RemovePlayer(int playerID) {
        PlayerUIs[playerID].SetEmpty();
        CartTypeUIs[playerID].SetSelectable(true);
        CartColorUIs[playerID].SetSelectable(true);
    }

    private void UpdatePlayer(RoomPlayer player) {
        if (player.PlayerID >= PlayerUIs.Length || player.PlayerID == -1) return;

        Debug.Log(string.Format("[ * Debug * ] Player [ {0} ] Information Updated!", (string) player.Nickname));
        
        PlayerUIs[player.PlayerID].SetPlayerInfo(player);
        UpdateKartTypeUI(player);
        UpdateKartColorUI(player);

        UpdateReadyButton();
    }

    #endregion

    #region Chatbox Section

    [Header("Chat Session")]
    [SerializeField] private TMP_InputField ChatInput;
    [SerializeField] private TMP_Text ChatText;

    private void OnGetMessage(string message, ChatClientNetwork.ChatType type) {
        ChatText.text += string.Format("<color={0}>{1}</color>\n", GetTextColor(type), message);
    }

    public void OnSendMessage(string message) {
        if (message == string.Empty) return;

        ChatClientNetwork.SendChatMessage(ChatInput.text, ChatClientNetwork.ChatType.GENERAL);
        ChatInput.text = string.Empty;
        ChatInput.ActivateInputField();
    }

    private string GetTextColor(ChatClientNetwork.ChatType type) {
        switch (type) {
            case ChatClientNetwork.ChatType.SYSTEM:
                return "red";

            case ChatClientNetwork.ChatType.GENERAL:
                return "black";
        }

        return "white";
    }

    #endregion

    #region Selection Section

    [Header("Selection Section")]
    [SerializeField] private KartTypeUI[] CartTypeUIs;
    [SerializeField] private KartColorUI[] CartColorUIs;

    private int[] PlayerCartTypes;
    private int[] PlayerCartColors;

    private void InitializeSelectionUI() {
        PlayerCartTypes = new int[RoomPlayer.MAX_PLAYER];
        for(int i=0; i < RoomPlayer.MAX_PLAYER; i++) {
            PlayerCartTypes[i] = KartTypeUI.KART_TYPE_EMPTY;
        }

        PlayerCartColors = new int[RoomPlayer.MAX_PLAYER];
        for(int i=0; i < RoomPlayer.MAX_PLAYER; i++) {
            PlayerCartColors[i] = KartColorUI.KART_COLOR_EMPTY;
        }
    }

    private void UpdateKartTypeUI(RoomPlayer player) {
        if (PlayerCartTypes[player.PlayerID] != KartTypeUI.KART_TYPE_EMPTY)
            CartTypeUIs[PlayerCartTypes[player.PlayerID]].SetSelectable(true);

        if(player.KartType != KartTypeUI.KART_TYPE_EMPTY)
            CartTypeUIs[player.KartType].SetSelectable(false);

        PlayerCartTypes[player.PlayerID] = player.KartType;

        PlayerKartUI.OnKartPicked(player.PlayerID, player.KartType);
    }

    private void UpdateKartColorUI(RoomPlayer player) {
        if (PlayerCartColors[player.PlayerID] != KartColorUI.KART_COLOR_EMPTY)
            CartColorUIs[PlayerCartColors[player.PlayerID]].SetSelectable(true);

        if (player.KartColor != KartColorUI.KART_COLOR_EMPTY)
            CartColorUIs[player.KartColor].SetSelectable(false);

        PlayerCartColors[player.PlayerID] = player.KartColor;
    }

    #endregion

    #region Ready Section

    [Header("Ready Section")]
    [SerializeField] private Button ReadyButton;
    [SerializeField] private Button GameStartButton;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite waitingSprite;
    [SerializeField] private Sprite readyXSprite;
    
    public async void OnClickLeave() {
        await GameLauncher.Instance.Reconnect();
    }

    public void OnClickReady() {
        RoomPlayer.Local.RPC_ChangeReadyState(!RoomPlayer.Local.IsReady);
        ReadyButton.GetComponent<Image>().sprite = RoomPlayer.Local.IsReady ? waitingSprite : readySprite;
    }

    public void OnClickGameStart() {
        LevelManager.LoadScene(LevelManager.GAME_SCENE);
    }

    private void UpdateReadyButton() {
        if(FusionSocket.State == FusionSocket.NetworkState.HOST) {
            GameStartButton.GetComponent<Image>().sprite = CanStartGame() ? readyXSprite : waitingSprite;
            GameStartButton.transform.GetChild(0).gameObject.SetActive(CanStartGame());
            GameStartButton.interactable = CanStartGame();

            ReadyButton.gameObject.SetActive(false);
            GameStartButton.gameObject.SetActive(true);
        } else {
            bool isReady = CanReady() ^ RoomPlayer.Local.IsReady;

            ReadyButton.GetComponent<Image>().sprite = isReady ? readySprite : waitingSprite;
            ReadyButton.interactable = isReady;

            GameStartButton.gameObject.SetActive(false);
            ReadyButton.gameObject.SetActive(true);
        }
    }

    private bool CanReady() {
        if (RoomPlayer.Local.KartType == KartTypeUI.KART_TYPE_EMPTY) return false;
        if (RoomPlayer.Local.KartColor == KartColorUI.KART_COLOR_EMPTY) return false;
        return true;
    }

    private bool CanStartGame() {
        //if (FusionSocket.Runner.SessionInfo.PlayerCount < 2) return false;
        if (!IsEveryoneReady()) return false;
        if (!CanReady()) return false;
        return true;
    }

    private bool IsEveryoneReady() {
        foreach (RoomPlayer player in RoomPlayer.Players) {
            if (player == null) continue;
            if (!player.IsReady)
                return false;
        }

        return true;
    }

    #endregion
}
