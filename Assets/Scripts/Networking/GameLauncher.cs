using Fusion;
using Fusion.Addons.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameLauncher : FusionSocket
{
    #region UNITY BASIC METHOD

    public static GameLauncher Instance { get; private set; } = null;

    private void Start() {
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 1;

        if(Instance == null)
            Instance = this;
    }

    #endregion

    #region FUSION NETWORK METHOD

    public async Task Entrance() {
        base.Open();
        await base.JoinLobby();
    }

    public async new Task CreateRoom(RoomInfo room) {
        await base.CreateRoom(room);
    }

    public async Task MatchRandom() {
        await base.JoinRandomRoom();
    }

    public async new Task JoinRoom(RoomInfo room) {
        await base.JoinRoom(room);
    }

    public async Task OnClose() {
        await base.Close();
    }

    public async Task Reconnect() {
        await base.Close();
        base.Open();
        await base.JoinLobby();
    }

    #endregion

    #region FUSION OVERRIDE PROCESSING

    protected override void AfterOpen() {
        RunnerSimulatePhysics3D sim3D = Runner.gameObject.AddComponent<RunnerSimulatePhysics3D>();
        sim3D.ClientPhysicsSimulation = ClientPhysicsSimulation.SimulateAlways;
    }

    #endregion

    #region FUSION CALLBACK METHOD

    public static List<SessionInfo> SessionList { get; private set; } = new List<SessionInfo>();

    /// <summary>
    /// 현재 접속한 로비 내 세션 목록이 업데이트 될 때 이벤트가 발생한다.
    /// </summary>
    private static Action SessionListUpdatedEvent;
    public static void AddSessionListUpdatedEventListener(Action method) => SessionListUpdatedEvent += method;

    /// <summary>
    /// 현재 접속한 로비 내 세션 목력이 업데이트될 때 갱신된 목록을 불러온다.
    /// <para>
    /// 로비 접속 시에도 1회 호출된다.
    /// </para>
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="sessionList">접속한 로비 내 세션 목록</param>
    public override void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        Debug.Log(string.Format("[ * Debug * ] SessionList Updated (Size : {0})", sessionList.Count));

        // 랜덤방이 아닌 목록들만 복사한다
        SessionList = new List<SessionInfo>();

        foreach(SessionInfo session in sessionList) {
            if (session.Properties["IsRandom"]) continue;
            SessionList.Add(session);
        }

        SessionListUpdatedEvent?.Invoke();
    }

    private static Action<PlayerRef> PlayerJoinedEvent;
    public static void AddPlayerJoinedEventListener(Action<PlayerRef> method) => PlayerJoinedEvent += method;

    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        Debug.Log(string.Format("[ * Debug * ] Room : {0} / Player : {1}", runner.SessionInfo.Name, player.PlayerId));

        PlayerJoinedEvent?.Invoke(player);
    }
    
    #endregion
}