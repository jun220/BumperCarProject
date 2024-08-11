using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject CartPrefab;
    public static List<CartControl> Carts = new List<CartControl>();

    private int RevivalCarts = 0;

    private void Awake() {
        CartControl.KnockedOut += CheckGameEnd;
    }

    public void SpwanPlayer(NetworkRunner runner, RoomPlayer player) {
        Vector3 spawnpoint = MapManager.Current.GetSpawnPoint(player.PlayerID);
        CartControl cart = runner.Spawn(CartPrefab, spawnpoint, Quaternion.identity, player.Object.InputAuthority).GetComponent<CartControl>();
        cart.transform.name = string.Format("Cart {0}", player.Nickname);
        cart.PlayerID = player.PlayerID;
        Carts.Add(cart);
        RevivalCarts++;
    }

    private void CheckGameEnd(int PlayerID) {
        RevivalCarts--;

        if(RevivalCarts == 1) {
            LevelManager.LoadScene(LevelManager.LOBBY_SCENE);
        }
    }
}
