using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltimateCartFights.Network {
    public class GameLauncher : FusionSocket {

        #region UNITY BASIC METHOD

        private void Start() {
            Application.runInBackground = true;
            QualitySettings.vSyncCount = 1;

            DontDestroyOnLoad(this.gameObject);

            SceneManager.LoadScene("Lobby");
        }

        private void Update() {
            FusionSocket.UpdateState();
        }

        #endregion

        #region LOBBY EVENT METHOD

        public static List<SessionInfo> Sessions { get; private set; } = new List<SessionInfo>();

        public override void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
            base.OnSessionListUpdated(runner, sessionList);

            Sessions = sessionList;
        }

        #endregion
    }
}