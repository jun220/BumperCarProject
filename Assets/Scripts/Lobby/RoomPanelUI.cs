using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanelUI : MonoBehaviour
{
    #region UNITY BASIC METHOD

    private void Awake() {
        RoomPlayer.PlayerJoined += AddPlayer;
        RoomPlayer.PlayerLeft += RemovePlayer;
        RoomPlayer.PlayerChanged += UpdatePlayer;
    }

    private void OnEnable() {
        // [ Initialize Player Section ]
        InitializePlayerUI();
          
        // [ Initialize Ready Section ]
        GameStartButton.interactable = false;

        if (FusionSocket.State == FusionSocket.NetworkState.HOST)
            GameStartButton.gameObject.SetActive(true);
        else
            ReadyButton.gameObject.SetActive(true);
    }

    private void OnDisable() {
        GameStartButton.interactable = false;

        GameStartButton.gameObject.SetActive(false);
        ReadyButton.gameObject.SetActive(false);
    }

    #endregion

    #region Player Section

    [Header("Player Section")]
    [SerializeField] private List<PlayerUI> PlayerUIs;

    private static readonly Dictionary<RoomPlayer, PlayerUI> PlayerList = new Dictionary<RoomPlayer, PlayerUI>();

    private void InitializePlayerUI() {
        Debug.Log(string.Format("[ * Debug * ] Max Player : {0}", FusionSocket.Runner.SessionInfo.MaxPlayers));

        for(int i=0; i < PlayerUIs.Count; i++) {
            if (i < FusionSocket.Runner.SessionInfo.MaxPlayers)
                PlayerUIs[i].SetEmpty();
            else
                PlayerUIs[i].SetBlocked();
        }
    }

    private void AddPlayer(RoomPlayer player) {
        foreach(PlayerUI playerUI in PlayerUIs) {
            if (PlayerList.ContainsValue(playerUI)) continue;

            Debug.Log(string.Format("[ * Debug * ] Player [ {0} ] Joined Room!", (string) player.Nickname));

            PlayerList.Add(player, playerUI);
            playerUI.SetPlayerInfo(player);
            return;
        }
    }

    private void RemovePlayer(RoomPlayer player) {
        if (!PlayerList.ContainsKey(player)) return;

        PlayerList[player].SetEmpty();
        PlayerList.Remove(player);
    }

    private void UpdatePlayer(RoomPlayer player) {
        if (!PlayerList.ContainsKey(player)) return;

        Debug.Log(string.Format("[ * Debug * ] Player [ {0} ] Information Updated!", (string) player.Nickname));
        
        PlayerList[player].SetPlayerInfo(player);
        UpdateKartTypeUI(player);
        UpdateKartColorUI(player);

        UpdateReadyButton();
    }

    #endregion

    #region Chatbox Section
    #endregion

    #region Selection Section

    [Header("Selection Section")]
    [SerializeField] private List<KartTypeUI> KartTypeUIs;
    [SerializeField] private List<KartColorUI> KartColorUIs;

    private static readonly Dictionary<RoomPlayer, KartTypeUI> TypeList = new Dictionary<RoomPlayer, KartTypeUI>();
    private static readonly Dictionary<RoomPlayer, KartColorUI> ColorList = new Dictionary<RoomPlayer, KartColorUI>();

    private void UpdateKartTypeUI(RoomPlayer player) {
        if (TypeList.ContainsKey(player)) {
            Debug.Log(string.Format("[ * Debug * ] Player [ {0} ] Before Type : {1}", (string) player.Nickname, KartTypeUIs.IndexOf(TypeList[player])));

            TypeList[player].SetSelectable(true);
            TypeList.Remove(player);
        }

        if (player.KartType != KartTypeUI.KART_TYPE_EMPTY) {
            Debug.Log(string.Format("[ * Debug * ] Player [ {0} ] After Type : {1}", (string) player.Nickname, player.KartType));

            KartTypeUI typeUI = KartTypeUIs[player.KartType];

            typeUI.SetSelectable(false);
            TypeList.Add(player, typeUI);
        }

        int PlayerUID = PlayerUIs.IndexOf(PlayerList[player]);
        if (PlayerUID != -1)
            PlayerKartUI.OnKartPicked(PlayerUID, player.KartType);
    }

    private void UpdateKartColorUI(RoomPlayer player) {
        if (ColorList.ContainsKey(player)) {
            Debug.Log(string.Format("[ * Debug * ] Player [ {0} ] Before Color : {1}", (string) player.Nickname, KartColorUIs.IndexOf(ColorList[player])));

            ColorList[player].SetSelectable(true);
            ColorList.Remove(player);
        }

        if (player.KartColor != KartColorUI.KART_COLOR_EMPTY) {
            Debug.Log(string.Format("[ * Debug * ] Player [ {0} ] After Color : {1}", (string) player.Nickname, player.KartColor));

            KartColorUI colorUI = KartColorUIs[player.KartColor];

            colorUI.SetSelectable(false);
            ColorList.Add(player, colorUI);
        }
    }

    #endregion

    #region Ready Section

    [Header("Ready Section")]
    [SerializeField] private Button ReadyButton;
    [SerializeField] private Button GameStartButton;
    
    public async void OnClickLeave() {
        await GameLauncher.Instance.Reconnect();
    }

    public void OnClickReady() {
        RoomPlayer.Local.RPC_ChangeReadyState(!RoomPlayer.Local.IsReady);
    }

    public void OnClickGameStart() {
        Debug.Log("[ * Debug * ] GameStart!");
    }

    private void UpdateReadyButton() {
        GameStartButton.interactable = CanStartGame();
        ReadyButton.interactable = CanReady();
    }

    private bool CanReady() {
        if (RoomPlayer.Local.KartType == KartTypeUI.KART_TYPE_EMPTY) return false;
        if (RoomPlayer.Local.KartColor == KartColorUI.KART_COLOR_EMPTY) return false;
        return true;
    }

    private bool CanStartGame() {
        if (RoomPlayer.Players.Count < 2) return false;
        if (!RoomPlayer.Local.IsHost) return false;
        if (!IsEveryoneReady()) return false;
        if (!CanReady()) return false;
        return true;
    }

    private bool IsEveryoneReady() {
        foreach (RoomPlayer player in RoomPlayer.Players) {
            if (!player.IsReady)
                return false;
        }

        return true;
    }

    #endregion
}
