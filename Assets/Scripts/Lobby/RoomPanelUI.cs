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

        if(CanStartGame())
            GameStartButton.interactable = true;

        // 카트 번호 업데이트
    }

    #endregion

    #region Chatbox Section
    #endregion

    #region Selection Section
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

    private bool CanReady() {
        if (RoomPlayer.Local.KartType == 0) return false;
        if (RoomPlayer.Local.KartColor == 0) return false;
        return true;
    }

    private bool CanStartGame() {
        if (RoomPlayer.Players.Count < 2) return false;
        if (!RoomPlayer.Local.IsHost) return false;
        if (!IsEveryoneReady()) return false;
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
