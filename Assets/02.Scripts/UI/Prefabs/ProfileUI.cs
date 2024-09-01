using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UltimateCartFights.Game;
using UltimateCartFights.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public class ProfileUI : MonoBehaviour {
        [Header("Profile UI")]
        [SerializeField] private Image Character;
        [SerializeField] private TMP_Text Nickname;
        [SerializeField] private TMP_Text Damage;
        [SerializeField] private Image KnockoutUI;

        [Header("Damage Font Setting")]
        [SerializeField] private float INTEGER_FONT_SIZE;
        [SerializeField] private float DECIMAL_FONT_SIZE;

        private int PlayerID = -1;

        public void Initialize(int playerID, string nickname, int character) {
            Character.sprite = ResourceManager.Instance.Characters[character];
            Nickname.text = nickname;
            SetDamage(0.0f);
            KnockoutUI.gameObject.SetActive(false);

            PlayerID = playerID;
        }

        public void SetDamage(float damage) {
            int integerNumber = (int) damage;
            int decimalNumber = ((int) damage * 10) % 10;

            string color = GetDamageColor(damage);

            Damage.text = string.Format("<#{0}><size={1}>{2}</size><size={3}>.{4}%</size></color>",
                color, INTEGER_FONT_SIZE, integerNumber, DECIMAL_FONT_SIZE, decimalNumber);
        }

        public void Knockout() {
            KnockoutUI.gameObject.SetActive(true);
        }

        public void OnClickObserving() {
            if (PlayerID == -1) return;
            if (KnockoutUI.IsActive()) return;
            if (CartController.Local != null) return;

            CartController cart = CartController.Carts.FirstOrDefault(x => x.PlayerID == PlayerID);
            if (cart != null) CartCamera.SetTarget(cart);
        }

        private string GetDamageColor(float damage) {
            if (damage < (CartController.DAMAGE_LIMIT / 3f))        return "ffe404";
            if (damage < (CartController.DAMAGE_LIMIT * 2f / 3f))   return "f77618";
            return "f71818";
        }
    }
}
