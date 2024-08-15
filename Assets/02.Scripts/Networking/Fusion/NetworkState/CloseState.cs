using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class CloseState : INetworkState {
        public void Start() {
            PanelUI.Instance.SetPanel(PanelUI.Panel.INTRO);
        }

        public void Update() { }

        public void Terminate() { }

        public void Abort() { }
    }
}
