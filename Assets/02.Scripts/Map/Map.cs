using Fusion;
using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.Network;
using UltimateCartFights.Utility;
using UnityEngine;

namespace UltimateCartFights.Game {
    public class Map : NetworkBehaviour {

        #region Map Property

        public static Map Current { get; private set; }

        [SerializeField] private Transform[] SpawnPoints;

        [Networked] public TickTimer StartGameTimer { get; private set; }

        private const float INTRO_DURATION = 9f;

        #endregion

        private void Awake() {
            Current = this;
        }

        public override void Spawned() {
            base.Spawned();

            if (FusionSocket.IsHost)
                StartGameTimer = TickTimer.CreateFromSeconds(Runner, INTRO_DURATION);
        }

        private void OnDestroy() {
            Current = null;
        }

        public void SpawnPlayer(NetworkRunner runner, ClientPlayer player) {
            Transform point = SpawnPoints[player.PlayerID];
            PlayerRef inputAuthority = player.Object.InputAuthority;

            NetworkObject cart = runner.Spawn(ResourceManager.Instance.Cart, point.position, point.rotation, inputAuthority);
            CartCustom custom = cart.GetComponent<CartCustom>();
            custom.SetCharacter(player.Character);
            custom.SetColor(player.CartColor);

            cart.transform.name = string.Format("Cart_{0}_{1}", player.PlayerID, player.Nickname);
        }

        public static bool IsGameStarted() {
            if (ClientPlayer.Local == null) return false;
            if (Current == null) return false;

            return Current.StartGameTimer.Expired(Current.Runner);
        }

        public static float GetRemainingTime() {
            if (Current == null) return Mathf.Infinity;
            if (!Current.StartGameTimer.IsRunning) return Mathf.Infinity;
            return (float) Current.StartGameTimer.RemainingTime(Current.Runner);
        }
    }
}
