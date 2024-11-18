using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateCartFights.Game;
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

        // 플레이어 참가 - Host 측에서 PlayerClient 프리팹 객체 생성
        public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
            base.OnPlayerJoined(runner, player);
            if (!runner.IsServer) return;

            runner.Spawn(ResourceManager.Instance.Client, Vector3.zero, Quaternion.identity, player);
        }

        // 플레이어 탈퇴 - Host / Client 각자 해당 플레이어 PlayerClient 프리팹 객체 제거
        public override void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
            base.OnPlayerLeft(runner, player);
            ClientPlayer.RemovePlayer(runner, player);

            // if문 검사는 게임 플레이 중 일어나는 경우 체크 -> 6주차 분량
            if (NetworkState == INetworkState.STATE.GAME) {
                SendKnockedOutEvent(player);
                CartController.RemoveCart(runner, player);
                CheckGameEnded();
            }
        }

        #endregion

        #region GAME EVENT METHOD

        public static Action GameStarted;
        public static Action GameEnded;

        public static int WinnerID;

        public static void AddGameEvents() {
            CartController.Knockedout += OnPlayerDead;
        }

        public static void RemoveGameEvents() {
            CartController.Knockedout -= OnPlayerDead;
        }

        public static void CheckGameStarted() {
            if (Map.IsGameStarted()) GameStarted?.Invoke();
        }

        public static void CheckGameEnded() {
            if (CartController.Carts.Count != 1) return;
            Debug.Log("[ * Debug * ] Game is end!");

            int winner = CartController.Carts.First().PlayerID;
            RPC_ShowGameResult(Runner, winner);
        }

        private static void OnPlayerDead(int playerID) {
            if (!IsHost) return;
            if (CartController.Carts.Count == 1) return;

            CartController cart = CartController.Carts.FirstOrDefault(x => x.PlayerID == playerID);
            if (cart == null) return;

            Runner.Despawn(cart.Object);
            CheckGameEnded();
        }

        private void SendKnockedOutEvent(PlayerRef player) {
            CartController cart = CartController.Carts.FirstOrDefault(x => x.Object.InputAuthority == player);
            if (cart == null) return;

            CartController.Knockedout?.Invoke(cart.PlayerID);
        }

        [Rpc]
        public static void RPC_ShowGameResult(NetworkRunner runner, int winner) {
            WinnerID = winner;
            GameEnded?.Invoke();
            ShowGameResult();
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