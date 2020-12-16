using System;
using System.Globalization;
using TMPro;
using UnityEngine;
public class APITest : MonoBehaviour
{
    public TMP_Text apiInfo;
    public TMP_Text timeFromStartUp;
    private void Awake()
    {
        apiInfo.SetText("Application name is  " + 
                        Application.productName + " with identifier  " + Application.identifier 
                        +" saved in "+ Application.dataPath +" and made in Unity "+ Application.unityVersion);
    }

    private void TimeSinceStartup()
    {
        var timeSpan = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
        var timeText = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}:{timeSpan.Milliseconds:D2}";
        timeFromStartUp.SetText(timeText);
    }

    private void Update()
    {
        TimeSinceStartup();
    }
}
