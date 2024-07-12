using UnityEngine;
using UnityEngine.UI;

public class BumperCarController : MonoBehaviour
{
    public float acceleration = 10f; // �⺻ ���ӷ�
    public float steering = 100f; // ȸ����
    public float maxSpeed = 20f; // �ִ� �ӵ�
    public float boosterMaxSpeed = 30f; // �ν��͸� ������� �� �ִ� �ӵ�

    public float deceleration = 5f; // ���� ���� �� �����ϴ� ��
    private Rigidbody rb;

    public Text speedText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // �浹 Ž�� ��带 continuous�� ����
        // rb.interpolation = RigidbodyInterpolation.Interpolate; // ������ interpolate�� ����
    }


    void FixedUpdate()
    {
        // ���� ��ȯ
        float steer = Input.GetAxis("Horizontal");
        transform.Rotate(0, steer * steering * Time.fixedDeltaTime, 0);

        // ����/����
        float move = Input.GetAxis("Vertical");
        Vector3 forward = transform.forward * move * acceleration * Time.fixedDeltaTime;

        if (move != 0)
        {
            // ����/���� ��
            if (rb.velocity.magnitude < maxSpeed || move < 0) // �ִ� �ӵ� ����
            {
                rb.AddForce(forward, ForceMode.VelocityChange);
            }
        }
        else
        {
            // Ű���� �Է��� ���� �� �ӵ� ����
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }


        // ���� �ӵ� ���
        float currentSpeed = rb.velocity.magnitude;
        float roundedSpeed = Mathf.Round(currentSpeed * 10f) / 10f;

        UpdateSpeedText(roundedSpeed);
    }


    public void UpdateSpeedText(float roundedSpeed)
    {
        speedText.text = "Current Speed\n" + roundedSpeed;
    }
}

