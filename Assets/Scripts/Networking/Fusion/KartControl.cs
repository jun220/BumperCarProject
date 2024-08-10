using Fusion;
using System.Collections.Generic;
using UnityEngine;

public abstract class KartControl : NetworkBehaviour
{

    #region UNITY LIFECYCLE METHOD

    public bool IsMine { get => Object.HasInputAuthority; }

    public float DeltaTime { get => Runner.DeltaTime; }

    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        FixedUpdateCollision();
        FixedUpdateInput();
    }

    public override void Render()
    {
        base.Render();

        foreach (string change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                // TODO : 시각적 효과 필요시 추가
            }
        }
    }



    #endregion

    #region KART COLLISION METHOD

    private const int LAYER_COLLISABLE = 30;
    
    private class CollisionHistory {
        public enum CollisionState { ENTER, STAY, EXIT, NONE }

        public bool Current { get; private set; }
        public bool Before { get; private set; }

        public CollisionHistory(bool current, bool before) {
            Current = current; 
            Before = before;
        }

        public void SetCollision() => Current = true;
        public void SetBefore() {
            Before = Current;
            Current = false;
        }

        public CollisionState GetState() {
            if (Current) return Before ? CollisionState.STAY : CollisionState.ENTER;
            else return Before ? CollisionState.EXIT : CollisionState.NONE;
        }
    }
    
    private Dictionary<GameObject, CollisionHistory> CollisionList = new Dictionary<GameObject, CollisionHistory>();

    private void FixedUpdateCollision() {
        if (!Runner.IsForward) return;

        foreach (GameObject obj in CollisionList.Keys) {
            switch(CollisionList[obj].GetState()) {
                case CollisionHistory.CollisionState.ENTER:
                    CollisionEnter(obj);
                    break;

                case CollisionHistory.CollisionState.STAY:
                    CollisionStay(obj);
                    break;

                case CollisionHistory.CollisionState.EXIT:
                    CollisionExit(obj);
                    break;
            }

            CollisionList[obj].SetBefore();
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.layer != LAYER_COLLISABLE) return;

        if (!CollisionList.ContainsKey(collision.gameObject))
            CollisionList.Add(collision.gameObject, new CollisionHistory(true, false));
        else
            CollisionList[collision.gameObject].SetCollision();
    }

    #endregion

    #region KART CONTROL METHOD


    /// <summary>
    /// 현재 입력된 조작
    /// </summary>
    [Networked] private KartInput.NetworkInputData Inputs { get; set; }

    /// <summary>
    /// 현재 움직일 수 있는 상태인가
    /// </summary>
    [Networked] protected bool CanMove { get; set; } = true;

    private void FixedUpdateInput()
    {
        if (!CanMove) return;

        if (GetInput(out KartInput.NetworkInputData input))
            Inputs = input;

        Move(Inputs);
        Accelate(Inputs);
        Steer(Inputs);

        if (Inputs.GetButtonDown(KartInput.NetworkInputData.ButtonType.DASH))
            Dash();
    }

    /// <summary>
    /// 입력에 따른 처리가 필요할 때에 사용하는 메서드
    /// </summary>
    /// <param name="input"></param>
    protected abstract void Move(KartInput.NetworkInputData input);

    /// <summary>
    /// 앞뒤 이동 구현 메서드
    /// </summary>
    /// <remarks>
    /// transfrom을 사용하는 것은 권장되지 않는다. <br></br>
    /// 대신 Rigidbody.velocity = Rigidbody.rotation * Vector3(이동 방향) 으로 사용을 권장한다. <br></br>
    /// 회전의 경우 Rigidbody.MoveRotation() 함수 사용을 권장한다.
    /// </remarks>
    /// <param name="vertical"></param>
    protected abstract void Accelate(KartInput.NetworkInputData input);

    /// <summary>
    /// 좌우 회전 구현 메서드
    /// </summary>
    /// <remarks>
    /// transform을 사용하는 것은 권장되지 않는다. <br></br>
    /// 대신 Rigidbody.velocity = Rigidbody.rotation * Vector3(이동 방향) 으로 사용을 권장한다. <br></br>
    /// 회전의 경우 Rigidbody.MoveRotation() 함수 사용을 권장한다.
    /// </remarks>
    /// <param name="horizon"></param>
    protected abstract void Steer(KartInput.NetworkInputData input);

    /// <summary>
    /// 대쉬키를 누른 해당 프레임에 호출되는 메서드
    /// </summary>
    protected abstract void Dash();
    
    protected abstract void CollisionStay(GameObject other);

    protected abstract void CollisionEnter(GameObject other);

    protected abstract void CollisionExit(GameObject other);

    #endregion
}
