using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;

//Banner ad
public class Admob : MonoBehaviour
{
	private BannerView bannerView;

	[SerializeField] private string idApp, idBanner;
	private void Start ()
	{
		List<string> deviceIds = new List<string>();
		deviceIds.Add("243ba7cc74032223f05eob1c1e47fa69");
		RequestConfiguration requestConfiguration = new RequestConfiguration
				.Builder()
			.SetTestDeviceIds(deviceIds)
			.build();
		
		MobileAds.SetRequestConfiguration(requestConfiguration);
		
		MobileAds.Initialize(idApp);
		Debug.Log("init");

		RequestBannerAd();
	}

	#region Banner Methods
	public void RequestBannerAd()
	{
		// replace this id with your orignal admob id for banner ad
        //string adUnitId = "ca-app-pub-3940256099942544/6300978111";
 
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(idBanner, AdSize.Banner, AdPosition.Bottom);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
        bannerView.OnAdLoaded += HandleOnAdLoaded;
	}

	public void DestroyBannerAd ()
	{
		bannerView?.Destroy ();
	}

	#endregion

	
	//------------------------------------------------------------------------
	AdRequest AdRequestBuild ()
	{
		return new AdRequest.Builder ().Build ();
	}
	void HandleOnAdLoaded(object a, EventArgs args)
	{
		print("loaded");
		bannerView.Show();
	}

	void OnDestroy ()
	{
		DestroyBannerAd ();
	}
}
