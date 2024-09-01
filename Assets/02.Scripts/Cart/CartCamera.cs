using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateCartFights.Game {
    public class CartCamera : MonoBehaviour {

        public const float MIN_FOV = 45f;
        public const float MAX_FOV = 90f;

        public const float MIN_SPEED = 3f;
        public const float MAX_SPEED = 10f;

        public const float MIN_DISTANCE = 0f;
        public const float MAX_DISTANCE = 2.5f;

        public const float SMOOTH_SPEED = 0.125f;

        public readonly Vector3 CAMERA_OFFSET = new Vector3(0, 0.5f, -1.5f);
        public readonly Vector3 LOOK_OFFSET = new Vector3(0, 0.5f, 0);

        private static CartController Target = null;

        private Camera MainCamera = null;

        private void Awake() {
            MainCamera = GetComponent<Camera>();
        }

        private void FixedUpdate() {
            if (Target == null) return;

            float speed = Mathf.Abs(Target.AppliedSpeed);

            Vector3 targetPosition = Target.transform.position + Target.transform.rotation * CAMERA_OFFSET;
            targetPosition -= Target.transform.forward * GetDistance(speed);

            Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, SMOOTH_SPEED);
            transform.position = smoothPosition;

            Vector3 lookPosition = Target.transform.position + Target.transform.rotation * LOOK_OFFSET;
            transform.LookAt(lookPosition);

            float targetFOV = GetFOV(speed);
            MainCamera.fieldOfView = targetFOV;
        }

        public static void SetTarget(CartController target) => Target = target;

        // -> 그냥 CartController.Carts.First()를 SetTarget 함수에 대입하는 걸로 해결
        public static void SetRandomTarget() => Target = CartController.Carts.First();

        // -> 함수 사용 (프로퍼티 나오면 안되서)
        public static bool IsFocused(CartController target) => Target == target;

        private float GetFOV(float speed) {
            if (speed < MIN_SPEED) return MIN_FOV;
            if (speed >= MAX_SPEED) return MAX_FOV;

            float proportion = Mathf.InverseLerp(MIN_SPEED, MAX_SPEED, speed);
            return Mathf.Lerp(MIN_FOV, MAX_FOV, proportion);
        }

        private float GetDistance(float speed) {
            if (speed < MIN_SPEED) return MIN_DISTANCE;
            if (speed >= MAX_SPEED) return MAX_DISTANCE;

            float proportion = Mathf.InverseLerp(MIN_SPEED, MAX_SPEED, speed);
            return Mathf.Lerp(MIN_DISTANCE, MAX_DISTANCE, proportion);
        }
    }
}
