using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
using JetBrains.Annotations;

// This class is intended to be used the the AppsFlyerObject.prefab

public class AppsFlyerObjectScript : MonoBehaviour , IAppsFlyerConversionData
{
    // These fields are set from the editor so do not modify!
    //******************************//
    public string devKey;
    public string appID;
    public bool isDebug;
    public bool getConversionData;
    [CanBeNull] public Dictionary<string, object> conversionDataDictionary;
    [CanBeNull] public Action<Dictionary<string, object>> conversionDataCallback;
        //******************************//

    private void Start()
    {
        // These fields are set from the editor so do not modify!
        //******************************//
        AppsFlyer.setIsDebug(isDebug);
        AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);
        //******************************//
        AppsFlyer.startSDK();
    }

    public void GetConversionData(Action<Dictionary<string, object>> callback)
    {
        callback(conversionDataDictionary);
    }

    // Mark AppsFlyer CallBacks
    public void onConversionDataSuccess(string conversionData)
    {
        if(isDebug)
            AppsFlyer.AFLog("didReceiveConversionData", conversionData);
        conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        conversionDataCallback?.Invoke(conversionDataDictionary);
        // add deferred deeplink logic here
    }

    public void onConversionDataFail(string error)
    {
        if(isDebug)
            AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        if(isDebug)
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        if(isDebug)
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }
}
