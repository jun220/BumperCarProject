using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public CartControl Cart;

    public Camera mainCamera; // 메인 카메라
    public float minFOV = 60f; // 최소 FOV
    public float maxFOV = 90f; // 최대 FOV
    public float minSpeed = 5f; // 최소 속도 (이 속도 이하에서는 FOV 조절 안 함)
    public float maxSpeed = 20f; // 최대 속도 (FOV가 최대가 되는 속도)

    public bool isActive;

    public Color fieldCameraFilter;

    public void ActivateDynamicCamera(Camera camera)
    {
        mainCamera = camera;
        isActive = true;
    }

    public void ToggleFieldEffect(bool isActive)
    {
        if(isActive)
        {
            Debug.Log("자기장 상태");
        }
        else
        {
            Debug.Log("자기장 탈출 상태");
        }
    }


    void Update()
    {
        if (!isActive) return;

        float speed = Cart.Speed;

        // 최소 속도에 도달하기 전까지는 FOV를 조절하지 않음
        if (speed < minSpeed)
        {
            mainCamera.fieldOfView = minFOV;
            return;
        }

        // 최소 속도를 초과했을 때 FOV를 부드럽게 조절함
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);
        mainCamera.fieldOfView = targetFOV;
    }

}
