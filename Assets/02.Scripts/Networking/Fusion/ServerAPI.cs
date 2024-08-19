using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateCartFights.Utility;
using System;

namespace UltimateCartFights.Network {
    public class ServerAPI : NetworkBehaviour {

        #region UNITY Lifecycle Method

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Room Information Field

        private static bool[] IsUsedID = new bool[RoomInfo.MAX_PLAYER];

        private static bool[] IsUsedColor = new bool[RoomInfo.MAX_PLAYER];

        private static bool[] IsUsedCharacter = new bool[RoomInfo.MAX_PLAYER];

        #endregion

        #region Server RPC Process Method

        [Rpc(HostMode = RpcHostMode.SourceIsHostPlayer)]
        private static void RPC_CreateClient(NetworkRunner runner, string nickname, RpcInfo info = default) {
            if (!FusionSocket.IsHost) return;

            int playerID = GetEmptyIndex(IsUsedID);
            int cartColor = GetEmptyIndex(IsUsedColor);
            int character = GetEmptyIndex(IsUsedCharacter);

            ClientPlayer client = FusionSocket.Spawn(ResourceManager.Instance.Client, Vector3.zero, Quaternion.identity, info.Source)
                                              .GetComponent<ClientPlayer>();

            SetValue(IsUsedID, playerID, true);
            SetValue(IsUsedColor, cartColor, true);
            SetValue(IsUsedCharacter, character, true);
            client.RPC_Initialized(nickname, playerID, cartColor, character);
        }

        [Rpc]
        private static void RPC_ChangeColor(NetworkRunner runner, int playerID, int cartColor) {
            Debug.Log(string.Format("[ * Debug * ] RPC_ChangeColor Called! - Player ID : {0} / Cart Color : {1}", playerID, cartColor));

            if (!FusionSocket.IsHost) return;
            if (cartColor != -1 && IsUsedColor[cartColor]) return;

            ClientPlayer client = ClientPlayer.GetPlayer(playerID);

            SetValue(IsUsedColor, client.CartColor, false);
            SetValue(IsUsedColor, cartColor, true);

            client.RPC_SetCartColor(cartColor);

        }

        [Rpc]
        private static void RPC_ChangeCharacter(NetworkRunner runner, int playerID, int character) {
            if (!FusionSocket.IsHost) return;
            if (character != -1 && IsUsedCharacter[character]) return;

            ClientPlayer client = ClientPlayer.GetPlayer(playerID);

            SetValue(IsUsedCharacter, client.Character, false);
            SetValue(IsUsedCharacter, character, true);

            client.RPC_SetCharacter(character);
        }

        [Rpc]
        private static void RPC_ChangeReady(NetworkRunner runner, int playerID, NetworkBool isReady) {
            if (!FusionSocket.IsHost) return;

            ClientPlayer client = ClientPlayer.GetPlayer(playerID);
            if (isReady && !client.CanReady) return;

            client.RPC_SetReady(isReady);
        }

        [Rpc]
        private static void RPC_RemoveClient(NetworkRunner runner, int playerID) {
            ClientPlayer client = ClientPlayer.GetPlayer(playerID);
            if (client == null) return;

            FusionSocket.Despawn(client.Object);
        }

        #endregion

        #region Client Request Method

        public static void CreateClient(string nickname) => RPC_CreateClient(FusionSocket.Runner, nickname);
        public static void ChangeCartColor(int cartColor) => RPC_ChangeColor(FusionSocket.Runner, ClientPlayer.LocalID, cartColor);
        public static void ChangeCharacter(int character) => RPC_ChangeCharacter(FusionSocket.Runner, ClientPlayer.LocalID, character);
        public static void ChangeReady(bool isReady) => RPC_ChangeReady(FusionSocket.Runner, ClientPlayer.LocalID, isReady);
        public static void RemoveClient() => RPC_RemoveClient(FusionSocket.Runner, ClientPlayer.LocalID);

        private static int GetEmptyIndex(bool[] isUsed) {
            for (int i = 0; i < FusionSocket.SessionInfo.MaxPlayers; i++) {
                if (!isUsed[i])
                    return i;
            }

            return -1;
        }

        private static void SetValue(bool[] isUsed, int index, bool value) {
            if (index == -1) return;
            isUsed[index] = value;
        }

        private static void ShowList(bool[] isUsed) {
            string log = "[ * Debug * ] Show List - ";

            for(int i = 0; i < RoomInfo.MAX_PLAYER; i++) {
                log += string.Format("[ {0} ] {1} / ", i, isUsed[i]);
            }

            Debug.Log(log);
        }

        #endregion

    }
}
