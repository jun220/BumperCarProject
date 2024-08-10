using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BumperCarProject.Car.SO;
using BumperCarProject.UI.View;
using BumperCarProject.UI.Presenter;
using BumperCarProject.Car;
using static UnityEngine.Rendering.DebugUI;
using System.Runtime.CompilerServices;

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

    public TempKartController _opponentCar;

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

    public float _currentSpeed;

    public Camera mainCamera;
    [SerializeField]
    private DynamicCamera _dynamicCamera;

    [SerializeField]
    public float Damage;
    public float GetDamage()
    {
        return Damage;
    }
    public void TakeDamage(float damage)
    {
        Damage += damage;
        DashboardView.presenter.UpdateCurDamage(Damage);
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

            _dynamicCamera.ActivateDynamicCamera(mainCamera);

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
            targetSpeed = Mathf.Lerp(_currentSpeed, _maxSpeed, _acceleration * DeltaTime);
        }
        else if (input.GetButton(KartInput.NetworkInputData.ButtonType.REVERSE))
        {
            targetSpeed = Mathf.Lerp(_currentSpeed, -_maxSpeed, _acceleration * DeltaTime);
        }
        else
        {
            targetSpeed = Mathf.Lerp(_currentSpeed, 0, MOVE_DECELERATION * DeltaTime);
        }

        Vector3 forward = Rigidbody.rotation * Vector3.forward;
        Vector3 velocity = forward * targetSpeed + Vector3.up * Rigidbody.velocity.y;
        Rigidbody.velocity = velocity;

        _currentSpeed = Vector3.Dot(velocity, forward) > 0 ? velocity.magnitude : -velocity.magnitude;
        Speed = velocity.magnitude;
        previousVelocity = velocity;
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

    private int CalculateDamage(Vector3 impulse)
    {
        return Mathf.RoundToInt(impulse.magnitude);
    }

    private void ApplyForce(GameObject other, Vector3 bounceDirection, float impactForce, bool isAttacker)
    {
        _opponentCar = other.GetComponent<TempKartController>();
        if( _opponentCar == null )
        {
            Debug.Log("카트를 찾을 수 없음");
            return;
        }
        else
        {
            Debug.Log("카트를 찾음");
        }

        if (isAttacker)
        {
            _opponentCar.TakeDamage(impactForce);
        }
        else
        {
            _opponentCar.TakeDamage(impactForce * 0.2f);
        }
        // 전달된 힘만큼 충격받음
        

        return;

        // 상대 범퍼카의 누적 대미지를 가져옴
        float opponentDamage = _opponentCar.Damage;
        float bounceStrength = opponentDamage * 0.5f; // 대미지를 기반으로 반발력 조정

        // 충돌 후 반발력 조정
        Rigidbody rb = other.GetComponent<Rigidbody>();
        //Vector3 bounceDirection = -collision.relativeVelocity.normalized;
        Vector3 additionalForce = bounceDirection * bounceStrength;

        // Z축 반발력 제거
        additionalForce.z = 0;

        // 반발력 적용
        Debug.Log($"추가할 힘: {additionalForce.x}, {additionalForce.y}. (누적 대미지: {opponentDamage})");
        rb.AddForce(additionalForce, ForceMode.Impulse);



        // Z축 회전력 제거
        rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y, 0);

        // Z축 속도 제거
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);

        // 회전력 최소화
        rb.angularVelocity = Vector3.zero;

        // 충돌 각도에 따라 뒤로 튕겨나게 조정
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = 0; // 좌우 이동 속도 제거
        rb.velocity = transform.TransformDirection(localVelocity);
    }

    public float collisionCooldown = 0.5f;
    private float lastCollisionTime = 0f;
    public Vector3 previousVelocity;

    //protected override void CollisionEnter(GameObject other) {
    //    Debug.Log($"충돌 ({other.name})");

    //    // 충돌 간격 체크
    //    if (Time.time - lastCollisionTime < collisionCooldown)
    //    {
    //        Debug.Log("간격이 너무 좁아요");
    //        //순간적으로 여러번의 충돌이 벌어지는 경우 차단
    //        return;
    //    }

    //    Rigidbody thisRigidbody = GetComponent<Rigidbody>();
    //    Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();

    //    if (thisRigidbody == null || otherRigidbody == null) return;

    //    Vector3 relativeVelocity = thisRigidbody.velocity - otherRigidbody.velocity;

    //    float impactForce = relativeVelocity.magnitude;

    //    Debug.Log($"충돌한 객체: {other.name}, 충격량: {impactForce}");

    //    // 대미지 계산 및 누적
    //    float damage = impactForce;
    //    Damage += damage;
    //    Debug.Log($"현재 누적 대미지: {Damage}");

    //    lastCollisionTime = Time.time;

    //    ApplyAdditionalForce(other);
    //}


    protected override void CollisionEnter(GameObject other)
    {
        Debug.Log($"나: {this.gameObject.name}, 상대: {other.name}");
        return;
        bool isAttacker;

        if (Time.time - lastCollisionTime < collisionCooldown)
        {
            return;
        }

        lastCollisionTime = Time.time;

        Rigidbody thisRigidbody = GetComponent<Rigidbody>();

        // 현재 속도와 이전 속도의 차이를 계산
        Vector3 currentVelocity = thisRigidbody.velocity;
        Vector3 velocityChange = previousVelocity - currentVelocity;

        Vector3 opponentVelocity = other.GetComponent<Rigidbody>().velocity;

        if(previousVelocity.magnitude > opponentVelocity.magnitude)
        {
            Debug.Log($"{this.gameObject.name}이 공격자입니다");
            isAttacker = true;
        }
        else
        {
            isAttacker = false;
            return;
        }
        

        // 반발 방향을 계산
        Vector3 bounceDirection = velocityChange.normalized;

        Debug.Log($"충돌한 객체: {other.name}, 반발 방향: {bounceDirection}");

        // 대미지 계산 및 누적
        float impactForce = velocityChange.magnitude;
        Debug.Log($"impactForce: {impactForce}");
        //Damage += impactForce;

        if (isAttacker)
        {
            ApplyForce(other, bounceDirection, impactForce, isAttacker);
        }
        // 자기도 기본 대미지 받게 테스트
        //TakeDamage(1.0f);
        // 추가적인 반발력 적용
        

        previousVelocity = currentVelocity;
    }

    protected override void CollisionExit(GameObject other)
    {
        Debug.Log("충돌끝");
    }

    protected override void CollisionStay(GameObject other)
    {
        Debug.Log("충돌중");
    }

    protected override void OnTriggerEnter(Collider other)
    {
        _dynamicCamera.ToggleFieldEffect(true);
    }

    protected override void OnTriggerStay(Collider other)
    {
        Damage += 0.5f;
    }

    protected override void OnTriggerExit(Collider other)
    {
        _dynamicCamera.ToggleFieldEffect(false);
    }



    #endregion
}
