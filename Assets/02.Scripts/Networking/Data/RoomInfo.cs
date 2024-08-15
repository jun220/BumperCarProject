using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class RoomInfo {
        public const int MAX_PLAYER = 6;

        public string RoomID { get; private set; }
        public string RoomName { get; private set; }
        public int MaxPlayer { get; private set; }
        public string HostNickname { get; private set; }
        public NetworkBool IsRandom { get; private set; }
    
        public RoomInfo(string roomID, string roomName, int maxPlayer, string hostNickname, bool isRandom) {
            this.RoomID = roomID;
            this.RoomName = roomName;
            this.MaxPlayer = maxPlayer;
            this.HostNickname = hostNickname;
            this.IsRandom = isRandom;
        }

        public RoomInfo(string roomName, int maxPlayer, string hostNickname) {
            this.RoomID = string.Empty;
            this.RoomName = roomName;
            this.MaxPlayer = maxPlayer;
            this.HostNickname = hostNickname;
            this.IsRandom = false;
        }

        public RoomInfo() {
            this.RoomID = string.Empty;
            this.RoomName = string.Empty;
            this.MaxPlayer = MAX_PLAYER;
            this.HostNickname = string.Empty;
            this.IsRandom = true;
        }
    }
}
