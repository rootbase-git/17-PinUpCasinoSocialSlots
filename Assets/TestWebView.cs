using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWebView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var webView = gameObject.AddComponent<WebViewObject>();
        webView.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
