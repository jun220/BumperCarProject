using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class LobbyState : INetworkState {

        public void Start() {
            PanelUI.Instance.SetLobby();
        }

        public void Abort() { }
        
        public void Terminate() { }

        public void Update() { }
    }
}
