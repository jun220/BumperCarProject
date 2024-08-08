using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class KartInput : NetworkBehaviour, INetworkRunnerCallbacks
{
    #region DEBUGING
    Text inputText;
    Text valueText;
    #endregion

    #region NETWORK INPUT DATA STRUCTURE

    public struct NetworkInputData : INetworkInput {
        public const int KEY_TYPE_SIZE = 5;

        public enum ButtonType { 
            ACCELERATION = 1 << 0, // 가속(1)
            REVERSE = 1 << 1, // 후진(2)
            LEFT_STEER = 1 << 2, // 왼쪽 회전(4)
            RIGHT_STEER = 1 << 3, // 오른쪽 회전(8)
            DASH = 1 << 4, // 대시(16)
        };

        public uint Buttons; // 현재 눌린 버튼
        public uint ButtonsDown; // 이번 프레임에서 눌린 버튼
        public uint ButtonsUp; // 이번 프레임에서 떼어진 버튼

        private int _acceleration;
        public float Acceleration {
            get => _acceleration * 0.001f; // 반환할때 1000분의 1로 반환
            set => _acceleration = (int) (value * 1000); // 저장할 때 1000배로 저장
        }

        private int _steer; // 조향 값?
        // 이것도 저장할 때는 1000배. 반환할 때는 1000분의 1
        public float Steer {
            get => _steer * 0.001f;
            set => _steer = (int) (value * 1000);
        }

        // 특정 버튼이 현재 눌려있는지 확인
        public bool GetButton(ButtonType type) => (Buttons & ((uint)type)) != 0;
        public bool GetButtonDown(ButtonType type) => (ButtonsDown & ((uint) type)) != 0;
        public bool GetButtonUp(ButtonType type) => (ButtonsUp & ((uint) type)) != 0;
    }

    #endregion

    #region NETWORK LIFECYCLE METHOD

    public override void Spawned() {
        base.Spawned();

        Runner.AddCallbacks(this);

        front = front.Clone();
        back = back.Clone();
        left = left.Clone();
        right = right.Clone();
        dash = dash.Clone();
        accelerate = accelerate.Clone();
        steer = steer.Clone();

        front.Enable();
        back.Enable();
        left.Enable();
        right.Enable();
        dash.Enable();
        accelerate.Enable();
        steer.Enable();
    }

    public override void Despawned(NetworkRunner runner, bool hasState) {
        base.Despawned(runner, hasState);

        Runner.RemoveCallbacks(this);
        DisposeInputs();
    }

    private void OnDestroy() {
        DisposeInputs();
    }

    #endregion

    #region KART INPUT METHOD 

    [SerializeField] private InputAction front; // W키 눌렀을 때
    [SerializeField] private InputAction back; // A키 눌렀을 때
    [SerializeField] private InputAction left; // S키 눌렀을 때
    [SerializeField] private InputAction right; // D키 눌렀을 때
    [SerializeField] private InputAction dash; // 대시 키(Q) 눌렀을 때

    [SerializeField] private InputAction accelerate; // 가속?
    [SerializeField] private InputAction steer; // 조종?
    
    private Gamepad gamepad;
    private uint before;


    private void Start()
    {
        inputText = GameObject.Find("InputText").GetComponent<Text>();
        valueText = GameObject.Find("ValueText").GetComponent<Text>();  
    }


    public void OnInput(NetworkRunner runner, NetworkInput input) {
        // 게임패드 등록
        gamepad = Gamepad.current;

        // 네트워크 상태를 생성?
        NetworkInputData current = new NetworkInputData();

        current.Buttons = 0;

        // 각 버튼의 입력 상태를 ReadBool을 통해 설정
        if (ReadBool(front)) current.Buttons |= (uint) NetworkInputData.ButtonType.ACCELERATION;
        if (ReadBool(back)) current.Buttons |= (uint) NetworkInputData.ButtonType.REVERSE;
        if (ReadBool(left)) current.Buttons |= (uint) NetworkInputData.ButtonType.LEFT_STEER;
        if (ReadBool(right)) current.Buttons |= (uint) NetworkInputData.ButtonType.RIGHT_STEER;
        if (ReadBool(dash)) current.Buttons |= (uint) NetworkInputData.ButtonType.DASH;

        // 테스트 출력
        //Debug.Log(Convert.ToString(current.Buttons, 2));

        // 가속 값과 조향 값을 가져옴
        current.Acceleration = ReadFloat(accelerate);
        current.Steer = ReadFloat(steer);

        // 이번 프레임에서 뗀 버튼과 누른 버튼을 체크
        current.ButtonsDown = Diff(current.Buttons, before);
        current.ButtonsUp = Diff(before, current.Buttons);

        input.Set(current);
        before = current.Buttons;
        if(inputText == null)
        {
            inputText = GameObject.Find("InputText").GetComponent<Text>();
        }
        else
        {
            inputText.text = $"Input status: {current.Buttons}";
        }

        if (valueText == null)
        {
            valueText = GameObject.Find("ValueText").GetComponent<Text>();
        }
        else
        {
            valueText.text = $"acceleation: {current.Acceleration}\nsteer: {current.Steer}";
        }

        //Debug.Log(string.Format("[ * Debug * ] Player [ {0} ] Controlled! (Key : {1})", runner, current.Buttons));
    }

    private void DisposeInputs() {
        front.Disable();
        back.Disable();
        left.Disable();
        right.Disable();
        dash.Dispose();
        accelerate.Dispose(); 
        steer.Dispose(); 
    }

    private static bool ReadBool(InputAction action) => action.ReadValue<float>() != 0;
    private static float ReadFloat(InputAction action) => action.ReadValue<float>();

    /// <summary>
    /// origin에서 1, compare에서 0이었던 위치의 비트만 1로 켜서 반환한다
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="compare"></param>
    /// <returns></returns>
    private static uint Diff(uint origin, uint compare) => (origin ^ compare) & origin;

    #endregion

    #region INETWORKRUNNERCALLBACK METHOD

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }

    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }

    #endregion
}
