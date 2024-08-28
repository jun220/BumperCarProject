using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using UnityEngine;
using Photon.Realtime;
using System;
using UltimateCartFights.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSocketSharp;

namespace UltimateCartFights.Network {
    public class ChatClientNetwork : MonoBehaviour, IChatClientListener {

        #region UNITY LIFECYCLE METHOD

        private void Update() {
            UpdateChat();
        }

        private void OnDestroy() {
            Close();
        }

        private void OnApplicationQuit() {
            Close();
        }

        #endregion

        #region CHAT CONNECTION METHOD

        public enum ChatType { SYSTEM, GENERAL, NONE };

        private static ChatClient ChatClient = null;
        private static string ChatChannel = string.Empty;

        private ChatAppSettings GetChatSettings() {
            AppSettings PhotonSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

            ChatAppSettings ChatAppSettings = new ChatAppSettings {
                AppIdChat = PhotonSettings.AppIdChat,
                AppVersion = PhotonSettings.AppVersion,
                FixedRegion = PhotonSettings.IsBestRegion ? null : PhotonSettings.FixedRegion,
                NetworkLogging = PhotonSettings.NetworkLogging,
                Protocol = PhotonSettings.Protocol,
                EnableProtocolFallback = PhotonSettings.EnableProtocolFallback,
                Server = PhotonSettings.IsDefaultNameServer ? null : PhotonSettings.Server,
                Port = (ushort) PhotonSettings.Port,
                ProxyServer = PhotonSettings.ProxyServer,
            };

            return ChatAppSettings;
        }

        public void Open(string channel) {
            ChatClient = new ChatClient(this);
            ChatClient.UseBackgroundWorkerForSending = true;
            ChatClient.AuthValues = new AuthenticationValues();
            ChatClient.ConnectUsingSettings(GetChatSettings());

            ChatChannel = channel;
        }

        private void UpdateChat() {
            if (ChatClient != null)
                ChatClient.Service();
        }

        public void Close() {
            if (ChatClient != null) {
                ChatClient.Disconnect();
            }

            ChatClient = null;
            ChatChannel = string.Empty;
        }

        public static void SendChatMessage(string message, ChatType type) {
            if (ChatClient == null) return;
            if (ChatChannel.IsNullOrEmpty()) return;
            if (string.IsNullOrEmpty(message)) return;

            string messagePacket = AssembleChat(message, type);
            bool result = ChatClient.PublishMessage(ChatChannel, messagePacket);

            if (!result) GetMessage?.Invoke("[ SYSTEM : 채팅을 이용할 수 없습니다. 잠시 후에 시도해보세요. ]", ChatType.SYSTEM);
        }

        private static string AssembleChat(string message, ChatType type) {
            switch (type) {
                case ChatType.SYSTEM:
                    return string.Format("{0} [ SYSTEM : {1} ]", "<SYSTEM>", message);

                case ChatType.GENERAL:
                    return string.Format("{0} {1} : {2}", "<GENERAL>", ClientInfo.Nickname, message);
            }

            return string.Empty;
        }

        private ChatType GetMessageType(string messagePacket) {
            string[] tokens = messagePacket.Split(new char[] { ' ' }, 2);

            if (tokens[0].Equals("<SYSTEM>")) return ChatType.SYSTEM;
            if (tokens[0].Equals("<GENERAL>")) return ChatType.GENERAL;
            return ChatType.NONE;
        }

        private string GetMessageBody(string messagePacket) => messagePacket.Split(new char[] { ' ' }, 2)[1];

        #endregion

        #region CHAT EVENT METHOD

        public static Action<string, ChatType> GetMessage;

        public void OnConnected() {
            ChatClient.Subscribe(new string[] { ChatChannel });
        }

        public void OnSubscribed(string[] channels, bool[] results) {
            SendChatMessage(string.Format("{0} 님이 입장했습니다!", ClientInfo.Nickname), ChatType.SYSTEM);
        }
        
        public void OnGetMessages(string channelName, string[] senders, object[] messages) {
            foreach (string message in messages) {
                ChatType type = GetMessageType(message);
                string body = GetMessageBody(message);
                GetMessage?.Invoke(body, type);
            }
        }

        #endregion

        #region CHAT OVERRIDE METHOD

        public void OnDisconnected() { }

        public void DebugReturn(DebugLevel level, string message) { }

        public void OnChatStateChange(ChatState state) { }

        public void OnPrivateMessage(string sender, object message, string channelName) { }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }

        public void OnUnsubscribed(string[] channel) { }
        
        public void OnUserSubscribed(string channel, string user) { }

        public void OnUserUnsubscribed(string channel, string user) { }
        
        #endregion
    }
}
