using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartStateUI : MonoBehaviour
{
    [SerializeField] private TMP_Text Speed;
    [SerializeField] private GameObject DashIcon;
    [SerializeField] private Image DashCoolImage;
    [SerializeField] private TMP_Text DashCoolText;

    private int PlayerID = -1;
    private float DashCoolTime = 0.0f;

    private void Awake() {
        CartControl.OnDash += OnDash;
        CartControl.KnockedOut += KnockedOut;
    }

    private void Update() {
        if (PlayerID == -1) return;
        if (GameManager.Carts.Count <= PlayerID) return;

        Speed.text = string.Format("{0:N2}", GameManager.Carts[PlayerID].Speed);
    }

    public void Initialize(int playerID, float dashCoolTime) {
        PlayerID = playerID;
        DashCoolTime = dashCoolTime;
    }

    private void OnDash(int playerID) {
        if (PlayerID != playerID) return;

        DashCoolImage.fillAmount = 1;
        DashCoolText.text = string.Format("{0:N1}s", DashCoolTime);
        DashCoolText.gameObject.SetActive(true);

        StartCoroutine(CountDashCool());
    }

    private void KnockedOut(int playerID) {
        if (PlayerID != playerID) return;

        Speed.gameObject.SetActive(false);
        DashIcon.SetActive(false);

        CartControl.OnDash -= OnDash;
    }

    private IEnumerator CountDashCool() {
        float timer = DashCoolTime;

        while (timer > 0.0f) {
            timer -= 0.1f;

            DashCoolImage.fillAmount = timer / DashCoolTime;
            DashCoolText.text = string.Format("{0:N1}s", timer);

            yield return new WaitForSeconds(0.1f);
        }

        DashCoolImage.fillAmount = 0;
        DashCoolText.gameObject.SetActive(false);
    }
}
