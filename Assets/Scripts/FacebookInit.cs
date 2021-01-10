using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Facebook.Unity.Example;
using UnityEngine;

public class FacebookInit : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        FB.Init(() =>
        {
            OnInitComplete();
            FB.ActivateApp();
            FB.Mobile.FetchDeferredAppLinkData(deeplink =>
            {
                Debug.Log("_________________");
                Debug.Log(deeplink.Url);
                Debug.Log("_________________");
            });
        });
        // TODO advertiser id info
    }
    private void OnInitComplete()
    {
        var logMessage = $"OnInitCompleteCalled IsLoggedIn='{FB.IsLoggedIn}' IsInitialized='{FB.IsInitialized}'";
        LogView.AddLog(logMessage);
        if (AccessToken.CurrentAccessToken != null)
        {
            LogView.AddLog(AccessToken.CurrentAccessToken.ToString());
        }
    }
}
