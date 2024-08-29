using AYellowpaper.SerializedCollections;
using Fusion;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UltimateCartFights.Network;
using UltimateCartFights.Utility;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace UltimateCartFights.UI {
    public class LobbyPanelUI : MonoBehaviour {

        #region Unity Basic Method

        private void Awake() {
            Nickname.text = ClientInfo.Nickname;
        }

        #endregion

        #region Nickname Section

        [Header("Nickname Section")]
        [SerializeField] private TMP_InputField Nickname;

        public void OnModifyNickname() {
            if(!Nickname.text.Trim().IsNullOrEmpty())
                ClientInfo.Nickname = Nickname.text.Trim();

            Nickname.text = ClientInfo.Nickname;
            SetButtonInteractable(!ClientInfo.Nickname.IsNullOrEmpty());
        }

        #endregion

        #region Mode Section

        [Header("Mode Section")]
        [SerializeField] private List<Button> buttons;

        public void SetButtonInteractable(bool active) {
            if(active) {
                if (ClientInfo.Nickname.IsNullOrEmpty()) return;
                if (!FusionSocket.IsNetworked) return;
            }

            foreach(Button button in buttons)
                button.interactable = active;
        }

        #endregion

        #region Search Part

        [Header("Search Part")]
        [SerializeField] private TMP_InputField Search;

        public void OnClickSearch() {
            string query = Search.text;

            List<SessionInfo> result = (from session in GameLauncher.Sessions
                                        where IsValidRoom(session)
                                           && ((string) session.Properties["RoomName"]).Contains(query)
                                        select session).ToList();

            ShowRoomList(result);
        }

        public void OnClickRefresh() {
            if (!Search.text.IsNullOrEmpty()) {
                OnClickSearch();
                return;
            }

            List<SessionInfo> result = (from session in GameLauncher.Sessions
                                        where IsValidRoom(session)
                                        select session).ToList();
            
            ShowRoomList(result);
        }

        #endregion

        #region Room List Part

        public enum ScreenState { LOADING, EMPTY, ROOMLIST }

        [Header("Room List Part")]
        [SerializedDictionary("Screen State", "Screen GameObject")]
        [SerializeField] private SerializedDictionary<ScreenState, GameObject> Screens;
        [SerializeField] private GameObject RoomContainer;
        [SerializeField] private RoomItem RoomItem;

        private GameObject CurrentScreen = null;
        private List<RoomItem> RoomItems = new List<RoomItem>();

        public void SetScreen(ScreenState state) {
            if (CurrentScreen != null)
                CurrentScreen.SetActive(false);

            CurrentScreen = Screens[state];
            CurrentScreen.SetActive(true);
        }

        private void ShowRoomList(List<SessionInfo> sessions) {
            ClearRoomList();

            if(sessions.Count == 0) {
                SetScreen(ScreenState.EMPTY);
            } else {
                CreateRoomList(sessions);
                SetScreen(ScreenState.ROOMLIST);
            }

        }

        private void ClearRoomList() {
            foreach(RoomItem roomItem in RoomItems)
                Destroy(roomItem.gameObject);

            RoomItems = new List<RoomItem>();
        }

        private void CreateRoomList(List<SessionInfo> sessions) {
            foreach(SessionInfo session in sessions) {
                RoomItem room = Instantiate(RoomItem);
                room.SetRoom(session);
                room.SetParent(RoomContainer.transform);

                RoomItems.Add(room);
            }
        }

        private bool IsValidRoom(SessionInfo session) {
            if (!session.Properties.ContainsKey("RoomName"))        return false;
            if (!session.Properties.ContainsKey("HostNickname"))    return false;
            if (!session.Properties.ContainsKey("IsRandom"))        return false;
            if ((bool) session.Properties["IsRandom"])              return false;
            return true;
        }

        #endregion
    }
}
