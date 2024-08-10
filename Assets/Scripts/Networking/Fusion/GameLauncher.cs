using Fusion;
using Fusion.Addons.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLauncher : FusionSocket
{
    #region UNITY BASIC METHOD

    public static GameLauncher Instance { get; private set; } = null;
    public static GameManager Manager { get; private set; } = null;

    private void Start() {
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 1;

        if(Instance == null)
            Instance = this;

        DontDestroyOnLoad(Instance);

        SceneManager.LoadScene("Lobby");
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

    #region FUSION POSTPROCESS METHOD

    protected override void AfterCreateRoom(RoomInfo room) {
        base.AfterCreateRoom(room);

        ChatClientNetwork chat = Runner.gameObject.GetComponent<ChatClientNetwork>();
        chat.Open();
    }

    protected override void AfterJoinRoom(RoomInfo room)  {
        base.AfterJoinRoom(room);

        ChatClientNetwork chat = Runner.gameObject.GetComponent<ChatClientNetwork>();
        chat.Open();
    }

    #endregion

    #region FUSION CALLBACK METHOD

    /// <summary>
    /// 현재 접속한 로비 내 세션 목력이 업데이트될 때 갱신된 목록을 불러온다.
    /// <para>
    /// 로비 접속 시에도 1회 호출된다.
    /// </para>
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="sessionList">접속한 로비 내 세션 목록</param>
    public override void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        base.OnSessionListUpdated(runner, sessionList);
    }

    [SerializeField] private GameObject GameManager;
    [SerializeField] private GameObject RoomPlayerPrefab;

    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        Debug.Log(string.Format("[ * Debug * ] Joined Room : {0} / Player : {1}", runner.SessionInfo.Name, player.PlayerId));

        if(runner.IsServer) {
            if (State == NetworkState.HOST)
                Manager = runner.Spawn(GameManager, Vector3.zero, Quaternion.identity).GetComponent<GameManager>();
            runner.Spawn(RoomPlayerPrefab, Vector3.zero, Quaternion.identity, player);
        }

        base.OnPlayerJoined(runner, player);
    }

    public override void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        Debug.Log(string.Format("[ * Debug * ] Left Room : {0} / Player : {1}", runner.SessionInfo.Name, player.PlayerId));
        base.OnPlayerLeft(runner, player);

        RoomPlayer.RemovePlayer(runner, player);
    }

    #endregion
}