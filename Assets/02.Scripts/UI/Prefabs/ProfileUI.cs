using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        public void Initialize(string nickname, int character) {
            Character.sprite = ResourceManager.Instance.Characters[character];
            Nickname.text = nickname;
            SetDamage(0.0f);
            KnockoutUI.gameObject.SetActive(false);
        }

        private void SetDamage(float damage) {
            int integerNumber = (int) damage;
            int decimalNumber = ((int) damage * 10) % 10;

            Damage.text = string.Format("<size={0}>{1}</size><size={2}>.{3}%</size>",
                INTEGER_FONT_SIZE, integerNumber, DECIMAL_FONT_SIZE, decimalNumber);
        }

        private void Knockout() {
            KnockoutUI.gameObject.SetActive(true);
        }
    }
}
