using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCartFights.Utility {
    public static class ClientInfo {
        public static string Nickname {
            get => PlayerPrefs.GetString("C_Nickname", string.Empty);
            set => PlayerPrefs.SetString("C_Nickname", value.Trim());
        }

        public static bool IsInRandom {
            get => PlayerPrefs.GetInt("IsRandomRoom", 0) == 1;
            set => PlayerPrefs.SetInt("IsRandomRoom", value ? 1 : 0);
        }
    }
}
