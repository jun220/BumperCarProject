using UnityEngine;

public class CreateCubes : MonoBehaviour
{
    public int numberOfCubes = 10; // 생성할 큐브의 수
    public GameObject cubePrefab; // 생성할 큐브의 프리팹

    void Start()
    {
        for (int i = 0; i < numberOfCubes; i++)
        {
            // x와 z는 0에서 40 사이의 난수, y는 0.5로 고정
            float x = Random.Range(0f, 40f);
            float z = Random.Range(0f, 40f);
            float y = 0.5f;

            // 큐브 생성 및 위치 설정
            Vector3 position = new Vector3(x, y, z);
            Instantiate(cubePrefab, position, Quaternion.identity);
        }
    }
}
