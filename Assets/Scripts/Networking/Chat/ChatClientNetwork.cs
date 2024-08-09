using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Fusion;
using System;

public class ChatClientNetwork : MonoBehaviour, IChatClientListener 
{

    #region UNITY LIFECYCLE METHOD

    private void Start() {
        DontDestroyOnLoad(this.gameObject);

        Open();
    }

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

    private static ChatClient chatClient;

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

    private void Open() {
        chatClient = new ChatClient(this);
        chatClient.UseBackgroundWorkerForSending = true;
        chatClient.AuthValues = new AuthenticationValues(ClientInfo.Nickname);
        chatClient.ConnectUsingSettings(GetChatSettings());
    }

    private void UpdateChat() {
        if (chatClient != null)
            chatClient.Service();
    }

    private void Close() {
        if (chatClient != null)
            chatClient.Disconnect();

        chatClient = null;
    }

    public static void SendChatMessage(string message, ChatType type) {
        if (string.IsNullOrEmpty(message)) return;

        string messagePacket = AssembleChat(message, type);
        bool result = chatClient.PublishMessage(FusionSocket.Runner.SessionInfo.Name, messagePacket);

        if (!result) GetMessage?.Invoke("[ SYSTEM : 채팅을 이용할 수 없습니다. 잠시 후에 시도해보세요. ]", ChatType.SYSTEM);
    }

    private static string AssembleChat(string message, ChatType type) {
        switch(type) {
            case ChatType.SYSTEM:
                return string.Format("{0} [ SYSTEM : {1} ]", "<SYSTEM>", message);

            case ChatType.GENERAL:
                return string.Format("{0} {1} : {2}", "<GENERAL>", ClientInfo.Nickname, message);
        }

        return string.Empty;
    }

    private ChatType GetMessageType(string messagePacket) {
        string[] tokens = messagePacket.Split(new char[] {' '}, 2);

        if (tokens[0].Equals("<SYSTEM>"))   return ChatType.SYSTEM;
        if (tokens[0].Equals("<GENERAL>"))  return ChatType.GENERAL;
        return ChatType.NONE;
    }

    private string GetMessageBody(string messagePacket) => messagePacket.Split(new char[] {' '}, 2)[1];

    #endregion

    #region CHAT EVENT METHOD

    public static Action<string, ChatType> GetMessage;

    public void OnConnected() {
        string[] Channels = { FusionSocket.Runner.SessionInfo.Name };
        chatClient.Subscribe(Channels);
    }

    public void OnSubscribed(string[] channels, bool[] results) {
        SendChatMessage(string.Format("{0} 님이 참가했습니다!", ClientInfo.Nickname), ChatType.SYSTEM);
    }

    public void OnUserUnsubscribed(string channel, string user) {
        GetMessage?.Invoke(string.Format("[ SYSTEM : {0} 님이 나갔습니다! ]", user), ChatType.SYSTEM);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages) { 
        foreach(string message in messages) {
            ChatType type = GetMessageType(message);
            string body = GetMessageBody(message);
            GetMessage?.Invoke(body, type);
        }
    }

    #endregion

    #region CHAT OVERRIDE METHOD

    public void DebugReturn(DebugLevel level, string message) { }

    public void OnChatStateChange(ChatState state) { }

    public void OnPrivateMessage(string sender, object message, string channelName) { }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }

    public void OnDisconnected() { }

    public void OnUserSubscribed(string channel, string user) { }

    public void OnUnsubscribed(string[] channels) { }

    #endregion
}
