using Fusion;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UltimateCartFights.Network;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public class RoomPanelUI : MonoBehaviour {

        #region Player Section 

        [Header("Player Section")]
        [SerializeField] private List<PlayerUI> PlayerUIs;

        private void UpdateProfile() {
            for (int i = 0; i < RoomInfo.MAX_PLAYER; i++)
                UpdatePlayerProfile(i);
        }

        private void UpdatePlayerProfile(int playerID) {
            PlayerUI playerUI = PlayerUIs[playerID];

            if(playerID >= FusionSocket.SessionInfo.MaxPlayers) {
                playerUI.SetBlocked();
            } else {
                ClientPlayer client = ClientPlayer.Players.FirstOrDefault(player => player.PlayerID == playerID);

                if (client == null) playerUI.SetEmpty();
                else playerUI.SetPlayerInfo((string) client.Nickname, client.Character, client.IsReady);
            }
        }

        public void OnClickLeaveRoom() {
            FusionSocket.Open();
        }

        #endregion

        #region Chatbox Section

        [Header("Chat Session")]
        [SerializeField] private TMP_InputField ChatInput;
        [SerializeField] private TMP_Text ChatText;

        private void InitializeChat() {
            ChatText.text = string.Empty;
        }

        private void OnGetMessage(string message, ChatClientNetwork.ChatType type) {
            ChatText.text += string.Format("<color={0}>{1}</color>\n", GetTextColor(type), message);
        }

        public void OnSubmitMessage(string message) {
            if (message == string.Empty) return;

            ChatClientNetwork.SendChatMessage(ChatInput.text, ChatClientNetwork.ChatType.GENERAL);
            ChatInput.text = string.Empty;
            ChatInput.ActivateInputField();
        }

        public void OnSendMessage() => OnSubmitMessage(ChatInput.text);

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
        [SerializeField] private List<CharacterSelectionUI> CharacterTypes;
        [SerializeField] private List<ColorSelectionUI> ColorTypes;

        private int CurrentCharacterPage = 0;

        private void InitializeSelection() {
            for (int i = 0; i < CharacterTypes.Count; i++) {
                CharacterTypes[i].Initialize(i);
                CharacterTypes[i].gameObject.SetActive(false);

                ClientPlayer client = ClientPlayer.Players.FirstOrDefault(x => x.Character == i);
                CharacterTypes[i].SetSelected(client != null); ;
            }

            CharacterTypes[CurrentCharacterPage].gameObject.SetActive(true);

            for (int i = 0; i < ColorTypes.Count; i++) {
                ColorTypes[i].Initialize(i);

                ClientPlayer client = ClientPlayer.Players.FirstOrDefault(x => x.CartColor == i);
                ColorTypes[i].SetSelected(client != null); ;
            }
        }

        public void OnNextCharacter() {
            CharacterTypes[CurrentCharacterPage].gameObject.SetActive(false);
            CurrentCharacterPage = (CurrentCharacterPage + 1) % CharacterTypes.Count;
            CharacterTypes[CurrentCharacterPage].gameObject.SetActive(true);
        }

        public void OnPrevCharacter() {
            CharacterTypes[CurrentCharacterPage].gameObject.SetActive(false);
            CurrentCharacterPage = (CharacterTypes.Count + CurrentCharacterPage - 1) % CharacterTypes.Count;
            CharacterTypes[CurrentCharacterPage].gameObject.SetActive(true);
        }

        private void UpdateCharacter() {
            foreach(CharacterSelectionUI characterUI in CharacterTypes)
                characterUI.SetSelected(false);

            foreach (ClientPlayer player in ClientPlayer.Players) {
                if (player.Character == -1) continue;
                CharacterTypes[player.Character].SetSelected(true);
            }
        }

        private void UpdateColor() {
            foreach (ColorSelectionUI colorUI in ColorTypes)
                colorUI.SetSelected(false);

            foreach (ClientPlayer player in ClientPlayer.Players) {
                if (player.CartColor == -1) continue;
                ColorTypes[player.CartColor].SetSelected(true);
            }
        }


        #endregion

        #region Ready Section

        [Header("Ready Section")]

        [SerializeField] private Button ReadyButton;
        [SerializeField] private Button GameStartButton;
        [SerializeField] private TMP_Text GameStartText;

        [SerializeField] private Sprite WaitSprite;
        [SerializeField] private Sprite ReadySprite;
        [SerializeField] private Sprite GameStartSprite;


        private void InitializeReady() {
            if(FusionSocket.IsHost) {
                GameStartButton.GetComponent<Image>().sprite = WaitSprite;
                GameStartButton.interactable = false;
                GameStartButton.gameObject.SetActive(true);

                GameStartText.gameObject.SetActive(false);
                ReadyButton.gameObject.SetActive(false);
            } else {
                ReadyButton.GetComponent<Image>().sprite = WaitSprite;
                ReadyButton.interactable = false;
                ReadyButton.gameObject.SetActive(true);

                GameStartButton.gameObject.SetActive(false);
            }

            UpdateReady();
        }

        public void OnClickReady() {
            if (ClientPlayer.Local == null) return;
            ClientPlayer.Local.RPC_SetReadyState(!ClientPlayer.Local.IsReady);
        }

        public void OnStartGame() {
            GameStartButton.interactable = false;
            FusionSocket.LoadGameScene(SceneManager.SCENE.MAP_GROCERY);
        }

        private void UpdateReady() {
            if (FusionSocket.IsHost) UpdateGameStartButton();
            else UpdateReadyButton();
        }

        private void UpdateReadyButton() {
            if (ClientPlayer.Local != null && ClientPlayer.Local.CanReady) {
                ReadyButton.GetComponent<Image>().sprite = ReadySprite;
                ReadyButton.interactable = true;
            } else {
                ReadyButton.GetComponent<Image>().sprite = WaitSprite;
                ReadyButton.interactable = false;
            }
        }

        private void UpdateGameStartButton() {
            if (ClientPlayer.CanStartGame) {
                GameStartButton.GetComponent<Image>().sprite = GameStartSprite;
                GameStartText.gameObject.SetActive(true);
                GameStartButton.interactable = true;
            } else {
                GameStartButton.GetComponent<Image>().sprite = WaitSprite;
                GameStartText.gameObject.SetActive(false);
                GameStartButton.interactable = false;
            }
        }

        #endregion

        #region Others

        public void Initialized() {
            UpdateProfile();
            InitializeChat();
            InitializeSelection();
            InitializeReady();

            AddClientEvent();
        }

        public void LeaveRoom() {
            RemoveClientEvent();
        }

        private void AddClientEvent() {
            ClientPlayer.PlayerUpdated += UpdateProfile;
            ClientPlayer.PlayerUpdated += UpdateCharacter;
            ClientPlayer.PlayerUpdated += UpdateColor;
            ClientPlayer.PlayerUpdated += UpdateReady;

            ChatClientNetwork.GetMessage += OnGetMessage;
            ChatInput.onSubmit.AddListener(OnSubmitMessage);
        }

        public void RemoveClientEvent() {
            ClientPlayer.PlayerUpdated -= UpdateProfile;
            ClientPlayer.PlayerUpdated -= UpdateCharacter;
            ClientPlayer.PlayerUpdated -= UpdateColor;
            ClientPlayer.PlayerUpdated -= UpdateReady;

            ChatClientNetwork.GetMessage -= OnGetMessage;
            ChatInput.onSubmit.RemoveListener(OnSubmitMessage);
        }



        #endregion
    }
}
