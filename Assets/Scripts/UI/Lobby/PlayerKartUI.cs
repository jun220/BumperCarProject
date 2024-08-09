using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKartUI : MonoBehaviour
{
    [SerializeField] List<KartSelectionRT> KartSelections;

    private static Action<int, int> KartPicked;

    private void Awake() {
        KartPicked += OnPlayerPickedKart;
    }

    private void OnPlayerPickedKart(int PlayerID, int KartType) {
        KartSelections[PlayerID].SelectKartModel(KartType);
    }

    public static void OnKartPicked(int PlayerID, int KartType) => KartPicked?.Invoke(PlayerID, KartType);
}
