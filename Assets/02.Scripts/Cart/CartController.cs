using Fusion;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateCartFights.Network;
using UnityEngine;

namespace UltimateCartFights.Game {
    public class CartController : NetworkBehaviour {

        #region Cart Properties

        public static readonly List<CartController> Carts = new List<CartController>();

        public static CartController Local = null;

        /* State Property */
        
        [Networked] public int PlayerID { get; private set; }

        public bool IsInitialized { get; private set; } = false;

        public bool IsGameStarted { get; private set; } = false;

        /* Damage Property */

        public const float DAMAGE_LIMIT = 999.9f;

        public static Action<int, float> GetDamage;

        public static Action<int> Knockedout;

        [Networked] public float Damage { get; private set; } = 0f;

        /* Dash Property */

        public const float DASH_DURATION = 3f;

        public const float DASH_COOLTIME = 15f;

        [Networked] public TickTimer DashTimer { get; private set; }

        [Networked] public TickTimer DashCoolTimer { get; private set; }

        public float DashCoolTime => DashCoolTimer.ExpiredOrNotRunning(Runner) ? 0f : (float) DashCoolTimer.RemainingTime(Runner);

        /* Cart Controller Property */

        public const float NORMAL_MAX_SPEED = 7;

        public const float DASH_MAX_SPEED = 15;

        public const float ACCELERATION = 0.5f;

        public const float DECELARATION = 0.75f;

        public const float STEER_AMOUNT = 25f;

        [Networked] public CartInput.NetworkInputData Input { get; private set; }

        [Networked] public float MaxSpeed { get; private set; } = NORMAL_MAX_SPEED;

        [Networked] public float AppliedSpeed { get; private set; } = 0;

        private bool CanDrive => IsInitialized && IsGameStarted && !IsBumped;

        /* Collider Property */

        public const float COLLISION_FORCE_WEIGHT = 3f;

        public const float COLLISION_DAMAGE_WEIGHT = 20f;

        [Networked] public TickTimer BumpTimer { get; set; }

        [SerializeField, Layer] private int CART_LAYER;

        [SerializeField, Layer] private int WALL_LAYER;

        [SerializeField] private Rigidbody Rigidbody;
        
        private bool IsBumped => !BumpTimer.ExpiredOrNotRunning(Runner);

        #endregion

        #region Cart LifeCycle Method

        public bool IsMine { get => Object.HasInputAuthority; }

        private ChangeDetector changeDetector;

        public override void Spawned() {
            base.Spawned();
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            Carts.Add(this);

            ClientPlayer client = ClientPlayer.Players.FirstOrDefault(x => x.Object.InputAuthority == Object.InputAuthority);
            if (client != null) PlayerID = client.PlayerID;

            if (IsMine) {
                Local = this;
                CartCamera.SetTarget(this);
            }

            // Add event methods
            AddGameEvents();

            IsInitialized = true;

            Debug.Log(string.Format("[ * Debug * ] Cart Spawned - Player ID : {0} (Total : {1})", PlayerID, Carts.Count));
        }

        public override void FixedUpdateNetwork() {
            base.FixedUpdateNetwork();

            if (!IsInitialized) return;

            if (GetInput(out CartInput.NetworkInputData input))
                Input = input;

            if (CanDrive)
                Move(Input);
            else
                RefreshAppliedSpeed();

            Steer(Input);
            Dash(Input);
        }

        public override void Render() {
            base.Render();

            foreach(string change in changeDetector.DetectChanges(this)) {
                switch(change) {
                    case nameof(Damage):
                        GetDamage?.Invoke(PlayerID, Damage);
                        break;
                }
            }
        }

        private void OnDisable() {
            if (Local == this)
                Local = null;

            IsInitialized = false;
            Carts.Remove(this);
            RemoveGameEvents();

            if (CartCamera.IsFocused(this))
                CartCamera.SetRandomTarget();
        }

        #endregion

        #region Cart Event Method

        private void AddGameEvents() {
            GameLauncher.GameStarted += OnGameStarted;
            GameLauncher.GameEnded += OnGameEnded;
            GetDamage += OnGetDamage;
        }

        private void RemoveGameEvents() {
            GameLauncher.GameStarted -= OnGameStarted;
            GameLauncher.GameEnded -= OnGameEnded;
            GetDamage -= OnGetDamage;
        }

        private void OnGameStarted() => IsGameStarted = true;
        private void OnGameEnded() => IsGameStarted = false;

        private void OnGetDamage(int playerID, float damage) {
            if (PlayerID != playerID) return;

            Debug.Log(string.Format("[ * Debug * ] OnGetDamage - Player ID : {0} / Damage : {1}", playerID, damage));

            if (damage >= DAMAGE_LIMIT)
                Knockedout?.Invoke(playerID);
        }

        #endregion

        #region Cart Control Method

        private void RefreshAppliedSpeed() => AppliedSpeed = transform.InverseTransformDirection(Rigidbody.velocity).z;

        private void Move(CartInput.NetworkInputData input) {
            if (input.Acceleration > 0)
                AppliedSpeed = Mathf.Lerp(AppliedSpeed, MaxSpeed, ACCELERATION * Runner.DeltaTime);
            else if (input.Acceleration < 0)
                AppliedSpeed = Mathf.Lerp(AppliedSpeed, -NORMAL_MAX_SPEED, ACCELERATION * Runner.DeltaTime);
            else
                AppliedSpeed = Mathf.Lerp(AppliedSpeed, 0, DECELARATION * Runner.DeltaTime);

            Vector3 velocity = (Rigidbody.rotation * Vector3.forward * AppliedSpeed) + (Vector3.up * Rigidbody.velocity.y);
            Rigidbody.velocity = velocity;
        }

        private void Steer(CartInput.NetworkInputData input) {
            if (!CanDrive) return;

            Vector3 targetRot = Rigidbody.rotation.eulerAngles + Vector3.up * STEER_AMOUNT * input.Steer;
            Vector3 rot = Vector3.Lerp(Rigidbody.rotation.eulerAngles, targetRot, 3 * Runner.DeltaTime);
            Rigidbody.MoveRotation(Quaternion.Euler(rot));
        }

        private void Dash(CartInput.NetworkInputData input) {
            if(input.GetButtonDown(CartInput.NetworkInputData.ButtonType.DASH))
                OnGetDash();

            float dashRemained = DashTimer.ExpiredOrNotRunning(Runner) ? 0f : (float) DashTimer.RemainingTime(Runner);
            if (dashRemained > 0f) {
                MaxSpeed = DASH_MAX_SPEED;
                AppliedSpeed = Mathf.Lerp(AppliedSpeed, MaxSpeed, Runner.DeltaTime);
            } else {
                MaxSpeed = NORMAL_MAX_SPEED;
            }
        }

        private void OnGetDash() {
            if (!CanDrive) return;
            if (!DashCoolTimer.ExpiredOrNotRunning(Runner)) return;

            DashTimer = TickTimer.CreateFromSeconds(Runner, DASH_DURATION);
            DashCoolTimer = TickTimer.CreateFromSeconds(Runner, DASH_COOLTIME);
        }

        #endregion

        #region Cart Collider Method

        private void OnCollisionStay(Collision collision) {
            if (!IsInitialized) return;
            if (IsBumped) return;

            int layer = collision.gameObject.layer;
            if(layer == CART_LAYER) {
                Vector3 bounceDir = collision.impulse.normalized;
                float bouncePower = Mathf.Max(collision.impulse.magnitude, 2f);
                Rigidbody.AddForce(bounceDir * bouncePower * -COLLISION_FORCE_WEIGHT, ForceMode.Impulse);

                if(FusionSocket.IsHost) {
                    float otherSpeed = Mathf.Abs(collision.gameObject.GetComponent<CartController>().AppliedSpeed);
                    Damage += otherSpeed * COLLISION_DAMAGE_WEIGHT;
                }
                
                // 충돌 타이머 생성
                BumpTimer = TickTimer.CreateFromSeconds(Runner, 0.3f);
            }
        }

        #endregion

        #region Others

        public static void RemoveCart(NetworkRunner runner, PlayerRef player) {
            CartController cart = Carts.FirstOrDefault(x => x.Object.InputAuthority == player);
            if (cart == null) return;

            Carts.Remove(cart);
            runner.Despawn(cart.Object);
        }

        #endregion
    }
}
