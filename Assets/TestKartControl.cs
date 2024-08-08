using Fusion;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.Windows;

public class TestKartControl : KartControl
{
    public float acceleration = 10f; // 기본 가속력
    public float steering = 100f; // 회전력
    public float maxSpeed = 20f; // 최대 속도
    public float boosterMaxSpeed = 30f; // 부스터를 사용했을 때 최대 속도

    public float deceleration = 5f; // 손을 뗐을 때 감속하는 힘
    private Rigidbody rb;

    public bool isMine;

    [SerializeField] private float speed;
    [SerializeField] private float damage;

    private void Start()
    {
        CheckControl();

        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 충돌 탐지 모드를 continuous로 설정
        // rb.interpolation = RigidbodyInterpolation.Interpolate; // 보간을 interpolate로 설정
    }

    public void CheckControl()
    {
        if (isMine)
        {
            CanMove = true;
        }
    }
    

    protected override void Move(KartInput.NetworkInputData input)
    {
        // 방향 전환
        float steer = input.Steer; // input에서 방향 전환 값 가져오기
        transform.Rotate(0, steer * steering * Time.fixedDeltaTime, 0);


        // 가속
        float move = input.Acceleration; //  input에서 가속도 값 가져오기

        Vector3 force = transform.forward * move * acceleration;
        rb.AddForce(force);

        //Vector3 forward = transform.forward * move * acceleration * Time.fixedDeltaTime;
        //Vector3 moveDirection = transform.forward * move * acceleration;
        //rb.AddForce(moveDirection, ForceMode.VelocityChange);




        //if (move != 0)
        //{
        //    // 움직이는 중
        //    if (rb.velocity.magnitude < maxSpeed || move < 0) // 최대 속도 제한
        //    {
        //        rb.AddForce(forward * acceleration);
        //    }
        //}
        //else
        //{
        //    // 키보드 입력이 없을 때 속도 감소
        //    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        //}
    }

    protected override void Accelate(float vertical)
    {
        //Debug.Log("Accelate!");
    }

    protected override void Steer(float horizon)
    {
        //Debug.Log("Steer!");
    }

    protected override void Dash()
    {
        //Debug.Log("Dash!");
    }

    protected override void CollisionStay(Collision collision)
    {

    }

    protected override void CollisionEnter(Collision collision)
    {

    }

    protected override void CollisionExit(Collision collision)
    {

    }
}
