using BumperCarProject.Car.SO;
using BumperCarProject.UI.View;
using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Runtime.CompilerServices;

namespace BumperCarProject.Car 
{
    public class BumperCarController : MonoBehaviour
    {
        public bool isMine;
        public bool canControl;

        [SerializeField]
        private BumperCar bumperCar;

        private Rigidbody _rb;
        private PhysicMaterial _physicMaterial;

        [SerializeField]
        private float _damage;
        public float Damage {
            get => _damage;
            set {
                _damage = value;
                DashboardView.presenter.UpdateCurDamage(value);
            }
        }

        private bool _canBoost;
        private bool _isBoosting;
        public bool IsBoosting {
            get => _isBoosting;
            set {
                _isBoosting = value;
                if (value) {
                    _maxSpeed = bumperCar.boosterMaxSpeed;
                    _acceleration *= 1.2f;
                    _canBoost = false;
                    // StopBoostingAsync().Forget();
                }
                else {
                    _maxSpeed = bumperCar.maxSpeed;
                    _acceleration = bumperCar.acceleration;
                }
            }
        }

        private readonly float _boostingTime = 6f;
        private float _maxSpeed;
        private float _acceleration;

        private void Awake()
        {
            if (!isMine)
            {
                canControl = false;
                return;
            }

            _rb = GetComponent<Rigidbody>();
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void Start() {


            _physicMaterial = Instantiate(bumperCar.physicMaterial);
            Span<Collider> colliders = GetComponentsInChildren<Collider>();
            for(int i = 0; i < colliders.Length; ++i) {
                colliders[i].material = _physicMaterial;
            }

            _canBoost = true;
            _isBoosting = false;

            _maxSpeed = bumperCar.maxSpeed;
            _acceleration = bumperCar.acceleration;
        }

        private void FixedUpdate()
        {
            if (!_rb) return;

            // Z축 회전 강제로 제거
            _rb.rotation = Quaternion.Euler(_rb.rotation.eulerAngles.x, _rb.rotation.eulerAngles.y, 0);

            if (!canControl) return;

            // 이동 및 회전 처리
            float steer = Input.GetAxis("Horizontal");
            transform.Rotate(0, steer * bumperCar.steering * Time.fixedDeltaTime, 0);

            float move = Input.GetAxis("Vertical");
            Vector3 forward = _acceleration * move * Time.fixedDeltaTime * transform.forward;

            if (move != 0)
            {
                if (_rb.velocity.magnitude < _maxSpeed || move < 0)
                {
                    _rb.AddForce(forward, ForceMode.VelocityChange);
                }
            }
            else
            {
                _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, bumperCar.deceleration * Time.fixedDeltaTime);
            }

            float currentSpeed = _rb.velocity.magnitude;
            DashboardView.presenter.UpdateCurSpeed(currentSpeed);

            // 부스트 처리
            if (Input.GetKeyDown(KeyCode.Q) && _canBoost)
            {
                IsBoosting = true;
                // CoolDownBoostTimeAsync(bumperCar.boostCoolTime).Forget();
            }
        }

        //private void FixedUpdate() {
        //    if (!_rb) return;
        //    Quaternion currentRotation = _rb.rotation;
        //    currentRotation.z = 0;
        //    _rb.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, 0);

        //    if (!canControl) return;
        //    float steer = Input.GetAxis("Horizontal");
        //    transform.Rotate(0, steer * bumperCar.steering * Time.fixedDeltaTime, 0);

        //    float move = Input.GetAxis("Vertical");
        //    Vector3 forward = _acceleration * move * Time.fixedDeltaTime * transform.forward;

        //    if (move != 0) {
        //        if (_rb.velocity.magnitude < _maxSpeed || move < 0) {
        //            _rb.AddForce(forward, ForceMode.VelocityChange);
        //        }
        //    }
        //    else {
        //        _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, bumperCar.deceleration * Time.fixedDeltaTime);
        //    }

        //    float currentSpeed = _rb.velocity.magnitude;
        //    DashboardView.presenter.UpdateCurSpeed(currentSpeed);

        //    // boost
        //    if (Input.GetKeyDown(KeyCode.Q) && _canBoost) {
        //        IsBoosting = true;
        //        CoolDownBoostTimeAsync(bumperCar.boostCoolTime).Forget();
        //    }
        //}

        /*private async UniTaskVoid CoolDownBoostTimeAsync(float coolTime) {
            float elapsed = 0f;
            float oneTenthTime = 0f;

            while(elapsed < coolTime) {
                elapsed += Time.deltaTime;
                if(oneTenthTime + 0.1f < elapsed) {
                    oneTenthTime += 0.1f;
                    DashboardView.presenter.UpdateBoostCoolTimeText(coolTime - oneTenthTime);
                }
                DashboardView.presenter.UpdateBoostCoolTimeImage(1 - (elapsed / coolTime));
                await UniTask.Yield();
            }

            DashboardView.presenter.UpdateBoostCoolTimeText(0f);
            DashboardView.presenter.UpdateBoostCoolTimeImage(0f);
        }*/

        /*private async UniTaskVoid StopBoostingAsync() {
            await UniTask.WaitForSeconds(_boostingTime);
            IsBoosting = false;
        }*/

        //private void OnCollisionEnter(Collision collision) {
        //    if (collision.transform.CompareTag("Bounceable")) {
        //        ContactPoint cp = collision.GetContact(0);
        //        Vector3 incidentVec = cp.point - transform.position;
        //        float collisionWeight = Mathf.Abs(Vector3.Dot(incidentVec, transform.forward));
        //        Damage += Mathf.Pow(_rb.velocity.magnitude / bumperCar.maxSpeed, 1.5f) * collisionWeight + Damage * 0.09f;
        //    }
        //}

        private float lastCollisionTime = 0;
        private float collisionCooldown = 0.1f; // 100ms 쿨다운

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.transform.CompareTag("Damagable")) return;

            // 충돌 간격 체크
            if (Time.time - lastCollisionTime < collisionCooldown)
            {
                //순간적으로 여러번의 충돌이 벌어지는 경우 차단
                return;
            }

            lastCollisionTime = Time.time;

            // 충돌한 객체 정보
            if (collision.impulse != Vector3.zero && collision.rigidbody != null)
            {
                Debug.Log($"충돌한 객체: {collision.gameObject.name}, 힘: {collision.impulse}");

                // 대미지 계산 및 누적
                int damage = CalculateDamage(collision.impulse);
                Damage += damage;
                Debug.Log($"현재 누적 대미지: {Damage}");

                // 추가적인 반발력 적용
                ApplyAdditionalForce(collision);
            }
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    // 대미지 계산이 필요없는 객체라면 조기 리턴
        //    if (!collision.transform.CompareTag("Damagable")) return;

        //    // 충돌한 객체 정보
        //    //Debug.Log($"충돌한 객체: {collision.gameObject.name}");
        //    if (collision.impulse != Vector3.zero && collision.rigidbody != null)
        //    {
        //        Debug.Log($"충돌한 객체: {collision.gameObject.name}, 힘: {collision.impulse}");
        //    }

        //    // 충돌 상대 속도
        //    //Debug.Log($"충돌 상대 속도: {collision.relativeVelocity}");

        //        // 충돌 힘 (Approximation)
        //        //if (collision.impulse != Vector3.zero && collision.rigidbody != null)
        //        //{
        //        //    Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        //        //    //Debug.Log($"충돌 힘: {collisionForce}");

        //        //    // 대미지 계산 및 누적
        //        //    int damage = CalculateDamage(collision.impulse);
        //        //    Damage += damage;
        //        //    Debug.Log($"현재 누적 대미지: {Damage}");
        //        //    //ApplyAdditionalForce(collision);
        //        //    //if (isMine)
        //        //    //{
        //        //    //    ApplyAdditionalForce(collision);
        //        //    //}




        //        //}
        //        //else
        //        //{
        //        //    if (collision.impulse == Vector3.zero)
        //        //    {
        //        //        //Debug.Log("충돌에서 impulse가 0입니다.");
        //        //    }
        //        //}
        //}

        // 충격을 대미지 값으로 변환
        private int CalculateDamage(Vector3 impulse)
        {
            return Mathf.RoundToInt(impulse.magnitude);
        }

        private void ApplyAdditionalForce(Collision collision)
        {
            BumperCarController _opponentCar;
            try
            {
                _opponentCar = collision.gameObject.GetComponent<BumperCarController>();
            }
            catch
            {
                Debug.LogError("범퍼카를 찾을 수 없음");
                return;
            }

            // 상대 범퍼카의 누적 대미지를 가져옴
            float opponentDamage = _opponentCar.Damage;
            float bounceStrength = opponentDamage * 0.5f; // 대미지를 기반으로 반발력 조정

            // 충돌 후 반발력 조정
            Rigidbody rb = collision.rigidbody;
            Vector3 bounceDirection = -collision.relativeVelocity.normalized;
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

        private void ApplyAdditionalForce(Collision collision, int damage)
{
    BumperCarController opponentCar = collision.gameObject.GetComponent<BumperCarController>();
    if (opponentCar == null)
    {
        Debug.LogError("범퍼카를 찾을 수 없음");
        return;
    }

    // 상대 범퍼카의 누적 대미지를 고려한 반발력 계산
    float opponentDamage = opponentCar.Damage;
    float bounceStrength = opponentDamage * 0.5f; // 상대 누적 대미지 기반 반발력 조정

    // 충돌 속도 기반 추가 반발력 계산
    float collisionSpeedFactor = collision.relativeVelocity.magnitude;
    float additionalForceFactor = damage * collisionSpeedFactor * 0.1f; // 충돌 속도와 대미지 기반 추가 반발력 조정

    // 반발력 적용
    Rigidbody rb = collision.rigidbody;
    Vector3 bounceDirection = -collision.relativeVelocity.normalized;
    Vector3 additionalForce = bounceDirection * (bounceStrength + additionalForceFactor);

    // Z축 반발력 제거
    additionalForce.z = 0;

    rb.AddForce(additionalForce, ForceMode.Impulse);

    // Z축 회전력 제거
    rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y, 0);

    // Z축 속도 제거
    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);

    // 충돌 각도에 따라 뒤로 튕겨나게 조정
    Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
    localVelocity.x = 0; // 좌우 이동 속도 제거
    rb.velocity = transform.TransformDirection(localVelocity);
}

        private IEnumerator StopControl()
        {
            // canControl을 false로 설정합니다.
            canControl = false;
            Debug.Log("canControl: false");

            // 5초 동안 대기합니다.
            yield return new WaitForSeconds(bumperCar.stunDuration);

            // canControl을 true로 설정합니다.
            canControl = true;
            Debug.Log("canControl: true");
        }
    }
}
