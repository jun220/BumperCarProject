using Fusion;
using UnityEngine;

public class TempNetworkConnection : FusionSocket
{
    public Vector3[] spawnPoints;
    public Vector3 spawnPoint;
    public GameObject KartPrefab;
    //public string RoomName;

    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 1;

        JoinRoom();
    }

    private async void JoinRoom()
    {
        Open();
        await JoinRandomRoom();
        //await CreateRoom(new RoomInfo(RoomName, "Test", 2, "TestNick"));
    }

    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        base.OnPlayerJoined(runner, player);

        if (Runner.IsServer)
        {
            runner.Spawn(KartPrefab, spawnPoint, Quaternion.identity, player);
        }
    }
}