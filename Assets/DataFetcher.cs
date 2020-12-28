using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataFetcher : MonoBehaviour
{
    public GameObject webView;
    public string url;
    private string _jsonResult;

    private const string IsUserKey = "IsUser";
    private const string JsonInfoKey = "JsonInfo";
    private void Start()
    {
        //Debug.Log(GetInstallReferrer());
        if(PlayerPrefs.HasKey(JsonInfoKey))
        {
            _jsonResult = PlayerPrefs.GetString(JsonInfoKey);
            Debug.Log(IsUser(_jsonResult));

            webView.SetActive(IsUser(_jsonResult));
        }
        else
        {
            Debug.Log(PlayerPrefs.HasKey(JsonInfoKey));

            StartCoroutine(SendRequestByUrl(url,SaveJsonResult));
        }
    }

    #region JsonCallback
    private IEnumerator SendRequestByUrl(string serverUrl, Action<string> callback)
    {
        using (var webRequest = UnityWebRequest.Get(serverUrl))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                callback(webRequest.downloadHandler.text);
            }
        }
    }
    private void SaveJsonResult(string jsonResult)
    {
        if (string.IsNullOrEmpty(jsonResult))
        {
            Debug.Log("Json is null or empty");
        }
        else
        {
            PlayerPrefs.SetString(JsonInfoKey, jsonResult);
            PlayerPrefs.Save();
            
            _jsonResult = jsonResult;
            webView.SetActive(IsUser(_jsonResult));
        }
    }
    #endregion


    private static bool IsUser(string json)
    {
        if (PlayerPrefs.HasKey(IsUserKey))
        {
            return PlayerPrefs.GetInt(IsUserKey) != 0;
        }

        var data = JsonUtility.FromJson<UrlJsonData>(json);
        
        PlayerPrefs.SetInt(IsUserKey, data.user ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log(PlayerPrefs.GetInt(IsUserKey) != 0);
        return PlayerPrefs.GetInt(IsUserKey) != 0;
    }



    /*private string GetInstallReferrer()
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
    }*/
}

public class UrlJsonData
{
    public bool naming;
    public bool deeplink;
    public string integration_version;
    public string track_domain;
    public string organic;
    public bool user;
    public string media_sources;
}
