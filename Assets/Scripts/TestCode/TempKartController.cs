using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempKartController : KartControl {

    public float MAX_MOVE_SPEED;
    public float ROT_SPEED;
    public float MOVE_ACCELERATION;
    public float MOVE_DECELERATION;

    public Rigidbody Rigidbody;

    private float moveSpeed;

    public Camera mainCamera;

    private void Start()
    {
        SetCamera();
    }

    protected override void Move(KartInput.NetworkInputData input) { }

    protected override void Accelate(KartInput.NetworkInputData input) {
        if(input.GetButton(KartInput.NetworkInputData.ButtonType.ACCELERATION)) {
            moveSpeed = Mathf.Lerp(moveSpeed, MAX_MOVE_SPEED, MOVE_ACCELERATION * DeltaTime);
        } else if(input.GetButton(KartInput.NetworkInputData.ButtonType.REVERSE)) {
            moveSpeed = Mathf.Lerp(moveSpeed, -MAX_MOVE_SPEED, MOVE_ACCELERATION * DeltaTime);
        } else {
            moveSpeed = Mathf.Lerp(moveSpeed, 0, MOVE_DECELERATION * DeltaTime);
        }

        Vector3 velocity = Rigidbody.rotation * Vector3.forward * moveSpeed + Vector3.up * Rigidbody.velocity.y;
        Rigidbody.velocity = velocity;
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
}
