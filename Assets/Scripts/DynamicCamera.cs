using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public Transform car; // ���� Transform
    public Rigidbody carRigidbody; // ���� Rigidbody
    public Camera mainCamera; // ���� ī�޶�
    public float minFOV = 60f; // �ּ� FOV
    public float maxFOV = 90f; // �ִ� FOV
    public float maxSpeed = 20f; // �ִ� �ӵ� (FOV�� �ִ밡 �Ǵ� �ӵ�)

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        float speed = carRigidbody.velocity.magnitude;
        float t = Mathf.Clamp01(speed / maxSpeed);
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);
        mainCamera.fieldOfView = targetFOV;
    }
}
