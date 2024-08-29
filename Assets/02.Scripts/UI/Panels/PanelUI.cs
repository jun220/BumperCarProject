using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateCartFights.Network;
using Unity.VisualScripting;
using UnityEngine;
using WebSocketSharp;

namespace UltimateCartFights.UI {
    public class PanelUI : MonoBehaviour {

        #region PanelUI Singleton

        public static PanelUI Instance { get; private set; }

        private void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        #endregion

        #region Letterbox UI Method

        [Header("Letterbox")]
        [SerializeField] private GameObject Letterbox;

        private void SetLetterbox(Panel type) {
            switch(type) {
                case Panel.INTRO:
                case Panel.LOBBY:
                case Panel.ROOM:
                case Panel.LOADING:
                case Panel.FADE:
                    Letterbox.SetActive(true);
                    break;

                default:
                    Letterbox.SetActive(false);
                    break;
            }
        }

        #endregion

        #region Panel UI METHOD

        public enum Panel { INTRO, LOBBY, ROOM, LOADING, GAME, RESULT, FADE }
        
        [Header("Panel")]
        [SerializedDictionary("Network State", "Panel GameObject")]
        [SerializeField] private SerializedDictionary<Panel, GameObject> Panels;
        private GameObject CurrentPanel = null;

        public void SetPanel(Panel type) {
            if (CurrentPanel != null)
                CurrentPanel.SetActive(false);

            CurrentPanel = Panels[type];
            CurrentPanel.SetActive(true);

            SetLetterbox(type);
        }

        #endregion

        #region Intro Panel UI Method

        public void OnClickStarting() => FusionSocket.Open();
        public void OnClickOption() => PopupUI.Instance.Open(PopupUI.PopUp.OPTION);

        #endregion

        #region Lobby Panel UI Method

        [Header("Lobby Panel UI")]
        [SerializeField] private LobbyPanelUI LobbyUI;

        public void SetLoading() {
            LobbyUI.SetButtonInteractable(false);
            LobbyUI.SetScreen(LobbyPanelUI.ScreenState.LOADING);
        }

        public void SetLobby() {
            LobbyUI.SetButtonInteractable(true);
            LobbyUI.OnClickRefresh();
        }

        public void OnClickQuickMatch() {
            PopupUI.Instance.Open(PopupUI.PopUp.QUICK_MATCH);
            FusionSocket.JoinQuickMatch();
        }

        public void OnClickRoomCreation() {
            PopupUI.Instance.Open(PopupUI.PopUp.ROOM_CREATION);
        }

        public void OnSessionUpdated() {
            LobbyUI.OnClickRefresh();
        }

        #endregion

        #region Room Panel UI Method

        [Header("Room Panel UI")]
        [SerializeField] private RoomPanelUI RoomUI;

        public void InitializeRoom() => RoomUI.Initialized();

        public void LeaveRoom() => RoomUI.LeaveRoom();

        #endregion

        #region Loading Panel UI Method

        [Header("Loading Panel UI")]
        [SerializeField] LoadingPanelUI LoadingUI;

        public void InitializeLoading() => LoadingUI.Initialize();

        public void DisableLoading() => LoadingUI.Disabled();

        public void SetLoadingProgress(float progress) => LoadingUI.SetLoadingProgress(progress);

        public void ShowRandomTip() => LoadingUI.ShowRandomTip();

        #endregion

        #region Game Panel UI Method

        [Header("Game Panel UI")]
        [SerializeField] private GamePanelUI GameUI;

        public void InitializeGame() => GameUI.Initialize(ClientPlayer.Players);

        public void DisableGameUI() => GameUI.Disabled();

        #endregion

        #region Result Panel UI Method

        [Header("Result Panel UI")]
        [SerializeField] private TMP_Text Winner;

        public void InitializeResult(string winner) {
            Winner.text = winner;
        }

        #endregion
    }
}
