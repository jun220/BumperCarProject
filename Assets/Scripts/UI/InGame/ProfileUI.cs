using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] private Image Character;
    [SerializeField] private TMP_Text Nickname;
    [SerializeField] private TMP_Text Damage;
    [SerializeField] private GameObject KnockOut;
    [SerializeField] private Sprite[] CharacterSprites;

    private int PlayerID = -1;

    private void Start() {
        CartControl.DamageChanged += SetDamage;
        CartControl.KnockedOut += KnockedOut;
    }

    public void Initialize(RoomPlayer player) {
        PlayerID = player.PlayerID;

        // Character.sprite = CharacterSprites[player.KartType];
        Nickname.text = (string) player.Nickname;
        KnockOut.SetActive(false);
        SetDamage(player.PlayerID, 0f);
    }

    private void SetDamage(int playerID, float damage) {
        if (playerID != PlayerID) return;

        Debug.Log(string.Format("Set Damage - Player : {0} / UI : {1}", playerID, PlayerID));

        string color;
        if (damage < 333.3f) color = "ffe404";
        else if (damage < 666.6f) color = "f77618";
        else color = "f71818";

        Damage.text = string.Format("<color=#{0}>{1}<size=45>.{2}%</size></color>", color, (int)damage, (int)((damage - (int)damage) * 10f));
    }

    private void KnockedOut(int playerID) {
        if (playerID != PlayerID) return;
        if (KnockOut == null) return;
        KnockOut.SetActive(true);

        CartControl.DamageChanged -= SetDamage;
    }
}
