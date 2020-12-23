using UnityEngine;

public class OSInit : MonoBehaviour
{
    private const string AppId = "d9a15521-0f6d-4cb5-b561-dfe3db257518";
    private void Start () {
        // Uncomment this method to enable OneSignal Debugging log output 
        OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.NONE);
  
        OneSignal.StartInit(AppId)
            .EndInit();
        
        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
    }
}
