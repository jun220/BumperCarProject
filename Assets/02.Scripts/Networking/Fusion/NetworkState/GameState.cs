using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateCartFights.Game;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class GameState : INetworkState {

        public void Start() {
            Debug.Log("[ * Debug * ] Start Game!");

            PanelUI.Instance.SetPanel(PanelUI.Panel.GAME);
            PanelUI.Instance.InitializeGame();

            GameLauncher.AddGameEvents();
            GameLauncher.CheckGameEnded();
        }

        public void Update() {
            GameLauncher.CheckGameStarted();
        }

        public void Terminate() { 
            PanelUI.Instance.DisableGameUI();

            CartController.Carts.Clear();
            GameLauncher.RemoveGameEvents();
        }

        public void Abort() {
            CartController.Carts.Clear();
        }
    }
}
