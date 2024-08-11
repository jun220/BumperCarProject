using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    private void Start() {
        InitializeLocalUI();
        InitializeOthersUI();
    }

    [Header("Local Player UI")]
    [SerializeField] private ProfileUI LocalProfile;
    [SerializeField] private CartStateUI CartState;

    private void InitializeLocalUI() {
        Debug.Log(string.Format("Player {0} : {1}", RoomPlayer.Local.PlayerID, RoomPlayer.Local.Nickname));
        LocalProfile.Initialize(RoomPlayer.Local);
        CartState.Initialize(RoomPlayer.Local.PlayerID, 15f);
    }

    [Header("Other Players UI")]
    [SerializeField] private ProfileUI ProfilePrefab;
    [SerializeField] private GameObject Others;

    private void InitializeOthersUI() {
        foreach (RoomPlayer player in RoomPlayer.Players) {
            if (player == null) continue;
            if (player.IsMine) continue;
            Debug.Log(string.Format("Player {0} : {1}", player.PlayerID, player.Nickname));
            CreateProfile(player);
        }
    }

    private void CreateProfile(RoomPlayer player) {
        ProfileUI profile = Instantiate(ProfilePrefab);
        profile.transform.SetParent(Others.transform);
        profile.Initialize(player);
    }


}
