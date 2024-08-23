using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCartFights.Utility {
    public static class ClientInfo {
        public static string Nickname {
            get => PlayerPrefs.GetString("C_Nickname", string.Empty);
            set => PlayerPrefs.SetString("C_Nickname", value);
        }
    }
}
