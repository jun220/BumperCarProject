using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCartFights.Network {
    public struct PlayerInfo : INetworkStruct {
        public int PlayerID;
        public NetworkString<_32> Nickname;
        public int CartColor;
        public int Character;
        public NetworkBool IsReady;

        public bool IsLeader { get => PlayerID == 0; }
        public bool CanReady { get => CartColor != -1 && Character != -1; }

        public PlayerInfo(int playerID, string nickname, int cartColor, int character) {
            PlayerID = playerID;
            Nickname = nickname;
            CartColor = cartColor;
            Character = character;
            
            IsReady = (playerID == 0) ? (cartColor != -1 && character != -1) : false; 
        }
    }
}
