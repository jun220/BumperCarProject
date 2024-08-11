using UnityEngine;

namespace BumperCarProject.Car.SO
{
    [CreateAssetMenu(fileName = "BumperCar", menuName = "ScriptableObjects/BumperCar")]
    public class CartPhysicsSetting : ScriptableObject 
    {
        [Tooltip("가속력")]
        public float Acceleration = 1f;
        [Tooltip("감속력")]
        public float Deceleration = 1f;
        [Tooltip("회전력")]
        public float Steering = 100f;
        [Tooltip("최대 속도")]
        public float MaxSpeed = 7f;
        [Tooltip("대시 상태 시의 최대 속도")]
        public float DashMaxSpeed = 12f;
        [Tooltip("대시 상태 지속 시간 (초)")]
        public float DashDuration = 3f;
        [Tooltip("대시 쿨타임 (초)")]
        public float DashCoolTime = 15f;
        [Tooltip("충돌 시 경직 시간 (초)")]
        public float StunDuration = 1f;
        [Tooltip("물리 머테리얼")]
        public PhysicMaterial PhysicMaterial;
    }
}