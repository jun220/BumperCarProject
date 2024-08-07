using Fusion;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class KartControll : NetworkBehaviour {

    #region UNITY LIFECYCLE METHOD

    private ChangeDetector _changeDetector;

    public override void Spawned() {
        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork() {
        base.FixedUpdateNetwork();
        
        FixedUpdateCollision();
        FixedUpdateInput();
    }

    public override void Render() {
        base.Render();

        foreach (string change in _changeDetector.DetectChanges(this)) {
            switch (change) {
                // TODO : 시각적 효과 필요시 추가
            }
        }
    }

    #endregion

    #region KART COLLISION METHOD

    private struct CollisionHistory {
        public enum CollisionState { ENTER, STAY, EXIT, NONE }

        public bool current { get; private set; }
        public bool before { get; private set; }

        public CollisionHistory(bool current, bool before) {
            this.current = current; 
            this.before = before;
        }

        public void SetCollision() => current = true;
        public void SetBefore() {
            before = current;
            current = false;
        }

        public CollisionState GetState() {
            if (current) return before ? CollisionState.STAY : CollisionState.ENTER;
            else return before ? CollisionState.EXIT : CollisionState.NONE;
        }
    }

    

    private readonly Dictionary<Collision, CollisionHistory> CollisionList = new Dictionary<Collision, CollisionHistory>();

    private bool IsCollisionMethodRun = false;

    private void FixedUpdateCollision() {
        if (!IsCollisionMethodRun) return;

        List<Collision> removeItems = new List<Collision>();
        foreach (KeyValuePair<Collision, CollisionHistory> obj in CollisionList) {
            switch(obj.Value.GetState()) {
                case CollisionHistory.CollisionState.ENTER:
                    CollisionEnter(obj.Key); 
                    break;

                case CollisionHistory.CollisionState.STAY:
                    CollisionStay(obj.Key);
                    break;

                case CollisionHistory.CollisionState.EXIT:
                    CollisionExit(obj.Key);
                    break;

                case CollisionHistory.CollisionState.NONE:
                    removeItems.Add(obj.Key);
                    break;
            }

            obj.Value.SetBefore();
        }

        foreach(Collision remove in removeItems)
            CollisionList.Remove(remove);

        IsCollisionMethodRun = false;
    }

    private void OnCollisionStay(Collision collision) {
        if(!CollisionList.ContainsKey(collision))
            CollisionList.Add(collision, new CollisionHistory(true, false));
        else
            CollisionList[collision].SetCollision();

        IsCollisionMethodRun = true;
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
    [Networked] protected bool CanMove { get; set; }

    private void FixedUpdateInput() {
        if (!CanMove) return;

        if (GetInput(out KartInput.NetworkInputData input))
            Inputs = input;

        Move(Inputs);
        Accelate(Inputs.Acceleration);
        Steer(Inputs.Steer);

        if (Inputs.GetButtonDown(KartInput.NetworkInputData.ButtonType.DASH))
            Dash();
    }

    /// <summary>
    /// 입력에 따른 처리가 필요할 때에 사용하는 메서드
    /// </summary>
    /// <param name="input"></param>
    protected abstract void Move(KartInput.NetworkInputData input);

    /// <summary>
    /// 앞/뒤로 움직이도록 하는 메서드
    /// </summary>
    /// <param name="vertical"></param>
    protected abstract void Accelate(float vertical);

    /// <summary>
    /// 좌우로 움직이도록 하는 메서드
    /// </summary>
    /// <param name="horizon"></param>
    protected abstract void Steer(float horizon);

    /// <summary>
    /// 대쉬키를 누른 해당 프레임에 호출되는 메서드
    /// </summary>
    protected abstract void Dash();
    
    protected abstract void CollisionStay(Collision collision);

    protected abstract void CollisionEnter(Collision collision);

    protected abstract void CollisionExit(Collision collision);

    #endregion
}
