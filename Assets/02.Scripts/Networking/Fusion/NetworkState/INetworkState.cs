using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCartFights.Network {
    public interface INetworkState {

        public enum STATE { 
            NONE, 
            CLOSED, 
            LOBBY_LOADING, 
            LOBBY, 
            ROOM_RANDOM, 
            ROOM_GENERAL, 
            GAME_LOADING, 
            GAME, 
            RESULT,
        };

        public void Start();
        public void Update();
        public void Abort();
        public void Terminate();
    }
}
