using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class GameLoadingState : INetworkState {

        private const float TIP_CHANGE_COOL = 3.0f;
        private float timer = 0.0f;

        public void Start() {
            PanelUI.Instance.SetPanel(PanelUI.Panel.LOADING);
            PanelUI.Instance.InitializeLoading();
        }

        public void Update() { 
            timer += Time.deltaTime;

            if(timer >= TIP_CHANGE_COOL) {
                PanelUI.Instance.ShowRandomTip();
                timer = 0.0f;
            }
        }

        public void Abort() { }

        public void Terminate() { }

        
    }
}
