using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class LoadingState : INetworkState {

        public void Start() {
            // 3주차 내용
            PanelUI.Instance.SetPanel(PanelUI.Panel.LOBBY);
            PanelUI.Instance.SetLoading();
            FusionSocket.JoinLobby();

            // 이후 추가
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
