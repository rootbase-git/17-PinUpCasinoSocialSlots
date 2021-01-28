using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Facebook.Unity.Example;
using UnityEngine;

public class FacebookInit : MonoBehaviour
{
    private void Awake()
    {
        if (!FB.IsInitialized){
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        } else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }


    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        } else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        Time.timeScale = !isGameShown ? 0 : 1;
    }
}
