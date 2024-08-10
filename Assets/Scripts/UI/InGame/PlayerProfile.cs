using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile : OtherPlayerProfile
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Image boostImage;
    [SerializeField] private TextMeshProUGUI boostCoolTimeText;

    private Color color;

    private void Start() {
        color = boostImage.color;
    }

    public void UpdateSpeedText(int kartType, float speed) {
        if (this.KartType != kartType || IsKnockOuted) return;

        speedText.text = speed.ToString("N1");
    }

    public void StartCountingBoostCoolTime(int kartType, float coolTime) {
        if (this.KartType != kartType || IsKnockOuted)
            return;

        StartCoroutine(CountCoolTime(coolTime));
        boostImage.color = color * 0.2f;
    }

    private IEnumerator CountCoolTime(float coolTime) {
        while (coolTime > 0.01f) {
            coolTime -= 0.1f;
            boostCoolTimeText.text = coolTime.ToString("N1") + "s";
            yield return new WaitForSeconds(0.1f);
        }

        boostCoolTimeText.text = string.Empty;
        boostImage.color = color;
    }
}