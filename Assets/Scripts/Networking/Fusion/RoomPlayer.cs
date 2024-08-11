using Fusion;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomPlayer : NetworkBehaviour
{
    public const int MAX_PLAYER = 6;

    public static readonly RoomPlayer[] Players = new RoomPlayer[MAX_PLAYER];

    public static Action<RoomPlayer> PlayerJoined;
    public static Action<int> PlayerLeft;
    public static Action<RoomPlayer> PlayerChanged;

    public static RoomPlayer Local { get; private set; }
    public bool IsMine { get { return Local == this; } }
    [Networked] public int PlayerID { get; private set; }
    [Networked] public NetworkString<_32> Nickname { get; private set; }
    [Networked] public NetworkBool IsHost { get; private set; }
    [Networked] public NetworkBool IsReady { get; private set; }
    [Networked] public int KartType { get; private set; }
    [Networked] public int KartColor { get; private set; }

    private ChangeDetector changeDetector;

    #region FUSION LIFECYCLE METHOD

    public override void Spawned() {
        base.Spawned();

        Debug.Log(string.Format("Player ID : {0} / Nickname : {1}", PlayerID, Nickname));
        
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if(FusionSocket.State == FusionSocket.NetworkState.HOST) {
            int id = 0;
            while (Players[id] != null) {
                if (id == FusionSocket.Runner.SessionInfo.MaxPlayers - 1) break;
                id++;
            }
            
            RPC_SetPlayerID(id);
        }

        if(Object.HasInputAuthority) {
            Local = this;

            PlayerChanged?.Invoke(this);
            RPC_SetPlayer(ClientInfo.Nickname, FusionSocket.State == FusionSocket.NetworkState.HOST);
        } else if(FusionSocket.State == FusionSocket.NetworkState.CLIENT) {
            Players[PlayerID] = this;
            PlayerJoined?.Invoke(this);
        }

        DontDestroyOnLoad(gameObject);
    }

    public override void Render() {
        foreach(string change in changeDetector.DetectChanges(this)) {
            switch(change) {
                case nameof(Nickname):
                case nameof(IsReady):
                case nameof(KartType):
                case nameof(KartColor):
                    OnStateChanged(this);
                    break;
            }
        }
    }

    private void OnDisable() {
        int playerID = Array.IndexOf(Players, this);
        if (playerID == -1) return;

        Players[playerID] = null;
        PlayerLeft?.Invoke(playerID);
    }

    #endregion

    #region PLAYER EVENT METHOD

    private static void OnStateChanged(RoomPlayer changed) => PlayerChanged?.Invoke(changed);

    public static void RemovePlayer(NetworkRunner runner, PlayerRef player) {
        int playerID = Array.IndexOf(Players, player);
        if (playerID == -1) return;

        RoomPlayer roomPlayer = Players[playerID];
        Players[playerID] = null;
        runner.Despawn(roomPlayer.Object);
    }

    #endregion

    #region RPC METHOD

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RPC_SetPlayerID(int id) {
        PlayerID = id;
        Players[id] = this;

        PlayerJoined?.Invoke(this);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_SetPlayer( NetworkString<_32> nickname, NetworkBool isHost) {
        Nickname = nickname;
        IsReady = isHost;
        IsHost = isHost;
        KartType = KartTypeUI.KART_TYPE_EMPTY;
        KartColor = KartColorUI.KART_COLOR_EMPTY;
    } 

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_ChangeReadyState(NetworkBool state) => IsReady = state;

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetKartType(int type) {
        if (!IsHost && type == KartTypeUI.KART_TYPE_EMPTY)
            IsReady = false;

        KartType = type;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetKartColor(int color) {
        if(!IsHost && color == KartColorUI.KART_COLOR_EMPTY)
            IsReady = false;

        KartColor = color;
    }

    #endregion
}
