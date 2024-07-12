using UnityEngine;

public class CreateCubes : MonoBehaviour
{
    public int numberOfCubes = 10; // ������ ť���� ��
    public GameObject cubePrefab; // ������ ť���� ������

    void Start()
    {
        for (int i = 0; i < numberOfCubes; i++)
        {
            // x�� z�� 0���� 40 ������ ����, y�� 0.5�� ����
            float x = Random.Range(0f, 40f);
            float z = Random.Range(0f, 40f);
            float y = 0.5f;

            // ť�� ���� �� ��ġ ����
            Vector3 position = new Vector3(x, y, z);
            Instantiate(cubePrefab, position, Quaternion.identity);
        }
    }
}
