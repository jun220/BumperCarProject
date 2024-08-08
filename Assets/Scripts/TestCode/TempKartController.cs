using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempKartController : KartControl {

    public int MoveSpeed;

    protected override void Move(KartInput.NetworkInputData input) {
        //Debug.Log(string.Format("[ * Debug * ] Input - Vertical : {0} / Horizontal : {1}", input.Steer, input.Acceleration));
        Vector3 moveDir = new Vector3(input.Steer, 0, input.Acceleration);

        transform.Translate(moveDir.normalized * MoveSpeed * DeltaTime, Space.Self);
    }

    protected override void Accelate(float vertical) {
        //Debug.Log(string.Format("[ * Debug * ] Controller - Accel : {0}", vertical));
    }

    protected override void Steer(float horizon) {
        //Debug.Log(string.Format("[ * Debug * ] Controller - Steer : {0}", horizon));
    }

    protected override void Dash() {
        //Debug.Log(string.Format("[ * Debug * ] Controller - Dash!"));
    }

    protected override void CollisionEnter(GameObject other) {
        Debug.Log(string.Format("[ * Debug * ] Object < {0} > Collision Enter to < {1} >", gameObject.name, other.name));
    }

    protected override void CollisionExit(GameObject other) {
        Debug.Log(string.Format("[ * Debug * ] Object < {0} > Collision Exit to < {1} >", gameObject.name, other.name));
    }

    protected override void CollisionStay(GameObject other) {
        Debug.Log(string.Format("[ * Debug * ] Object < {0} > Collision Stay to < {1} >", gameObject.name, other.name));
    }
}
