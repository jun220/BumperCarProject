using UnityEngine;
using UnityEngine.UI;

public class BumperCarController : MonoBehaviour
{
    public float acceleration = 10f; // 기본 가속력
    public float steering = 100f; // 회전력
    public float maxSpeed = 20f; // 최대 속도
    public float boosterMaxSpeed = 30f; // 부스터를 사용했을 때 최대 속도

    public float deceleration = 5f; // 손을 뗐을 때 감속하는 힘
    private Rigidbody rb;

    public Text speedText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 충돌 탐지 모드를 continuous로 설정
        // rb.interpolation = RigidbodyInterpolation.Interpolate; // 보간을 interpolate로 설정
    }


    void FixedUpdate()
    {
        // 방향 전환
        float steer = Input.GetAxis("Horizontal");
        transform.Rotate(0, steer * steering * Time.fixedDeltaTime, 0);

        // 가속/감속
        float move = Input.GetAxis("Vertical");
        Vector3 forward = transform.forward * move * acceleration * Time.fixedDeltaTime;

        if (move != 0)
        {
            // 가속/감속 중
            if (rb.velocity.magnitude < maxSpeed || move < 0) // 최대 속도 제한
            {
                rb.AddForce(forward, ForceMode.VelocityChange);
            }
        }
        else
        {
            // 키보드 입력이 없을 때 속도 감소
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }


        // 현재 속도 출력
        float currentSpeed = rb.velocity.magnitude;
        float roundedSpeed = Mathf.Round(currentSpeed * 10f) / 10f;

        UpdateSpeedText(roundedSpeed);
    }


    public void UpdateSpeedText(float roundedSpeed)
    {
        speedText.text = "Current Speed\n" + roundedSpeed;
    }
}

