using Fusion;
using UnityEngine;

public class TempNetworkConnection : FusionSocket
{
    public GameObject KartPrefab;
    public string RoomName;

    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 1;

        JoinRoom();
    }

    private async void JoinRoom() {
        Open();
        await CreateRoom(new RoomInfo(RoomName, "Test", 1, "TestNick"));
    }

    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        base.OnPlayerJoined(runner, player);

        if(Runner.IsServer) {
            runner.Spawn(KartPrefab, Vector3.zero, Quaternion.identity, player);
        }
    }
}
