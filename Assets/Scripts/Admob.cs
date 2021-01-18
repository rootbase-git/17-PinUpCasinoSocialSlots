using UnityEngine;
using GoogleMobileAds.Api;
using System;
using GameScripts;

public class Admob : MonoBehaviour
{
	//private BannerView bannerView;
	private InterstitialAd interstitial;

	[SerializeField] private string idApp;
	[SerializeField] private string adId;
	private void Start ()
	{
		MobileAds.Initialize(idApp);
		//RequestBannerAd();
	}

	#region AdMethods
	private void RequestBannerAd()
	{
        interstitial = new InterstitialAd(adId);
        
        AdRequest request = new AdRequest.Builder().Build();
        
        interstitial.LoadAd(request);
        interstitial.OnAdLoaded += HandleOnAdLoaded;
        interstitial.OnAdClosed += DestroyBannerAd;
	}

	private void DestroyBannerAd()
	{
		interstitial?.Destroy ();
	}
	private void DestroyBannerAd(object a, EventArgs args)
	{
		interstitial?.Destroy();
	}
	#endregion

	private void HandleOnAdLoaded(object a, EventArgs args)
	{
		interstitial.Show();
	}

	private void OnDestroy ()
	{
		DestroyBannerAd();
	}

	private void OnEnable()
	{
		CoreLogic.ActivateAdss += RequestBannerAd;
	}

	private void OnDisable()
	{
		CoreLogic.ActivateAdss -= RequestBannerAd;
	}
}
