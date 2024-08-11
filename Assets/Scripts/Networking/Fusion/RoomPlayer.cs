using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomPlayer : NetworkBehaviour
{
    public static readonly List<RoomPlayer> Players = new List<RoomPlayer>();

    public static Action<RoomPlayer> PlayerJoined;
    public static Action<RoomPlayer> PlayerLeft;
    public static Action<RoomPlayer> PlayerChanged;

    public static RoomPlayer Local { get; private set; }

    public bool IsMine { get { return Local == this; } }
    [Networked] public NetworkString<_32> Nickname { get; private set; }
    [Networked] public NetworkBool IsHost { get; private set; }
    [Networked] public NetworkBool IsReady { get; private set; }
    [Networked] public int KartType { get; private set; }
    [Networked] public int KartColor { get; private set; }

    private ChangeDetector changeDetector;

    #region FUSION LIFECYCLE METHOD

    public override void Spawned() {
        base.Spawned();
        
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if(Object.HasInputAuthority) {
            Local = this;

            PlayerChanged?.Invoke(this);
            RPC_SetPlayer(ClientInfo.Nickname, FusionSocket.State == FusionSocket.NetworkState.HOST);
        }

        Players.Add(this);
        PlayerJoined?.Invoke(this);

        DontDestroyOnLoad(gameObject);
    }

    public override void Render() {
        foreach(string change in changeDetector.DetectChanges(this)) {
            switch(change) {
                case nameof(Nickname):
                case nameof(IsHost):
                case nameof(IsReady):
                case nameof(KartType):
                case nameof(KartColor):
                    OnStateChanged(this);
                    break;
            }
        }
    }

    private void OnDisable() {
        PlayerLeft?.Invoke(this);
        Players.Remove(this);
    }

    #endregion

    #region PLAYER EVENT METHOD

    private static void OnStateChanged(RoomPlayer changed) => PlayerChanged?.Invoke(changed);

    public static void RemovePlayer(NetworkRunner runner, PlayerRef player) {
        RoomPlayer roomPlayer = Players.FirstOrDefault(x => x.Object.InputAuthority == player);
        if (roomPlayer == null) return;

        Players.Remove(roomPlayer);
        runner.Despawn(roomPlayer.Object);
    }

    #endregion

    #region RPC METHOD

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_SetPlayer(NetworkString<_32> nickname, NetworkBool isHost) {
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
