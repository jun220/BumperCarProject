using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class RoomGeneralState : INetworkState {

        public void Start() {
            PanelUI.Instance.SetPanel(PanelUI.Panel.ROOM);
        }

        public void Abort() { }

        public void Terminate() { }

        public void Update() { }
    }
}
