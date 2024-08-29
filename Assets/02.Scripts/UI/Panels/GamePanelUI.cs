using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UltimateCartFights.Game;
using UltimateCartFights.Network;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public class GamePanelUI : MonoBehaviour {

        #region Unity LifeCycle Method

        private void Update() {
            UpdateCount();
            UpdateCartUI();
        }

        #endregion

        #region Profile Damage Event

        private List<ProfileUI> Profiles = new List<ProfileUI>();

        public void Initialize(List<ClientPlayer> players) {
            ProfileUI[] others = OtherProfileGroup.GetComponentsInChildren<ProfileUI>();
            foreach(ProfileUI other in others)
                Destroy(other.gameObject);

            Profiles.Clear();

            for(int i = 0; i < RoomInfo.MAX_PLAYER; i++) {
                ClientPlayer player = players.FirstOrDefault(x => x.PlayerID == i);
                ProfileUI profile = null;

                if (player != null) {
                    if (player.IsLocal)
                        profile = SetMainProfile(player);
                    else
                        profile = SetOtherProfile(player);
                }
                
                Profiles.Add(profile);
            }

            IsCountStarted = false;

            CartController.GetDamage += OnGetDamage;
            CartController.Knockedout += OnKnockedOut;
        }

        public void Disabled() {
            CartController.GetDamage -= OnGetDamage;
            CartController.Knockedout -= OnKnockedOut;
        }

        private void OnGetDamage(int playerID, float damage) {
            ProfileUI profile = Profiles[playerID];

            if (profile != null)
                profile.SetDamage(damage);
        }

        private void OnKnockedOut(int playerID) {
            ProfileUI profile = Profiles[playerID];

            if (profile != null)
                profile.Knockout();
        }

        #endregion

        #region Main Profile Section

        [Header("Main Profile Section")]
        [SerializeField] private ProfileUI MainProfile;
        [SerializeField] private TMP_Text Speed;
        [SerializeField] private TMP_Text DashCoolText;
        [SerializeField] private Image DashCoolIcon;

        private ProfileUI SetMainProfile(ClientPlayer player) {
            MainProfile.Initialize(player.PlayerID, (string) player.Nickname, player.Character);
            return MainProfile;
        }

        private void SetSpeed(float speed) => Speed.text = string.Format("{0:f2}", speed);

        private void SetDashCool(float remain, float cooltime) {
            Assert.Check(remain <= cooltime);

            DashCoolText.gameObject.SetActive((remain > 0));
            DashCoolText.text = string.Format("{0:f1}s", remain);
            DashCoolIcon.fillAmount = (remain / cooltime);
        }

        private void UpdateCartUI() {
            if (CartController.Local == null) return;

            SetSpeed(Mathf.Abs(CartController.Local.AppliedSpeed));
            SetDashCool(CartController.Local.DashCoolTime, CartController.DASH_COOLTIME);
        }

        #endregion

        #region Other Profiles Section

        [Header("Other Profiles Section")]
        [SerializeField] private Transform OtherProfileGroup;
        [SerializeField] private ProfileUI ProfilePrefab;

        private ProfileUI SetOtherProfile(ClientPlayer player) {
            ProfileUI profile = Instantiate(ProfilePrefab, OtherProfileGroup);
            profile.Initialize(player.PlayerID, (string) player.Nickname, player.Character);
            return profile;
        }

        #endregion

        #region Countdown Section

        [Header("Countdown Section")]
        [SerializeField] private Animation CountAnimation;

        private bool IsCountStarted = false;

        private void UpdateCount() {
            if (IsCountStarted) return;

            if(Map.GetRemainingTime() <= 3f) {
                IsCountStarted = true;
                CountAnimation.Play();
            }
        }

        #endregion
    }
}
