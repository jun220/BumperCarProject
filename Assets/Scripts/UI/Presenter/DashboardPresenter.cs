using BumperCarProject.UI.View;

namespace BumperCarProject.UI.Presenter
{
    public class DashboardPresenter 
    {
        private readonly DashboardView _dashboardView;

        public DashboardPresenter(DashboardView dashboardView) {
            _dashboardView = dashboardView;
        }

        public void UpdateCurSpeed(float speed) {
            _dashboardView.UpdateSpeedText(speed);
        }

        public void UpdateCurDamage(float damage) {
            _dashboardView.UpdateDamageText(damage);
        }

        public void UpdateBoostCoolTimeText(float remainingCoolTime) {
            _dashboardView.UpdateBoostCoolTimeText(remainingCoolTime);
        }

        public void UpdateBoostCoolTimeImage(float remainingCoolTimeRate) {
            _dashboardView.UpdateBoostCoolTimeImage(remainingCoolTimeRate);
        }
    }
}