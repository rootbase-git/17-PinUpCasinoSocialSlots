using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSInit : MonoBehaviour
{
    void Start () {
        // Uncomment this method to enable OneSignal Debugging log output 
        OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.NONE);
  
        // Replace 'YOUR_ONESIGNAL_APP_ID' with your OneSignal App ID.
        OneSignal.StartInit("d9a15521-0f6d-4cb5-b561-dfe3db257518")
            .HandleNotificationOpened(HandleNotificationOpened)
            .Settings(new Dictionary<string, bool>() {
                { OneSignal.kOSSettingsAutoPrompt, false },
                { OneSignal.kOSSettingsInAppLaunchURL, false } })
            .EndInit();
  
        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
  
        // The promptForPushNotifications function code will show the iOS push notification prompt. We recommend removing the following code and instead using an In-App Message to prompt for notification permission.
        OneSignal.PromptForPushNotificationsWithUserResponse(OneSignal_promptForPushNotificationsResponse);
  
        void OneSignal_promptForPushNotificationsResponse(bool accepted) {
            Debug.Log("OneSignal_promptForPushNotificationsResponse: " + accepted);
        }
    }

// Gets called when the player opens the notification.
    private static void HandleNotificationOpened(OSNotificationOpenedResult result) {
    }
}
