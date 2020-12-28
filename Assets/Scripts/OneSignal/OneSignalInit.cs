using UnityEngine;

public class OneSignalInit: MonoBehaviour
{
    public string appId = "d9a15521-0f6d-4cb5-b561-dfe3db257518";
    private void Start()
    {
        OneSignal.StartInit(appId)
            .EndInit();
        
        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
    }
}
