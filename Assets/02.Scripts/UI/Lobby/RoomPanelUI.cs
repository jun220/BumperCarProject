using Photon.Realtime;
using Photon.Voice.PUN.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateCartFights.Network;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public class RoomPanelUI : MonoBehaviour {

        #region Player Section 

        [Header("Player Section")]
        [SerializeField] private List<PlayerUI> PlayerUIs;

        private void InitializeProfile() {
            for (int i = 0; i < PlayerUIs.Count; i++)
                UpdateProfile(i);
        }

        private void UpdateProfile(int playerID) {
            if (playerID >= FusionSocket.SessionInfo.MaxPlayers) {
                PlayerUIs[playerID].SetBlocked();
            } else if (ClientPlayer.GetPlayer(playerID) == null) {
                PlayerUIs[playerID].SetEmpty();
            } else {
                ClientPlayer client = ClientPlayer.GetPlayer(playerID);
                bool isReady = (client.IsLeader ? false : client.IsReady);
                PlayerUIs[playerID].SetPlayerInfo(client.Nickname, client.Character, isReady);
            }
        }

        public void OnClickLeaveRoom() {
            FusionSocket.ReconnectLobby();
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

        private List<int> SelectedCharacter = new List<int>();
        private List<int> SelectedColor = new List<int>();

        private int CurrentCharacterPage = 0;

        private void InitializeSelection() {
            for (int i = 0; i < CharacterTypes.Count; i++) {
                CharacterTypes[i].Initialize(i);
                CharacterTypes[i].SetSelected(false);
                CharacterTypes[i].gameObject.SetActive(false);

                SelectedCharacter.Add(-1);
            }

            CharacterTypes[CurrentCharacterPage].gameObject.SetActive(true);

            for (int i = 0; i < ColorTypes.Count; i++) {
                ColorTypes[i].Initialize(i);
                ColorTypes[i].SetSelected(false);

                SelectedColor.Add(-1);
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

        private void UpdateCharacter(int playerID) {
            ClientPlayer client = ClientPlayer.GetPlayer(playerID);
            if (client == null) return;

            if (SelectedCharacter[playerID] != -1)
                CharacterTypes[SelectedCharacter[playerID]].SetSelected(false);

            if(client.Character != -1)
                CharacterTypes[client.Character].SetSelected(true);

            SelectedCharacter[playerID] = client.Character; 
        }

        private void UpdateColor(int playerID) {
            ClientPlayer client = ClientPlayer.GetPlayer(playerID);
            if (client == null) return;

            if (SelectedColor[playerID] != -1)
                ColorTypes[SelectedColor[playerID]].SetSelected(false);

            if (client.CartColor != -1)
                ColorTypes[client.CartColor].SetSelected(true);

            SelectedColor[playerID] = client.CartColor;
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
                GameStartText.gameObject.SetActive(false);
                GameStartButton.interactable = false;

                ReadyButton.gameObject.SetActive(false);
            } else {
                ReadyButton.GetComponent<Image>().sprite = WaitSprite;
                ReadyButton.interactable = false;

                GameStartButton.gameObject.SetActive(false);
            }
        }

        public void OnClickReady() {
            ServerAPI.ChangeReady(!ClientPlayer.Local.IsReady);
        }

        public void OnStartGame() {
            Debug.Log("Start Game!");
        }

        private void UpdateReady() {
            if (FusionSocket.IsHost) UpdateGameStartButton();
            else UpdateReadyButton();
        }

        private void UpdateReadyButton() {
            if(ClientPlayer.Local.CanReady) {
                ReadyButton.GetComponent<Image>().sprite = WaitSprite;
                ReadyButton.interactable = false;
            } else {
                ReadyButton.GetComponent<Image>().sprite = ReadySprite;
                ReadyButton.interactable = true;
            }
        }

        private void UpdateGameStartButton() {
            if (!CanGameStart()) {
                GameStartButton.GetComponent<Image>().sprite = WaitSprite;
                GameStartText.gameObject.SetActive(false);
                GameStartButton.interactable = false;
            } else {
                GameStartButton.GetComponent<Image>().sprite = GameStartSprite;
                GameStartText.gameObject.SetActive(true);
                GameStartButton.interactable = true;
            }
        }

        private bool CanReady() {
            if (ClientPlayer.Local == null) return false;
            if (ClientPlayer.Local.Character == -1) return false;
            if (ClientPlayer.Local.CartColor == -1) return false;
            return true;
        }

        private bool CanGameStart() {
            if (FusionSocket.SessionInfo.PlayerCount == 1) return false;

            for (int i = 0; i < FusionSocket.SessionInfo.PlayerCount; i++) {
                ClientPlayer client = ClientPlayer.GetPlayer(i);

                if (client == null) return false;
                if (!client.IsReady) return false;
            }

            return true;
        }

        #endregion

        #region Others

        public void Initialized() {
            InitializeProfile();
            InitializeChat();
            InitializeSelection();
            AddClientEvent();
        }

        private void AddClientEvent() {
            ClientPlayer.PlayerJoined += UpdateProfile;
            ClientPlayer.PlayerChanged += UpdateProfile;
            ClientPlayer.PlayerLeft += UpdateProfile;

            ClientPlayer.PlayerJoined += UpdateCharacter;
            ClientPlayer.PlayerChanged += UpdateCharacter;
            ClientPlayer.PlayerLeft += UpdateCharacter;

            ClientPlayer.PlayerJoined += UpdateColor;
            ClientPlayer.PlayerChanged += UpdateColor;
            ClientPlayer.PlayerLeft += UpdateColor;
            
            ChatClientNetwork.GetMessage += OnGetMessage;
            ChatInput.onSubmit.AddListener(OnSubmitMessage);
        }

        public void RemoveClientEvent() {
            ClientPlayer.PlayerJoined -= UpdateProfile;
            ClientPlayer.PlayerChanged -= UpdateProfile;
            ClientPlayer.PlayerLeft -= UpdateProfile;

            ClientPlayer.PlayerJoined -= UpdateCharacter;
            ClientPlayer.PlayerChanged -= UpdateCharacter;
            ClientPlayer.PlayerLeft -= UpdateCharacter;

            ClientPlayer.PlayerJoined -= UpdateColor;
            ClientPlayer.PlayerChanged -= UpdateColor;
            ClientPlayer.PlayerLeft -= UpdateColor;

            ChatClientNetwork.GetMessage -= OnGetMessage;
            ChatInput.onSubmit.RemoveListener(OnSubmitMessage);
        }



        #endregion
    }
}
