using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UltimateCartFights.Utility;
using UltimateCartFights.UI;

namespace UltimateCartFights.Network {
    public class FusionSocket : NetworkBehaviour, INetworkRunnerCallbacks {

        #region FusionSocket Singleton
        
        private static FusionSocket Instance = null;
        private static SceneManager sceneManager;

        private void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);

            sceneManager = GetComponent<SceneManager>();

            statemachine.Start();
        }

        #endregion

        #region NETWORK STATE

        private static NetworkStateMachine statemachine = new();

        public static INetworkState.STATE NetworkState { get => statemachine.State; }

        protected static void UpdateState() => statemachine.Update();

        protected static void Abort() => statemachine.Abort(close);

        #endregion

        #region FUSION NETWORK FIELD

        // protect로 구현 후 scene 로딩 함수는 여기서 대신 실행해주어도 괜찮을 듯
        public static NetworkRunner Runner { get; private set; } = null;

        public static SessionInfo SessionInfo { get => Runner.SessionInfo; }

        public static bool IsNetworked {
            get {
                switch(NetworkState) {
                    case INetworkState.STATE.CLOSED:
                    case INetworkState.STATE.LOBBY_LOADING:
                        return false;

                    default:
                        return true;
                }
            }
        }

        public static bool IsHost => Runner == null ? false : Runner.IsServer;

        #endregion

        #region FUSION NETWORK - PUBLIC

        public static async Task Open() => await TryChangeNetworkState(open, INetworkState.STATE.LOBBY_LOADING);
        public static async Task<StartGameResult> JoinLobby() => await TryChangeNetworkState(joinLobby, INetworkState.STATE.LOBBY);
        public static async Task<StartGameResult> CreateRoom(RoomInfo roomInfo) => await TryChangeNetworkState(createRoom, roomInfo, INetworkState.STATE.ROOM_GENERAL);
        public static async Task<StartGameResult> JoinRoom(RoomInfo roomInfo) => await TryChangeNetworkState(joinRoom, roomInfo, INetworkState.STATE.ROOM_GENERAL);
        public static async Task<StartGameResult> JoinQuickMatch() => await TryChangeNetworkState(joinQuickMatch, INetworkState.STATE.ROOM_RANDOM);
        public static async Task LoadGameScene(SceneManager.SCENE scene) => await TryChangeNetworkState(sceneManager.LoadScene, scene, INetworkState.STATE.GAME_LOADING);
        public static async Task Loading() => await TryChangeNetworkState(INetworkState.STATE.GAME_LOADING);
        public static async Task StartGame() => await TryChangeNetworkState(INetworkState.STATE.GAME);
        public static async Task ShowGameResult() => await TryChangeNetworkState(INetworkState.STATE.RESULT);
        public static async Task ReturnRoom() => await TryChangeNetworkState(INetworkState.STATE.ROOM_GENERAL);
        public static async Task ReturnLobby() => await TryChangeNetworkState(open, INetworkState.STATE.CLOSED);
        public static async Task Close() => await TryChangeNetworkState(close, INetworkState.STATE.CLOSED);

        public static void LoadRoomScene() => sceneManager.LoadScene(SceneManager.SCENE.ROOM);

        #endregion

        #region FUSION NETWORK - PRIVATE

        private static async Task open() {
            // 만일 이미 실행중인 객체가 있다면 해당 객체를 제거한다
            if(Runner != null)
                await close();

            // 룸 접속을 위한 NetworkRunner 객체를 생성한다
            GameObject runnerObject = Instantiate(ResourceManager.Instance.Session);
            DontDestroyOnLoad(runnerObject);

            // NetworkRunner 컴포넌트를 할당한다
            Runner = runnerObject.GetComponent<NetworkRunner>();
            Runner.ProvideInput = true;
            Runner.AddCallbacks(Instance);
        }

        private static async Task<StartGameResult> joinLobby() {
            StartGameResult result = await Runner.JoinSessionLobby(SessionLobby.ClientServer);

            if (!result.Ok)
                throw new Exception(result.ErrorMessage);

            return result;
        }

        private static async Task<StartGameResult> createRoom(RoomInfo room) {
            StartGameResult result = await Runner.StartGame(new StartGameArgs {
                GameMode = GameMode.Host,
                PlayerCount = room.MaxPlayer,
                SessionProperties = GetRoomProperties(room),
                SceneManager = sceneManager,
            });

            if (!result.Ok)
                throw new Exception(result.ErrorMessage);

            ClientInfo.IsInRandom = false;
            Debug.Log(string.Format("[ * Debug * ] Create Room {0} Complete!", SessionInfo.Name));

            return result;
        }

        public static async Task<StartGameResult> joinRoom(RoomInfo room) {
            StartGameResult result = await Runner.StartGame(new StartGameArgs {
                GameMode = GameMode.Client,
                SessionName = room.RoomID,
                PlayerCount = room.MaxPlayer,
                SessionProperties = GetRoomProperties(room),
                SceneManager = sceneManager,
                EnableClientSessionCreation = false,
            });

            if (!result.Ok)
                throw new Exception(result.ErrorMessage);

            ClientInfo.IsInRandom = false;
            return result;
        }

        private static async Task<StartGameResult> joinQuickMatch() {
            StartGameResult result = await Runner.StartGame(new StartGameArgs {
                GameMode = GameMode.AutoHostOrClient,
                PlayerCount = RoomInfo.MAX_PLAYER,
                SessionProperties = GetRoomProperties(new RoomInfo()),
                SceneManager = sceneManager,
                EnableClientSessionCreation = true,
            });

            if (!result.Ok) 
                throw new Exception(result.ErrorMessage);

            ClientInfo.IsInRandom = true;
            return result;
        }

        private static async Task close() {
            if (Runner == null) return;

            ClientInfo.IsInRandom = false;

            sceneManager.BackToLobby(); 
            await Runner.Shutdown();
            Destroy(Runner.gameObject);
            Runner = null;
        }

        private static Dictionary<string, SessionProperty> GetRoomProperties(RoomInfo room) {
            Dictionary<string, SessionProperty> properties = new Dictionary<string, SessionProperty>();

            properties["RoomName"] = room.RoomName;
            properties["HostNickname"] = room.HostNickname;
            properties["IsRandom"] = (bool) room.IsRandom;

            return properties;
        }

        #endregion

        #region NETWORK EXCEPTION 

        private static async Task TryChangeNetworkState (INetworkState.STATE state) {
            try {
                statemachine.ChangeState(state);
            } catch (Exception e) {
                Debug.LogException(e);

                PopupUI.Instance.OpenError(e.Message);
                await statemachine.Abort(close);
            }
        }

        private static async Task TryChangeNetworkState<T> (Action<T> method, T param, INetworkState.STATE state) {
            try {
                statemachine.ChangeState(state, method, param);
            } catch (Exception e) {
                Debug.LogException(e);

                PopupUI.Instance.OpenError(e.Message);
                await statemachine.Abort(close);
            }
        }

        private static async Task TryChangeNetworkState(Func<Task> method, INetworkState.STATE state) {
            try {
                await statemachine.ChangeState(state, method);
            } catch (Exception e) {
                Debug.LogException(e);

                PopupUI.Instance.OpenError(e.Message);
                await statemachine.Abort(close);
            }
        }

        private static async Task TryChangeNetworkState<T> (Func<T, Task> method, T param, INetworkState.STATE state) {
            try {
                await statemachine.ChangeState(state, method, param);
            } catch (Exception e) {
                Debug.LogException(e);

                PopupUI.Instance.OpenError(e.Message);
                await statemachine.Abort(close);
            }
        }

        private static async Task<TResult> TryChangeNetworkState<TResult>(Func<Task<TResult>> method, INetworkState.STATE state) {
            try {
                TResult result = await statemachine.ChangeState(state, method);
                return result;
            } catch (Exception e) {
                Debug.LogException(e);

                PopupUI.Instance.OpenError(e.Message);
                await statemachine.Abort(close);
            }

            return default;
        }

        private static async Task<TResult> TryChangeNetworkState<T, TResult>(Func<T, Task<TResult>> method, T param, INetworkState.STATE state) {
            try {
                TResult result = await statemachine.ChangeState(state, method, param);
                return result;
            } catch (Exception e) {
                Debug.LogException(e);

                PopupUI.Instance.OpenError(e.Message);
                await statemachine.Abort(close);
            }

            return default;
        }

        #endregion

        #region NETWORK CALLBACK

        public virtual void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public virtual void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

        public virtual void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

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

        public virtual void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

        public virtual void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

        public virtual void OnSceneLoadDone(NetworkRunner runner) { }

        public virtual void OnSceneLoadStart(NetworkRunner runner) { }

        public virtual void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public virtual void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        #endregion
    }
}
