using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public Transform car; // 차의 Transform
    public Rigidbody carRigidbody; // 차의 Rigidbody
    public Camera mainCamera; // 메인 카메라
    public float minFOV = 60f; // 최소 FOV
    public float maxFOV = 90f; // 최대 FOV
    public float maxSpeed = 20f; // 최대 속도 (FOV가 최대가 되는 속도)

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
