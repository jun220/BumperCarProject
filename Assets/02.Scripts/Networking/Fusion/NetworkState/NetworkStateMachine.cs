using System.Collections.Generic;
using System.Linq.Expressions;

namespace UltimateCartFights.Network {
    public class NetworkStateMachine {
        private const INetworkState.STATE DEFAULT_STATE = INetworkState.STATE.CLOSED;
        private static Dictionary<INetworkState.STATE, INetworkState> States = new Dictionary<INetworkState.STATE, INetworkState>{
            { INetworkState.STATE.CLOSED, new CloseState() },
            { INetworkState.STATE.LOADING, new LoadingState() },
            { INetworkState.STATE.LOBBY, new LobbyState() },
            { INetworkState.STATE.ROOM_RANDOM, new RoomRandomState() },
            { INetworkState.STATE.ROOM_GENERAL, new RoomGeneralState() },
            { INetworkState.STATE.GAME, new GameState() }
        };

        private INetworkState current = States[DEFAULT_STATE];

        public INetworkState.STATE State { get; private set; } = DEFAULT_STATE;

        /// <summary>
        /// 현 상태의 Update()를 수행한다
        /// </summary>
        public void Update() => current.Update();

        /// <summary>
        /// 현 상태에서 Terminate()를 수행한 후, 상태를 state로 변경한다
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(INetworkState.STATE state) {
            current.Terminate();

            State = state;
            current = States[state];

            current.Start();
        }

        /// <summary>
        /// 현 상태에서 Abort()를 수행한 후, 상태를 Default로 변경한다
        /// </summary>
        public void Abort() {
            current.Abort();

            State = DEFAULT_STATE;
            current = States[DEFAULT_STATE];

            current.Start();
        }
    }
}
