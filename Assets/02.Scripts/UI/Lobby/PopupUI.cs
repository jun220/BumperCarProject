using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateCartFights.Network;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace UltimateCartFights.UI {
    public class PopupUI : MonoBehaviour {

        #region PopUp UI Singleton

        public static PopupUI Instance { get; private set; }

        private void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        #endregion

        #region PopUp UI

        public enum PopUp { QUICK_MATCH, ROOM_CREATION, OPTION }

        [SerializedDictionary("Network State", "Panel GameObject")]
        [SerializeField] private SerializedDictionary<PopUp, GameObject> Popups;

        public void Open(PopUp type) => Popups[type].SetActive(true);
        public void Close(PopUp type) => Popups[type].SetActive(false);

        public void CloseAll() {
            foreach(GameObject popup in Popups.Values)
                popup.SetActive(false);
        }

        #endregion

        #region Room Creation PopUp UI

        [Header("Room Creation PopUp UI")]
        [SerializeField] private TMP_InputField RoomName;
        [SerializeField] private TMP_Text MaxPlayerText;
        [SerializeField] private Slider MaxPlayer;
        [SerializeField] private Button Submit;

        public void OnModifyRoomName() {
            if (RoomName.text.IsNullOrEmpty()) return;
            Submit.interactable = true;
        }

        public void OnModifyMaxPlayer() {
            MaxPlayerText.text = ((int) MaxPlayer.value).ToString();
        }

        public void OnClickRoomCancle() => Close(PopUp.ROOM_CREATION);
        public void OnClickSubmit() {
            Submit.interactable = false;
            Close(PopUp.ROOM_CREATION);

            RoomInfo room = new RoomInfo(RoomName.text, (int) MaxPlayer.value, ClientInfo.Nickname);
            FusionSocket.CreateRoom(room);
        }

        #endregion

        #region Error PopUp UI

        [Header("Error PopUp UI")]
        [SerializeField] private GameObject ErrorPopup;
        [SerializeField] private TMP_Text ErrorTitle;
        [SerializeField] private TMP_Text ErrorText;

        public void OpenError(string ErrorMessage) {
            CloseAll();

            ErrorText.text = ErrorMessage;
            ErrorPopup.SetActive(true);
        }

        public void CloseError() => ErrorPopup.SetActive(false);

        #endregion
    }
}
