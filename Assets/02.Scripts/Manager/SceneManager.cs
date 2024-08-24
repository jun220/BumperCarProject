using Fusion;
using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class SceneManager : NetworkSceneManagerDefault {

        #region Scene Loading Method

        public enum SCENE { CLOSE, LOBBY, ROOM, MAP_GROCERY, };
        private static SCENE CurrentLoading;

        public static void LoadScene(SCENE scene) {
            CurrentLoading = scene;
            FusionSocket.Runner.LoadScene(SceneRef.FromIndex(GetSceneIndex(scene)));
        }

        protected override IEnumerator LoadSceneCoroutine(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams) {
            Debug.Log(string.Format("[ * Debug * ] SceneManager - OnLoadScene ( scene : {0} )", sceneRef.AsIndex));

            if (!FusionSocket.IsServer && IsGameScene(sceneRef.AsIndex))
                FusionSocket.Loading();

            yield return base.LoadSceneCoroutine(sceneRef, sceneParams);

            yield return null;

            if(FusionSocket.IsServer) {
                // 카트 생성
            }
        }

        /// <summary>
        /// 씬 로딩 중에 내부적으로 계속 호출되는 함수
        /// </summary>
        protected override void OnLoadSceneProgress(SceneRef sceneRef, float progress) {
            base.OnLoadSceneProgress(sceneRef, progress);

            if(IsGameScene(sceneRef.AsIndex))
                PanelUI.Instance.SetLoadingProgress(progress);

            Debug.Log(string.Format("[ * Debug * ] SceneManager - OnLoadSceneProgress ( progress : {0} )", progress));
        }

        /// <summary>
        /// 씬 로딩이 완료되었을 때 호출되는 함수
        /// </summary>
        protected override IEnumerator OnSceneLoaded(SceneRef sceneRef, UnityEngine.SceneManagement.Scene scene, NetworkLoadSceneParameters sceneParams) {
            Debug.Log(string.Format("[ * Debug * ] SceneManager - Scene Loading Complete! ( scene : {0} )", sceneRef.AsIndex));

            PanelUI.Instance.SetLoadingProgress(1f);
            yield return new WaitUntil(() => LoadingPanelUI.IsLoadingComplete);

            LoadNetwork(GetSceneType(sceneRef.AsIndex));

            yield return base.OnSceneLoaded(sceneRef, scene, sceneParams);
        }

        #endregion

        #region Others

        private static int GetSceneIndex(SCENE scene) {
            switch(scene) {
                case SCENE.CLOSE:
                case SCENE.LOBBY:
                case SCENE.ROOM:
                    return 1;

                case SCENE.MAP_GROCERY:
                    return 2;

                default:
                    return 0;
            }
        }

        private static SCENE GetSceneType(int sceneID) {
            if (sceneID == 1) return FusionSocket.InRoom ? SCENE.LOBBY : SCENE.ROOM;
            if (sceneID == 2) return SCENE.MAP_GROCERY;

            return SCENE.ROOM;
        }

        private async void LoadNetwork(SCENE scene) {
            switch(scene) {
                case SCENE.LOBBY:
                    await FusionSocket.Open();
                    break;

                case SCENE.ROOM:
                    await FusionSocket.ReturnRoom(); 
                    break;

                case SCENE.MAP_GROCERY:
                    await FusionSocket.StartGame();
                    break;
            }
        }

        private bool IsGameScene(int sceneId) => sceneId > 1;

        #endregion
    }
}
