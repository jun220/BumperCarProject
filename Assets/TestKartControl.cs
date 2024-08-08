using Fusion;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.Windows;
using BumperCarProject.UI.View;

public class TestKartControl : KartControl
{
    public float acceleration = 10f; // 기본 가속력
    public float steering = 100f; // 회전력
    public float maxSpeed = 20f; // 최대 속도
    public float boosterMaxSpeed = 30f; // 부스터를 사용했을 때 최대 속도

    public float deceleration = 5f; // 손을 뗐을 때 감속하는 힘
    private Rigidbody _rb;

    public bool isMine;

    [SerializeField] private float _speed;
    public float Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            DashboardView.presenter.UpdateCurSpeed(value);
        }
    }
    [SerializeField]
    private float _damage;
    public float Damage
    {
        get => _damage;
        set
        {
            _damage = value;
            DashboardView.presenter.UpdateCurDamage(value);
        }
    }

    public Camera mainCamera;

    private void Start()
    {
        CheckControl();

        _rb = GetComponent<Rigidbody>();
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 충돌 탐지 모드를 continuous로 설정

        SetCamera();
        
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
        _rb.AddForce(force);

        Speed = _rb.velocity.magnitude;

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

    public void SetCamera()
    {
        mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Debug.Log("Main Camera found and assigned.");

            // 카메라를 이 오브젝트의 자식으로 설정
            mainCamera.transform.SetParent(transform);

            // 카메라 위치와 회전을 설정
            mainCamera.transform.localPosition = new Vector3(0, 1, -3); // position (0, 1, -3)
            mainCamera.transform.localRotation = Quaternion.Euler(10, 0, 0); // rotation (10, 0, 0)

            Debug.Log("Main Camera has been set as a child of the player with the specified position and rotation.");
        }
        else
        {
            Debug.LogWarning("Main Camera not found!");
        }
    }
}
