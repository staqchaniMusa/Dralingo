
using CBS.Scriptable;

namespace CBS.UI
{
    public class PopupViewer
    {
        public void ShowSimplePopup(PopupRequest request)
        {
            var uiData = CBSScriptable.Get<PopupPrefabs>();
            var popupPrefab = uiData.SimplePopup;
            var popupObject = UIView.ShowWindow(popupPrefab);
            popupObject.GetComponent<SimplePopup>().Setup(request);
        }

        public void ShowYesNoPopup(YesOrNoPopupRequest request)
        {
            var uiData = CBSScriptable.Get<PopupPrefabs>();
            var popupPrefab = uiData.YesNoPopup;
            var popupObject = UIView.ShowWindow(popupPrefab);
            popupObject.GetComponent<YesNoPopup>().Setup(request);
        }

        public void ShowEditTextPopup(EditTextPopupRequest request)
        {
            var uiData = CBSScriptable.Get<PopupPrefabs>();
            var popupPrefab = uiData.EditTextPopup;
            var popupObject = UIView.ShowWindow(popupPrefab);
            popupObject.GetComponent<EditTextPopup>().Setup(request);
        }

        public void ShowUserInfo(string profileIDToShow)
        {
            
        }

       
        public void ShowLoadingPopup()
        {
            var uiData = CBSScriptable.Get<PopupPrefabs>();
            var loadingPrefab = uiData.LoadingPopup;
            UIView.ShowWindow(loadingPrefab);
        }

        public void HideLoadingPopup()
        {
            var uiData = CBSScriptable.Get<PopupPrefabs>();
            var loadingPrefab = uiData.LoadingPopup;
            UIView.HideWindow(loadingPrefab);
        }

        public void ShowRewardPopup(PopupRequest request)
        {
            var uiData = CBSScriptable.Get<PopupPrefabs>();
            var rewardPrefab = uiData.RewardPopup;
            var popupObject = UIView.ShowWindow(rewardPrefab);
            popupObject.GetComponent<RewardPopup>().Setup(request);
            // popupObject.GetComponent<RewardDrawer>().Display(prize);
        }

    }
}
