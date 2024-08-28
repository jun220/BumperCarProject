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

            ClientPlayer.Players.Clear();
        }

        public void Abort() {
            PanelUI.Instance.DisableLoading();
        }

        public void Terminate() { 
            PanelUI.Instance.DisableLoading();
        }

        public void Update() { }
    }
}
