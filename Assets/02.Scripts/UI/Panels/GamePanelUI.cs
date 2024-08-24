using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UltimateCartFights.Network;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public class GamePanelUI : MonoBehaviour {

        #region Profile Damage Event

        private List<ProfileUI> Profiles = new List<ProfileUI>();

        public void Initialize(List<ClientPlayer> players) {
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
        }

        private void OnGetDamage(int playerID, float damage) {
            ProfileUI profile = Profiles[playerID];

            if (profile != null)
                profile.SetDamage(damage);
        }

        #endregion

        #region Main Profile Section

        [Header("Main Profile Section")]
        [SerializeField] private ProfileUI MainProfile;
        [SerializeField] private TMP_Text Speed;
        [SerializeField] private TMP_Text DashCoolText;
        [SerializeField] private Image DashCoolIcon;

        private ProfileUI SetMainProfile(ClientPlayer player) {
            MainProfile.Initialize((string) player.Nickname, player.Character);
            return MainProfile;
        }

        public void SetSpeed(float speed) => Speed.text = string.Format("{0:f2}", speed);

        public void SetDashCool(float remain, float cooltime) {
            Assert.Check(remain <= cooltime);

            DashCoolText.gameObject.SetActive((remain > 0));
            DashCoolText.text = string.Format("{0:f1}s", remain);
            DashCoolIcon.fillAmount = (remain / cooltime);
        }

        #endregion

        #region Other Profiles Section

        [Header("Other Profiles Section")]
        [SerializeField] private Transform OtherProfileGroup;
        [SerializeField] private ProfileUI ProfilePrefab;

        private ProfileUI SetOtherProfile(ClientPlayer player) {
            ProfileUI profile = Instantiate(ProfilePrefab, OtherProfileGroup);
            profile.Initialize((string) player.Nickname, player.Character);
            return profile;
        }

        #endregion

    }
}
