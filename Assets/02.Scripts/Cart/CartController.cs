using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCartFights.Game {
    public class CartController : NetworkBehaviour {

        #region Cart Properties

        /* State Property */
        [Networked] public float Damage { get; set; }

        /* Collider Property */

        [Networked] public TickTimer BumpTimer { get; set; }

        [SerializeField] private Rigidbody rigidbody;
        [SerializeField, Layer] private int CART_LAYER;

        private bool IsBumped => !BumpTimer.ExpiredOrNotRunning(Runner);
        

        #endregion

        #region Cart LifeCycle Method

        public bool IsMine { get => Object.HasInputAuthority; }

        private ChangeDetector changeDetector;

        public override void Spawned() {
            base.Spawned();
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public override void FixedUpdateNetwork() {
            base.FixedUpdateNetwork();
        }

        public override void Render() {
            base.Render();

            foreach(string change in changeDetector.DetectChanges(this)) {
                switch(change) {

                }
            }
        }

        #endregion

        #region Cart Collider Method

        private void OnCollisionEnter(Collision collision) {
            if (IsBumped) return;

            if(collision.gameObject.layer == CART_LAYER) {
                // 카트 충돌 처리

                // 충돌 타이머 생성
                BumpTimer = TickTimer.CreateFromSeconds(Runner, 0.3f);
            }
        }

        #endregion

    }
}
