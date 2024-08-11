using BumperCarProject.Car.SO;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CartInput))]
[RequireComponent(typeof(NetworkRigidbody3D))]
public class CartController : CartControl {

    private DynamicCamera dynamicCamera;

    private void Start() {
        if (!IsMine) return;

        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 1, -3);
        Camera.main.transform.localRotation = Quaternion.Euler(10, 0, 0);

        dynamicCamera = GetComponent<DynamicCamera>();
        dynamicCamera.ActivateDynamicCamera(Camera.main);
    }

    #region CART CONTROL METHOD

    [SerializeField] private Rigidbody Rigidbody;

    [SerializeField] public float AppliedSpeed { get; set; }

    protected override void Move(CartInput.NetworkInputData input) { 
        Speed = Rigidbody.velocity.magnitude;
    }

    protected override void Accelate(CartInput.NetworkInputData input) {
        if (input.GetButton(CartInput.NetworkInputData.ButtonType.ACCELERATION))
            AppliedSpeed = Mathf.Lerp(AppliedSpeed, (IsDash ? setting.DashMaxSpeed : setting.MaxSpeed), setting.Acceleration * DeltaTime);
        else if (input.GetButton(CartInput.NetworkInputData.ButtonType.REVERSE))
            AppliedSpeed = Mathf.Lerp(AppliedSpeed, -setting.MaxSpeed, setting.Acceleration * DeltaTime);
        else
            AppliedSpeed = Mathf.Lerp(AppliedSpeed, 0, setting.Deceleration * DeltaTime);

        Vector3 velocity = (Rigidbody.rotation * Vector3.forward * AppliedSpeed) + (Vector3.up * Rigidbody.velocity.y);
        Rigidbody.velocity = velocity;
    }
    
    protected override void Steer(CartInput.NetworkInputData input) {
        Quaternion rotation = Quaternion.Euler(
            Vector3.Lerp(
                Rigidbody.rotation.eulerAngles,
                Rigidbody.rotation.eulerAngles + Vector3.up * input.Steer * setting.Steering,
                DeltaTime
            )
        );

        Rigidbody.MoveRotation(rotation);
    }

    #endregion

    #region CART COLLISION METHOD

    [Networked] private TickTimer BumpTimer { get; set; }

    private void OnCollisionStay(Collision collision) {
        //if (!FusionSocket.Runner.IsServer) return;
        if (!Object.IsValid) return;
        if (collision.gameObject.layer != 30) return;
        if (!BumpTimer.ExpiredOrNotRunning(Runner)) return;

        BumpTimer = TickTimer.CreateFromSeconds(Runner, 0.5f);
        
        // 반발 방향을 계산
        Vector3 bounceDirection = collision.impulse.normalized;

        // 대미지 계산 및 누적
        float impactForce = collision.impulse.magnitude;
        
        Rigidbody.AddForce(bounceDirection * -impactForce * 3, ForceMode.Impulse);
        Damage += impactForce * 20f;
    }

    private void OnTriggerStay(Collider other) {
        if (!Object.IsValid) return;

        Damage += 0.5f;
    }

    #endregion
}
