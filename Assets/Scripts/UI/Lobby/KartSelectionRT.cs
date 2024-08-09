using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSelectionRT : MonoBehaviour
{
    [SerializeField] private List<GameObject> KartModels;

    private GameObject ActiveKart = null;

    public void SelectKartModel(int KartType) {
        ActiveKart?.SetActive(false);

        if (KartType == KartTypeUI.KART_TYPE_EMPTY)
            ActiveKart = null;
        else
            ActiveKart = KartModels[KartType];

        ActiveKart?.SetActive(true);
    }
}
