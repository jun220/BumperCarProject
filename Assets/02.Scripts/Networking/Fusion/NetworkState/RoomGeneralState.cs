using Fusion;
using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class RoomGeneralState : INetworkState {

        public void Start() {
            FusionSocket.Runner.gameObject.GetComponent<ChatClientNetwork>().Open();

            PanelUI.Instance.SetPanel(PanelUI.Panel.ROOM);
            PanelUI.Instance.InitializeRoom();

            ServerAPI.CreateClient(ClientInfo.Nickname);
        }

        public void Terminate() {
            PanelUI.Instance.LeaveRoom();
            ServerAPI.RemoveClient();
        }

        public void Abort() {
            PanelUI.Instance.LeaveRoom();
            ServerAPI.RemoveClient();
        }

        public void Update() { }
    }
}
