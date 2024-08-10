using TMPro;
using UnityEngine;

public class OtherPlayerProfile : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI damageText;

    private int kartType;
    public int KartType => kartType; 

    private bool isKnockOuted;
    public bool IsKnockOuted => isKnockOuted;

    private void Start() {
        isKnockOuted = false;
    }

    public void Setup(string nickname, int kartType) {
        this.kartType = kartType;
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = nickname;
    }

    public void UpdateDamage(int kartType, float damage) {
        if (kartType != this.kartType || IsKnockOuted) return;
        var damageString = damage.ToString("N1").Split('.');

        damageText.text = $"{damageString[0]}<size=45>.{damageString[1]}%</size>";
    }

    public void SetKnockOut() {
        isKnockOuted = true;
        this.transform.GetChild(2).gameObject.SetActive(true);
    }
}
