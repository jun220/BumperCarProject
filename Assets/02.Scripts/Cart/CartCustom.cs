using System.Collections;
using System.Collections.Generic;
using UltimateCartFights.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateCartFights.Game {
    public class CartCustom : MonoBehaviour {
        [SerializeField] private SkinnedMeshRenderer Renderer;
        [SerializeField] private Image BackCharacter;
        [SerializeField] private Image FrontCharacter;

        public void SetCharacter(int character) {
            BackCharacter.sprite = ResourceManager.Instance.Characters[character];
            FrontCharacter.sprite = ResourceManager.Instance.Characters[character];
        }
        
        public void SetColor(int color) {
            Renderer.material = ResourceManager.Instance.ColorMaterials[color];
        }
    }
}
