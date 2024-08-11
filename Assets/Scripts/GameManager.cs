using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 간단한 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    // 인게임 씬에서만 동작
    public int targetFrameRate = 60;

    public GameObject otherPlayerPanel;
    public GameObject otherPlayerPrefab;

    public TempNetworkConnection networkConnector;

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작 시 타겟 프레임 레이트 설정
        Application.targetFrameRate = targetFrameRate;
    }

    void Update()
    {
        // float currentFrameRate = 1f / Time.deltaTime;
        //Debug.Log("현재 프레임 레이트: " + currentFrameRate.ToString("F1")); // 소수점 첫째 자리까지 출력
    }

    public void SpawnPlayers()
    {
        if (RoomPlayer.Local.IsHost)
        {
            // 현재는 비어있는 함수 호출
            networkConnector.SpawnPlayers();
        }
    }

    [ContextMenu("상대 플레이어 패널 생성")]
    public void InstantiateOtherPlayers()
    {
        // 상대 플레이어 패널 생성
        GameObject newObject = Instantiate(otherPlayerPrefab);

        // 생성된 오브젝트를 패널 자식으로
        newObject.transform.SetParent(otherPlayerPanel.transform);
    }
}
