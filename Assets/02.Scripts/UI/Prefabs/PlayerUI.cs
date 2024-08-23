using System.Collections.Generic;
using TMPro;
using UltimateCartFights.Network;
using UltimateCartFights.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.UI {
    public enum PlayerRoomState { STANBY, READY, MASTER }

    public class PlayerUI : MonoBehaviour {
        [SerializeField] private TMP_Text Nickname;
        [SerializeField] private GameObject NameImage;
        [SerializeField] private GameObject Ready;
        [SerializeField] private RawImage Character;
        [SerializeField] private GameObject Blocked;

        #region UI SETTING METHOD

        public void SetEmpty() {
            Nickname.text = string.Empty;

            NameImage.SetActive(false);
            Ready.gameObject.SetActive(false);
            Character.gameObject.SetActive(false);
            Blocked.gameObject.SetActive(false);
        }

        public void SetBlocked() {
            Nickname.text = string.Empty;

            NameImage.SetActive(false);
            Ready.gameObject.SetActive(false);
            Character.gameObject.SetActive(false);
            Blocked.gameObject.SetActive(true);
        }
        public void SetPlayerInfo(string nickname, int character, bool isReady) {
            Nickname.text = nickname;

            NameImage.SetActive(true);
            SetReady(isReady);
            SetCharacter(character);
            Blocked.gameObject.SetActive(false);
        }

        public void SetReady(bool state) => Ready.gameObject.SetActive(state);

        private void SetCharacter(int character) {
            if (character == -1) {
                Character.gameObject.SetActive(false);
            } else {
                Character.gameObject.SetActive(true);
                Character.texture = ConvertSpriteToTexture(ResourceManager.Instance.Characters[character]);
            }
        }

        public Texture2D ConvertSpriteToTexture(Sprite sprite) {
            try {
                if (sprite.rect.width != sprite.texture.width) {
                    Texture2D texture = new Texture2D((int) sprite.rect.width, (int) sprite.rect.height);
                    Color[] colors = sprite.texture.GetPixels((int) sprite.textureRect.x,
                                                              (int) sprite.textureRect.y,
                                                              (int) sprite.textureRect.width,
                                                              (int) sprite.textureRect.height);

                    texture.SetPixels(colors);
                    texture.Apply();

                    return texture;
                } else
                    return sprite.texture;
            } catch {
                return sprite.texture;
            }
        }

        #endregion
    }
}
