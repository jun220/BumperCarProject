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

        [SerializeField] private float LOADING_SPEED;
        private static float TargetProgress = 0.0f;
        private static float CurrentProgress = 0.0f;

        public static bool IsLoadingComplete { get => IsLoadingStarted && CurrentProgress >= 1.0f; }
        private static bool IsLoadingStarted = false;

        private void InitializeLoading() {
            CurrentProgress = 0.0f;
            IsLoadingStarted = true;
            ShowRandomTip();

            StartCoroutine(MoveLoading());
        }

        public void SetLoadingProgress(float progress) {
            TargetProgress = progress;
        }

        public void ShowRandomTip() {
            int index = UnityEngine.Random.Range(0, TipList.Count);
            Tip.text = TipList[index];
        }

        private void SetLoading(float progress) {
            LoadingBar.value = progress;
            LoadingPercent.text = string.Format("{0:0.#} %", progress * 100);
        }

        private IEnumerator MoveLoading() {
            const float DELTA_TIME = 0.02f;

            while(CurrentProgress < 1f) {
                float LerpProgress = CurrentProgress + (DELTA_TIME * LOADING_SPEED / 100f);
                CurrentProgress = Mathf.Min(TargetProgress, LerpProgress);
                SetLoading(CurrentProgress);

                Debug.Log(string.Format("[ * Debug * ] Loading Progress : {0} %", CurrentProgress * 100));

                yield return new WaitForSeconds(DELTA_TIME);
            }
            yield return null;
        }

        #endregion

        #region Others

        public void Initialize() {
            InitializeProfiles();
            InitializeLoading();
        }

        public void Disabled() {
            CurrentProgress = 0.0f;
            TargetProgress = 0.0f;
            IsLoadingStarted = false;

            StopCoroutine(MoveLoading());
        }

        #endregion
    }
}
