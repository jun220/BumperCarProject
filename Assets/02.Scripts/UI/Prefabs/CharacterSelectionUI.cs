using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.Network;
using UltimateCartFights.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public class CharacterSelectionUI : MonoBehaviour {

        [SerializeField] private Image Character;
        [SerializeField] private Image Blocked;
        private int CharacterType;

        public void Initialize(int character) {
            CharacterType = character;

            Character.sprite = ResourceManager.Instance.Characters[character];
            Character.gameObject.SetActive(true);
            Blocked.gameObject.SetActive(false);
        }

        public void OnChangeCharacter() {
            if (ClientPlayer.Local.Character == CharacterType)
                ClientPlayer.Local.RPC_SetCharacter(-1);
            else
                ClientPlayer.Local.RPC_SetCharacter(CharacterType);
        }

        public void SetSelected(bool isSelected) {
            Blocked.gameObject.SetActive(isSelected);
        }
    }
}
