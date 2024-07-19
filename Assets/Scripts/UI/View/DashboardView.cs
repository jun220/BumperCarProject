using BumperCarProject.UI.Model;
using BumperCarProject.UI.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BumperCarProject.UI.View
{
    public class DashboardView : MonoBehaviour
    {
        [SerializeField]
        private Text _speedText;
        [SerializeField]
        private Text _damageText;
        [SerializeField]
        private TMP_Text damageTMP;
        [SerializeField]
        private Text _boostCoolTime;
        [SerializeField]
        private Image _boostTimeImage;

        public static DashboardPresenter presenter;

        private void Awake() {
            presenter = new DashboardPresenter(this);
        }

        public void UpdateSpeedText(float curSpeed) {
            _speedText.text = curSpeed.ToString("N2");
        }

        public void UpdateDamageText(float curDamage) {
            damageTMP.text = curDamage.ToString("N1");
            _damageText.text = curDamage.ToString("N1");
        }

        public void UpdateBoostCoolTimeText(float coolTime) {
            _boostCoolTime.text = coolTime.ToString("N1");
        }

        public void UpdateBoostCoolTimeImage(float coolTimeRate) {
            _boostTimeImage.fillAmount = coolTimeRate;
        }
    }
}