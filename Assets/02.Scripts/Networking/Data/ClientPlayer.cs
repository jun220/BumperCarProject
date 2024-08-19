using Fusion;
using Photon.Realtime;
using System;
using System.Linq;
using UnityEngine;
using WebSocketSharp;

namespace UltimateCartFights.Network {
    public class ClientPlayer : NetworkBehaviour {

        #region Player State Management

        private readonly static ClientPlayer[] Players = new ClientPlayer[RoomInfo.MAX_PLAYER];

        public static ClientPlayer Local { get; private set; } = null;

        public static int LocalID { get => Local._playerID; }

        public static Action<int> PlayerJoined;
        public static Action<int> PlayerLeft;
        public static Action<int> PlayerChanged;

        private ChangeDetector changer;

        public static ClientPlayer GetPlayer(int playerID) => Players[playerID];
        private static void RemovePlayer(int playerID) => Players[playerID] = null;

        #endregion

        #region Player State Field

        public bool CanReady { get => (CartColor != -1 && Character != -1); }
        public bool IsLeader { get => PlayerID == 0; }

        private int _playerID = -1;

        [Networked] public int PlayerID { get; private set; } = -1;
        [Networked] public string Nickname { get; private set; } = string.Empty;
        [Networked] public int CartColor { get; private set; } = -1;
        [Networked] public int Character { get; private set; } = -1;
        [Networked] public NetworkBool IsReady { get; private set; } = false;

        #endregion

        #region Player Lifecycle Method

        public override void Spawned() {
            base.Spawned();

            changer = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if(Object.HasInputAuthority) {
                Local = this;
            }

            if(IsInitialized()) {
                _playerID = PlayerID;
                Players[PlayerID] = this;
                PlayerJoined?.Invoke(PlayerID);
            }
        }

        public override void Render() {
            base.Render();

            foreach (var change in changer.DetectChanges(this)) {
                switch (change) {
                    case nameof(CartColor):
                    case nameof(Character):
                    case nameof(IsReady):
                        PlayerChanged?.Invoke(PlayerID);
                        Debug.Log(string.Format("[ * Debug * ] Player ID : {0} / Nickname : {1} / Cart Color : {2} / Character : {3} / IsReady : {4}", PlayerID, Nickname, CartColor, Character, IsReady));
                        break;
                }
            }
        }

        private bool IsInitialized() {
            if (Nickname.IsNullOrEmpty()) return false;
            if (PlayerID == -1) return false;
            return true;
        }

        public void RemoveClient() {
            RemovePlayer(_playerID);
            PlayerLeft?.Invoke(_playerID);
        }

        #endregion

        #region Client RPC Method

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPC_Initialized(string nickcname, int playerID, int cartColor, int character) {
            Debug.Log(string.Format("[ * Debug * ] Initialize Player - Player ID : {0} / Nickname : {1} / Color : {2} / Character : {3}", playerID, nickcname, cartColor, character));

            PlayerID = playerID;
            Nickname = nickcname;
            CartColor = cartColor;
            Character = character;
            IsReady = FusionSocket.IsHost;

            _playerID = playerID;
            Players[playerID] = this;
            PlayerJoined?.Invoke(PlayerID);
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
        public void RPC_SetCartColor(int cartColor) {
            if (cartColor == -1) IsReady = false;
            CartColor = cartColor;

            if (FusionSocket.IsHost) IsReady = CanReady;
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
        public void RPC_SetCharacter(int character) {
            if (character == -1) IsReady = false;
            Character = character;

            if (FusionSocket.IsHost) IsReady = CanReady;
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
        public void RPC_SetReady(NetworkBool isReady) => IsReady = isReady;

        #endregion
    }
}
