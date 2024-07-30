using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FusionSocket : MonoBehaviour, INetworkRunnerCallbacks {
    public enum NetworkState { OPEN, LOBBY, RANDOM, ROOM, CLOSED };

    private static NetworkState _state = NetworkState.CLOSED;
    public static NetworkState State {
        get => _state;
        private set {
            Debug.Log(string.Format("[ Socket ] State Changed : {0} -> {1}", _state.ToString(), value.ToString()));

            _state = value;
            NetworkStateChangedEvent?.Invoke(value);
        }
    }

    public static NetworkRunner Runner { get; private set; } = null;

    #region FUSION NETWORK METHOD

    protected void Open() {
        Assert.Check(Runner == null);

        BeforeOpen();
        
        // Create NetworkRunner DontDestroy Object
        GameObject runnerObject = new GameObject("Session");
        DontDestroyOnLoad(runnerObject);

        // Add NetworkRunner Component
        Runner = runnerObject.AddComponent<NetworkRunner>();
        Runner.ProvideInput = true;
        Runner.AddCallbacks(this);

        State = NetworkState.OPEN;

        AfterOpen();
    }

    protected async Task<StartGameResult> JoinLobby() {
        Assert.Check(Runner != null);

        BeforeJoinLobby();

        StartGameResult result = await Runner.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok) {
            OnJoinLobbySuccess();
            State = NetworkState.LOBBY;
            AfterJoinLobby();
        } else {
            OnJoinLobbyFailure(result);
            await Close();
        }

        return result;
    }

    protected async Task<StartGameResult> CreateRoom(RoomInfo room) {
        Assert.Check(Runner != null);

        BeforeCreateRoom(room);

        Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
        customProperties["RoomName"] = room.Name;
        customProperties["HostNickname"] = room.HostNickname;
        customProperties["IsRandom"] = false;

        StartGameResult result = await Runner.StartGame(new StartGameArgs {
            GameMode = GameMode.Host,
            PlayerCount = room.MaxPlayer,
            SessionProperties = customProperties,
        });

        if (result.Ok) {
            OnCreateRoomSuccess();
            State = NetworkState.ROOM;
            AfterCreateRoom(room);
        } else {
            OnCreateRoomFailure(result);
            await Close();
        }

        return result;
    }

    protected async Task<StartGameResult> JoinRoom(RoomInfo room) {
        Assert.Check(Runner != null);

        BeforeJoinRoom(room);

        Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
        customProperties["RoomName"] = room.Name;
        customProperties["HostNickname"] = room.HostNickname;
        customProperties["IsRandom"] = false;

        StartGameResult result = await Runner.StartGame(new StartGameArgs {
            GameMode = GameMode.Client,
            SessionName = room.RoomID,
            PlayerCount = room.MaxPlayer,
            SessionProperties=customProperties,
            EnableClientSessionCreation = false,
        });

        if (result.Ok) {
            OnJoinRoomSuccess();
            State = NetworkState.ROOM;
            AfterJoinRoom(room);
        } else {
            OnJoinRoomFailure(result);
            await Close();
        }

        return result;
    }

    protected async Task<StartGameResult> JoinRandomRoom() {
        Assert.Check(Runner != null);

        BeforeJoinRandomRoom();

        Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();
        customProperties["IsRandom"] = true;

        StartGameResult result = await Runner.StartGame(new StartGameArgs {
            GameMode = GameMode.AutoHostOrClient,
            SessionProperties = customProperties,
            PlayerCount = RoomInfo.MAX_PLAYER,
            EnableClientSessionCreation = true,
        });

        if (result.Ok) {
            OnJoinRoomSuccess();
            State = NetworkState.RANDOM;
        } else {
            OnJoinRoomFailure(result);
            await Close();
        }

        AfterJoinRandomRoom();

        return result;
    }

    protected async Task Close() {
        BeforeClose();

        State = NetworkState.CLOSED;

        if (Runner != null) {
            await Runner.Shutdown();

            Destroy(Runner.gameObject);
            Runner = null;
        }

        AfterClose();
    }

    #endregion

    #region NETWORK PRE/POST PROCESSING

    protected virtual void BeforeOpen() { }
    protected virtual void BeforeJoinLobby() { }
    protected virtual void BeforeCreateRoom(RoomInfo room) { }
    protected virtual void BeforeJoinRoom(RoomInfo room) { }
    protected virtual void BeforeJoinRandomRoom() { }
    protected virtual void BeforeClose() { }

    protected virtual void AfterOpen() { }
    protected virtual void AfterJoinLobby() { }
    protected virtual void AfterCreateRoom(RoomInfo room) { }
    protected virtual void AfterJoinRoom(RoomInfo room) { }
    protected virtual void AfterJoinRandomRoom() { }
    protected virtual void AfterClose() { }

    #endregion

    #region NETWORK PROCESSING

    protected virtual void OnJoinLobbySuccess() { }
    protected virtual void OnJoinLobbyFailure(StartGameResult result) { }

    protected virtual void OnCreateRoomSuccess() { }
    protected virtual void OnCreateRoomFailure(StartGameResult result) { }

    protected virtual void OnJoinRoomSuccess() { }
    protected virtual void OnJoinRoomFailure(StartGameResult result) { }

    #endregion

    #region NETWORK EVENT 

    private static Action<NetworkState> NetworkStateChangedEvent;
    public static void AddNetworkStateChangedEventListener(Action<NetworkState> method) => NetworkStateChangedEvent += method;

    #endregion

    #region NETWORK CALLBACK

    public virtual void OnConnectedToServer(NetworkRunner runner) { }

    public virtual void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public virtual void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public virtual void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public virtual void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public virtual void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public virtual void OnInput(NetworkRunner runner, NetworkInput input) { }

    public virtual void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public virtual void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public virtual void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public virtual void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

    public virtual void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public virtual void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public virtual void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public virtual void OnSceneLoadDone(NetworkRunner runner) { }

    public virtual void OnSceneLoadStart(NetworkRunner runner) { }

    public virtual void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public virtual void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public virtual void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    #endregion
}
