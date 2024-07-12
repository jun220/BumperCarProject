using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int targetFrameRate = 60;

    void Start()
    {
        // 게임 시작 시 타겟 프레임 레이트 설정
        Application.targetFrameRate = targetFrameRate;
    }

    void Update()
    {
        float currentFrameRate = 1f / Time.deltaTime;
        //Debug.Log("현재 프레임 레이트: " + currentFrameRate.ToString("F1")); // 소수점 첫째 자리까지 출력
    }
}
