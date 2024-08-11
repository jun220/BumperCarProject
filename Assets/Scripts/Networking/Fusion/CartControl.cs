using BumperCarProject.Car.SO;
using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CartControl : NetworkBehaviour
{

    #region UNITY LIFECYCLE METHOD

    [Networked] public int PlayerID { get; set; }

    public bool IsMine { get => Object.HasInputAuthority; }

    public float DeltaTime { get => Runner.DeltaTime; }

    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        CanMove = true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        FixedUpdateInput();
        FixedUpdateDamage();
    }

    public override void Despawned(NetworkRunner runner, bool hasState) {
        Debug.Log("Cart Despawned!");

        base.Despawned(runner, hasState);

        KnockedOut?.Invoke(PlayerID);
    }

    public override void Render()
    {
        base.Render();

        foreach (string change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Damage):
                    Debug.Log(string.Format("Damage Change - Player : {0}", PlayerID));
                    DamageChanged?.Invoke(PlayerID, Damage);
                    break;
            }
        }
    }

    #endregion

    #region CART DAMAGE METHOD

    public const float MAX_DAMAGE = 999.9f;

    [Networked] protected float Damage { get; set; } = 0f;

    public static Action<int, float> DamageChanged;

    public static Action<int> KnockedOut;

    private void FixedUpdateDamage() {
        if (Damage >= MAX_DAMAGE) {
            KnockedOut?.Invoke(PlayerID);

            if (RoomPlayer.Local.IsHost)
                Runner.Despawn(this.Object);
        }
    }

    #endregion

    #region CART CONTROL METHOD

    public static Action<int> OnDash;

    /// <summary>
    /// 현재 입력된 조작
    /// </summary>
    [Networked] public CartInput.NetworkInputData Inputs { get; set; }

    /// <summary>
    /// 현재 움직일 수 있는 상태인가
    /// </summary>
    [Networked] public bool CanMove { get; set; }

    [Networked] public TickTimer DashTimer { get; set; }
    [Networked] public TickTimer DashCoolTimer { get; set; }

    [Networked] public float Speed { get; set; }

    [SerializeField] protected CartPhysicsSetting setting;

    protected bool IsDash { get => !DashTimer.ExpiredOrNotRunning(Runner); }
    protected bool IsDashCool { get => !DashCoolTimer.ExpiredOrNotRunning(Runner); }

    private void FixedUpdateInput()
    {
        if (!CanMove) return;

        if (GetInput(out CartInput.NetworkInputData input))
            Inputs = input;

        if (Inputs.GetButtonDown(CartInput.NetworkInputData.ButtonType.DASH) && DashCoolTimer.ExpiredOrNotRunning(Runner)) {
            DashTimer = TickTimer.CreateFromSeconds(Runner, setting.DashDuration);
            DashCoolTimer = TickTimer.CreateFromSeconds(Runner, setting.DashCoolTime);
            OnDash(PlayerID);
        }

        Move(Inputs);
        Accelate(Inputs);
        Steer(Inputs);
            
    }

    /// <summary>
    /// 입력에 따른 처리가 필요할 때에 사용하는 메서드
    /// </summary>
    /// <param name="input"></param>
    protected abstract void Move(CartInput.NetworkInputData input);

    /// <summary>
    /// 앞뒤 이동 구현 메서드
    /// </summary>
    /// <remarks>
    /// transfrom을 사용하는 것은 권장되지 않는다. <br></br>
    /// 대신 Rigidbody.velocity = Rigidbody.rotation * Vector3(이동 방향) 으로 사용을 권장한다. <br></br>
    /// 회전의 경우 Rigidbody.MoveRotation() 함수 사용을 권장한다.
    /// </remarks>
    /// <param name="vertical"></param>
    protected abstract void Accelate(CartInput.NetworkInputData input);

    /// <summary>
    /// 좌우 회전 구현 메서드
    /// </summary>
    /// <remarks>
    /// transform을 사용하는 것은 권장되지 않는다. <br></br>
    /// 대신 Rigidbody.velocity = Rigidbody.rotation * Vector3(이동 방향) 으로 사용을 권장한다. <br></br>
    /// 회전의 경우 Rigidbody.MoveRotation() 함수 사용을 권장한다.
    /// </remarks>
    /// <param name="horizon"></param>
    protected abstract void Steer(CartInput.NetworkInputData input);
    
    #endregion
}
