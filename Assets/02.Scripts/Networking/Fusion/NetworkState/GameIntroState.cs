using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class GameIntroState : INetworkState {

        private const float TIP_CHANGE_COOL = 5.0f;
        private float timer = 0.0f;

        public void Start() {
            PanelUI.Instance.SetPanel(PanelUI.Panel.LOADING);
            PanelUI.Instance.InitializeLoading();
        }

        public void Update() { 
            timer += Time.deltaTime;

            if(timer >= TIP_CHANGE_COOL) {
                PanelUI.Instance.ShowRandomTip();
                timer -= TIP_CHANGE_COOL;
            }
        }

        public void Abort() { }

        public void Terminate() { }

        
    }
}
