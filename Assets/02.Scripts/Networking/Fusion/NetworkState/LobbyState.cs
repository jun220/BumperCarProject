using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class LobbyState : INetworkState {

        public void Start() {
            PanelUI.Instance.SetLobby();
            GameLauncher.OnSessionUpdated += PanelUI.Instance.OnSessionUpdated;
        }

        public void Abort() {
            GameLauncher.OnSessionUpdated -= PanelUI.Instance.OnSessionUpdated;
        }
        
        public void Terminate() {
            GameLauncher.OnSessionUpdated -= PanelUI.Instance.OnSessionUpdated;
        }

        public void Update() { }
    }
}
