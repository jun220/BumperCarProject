using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.Network;
using UltimateCartFights.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public class ColorSelectionUI : MonoBehaviour {

        [SerializeField] private Image Color;
        [SerializeField] private Image Blocked;
        private int ColorType;

        public void Initialize(int character) {
            ColorType = character;

            Color.color = ResourceManager.Instance.Colors[character];
            Color.gameObject.SetActive(true);
            Blocked.gameObject.SetActive(false);
        }

        public void OnChangeCartColor() {
            if (ClientPlayer.Local.CartColor == ColorType)
                ClientPlayer.Local.RPC_SetCartColor(-1);
            else
                ClientPlayer.Local.RPC_SetCartColor(ColorType);
        }

        public void SetSelected(bool isSelected) {
            Blocked.gameObject.SetActive(isSelected);
        }
    }
}

