using AYellowpaper.SerializedCollections;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateCartFights.Game;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class SceneManager : NetworkSceneManagerDefault {

        #region Scene Loading Method

        public enum SCENE { ROOM, MAP_GROCERY, };

        [SerializedDictionary("Scene Type", "Scene Index Number")]
        [SerializeField] private SerializedDictionary<SCENE, int> Scenes;

         const int LOBBY_SCENE = 1;
        
        public void BackToLobby() {
            UnityEngine.SceneManagement.SceneManager.LoadScene(LOBBY_SCENE);
        }

        // -> 함수 자체를 GameLauncher로 넘겨도 괜찮을 듯
        public void LoadScene(SCENE scene) {
            FusionSocket.Runner.LoadScene(SceneRef.FromIndex(Scenes[scene]));
        }

        private bool IsGameScene(int sceneId) {
            SCENE scene = Scenes.FirstOrDefault(x => x.Value == sceneId).Key;
            return scene != SCENE.ROOM;
        }

        private void LoadGameObjects() {
            foreach(ClientPlayer player in ClientPlayer.Players) {
                Debug.Log(string.Format("[ * Debug * ] Cart Spawn - Player : {0}", player.PlayerID));
                Map.Current.SpawnPlayer(Runner, player);
            }
        }

        private bool IsAllSpawned => ClientPlayer.Players.Count == CartController.Carts.Count;

        #endregion

        #region Scene Loading Override Method

        protected override IEnumerator LoadSceneCoroutine(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams) {

            if (!IsGameScene(sceneRef.AsIndex))
                PanelUI.Instance.SetPanel(PanelUI.Panel.FADE);
            else if (!FusionSocket.IsHost)
                FusionSocket.Loading();

            yield return base.LoadSceneCoroutine(sceneRef, sceneParams);
            yield return null;

            if (IsGameScene(sceneRef.AsIndex)) {
                if(FusionSocket.IsHost) {
                    LoadGameObjects();

                    // Wait until all of objects are spawned
                    yield return new WaitUntil(() => IsAllSpawned);

                    Debug.Log("[ * Debug * ] Cart Loading Complete!");
                }

                // Wait until the loading animation is completed
                ClientPlayer.Local.RPC_SetReadyState(false);
                PanelUI.Instance.SetLoadingProgress(1f);
                yield return new WaitUntil(() => LoadingPanelUI.IsLoadingComplete);
            }

            Debug.Log("[ * Debug * ] Load Scene completed!");

            // Change NetworkState
            if (IsGameScene(sceneRef.AsIndex))
                FusionSocket.StartGame();
            else
                FusionSocket.ReturnRoom();
        }

        /// <summary>
        /// 씬 로딩 중에 내부적으로 계속 호출되는 함수
        /// </summary>
        protected override void OnLoadSceneProgress(SceneRef sceneRef, float progress) {
            base.OnLoadSceneProgress(sceneRef, progress);

            if(IsGameScene(sceneRef.AsIndex))
                PanelUI.Instance.SetLoadingProgress(progress);
        }

        #endregion
    }
}
