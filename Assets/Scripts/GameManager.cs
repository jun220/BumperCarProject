using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int targetFrameRate = 60;

    void Start()
    {
        // ���� ���� �� Ÿ�� ������ ����Ʈ ����
        Application.targetFrameRate = targetFrameRate;
    }

    void Update()
    {
        float currentFrameRate = 1f / Time.deltaTime;
        //Debug.Log("���� ������ ����Ʈ: " + currentFrameRate.ToString("F1")); // �Ҽ��� ù° �ڸ����� ���
    }
}
