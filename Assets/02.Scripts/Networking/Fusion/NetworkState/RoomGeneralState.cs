using System.Diagnostics;
using UltimateCartFights.UI;
using UltimateCartFights.Utility;

namespace UltimateCartFights.Network {
    public class RoomGeneralState : INetworkState {

        public void Start() {
            PanelUI.Instance.SetPanel(PanelUI.Panel.ROOM);
            PanelUI.Instance.InitializeRoom();
            
            ChatClientNetwork Chat = FusionSocket.Runner.GetComponent<ChatClientNetwork>();
            Chat.Open(FusionSocket.SessionInfo.Name);
        }

        public void Terminate() {
            PanelUI.Instance.LeaveRoom();
        }

        public void Abort() {
            PanelUI.Instance.LeaveRoom();
        }

        public void Update() { }
    }
}
