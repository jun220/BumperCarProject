using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartColorUI : MonoBehaviour
{
    public const int KART_COLOR_EMPTY = -1;

    [SerializeField] private int KART_COLOR;
    [SerializeField] private GameObject SelectedIcon;

    private Image image;
    private Color color;

    private bool CanSelect = true;

    private void Awake() {
        image = GetComponent<Image>();
        color = image.color;
    }

    public void SetSelectable(bool canSelect) {
        CanSelect = canSelect;

        if (RoomPlayer.Local == null) return;

        SelectedIcon.SetActive(RoomPlayer.Local.KartColor == KART_COLOR);

        if (CanSelect || RoomPlayer.Local.KartColor == KART_COLOR)
            image.color = color;
        else
            image.color = color * 0.2f;
    }

    public void OnClickKartColor() {
        if (CanSelect) {
            // 1. 해당 카트 타입을 아무도 고르지 않아 선택하는 경우
            RoomPlayer.Local.RPC_SetKartColor(KART_COLOR);
        } else if (RoomPlayer.Local.KartColor == KART_COLOR) {
            // 2. 해당 카트 타입을 본인이 고르고 있다가 선택 해제하는 경우
            RoomPlayer.Local.RPC_SetKartColor(KART_COLOR_EMPTY);
        }

        // 3. 그 외의 경우는 무시한다
    }
}
