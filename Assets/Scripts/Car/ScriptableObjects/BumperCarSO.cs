using UnityEngine;

namespace BumperCarProject.Car.SO
{
    [CreateAssetMenu(fileName = "BumperCar", menuName = "ScriptableObjects/BumperCar")]
    public class BumperCar : ScriptableObject 
    {
        [Tooltip("가속력")]
        public float acceleration = 10f;
        [Tooltip("감속력")]
        public float deceleration = 5f;
        [Tooltip("회전력")]
        public float steering = 100f;
        [Tooltip("최대 속도")]
        public float maxSpeed = 20f;
        [Tooltip("부스터 상태 시의 최대 속도")]
        public float boosterMaxSpeed = 30f;
        [Tooltip("부스트 쿨타임(초)")]
        public float boostCoolTime = 15f;
        [Tooltip("물리 머테리얼")]
        public PhysicMaterial physicMaterial;
    }
}