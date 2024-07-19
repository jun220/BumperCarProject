using BumperCarProject.Car.SO;
using BumperCarProject.UI.View;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using TMPro;
using System.Collections;

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

        public TMP_Text damageTMP;

        private float _damage;
        public float Damage {
            get => _damage;
            set {
                _damage = value;
                damageTMP.text = _damage.ToString("N1");
                //DashboardView.presenter.UpdateCurDamage(value);
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
                    StopBoostingAsync().Forget();
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

        private void Start() {
            if (!isMine)
            {
                canControl = false;
                return;
            }

            _rb = GetComponent<Rigidbody>();
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

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

        private void FixedUpdate() {
            if(!canControl) return;
            float steer = Input.GetAxis("Horizontal");
            transform.Rotate(0, steer * bumperCar.steering * Time.fixedDeltaTime, 0);

            float move = Input.GetAxis("Vertical");
            Vector3 forward = _acceleration * move * Time.fixedDeltaTime * transform.forward;

            if (move != 0) {
                if (_rb.velocity.magnitude < _maxSpeed || move < 0) {
                    _rb.AddForce(forward, ForceMode.VelocityChange);
                }
            }
            else {
                _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, bumperCar.deceleration * Time.fixedDeltaTime);
            }

            float currentSpeed = _rb.velocity.magnitude;
            DashboardView.presenter.UpdateCurSpeed(currentSpeed);

            // boost
            if (Input.GetKeyDown(KeyCode.Q) && _canBoost) {
                IsBoosting = true;
                CoolDownBoostTimeAsync(bumperCar.boostCoolTime).Forget();
            }
        }

        private async UniTaskVoid CoolDownBoostTimeAsync(float coolTime) {
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
        }

        private async UniTaskVoid StopBoostingAsync() {
            await UniTask.WaitForSeconds(_boostingTime);
            IsBoosting = false;
        }

        //private void OnCollisionEnter(Collision collision) {
        //    if (collision.transform.CompareTag("Bounceable")) {
        //        ContactPoint cp = collision.GetContact(0);
        //        Vector3 incidentVec = cp.point - transform.position;
        //        float collisionWeight = Mathf.Abs(Vector3.Dot(incidentVec, transform.forward));
        //        Damage += Mathf.Pow(_rb.velocity.magnitude / bumperCar.maxSpeed, 1.5f) * collisionWeight + Damage * 0.09f;
        //    }
        //}

        private void OnCollisionEnter(Collision collision)
        {
            //if (!isMine) return;

            if (!collision.transform.CompareTag("Damagable")) return;
            // 충돌한 객체 정보
            Debug.Log($"충돌한 객체: {collision.gameObject.name}");
            //StartCoroutine(StopControl());
            // 충돌 지점 정보
            foreach (ContactPoint contact in collision.contacts)
            {
                //Debug.Log($"충돌 지점: {contact.point}");
                //Debug.Log($"충돌 표면 법선 벡터: {contact.normal}");
            }

            // 충돌 상대 속도
            //Debug.Log($"충돌 상대 속도: {collision.relativeVelocity}");

            // 충돌 힘 (Approximation)
            if (collision.impulse != Vector3.zero && collision.rigidbody != null)
            {
                Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
                //Debug.Log($"충돌 힘: {collisionForce}");

                // 대미지 계산 및 누적
                int damage = CalculateDamage(collision.impulse);
                Damage += damage;
                Debug.Log($"현재 누적 대미지: {Damage}");

                // 충돌 후 반발력 조정
                Rigidbody rb = collision.rigidbody;
                Vector3 bounceDirection = -collision.relativeVelocity.normalized;
                float bounceStrength = damage * 0.5f; // 대미지를 기반으로 반발력 조정

                // 반발력 적용
                rb.AddForce(bounceDirection * bounceStrength, ForceMode.Impulse);

                // 회전력 최소화
                rb.angularVelocity = Vector3.zero;

                // 충돌 각도에 따라 뒤로 튕겨나게 조정
                Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
                localVelocity.x = 0; // 좌우 이동 속도 제거
                rb.velocity = transform.TransformDirection(localVelocity);
            }
            else
            {
                if (collision.impulse == Vector3.zero)
                {
                    //Debug.Log("충돌에서 impulse가 0입니다.");
                }
            }
        }

        // 충격을 대미지 값으로 변환
        private int CalculateDamage(Vector3 impulse)
        {
            return Mathf.RoundToInt(impulse.magnitude);
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
