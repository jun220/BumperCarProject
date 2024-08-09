using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public TempKartController kart;

    public Camera mainCamera; // 메인 카메라
    public float minFOV = 60f; // 최소 FOV
    public float maxFOV = 90f; // 최대 FOV
    public float minSpeed = 5f; // 최소 속도 (이 속도 이하에서는 FOV 조절 안 함)
    public float maxSpeed = 20f; // 최대 속도 (FOV가 최대가 되는 속도)

    public bool isActive;

    public void ActivateDynamicCamera(Camera camera)
    {
        mainCamera = camera;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        float speed = kart.Speed;

        // 최소 속도에 도달하기 전까지는 FOV를 조절하지 않음
        if (speed < minSpeed)
        {
            mainCamera.fieldOfView = minFOV;
            return;
        }

        // 최소 속도를 초과했을 때 FOV를 조절함
        float t = Mathf.Clamp01((speed - minSpeed) / (maxSpeed - minSpeed));
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);
        mainCamera.fieldOfView = targetFOV;
    }
}
