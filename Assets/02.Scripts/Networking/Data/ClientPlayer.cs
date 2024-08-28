using Fusion;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UltimateCartFights.Game;
using UltimateCartFights.UI;
using UltimateCartFights.Utility;
using UnityEngine;
using WebSocketSharp;

namespace UltimateCartFights.Network {
    public class ClientPlayer : NetworkBehaviour {

        #region Player State

        public readonly static List<ClientPlayer> Players = new List<ClientPlayer>();

        public static ClientPlayer Local { get; private set; } = null;

        public static bool CanStartGame {
            get {
                //if (Players.Count == 1) return false;

                foreach(ClientPlayer player in Players) {
                    if (!player.IsReady) return false;
                }
                
                return true;
            }
        }

        public bool CanReady {
            get {
                if (CartColor == -1) return false;
                if (Character == -1) return false;
                return true;
            }
        }

        public bool IsLocal { get => Local != null && Local.PlayerID == PlayerID; }

        public bool IsLeader { get => PlayerID == 0; }

        #endregion

        #region Networked Properties

        public static Action PlayerUpdated;

        [Networked] public int PlayerID { get; private set; } = -1;
        [Networked] public NetworkString<_32> Nickname { get; private set; } = string.Empty;
        [Networked] public int CartColor { get; private set; } = -1;
        [Networked] public int Character { get; private set; } = -1;
        [Networked] public NetworkBool IsReady { get; private set; } = false;

        #endregion

        #region Player Lifecycle Method

        private ChangeDetector _changeDetector;

        public override void Spawned() {
            base.Spawned();

            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (Object.HasInputAuthority) {
                Local = this;
                RPC_SetPlayerStats(ClientInfo.Nickname);
            }

            Players.Add(this);
            PlayerUpdated?.Invoke();

            DontDestroyOnLoad(gameObject);

            Debug.Log(string.Format("[ * Debug * ] User {0} is Spawned!", Object.InputAuthority.PlayerId));
        }

        public override void Render() {
            base.Render();

            foreach(string change in _changeDetector.DetectChanges(this)) {
                switch(change) {
                    case nameof(Nickname):
                    case nameof(CartColor):
                    case nameof(Character):
                    case nameof(IsReady):
                        PlayerUpdated?.Invoke();
                        break;
                }
            }
        }

        private void OnDisable() {
            if (Local == this)
                Local = null;

            Players.Remove(this);
            PlayerUpdated?.Invoke();
        }

        #endregion

        #region Client RPC Method

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void RPC_SetPlayerStats(NetworkString<_32> nickname) {
            PlayerID = GetEmptyID();
            Nickname = (string) nickname;
            CartColor = GetEmptyColor();
            Character = GetEmptyCharacter();
            IsReady = IsLeader ? CanReady : false;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void RPC_SetCartColor(int color) {
            if (IsUsedColor(color)) return;
            CartColor = color;

            if (IsLeader) IsReady = CanReady;
            else IsReady = false;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void RPC_SetCharacter(int character) {
            if (IsUsedCharacter(character)) return;
            Character = character;

            if (IsLeader) IsReady = CanReady;
            else IsReady = false;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void RPC_SetReadyState(NetworkBool state) {
            if (IsLeader) return;
            if (!CanReady && state) return;

            IsReady = state;
        }

        #endregion

        #region Others

        public static void RemovePlayer(NetworkRunner runner, PlayerRef player) {
            ClientPlayer client = Players.FirstOrDefault(x => x.Object.InputAuthority == player);
            if (client == null) return;

            Players.Remove(client);
            runner.Despawn(client.Object);
        }

        private int GetEmptyID() {
            bool[] isUsed = new bool[RoomInfo.MAX_PLAYER];

            foreach (ClientPlayer player in Players) {
                if (player.PlayerID != -1)
                    isUsed[player.PlayerID] = true;
            }

            for (int i = 0; i < isUsed.Length; i++) {
                if (!isUsed[i])
                    return i;
            }

            return -1;
        }

        private int GetEmptyColor() {
            bool[] isUsed = new bool[RoomInfo.MAX_PLAYER];

            foreach(ClientPlayer player in Players) {
                if(player.CartColor != -1)
                    isUsed[player.CartColor] = true;
            }

            for(int i = 0; i < isUsed.Length; i++) {
                if (!isUsed[i])
                    return i;
            }

            return -1;
        }

        private int GetEmptyCharacter() {
            bool[] isUsed = new bool[RoomInfo.MAX_PLAYER];

            foreach (ClientPlayer player in Players) {
                if (player.Character != -1)
                    isUsed[player.Character] = true;
            }

            for (int i = 0; i < isUsed.Length; i++) {
                if (!isUsed[i])
                    return i;
            }

            return -1;
        }

        private bool IsUsedColor(int color) {
            if (color == -1) return false;

            foreach(ClientPlayer player in Players) {
                if(color == player.CartColor) 
                    return true;
            }

            return false;
        }

        private bool IsUsedCharacter(int character) {
            if (character == -1) return false;

            foreach (ClientPlayer player in Players) {
                if (character == player.Character)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
