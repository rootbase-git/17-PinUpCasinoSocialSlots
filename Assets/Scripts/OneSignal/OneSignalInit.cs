using UnityEngine;

public class OneSignalInit: MonoBehaviour
{
    public string appId;
    private void Start()
    {
        OneSignal.StartInit(appId)
            .EndInit();

        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
    }
}
