using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AppsFlyerSDK;
using Facebook.Unity;
using Ugi.PlayInstallReferrerPlugin;
using UnityEngine;
using UnityEngine.Networking;

public class DataFetcher : MonoBehaviour
{
    [SerializeField] private AppsFlyerObjectScript _appsFlyer;
    private SceneLoader _sceneLoader;
    public string cloUrl;
    private string _jsonCloResponse;

    private const string IsUserKey = "IsUser";
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
        if (allowed)
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

            if (cloData == null || cloData.user == false)
            {
                preSave();
                return;
            }
            if (cloData.deeplink)
            {
                RequestTrackLinkDataFromDeepLink(deepLinkData =>
                {
                    if (deepLinkData != null)
                    {
                        GenerateLink(cloData, deepLinkData, preSave);
                        return;
                    }

                    CheckNamingOrCreateOrganicData(cloData, preSave);
                });
            }
            else
            {
                CheckNamingOrCreateOrganicData(cloData, preSave);
            }
        });
     
        _cloRequestEnumerator = RequestCloData(cloDataCallback);
        StartCoroutine(_cloRequestEnumerator);
    }

    private IEnumerator RequestCloData(Action<CloData> cloDataCallback)
    {
        var webRequest = UnityWebRequest.Get(cloUrl);
        webRequest.SetRequestHeader("user-agent","okhttp/3.14.9");
        
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            cloDataCallback(null);
            yield break;
        }

        string cloResponse = webRequest.downloadHandler.text;

        var cloData = SerializeCloData(cloResponse);

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
        _appsFlyer.GetConversionData((conversionData) =>
        {
            if (GetTrackLink() != null) return;
            
            if (conversionData == null)
            {
                trackLinkCallback(null);
                return;
            }

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
                                var position = conversionData.ContainsKey(source.key.name) ? conversionData[source.key.name].ToString()
                                    .Split(source.key.delimiter.ToCharArray()[0])
                                    .GetValue(source.key.position) : null;
                                key = (position ?? "none") as string;
                            }
                            else
                            {
                                key = conversionData.ContainsKey(source.key.name) ? (string) conversionData[source.key.name] : "none";
                            }

                            if (string.IsNullOrEmpty(key) || key.Equals("none"))
                            {
                                trackLinkCallback(null);
                                return;
                            }

                            string sub1;
                            if (source.sub1.split)
                            {
                                var position = conversionData.ContainsKey(source.sub1.name) ? conversionData[source.sub1.name].ToString()
                                    .Split(source.sub1.delimiter.ToCharArray()[0])
                                    .GetValue(source.sub1.position):null;
                                sub1 = (position ?? "none") as string;
                            }
                            else
                            {
                                sub1 = conversionData.ContainsKey(source.sub1.name) ? (string) conversionData[source.sub1.name] : "none";
                            }

                            string sub2;
                            if (source.sub2.split)
                            {
                                var position = conversionData.ContainsKey(source.sub2.name) ? conversionData[source.sub2.name].ToString()
                                    .Split(source.sub2.delimiter.ToCharArray()[0])
                                    .GetValue(source.sub2.position):null;
                                sub2 = (position ?? "none") as string;
                            }
                            else
                            {
                                sub2 = conversionData.ContainsKey(source.sub2.name) ? (string) conversionData[source.sub2.name] : "none";
                            }

                            string sub3;
                            if (source.sub3.split)
                            {
                                var position = conversionData.ContainsKey(source.sub3.name) ? conversionData[source.sub3.name].ToString()
                                    .Split(source.sub3.delimiter.ToCharArray()[0])
                                    .GetValue(source.sub3.position):null;
                                sub3 = (position ?? "none") as string;
                            }
                            else
                            {
                                sub3 = conversionData.ContainsKey(source.sub3.name) ? (string) conversionData[source.sub3.name] : "none";
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
                    //Debug.Log("Naming is not found");
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
                Debug.Log(appLinkResult?.Error + "Deep link is empty");
                trackLinkCallback(null);
            }
            else
            {
                try
                {
                    var deepLink = appLinkResult.TargetUrl.Split('?')[1];
                    
                    var query = HttpUtility.ParseQueryString(deepLink);
                    var key = query.Get("key") ?? "NoKey";
                    var sub1 = query.Get("sub1") ?? "NoSub1";
                    var sub2 = query.Get("sub2") ?? "NoSub2";
                    var sub3 = query.Get("sub3") ?? "NoSub3";

                    trackLinkCallback(new TrackLinkData {key = key, source = "fb", sub1 = sub1, sub2 = sub2, sub3 = sub3});
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "EXCEPTION!");
                    trackLinkCallback(null);
                }
            } 
        });
    }

    private void GenerateLink(CloData cloData, TrackLinkData trackLinkData, Action linkGeneratedCallback)
    {
        RequestAdvertiserId(advertiserId =>
        {
            RequestAppMetricaDeviceId((metricaId) =>
            {
                var queryMap = new Dictionary<string, string>();

                queryMap["key"] = HttpUtility.UrlEncode(trackLinkData.key, Encoding.UTF8);

                if (!string.IsNullOrEmpty(trackLinkData.sub1))
                    queryMap["sub1"] = HttpUtility.UrlEncode(trackLinkData.sub1, Encoding.UTF8);
                if (!string.IsNullOrEmpty(trackLinkData.sub2))
                    queryMap["sub2"] = HttpUtility.UrlEncode(trackLinkData.sub2, Encoding.UTF8);
                if (!string.IsNullOrEmpty(trackLinkData.sub3))
                    queryMap["sub3"] = HttpUtility.UrlEncode(trackLinkData.sub3, Encoding.UTF8);

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
                        Console.WriteLine(e + "EXCEPTION!!!!!!!!!!!!!!");
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

        saveCallback();
        yield return null;
    }

    private void RequestAppMetricaDeviceId(Action<string> metricaIdCallback)
    {
#if UNITY_EDITOR
        metricaIdCallback("none");
        return;
#endif
        AppMetrica.Instance.RequestAppMetricaDeviceID((s, error) =>
        {
            metricaIdCallback(string.IsNullOrEmpty(s) ? "none" : s);
        });
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
                    advertiserIdCallback("none");
                }
                else
                {
                    advertiserIdCallback(advertisingId);
                }
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
[Serializable]
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