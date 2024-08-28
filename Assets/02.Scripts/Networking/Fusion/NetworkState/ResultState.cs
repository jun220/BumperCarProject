using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateCartFights.Game;
using UltimateCartFights.UI;
using UltimateCartFights.Utility;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class ResultState : INetworkState {

        public void Start() {
            ClientPlayer client = ClientPlayer.Players.FirstOrDefault(x => x.PlayerID == GameLauncher.WinnerID);

            PanelUI.Instance.SetPanel(PanelUI.Panel.RESULT);
            PanelUI.Instance.InitializeResult((string) client.Nickname);

            CartController cart = CartController.Carts.FirstOrDefault(x => x.PlayerID == GameLauncher.WinnerID);
            CartCamera.SetTarget(cart);

            WaitAndReturn();
        }

        public void Abort() { }

        public void Terminate() { }

        public void Update() { }

        private async void WaitAndReturn() {
            await Task.Delay(5000);

            if (ClientInfo.IsInRandom)
                FusionSocket.ReturnLobby();
            else if(FusionSocket.IsHost)
                FusionSocket.LoadRoomScene();
        }
    }
}
