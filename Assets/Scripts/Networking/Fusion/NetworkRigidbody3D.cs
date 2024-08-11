using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[NetworkBehaviourWeaved(NetworkRBData.WORDS)]
public class NetworkRigidbody3D : NetworkRigidbody<Rigidbody, RunnerSimulatePhysics3D> {

    protected override void Awake() {
        base.Awake();

        AutoSimulateIsEnabled = UnityEngine.Physics.simulationMode != SimulationMode.Script;
    }

    #region RIGIDBODY OVERRIDE METHOD

    public override Vector3 RBPosition {
        get => _rigidbody.position;
        set => _rigidbody.position = value;
    }

    public override Quaternion RBRotation {
        get => _rigidbody.rotation;
        set => _rigidbody.rotation = value; 
    }

    public override bool RBIsKinematic {
        get => _rigidbody.isKinematic;
        set => _rigidbody.isKinematic = value;
    }

    public override void ResetRigidbody() {
        base.ResetRigidbody();

        Rigidbody rb = _rigidbody;
        if(!rb.isKinematic) {
            rb.velocity = default;
            rb.angularVelocity = default;
        }
    }

    protected override void ApplyExtras(Rigidbody rb, ref NetworkRBData data) {
        data.Mass = rb.mass;
        data.Drag = rb.drag;
        data.AngularDrag = rb.angularDrag;

        data.LinearVelocity = rb.velocity;
        data.AngularVelocity = rb.angularVelocity;
    }

    protected override void ApplyRBPositionRotation(Rigidbody rb, Vector3 pos, Quaternion rot) {
        rb.position = pos;
        rb.rotation = rot;
    }

    protected override void CaptureExtras(Rigidbody rb, ref NetworkRBData data) {
        data.Mass = rb.mass;
        data.Drag = rb.drag;
        data.AngularDrag = rb.angularDrag;

        data.LinearVelocity = rb.velocity;
        data.AngularVelocity = rb.angularVelocity;
    }

    protected override void CaptureRBPositionRotation(Rigidbody rb, ref NetworkRBData data) {
        data.TRSPData.Position = rb.position;

        if (UsePreciseRotation)
            data.FullPrecisionRotation = rb.rotation;
        else
            data.TRSPData.Rotation = rb.rotation;
    }

    protected override void ForceRBSleep(Rigidbody rb) => rb.Sleep();

    protected override void ForceRBWake(Rigidbody rb) => rb.WakeUp();

    protected override int GetRBConstraints(Rigidbody rb) => (int) rb.constraints;

    protected override NetworkRigidbodyFlags GetRBFlags(Rigidbody rb) {
        NetworkRigidbodyFlags flags = default(NetworkRigidbodyFlags);

        if (rb.isKinematic) flags |= NetworkRigidbodyFlags.IsKinematic;
        if (rb.IsSleeping()) flags |= NetworkRigidbodyFlags.IsSleeping;
        if (rb.useGravity) flags |= NetworkRigidbodyFlags.UseGravity;

        return flags;
    }

    protected override bool GetRBIsKinematic(Rigidbody rb) => rb.isKinematic;

    protected override bool IsRBSleeping(Rigidbody rb) => rb.IsSleeping();

    protected override bool IsRigidbodyBelowSleepingThresholds(Rigidbody rb) {
        float energy = rb.mass * rb.velocity.sqrMagnitude;
        Vector3 angVel = rb.angularVelocity;
        Vector3 inertia = rb.inertiaTensor;

        energy += inertia.x * angVel.x * angVel.x;
        energy += inertia.y * angVel.y * angVel.y;
        energy += inertia.z * angVel.z * angVel.z;

        energy /= 2.0f * rb.mass;

        return energy <= UnityEngine.Physics.sleepThreshold;
    }

    protected override bool IsStateBelowSleepingThresholds(NetworkRBData data) {
        float energy = data.Mass * ((Vector3) data.LinearVelocity).sqrMagnitude;
        Vector3 angVel = ((Vector3) data.AngularVelocity);
        Vector3 inertia = _rigidbody.inertiaTensor;

        energy += inertia.x * angVel.x * angVel.x;
        energy += inertia.y * angVel.y * angVel.y;
        energy += inertia.z * angVel.z * angVel.z;

        energy /= 2.0f * data.Mass;

        return energy <= UnityEngine.Physics.sleepThreshold;
    }

    protected override void SetRBConstraints(Rigidbody rb, int constraints) {
        rb.constraints = (RigidbodyConstraints) constraints;
    }

    protected override void SetRBIsKinematic(Rigidbody rb, bool kinematic) {
        if (rb.isKinematic != kinematic)
            rb.isKinematic = kinematic;
    }

    #endregion
}