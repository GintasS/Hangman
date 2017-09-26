using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;


public class AdsScriptas : MonoBehaviour {

    // Script that is responsible for managing ad system.

    #region [Ads]: Variables
    // Most important ad variables.
    [Header("Main Ad variables")]
    [SerializeField] private string AppId;
    [SerializeField] private string RewardVideoAdUnitId;
    [SerializeField] private string InterstitialAdUnitId;

    // Interstitial variables for events.
    [Header("Interstitial ad variables")]
    public int InterstitialAdTryingToLoad;
    public int InterstitialAdIsDisplaying;
    public int InterstitialAdIsLoaded;
    public int InterstitialAdWasClosed;

    public string InterstitialOriginatedFrom;

    // Reward video ad variables, including score given per watched video and etc.
    [Header("Reward Video ad variables")]
    public int RewardVideoSuccess;
    [Range(0,10000)] public int RewardVideoMinRewardPoints;
    [Range(0,10000)] public int RewardVideoMaxRewardPoints;

    public float RewardVideoEndTimer;
    [Range(1, 10)]  public float RewardVideoEndTimerDefault;

    public bool RewardVideoTimerEnabled,
                RewardVideoRewardWasGiven;

    private InterstitialAd interstitial;
    private RewardBasedVideoAd rewardBasedVideo;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private CustomLogger Cus;
    #endregion

    #region [Ads]: Start function
    // Initialize ad variables.
    void Start ()
    {
        Cus.LogAMessage("[ADmob]: Starting Ads Script...\n");
 
        InterstitialOriginatedFrom = "";

        InterstitialAdIsDisplaying = -1;
        InterstitialAdIsLoaded = -1;
        InterstitialAdWasClosed = -1;
        InterstitialAdTryingToLoad = -1;

        RewardVideoEndTimer = RewardVideoEndTimerDefault;

        RewardVideoSuccess = -1;

        RewardVideoTimerEnabled = false;

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(AppId);    
    }
    #endregion

    #region [Ads]: Create an Ad request
    // Function that creates a request for an ad.
    private AdRequest CreateAdRequest()
    {
        Cus.LogAMessage("[ADmob]: Creating Ad request...\n");


    }
    #endregion

    #region [Ads]: Request an Ad

    // Request an Interstitial ad.
    public void RequestInterstitial()
    {
        Cus.LogAMessage("[INTERSTITIAL AD]: Requesting Interstitial ad...\n");

        InterstitialAdIsDisplaying = -1;
        InterstitialAdIsLoaded = -1;
        InterstitialAdWasClosed = -1;

        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        // Create an interstitial.
        this.interstitial = new InterstitialAd(InterstitialAdUnitId);

        // Register for ad events.
        this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
        this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
        this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
        this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
        this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

        // Load an interstitial ad.
        this.interstitial.LoadAd(this.CreateAdRequest());
    }
    // Request an Reward Based Video ad.
    public void RequestRewardBasedVideo()
    {
        Cus.LogAMessage("[ADmob]: Requesting Reward video ad...\n");

        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
        this.rewardBasedVideo.OnAdLoaded += this.HandleRewardBasedVideoLoaded;
        this.rewardBasedVideo.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoad;
        this.rewardBasedVideo.OnAdOpening += this.HandleRewardBasedVideoOpened;
        this.rewardBasedVideo.OnAdStarted += this.HandleRewardBasedVideoStarted;
        this.rewardBasedVideo.OnAdRewarded += this.HandleRewardBasedVideoRewarded;
        this.rewardBasedVideo.OnAdClosed += this.HandleRewardBasedVideoClosed;
        this.rewardBasedVideo.OnAdLeavingApplication += this.HandleRewardBasedVideoLeftApplication;

        this.rewardBasedVideo.LoadAd(this.CreateAdRequest(), RewardVideoAdUnitId);
    }

    #endregion

    #region  [Ads]: Show an Ad
    // Show an Interstitial ad after loading it.
    public void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            Cus.LogAMessage("[INTERSTITIAL AD]: Showing Interstitial ad...\n");
            this.interstitial.Show();
        }
        else
        {
            Cus.LogAMessage("[INTERSTITIAL AD]: Interstitial ad is not ready yet...\n");
        }
    }
    // Show an Reward Based Video ad after loading it.
    public void ShowRewardBasedVideo()
    {
        if (this.rewardBasedVideo.IsLoaded())
        {
            Cus.LogAMessage("Showing the reward video\n");
            this.rewardBasedVideo.Show();
        }
        else
        {
            Cus.LogAMessage("Reward video is not ready\n");
        }
    }

    #endregion

    #region [Ads]: Interstitial callbacks
    // Interstitial callback: ad was loaded.
    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        Cus.LogAMessage("[ADmob][INTERSTITIAL AD]: Ad was loaded successfully ...\n");
        InterstitialAdIsLoaded = 1;
        ShowInterstitial();
    }
    // Interstitial callback: ad failed to load.
    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Cus.LogAMessage("[ADmob][INTERSTITIAL AD]: Ad failed to load because " + args.Message +"...\n");
        InterstitialAdIsLoaded = 0; 
    }
    // Interstitial callback: ad was opened.
    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        Cus.LogAMessage("[ADmob][INTERSTITIAL AD]: Ad was openend...\n");
        InterstitialAdIsDisplaying = 1;
    }
    // Interstitial callback: ad was closed.
    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        Cus.LogAMessage("[ADmob][INTERSTITIAL AD]: Ad was closed...\n");
        InterstitialAdIsDisplaying = 0;
    }
    // Interstitial callback: user left the app.
    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        Cus.LogAMessage("[ADmob][INTERSTITIAL AD]: Left application...\n");
    }
    #endregion

    #region  [Ads]: RewardBasedVideo callbacks
    // Reward Based Video callback: ad was loaded.
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        Cus.LogAMessage("[ADmob][Reward Video AD]: Ad was loaded successfully.\n");
        ShowRewardBasedVideo();
    }
    // Reward Based Video callback: ad failed to load.
    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Cus.LogAMessage("[ADmob][Reward Video AD]: Ad failed to load because " + args.Message+"...\n");
        RewardVideoSuccess = 0;
    }
    // Reward Based Video callback: ad was opened.
    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        Cus.LogAMessage("[ADmob][Reward Video AD]: Video Ad was opened ...\n");
    }
    // Reward Based Video callback: video was started.
    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        Cus.LogAMessage("[ADmob][Reward Video AD]: Video Ad was started...\n");
    }
    // Reward Based Video callback: ad was closed.
    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        Cus.LogAMessage("[ADmob][Reward Video AD]: Video Ad was closed..\n");
        RewardVideoSuccess = 0;
    }
    // Reward Based Video callback: reward the user.
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        RewardVideoSuccess = 1;
        RewardVideoRewardWasGiven = true;

        Cus.LogAMessage("[ADmob][Reward Video AD]: Got reward from Reward Video Ad, amount: " + amount + " , type: " + type +"..\n");
    }
    // Reward Based Video callback: user left the app.
    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        Cus.LogAMessage("[ADmob][Reward Video AD]: Left Ad..\n");
    }
    // Reward Based Video: generate random value reward between the ranges.
    public int PointsFromRewardVideo()
    {
        System.Random rnd = new System.Random();
        int sk = rnd.Next(RewardVideoMinRewardPoints, RewardVideoMaxRewardPoints);

        return sk;
    }

    #endregion

}
