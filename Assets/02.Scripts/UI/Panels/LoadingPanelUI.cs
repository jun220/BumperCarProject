using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UltimateCartFights.Network;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public class LoadingPanelUI : MonoBehaviour {

        #region Profile Section

        [Header("Profile Section")]
        [SerializeField] private GameObject ProfileGroup;
        [SerializeField] private PlayerUI ProfilePrefab;

        private List<PlayerUI> Profiles = new List<PlayerUI>();

        private void InitializeProfiles() {
            foreach(PlayerUI profile in Profiles) {
                Destroy(profile.gameObject);
            }

            Profiles.Clear();

            for(int i = 0; i < RoomInfo.MAX_PLAYER; i++) {
                ClientPlayer player = ClientPlayer.Players.FirstOrDefault(player => player.PlayerID == i);
                if (player == null) continue;

                PlayerUI profile = Instantiate(ProfilePrefab, ProfileGroup.transform);
                profile.SetPlayerInfo((string) player.Nickname, player.Character, false);
                Profiles.Add(profile);
            }
        }

        #endregion

        #region Loading Section

        [Header("Loading Section")]
        [SerializeField] private Slider LoadingBar;
        [SerializeField] private TMP_Text LoadingPercent;
        [SerializeField] private TMP_Text Tip;

        [SerializeField] private List<string> TipList;

        private void InitializeLoading() {
            SetLoadingProgress(0.0f);
            ShowRandomTip();
        }

        public void SetLoadingProgress(float progress) {
            LoadingBar.value = progress;
            LoadingPercent.text = string.Format("{0:0.#} %", progress * 100);
        }

        public void ShowRandomTip() {
            int index = UnityEngine.Random.Range(0, TipList.Count);
            Tip.text = TipList[index];
        }

        #endregion

        #region Others

        public void Initialize() {
            InitializeProfiles();
            InitializeLoading();
        }

        #endregion
    }
}
