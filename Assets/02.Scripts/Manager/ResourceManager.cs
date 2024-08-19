using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCartFights.Utility {
    public class ResourceManager : MonoBehaviour {


        [Header("Prefab Objects")]
        public GameObject Session;
        public GameObject Client;

        [Header("Character Sprite")]
        public List<Sprite> Characters;

        [Header("Color Types")]
        public List<Color> Colors;

        public static ResourceManager Instance = null;

        private void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }
    }
}
