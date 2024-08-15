using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class LoadingState : INetworkState {

        public void Start() {
            PanelUI.Instance.SetPanel(PanelUI.Panel.LOBBY);
            PanelUI.Instance.SetLoading();
            FusionSocket.JoinLobby();
        }

        public void Abort() { }

        public void Terminate() { }

        public void Update() { }
    }
}
