using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UltimateCartFights.Network {
    public class NetworkStateMachine {
        private const INetworkState.STATE DEFAULT_STATE = INetworkState.STATE.CLOSED;

        private static readonly Dictionary<INetworkState.STATE, INetworkState> States = new() {
            { INetworkState.STATE.NONE, new NoneState() },
            { INetworkState.STATE.CLOSED, new CloseState() },
            { INetworkState.STATE.LOBBY_LOADING, new LoadingState() },
            { INetworkState.STATE.LOBBY, new LobbyState() },
            // INetworkState.STATE.ROOM_RANDOM, new NoneState() },
            { INetworkState.STATE.ROOM_GENERAL, new RoomGeneralState() },
            { INetworkState.STATE.GAME_LOADING, new GameIntroState() },
            { INetworkState.STATE.GAME, new GameState() },
            { INetworkState.STATE.RESULT, new ResultState() },
        };

        private INetworkState current = States[DEFAULT_STATE];
        public INetworkState.STATE State { get; private set; } = DEFAULT_STATE;

        private void SetState(INetworkState.STATE state) {
            State = state;
            current = States[state];
        }

        public void Start() => current.Start();
        
        public void Update() => current.Update();
        
        public async Task ChangeState(INetworkState.STATE state, Func<Task> method) {
            current.Terminate();
            SetState(INetworkState.STATE.NONE);

            await method();
            
            SetState(state);
            current.Start();
        }

        public async Task ChangeState<T> (INetworkState.STATE state, Func<T, Task> method, T param) {
            current.Terminate();
            SetState(INetworkState.STATE.NONE);

            await method(param);

            SetState(state);
            current.Start();
        }

        public async Task<TResult> ChangeState<TResult>(INetworkState.STATE state, Func<Task<TResult>> method) {
            current.Terminate();
            SetState(INetworkState.STATE.NONE);

            TResult result = await method();

            SetState(state);
            current.Start();

            return result;
        }

        public async Task<TResult> ChangeState<T, TResult>(INetworkState.STATE state, Func<T, Task<TResult>> method, T param) {
            current.Terminate();
            SetState(INetworkState.STATE.NONE);

            TResult result = await method(param);

            SetState(state);
            current.Start();

            return result;
        }

        public void ChangeState(INetworkState.STATE state) {
            current.Terminate();

            SetState(state);
            current.Start();
        }

        public void ChangeState<T>(INetworkState.STATE state, Action<T> method, T param) {
            current.Terminate();
            SetState(INetworkState.STATE.NONE);

            method(param);

            SetState(state);
            current.Start();
        }

        public async Task Abort (Func<Task> AbortMethod) {
            current.Abort();
            SetState(INetworkState.STATE.NONE);

            await AbortMethod();
            
            SetState(DEFAULT_STATE);
            current.Start();
        }
    }
}
