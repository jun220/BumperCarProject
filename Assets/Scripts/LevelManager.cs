using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : NetworkSceneManagerDefault
{
    public const int LOBBY_SCENE = 1;
    public const int GAME_SCENE = 2;

    public static LevelManager Instance;

    private void Start()
    {
        Instance = this;
    }

    public static void LoadScene(int sceneIndex) {
        Instance.Runner.LoadScene(SceneRef.FromIndex(sceneIndex));
    }

    private void PreLoadScene(int sceneIndex) {
        if(sceneIndex == LOBBY_SCENE) {
            PanelUI.Instance.FocusRoom();
        } else if(sceneIndex == GAME_SCENE) {
            PanelUI.Instance.DeFocus();
        }
    }

    private void PostLoadScene(int sceneIndex) {

    }

    protected override IEnumerator LoadSceneCoroutine(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams)
    {
        PreLoadScene(sceneRef.AsIndex);

        yield return base.LoadSceneCoroutine(sceneRef, sceneParams);

        yield return null;
        
        if (sceneRef.AsIndex > LOBBY_SCENE)
        {
            if (Runner.GameMode == GameMode.Host)
            {
                GameLauncher.Manager.SpwanPlayer();
            }
        }

        PostLoadScene(sceneRef.AsIndex);
    }
}
