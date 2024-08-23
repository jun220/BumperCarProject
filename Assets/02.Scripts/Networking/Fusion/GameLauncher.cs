using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.Utility;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class GameLauncher : FusionSocket {

        #region UNITY BASIC METHOD

        private void Start() {
            Application.runInBackground = true;
            QualitySettings.vSyncCount = 1;

            DontDestroyOnLoad(this.gameObject);

            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }

        private void Update() {
            FusionSocket.UpdateState();
        }

        #endregion

        #region LOBBY EVENT METHOD

        public static Action OnSessionUpdated;

        public static List<SessionInfo> Sessions { get; private set; } = new List<SessionInfo>();

        public override void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
            base.OnSessionListUpdated(runner, sessionList);

            Sessions = sessionList;
            OnSessionUpdated?.Invoke();
        }

        #endregion

        #region ROOM EVENT METHOD

        public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
            base.OnPlayerJoined(runner, player);
            if (!runner.IsServer) return;

            runner.Spawn(ResourceManager.Instance.Client, Vector3.zero, Quaternion.identity, player);
        }

        public override void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
            base.OnPlayerLeft(runner, player);
            ClientPlayer.RemovePlayer(runner, player);
        }

        #endregion

        #region GAME EVENT METHOD

        public override void OnSceneLoadStart(NetworkRunner runner) {
            base.OnSceneLoadStart(runner);
        }

        public override void OnSceneLoadDone(NetworkRunner runner) {
            base.OnSceneLoadDone(runner);
        }

        #endregion

        #region CONNECTION EVENT METHOD

        public override void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {
            base.OnConnectRequest(runner, request, token);

            if (IsGameScene(runner))
                request.Refuse();
            else
                request.Accept();
        }

        public override void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
            base.OnConnectFailed(runner, remoteAddress, reason);
            Open();
        }

        public override void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {
            base.OnDisconnectedFromServer(runner, reason);
            Open();
        }

        public override void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
            base.OnShutdown(runner, shutdownReason);

            Debug.Log(string.Format("[ * Debug * ] Shutdown - Reason : {0}", shutdownReason.ToString()));

            switch(shutdownReason) {
                case ShutdownReason.DisconnectedByPluginLogic:
                case ShutdownReason.GameNotFound:
                case ShutdownReason.GameIsFull:
                case ShutdownReason.GameClosed:
                    Open();
                    break;
            }
        }

        private bool IsGameScene(NetworkRunner runner) {
            NetworkSceneInfo scene;
            if (!runner.TryGetSceneInfo(out scene)) return false;
            if (scene.SceneCount == 0) return false;
            return true;
        }

        #endregion
    }
}