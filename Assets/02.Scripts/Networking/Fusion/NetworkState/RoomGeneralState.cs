using System.Diagnostics;
using UltimateCartFights.UI;
using UltimateCartFights.Utility;

namespace UltimateCartFights.Network {
    public class RoomGeneralState : INetworkState {

        private ChatClientNetwork Chat;


        public void Start() {
            PanelUI.Instance.SetPanel(PanelUI.Panel.ROOM);
            PanelUI.Instance.InitializeRoom();
            
            Chat = FusionSocket.Runner.GetComponent<ChatClientNetwork>();
            if (Chat != null) Chat.Open(FusionSocket.SessionInfo.Name);
        }

        public void Terminate() {
            PanelUI.Instance.LeaveRoom();

            if (Chat != null) Chat.Close();
            Chat = null;
        }

        public void Abort() {
            PanelUI.Instance.LeaveRoom();

            if (Chat != null) Chat.Close();
            Chat = null;
        }

        public void Update() { }
    }
}
