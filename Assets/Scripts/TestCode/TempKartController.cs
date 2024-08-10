using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BumperCarProject.Car.SO;
using BumperCarProject.UI.View;
using BumperCarProject.UI.Presenter;

public class TempKartController : KartControl {
    [SerializeField]
    private BumperCar _bumperCar;

    public float ROT_SPEED;
    public float MOVE_ACCELERATION;
    public float MOVE_DECELERATION;

    private readonly float _boostingTime = 6f;
    private float _maxSpeed;
    private float _acceleration;
    private float _deceleration;

    public Rigidbody Rigidbody;

    private float _speed;
    public float Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            DashboardView.presenter.UpdateCurSpeed(value);
        }
    }

    public Camera mainCamera;

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

    private bool _canBoost;
    private bool _isBoosting;
    public bool IsBoosting
    {
        get => _isBoosting;
        set
        {
            _isBoosting = value;
            if (value)
            {
                _maxSpeed = _bumperCar.boosterMaxSpeed;
                _acceleration *= 1.2f;
                _canBoost = false;
                //StopBoostingAsync().Forget();
            }
            else
            {
                _maxSpeed = _bumperCar.maxSpeed;
                _acceleration = _bumperCar.acceleration;
            }
        }
    }

    private float targetSpeed;


    public void SetCamera()
    {
        if (!IsMine) return;
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

    public void Set()
    {
        SetCamera();

        _canBoost = true;
        _acceleration = _bumperCar.acceleration;
        _deceleration = _bumperCar.deceleration;
        _maxSpeed = _bumperCar.maxSpeed;
    }

    #region MonoBehaviour
    private void Start()
    {
        Set();

    }
    #endregion


    #region Override



    protected override void Move(KartInput.NetworkInputData input) { }

    protected override void Accelate(KartInput.NetworkInputData input) {

        //if(input.GetButton(KartInput.NetworkInputData.ButtonType.ACCELERATION)) {
        //    targetSpeed = Mathf.Lerp(Speed, _maxSpeed, _acceleration * DeltaTime);
        //} else
        //{
        //    if (input.GetButton(KartInput.NetworkInputData.ButtonType.REVERSE))
        //    {
        //        targetSpeed = Mathf.Lerp(Speed, -_maxSpeed, _acceleration * DeltaTime);
        //    }
        //    targetSpeed = Mathf.Lerp(Speed, 0, MOVE_DECELERATION * DeltaTime);
        //}

        if (input.GetButton(KartInput.NetworkInputData.ButtonType.ACCELERATION))
        {
            targetSpeed = Mathf.Lerp(Speed, _maxSpeed, _acceleration * DeltaTime);
        }
        else if (input.GetButton(KartInput.NetworkInputData.ButtonType.REVERSE))
        {
            targetSpeed = Mathf.Lerp(Speed, -_maxSpeed, _acceleration * DeltaTime);
        }
        else
        {
            targetSpeed = Mathf.Lerp(Speed, 0, MOVE_DECELERATION * DeltaTime);
        }


        Vector3 velocity = Rigidbody.rotation * Vector3.forward * targetSpeed + Vector3.up * Rigidbody.velocity.y;
        Rigidbody.velocity = velocity;
        Speed = velocity.magnitude;
    }

    protected override void Steer(KartInput.NetworkInputData input) {
        Quaternion rotation = Quaternion.Euler(
            Vector3.Lerp(
                Rigidbody.rotation.eulerAngles,
                Rigidbody.rotation.eulerAngles + Vector3.up * ROT_SPEED * input.Steer,
                Runner.DeltaTime
            )
        );

        Rigidbody.MoveRotation(rotation);
    }

    protected override void Dash() { }

    protected override void CollisionEnter(GameObject other) { }

    protected override void CollisionExit(GameObject other) { }

    protected override void CollisionStay(GameObject other) { }



    #endregion
}
