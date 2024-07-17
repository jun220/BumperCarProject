using BumperCarProject.Car.SO;
using BumperCarProject.UI.View;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace BumperCarProject.Car 
{
    public class BumperCarController : MonoBehaviour
    {
        [SerializeField]
        private BumperCar bumperCar;

        private Rigidbody _rb;
        private PhysicMaterial _physicMaterial;

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

        private void OnCollisionEnter(Collision collision) {
            if (collision.transform.CompareTag("Bounceable")) {
                ContactPoint cp = collision.GetContact(0);
                Vector3 incidentVec = cp.point - transform.position;
                float collisionWeight = Mathf.Abs(Vector3.Dot(incidentVec, transform.forward));
                Damage += Mathf.Pow(_rb.velocity.magnitude / bumperCar.maxSpeed, 1.5f) * collisionWeight + Damage * 0.09f;
            }
        }
    }
}
