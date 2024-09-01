using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCartFights.Utility {
    public class ResourceManager : MonoBehaviour {

        // -> GameLauncher.cs 에서 사용
        [Header("Prefab Objects")]
        public GameObject Session;
        public GameObject Client;
        public GameObject Cart;

        // -> ClientPlayer.cs 에서 사용
        [Header("Character Sprites")]
        public List<Sprite> Characters;

        [Header("Color Types")]
        public List<Color> Colors;

        // -> CartCustom.cs 에서 사용
        [Header("Cart Color Materials")]
        public List<Material> ColorMaterials;

        public static ResourceManager Instance = null;

        private void Awake() {
            if(Instance) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
