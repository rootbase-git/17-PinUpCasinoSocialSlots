using System;
using TMPro;
using UnityEngine;
public class APITest : MonoBehaviour
{
    public TMP_Text apiInfo;
    public TMP_Text timeFromStartUp;
    private const string JsonExample = "{\"userId\": 1,\"id\": 1,\"title\": \"delectus aut autem\",\"completed\": false}";

    private void Awake()
    {
        var userInfo = JsonUtility.FromJson<UserInfo>(JsonExample);
        apiInfo.SetText("UserID: " + userInfo.userId +  "\n Title: "+ userInfo.title + "\n Completed: " + userInfo.isCompleted);
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

[Serializable]
public class UserInfo
{
    public int userId;
    public int id;
    public string title;
    public bool isCompleted;
}
