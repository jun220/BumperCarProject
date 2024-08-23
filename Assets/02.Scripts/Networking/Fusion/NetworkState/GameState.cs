using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class GameState : INetworkState {

        public void Start() {
            PanelUI.Instance.SetPanel(PanelUI.Panel.GAME);
        }

        public void Update() { }

        public void Terminate() { 
            // Scene으로 돌아가기 전 UI들을 정리하고 오브젝트들을 초기화해놓는다
            // 이후 RoomGeneralState로 돌아간다
            // (랜덤 방의 경우 다시 로비 로딩 화면으로 돌아간다)
        }

        public void Abort() { 
            // 도중에 연결이 끊긴 경우 UI 후처리 및 기타 오브젝트를 정리한다
            // 이후 다시 로비 로딩 화면으로 돌아간다
        }

    }
}
