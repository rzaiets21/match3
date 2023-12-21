using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace ITSoft {
    public class AdsManager : MonoBehaviour
    {
        [SerializeField] private string interId;
        [SerializeField] private string rewardedId;
        
        public static Action OnCompleteRewardVideo;
        public static Action OnCompleteInterVideo;

        private InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;
        
        private static bool removeAds = false;

        private static AdsManager instance;
        
        private void Awake()
        {
#if UNITY_EDITOR
            //GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.coins, -(int)GGPlayerSettings.instance.walletManager.CurrencyCount(CurrencyType.coins));   
#endif
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            bool.TryParse(PlayerPrefs.GetString("addfreechk", "false"), out removeAds);

            Init();
            LoadInterstitial();
        }

        private void ErrorShowingReward(object sender, EventArgs e)
        {
            CreateAndLoadRewardAd();
        }
        
        private void LoadInterstitial()
        {
            CreateAndLoadInterAd();
        }

        private void OnDisable()
        {
            interstitialAd.OnAdFullScreenContentClosed -= OnInterstitialAdFullScreenContentClosed;
            interstitialAd.OnAdFullScreenContentFailed -= OnInterstitialAdFullScreenContentFailed;
            rewardedAd.OnAdFullScreenContentClosed -= OnRewardedAdFullScreenContentClosed;
            rewardedAd.OnAdFullScreenContentFailed -= OnRewardedAdFullScreenContentFailed;
        }

        private void Init()
        {
            MobileAds.Initialize(initStatus => { });

            CreateAndLoadInterAd();
            CreateAndLoadRewardAd();
        }

        private void CreateAndLoadInterAd()
        {
            if(Application.internetReachability == NetworkReachability.NotReachable)
                return;
            
            if (interstitialAd != null)
            {
                interstitialAd.OnAdFullScreenContentClosed -= OnInterstitialAdFullScreenContentClosed;
                interstitialAd.OnAdFullScreenContentFailed -= OnInterstitialAdFullScreenContentFailed;
                interstitialAd.Destroy();
                interstitialAd = null;
            }
            
            var request = new AdRequest();
            
            InterstitialAd.Load(interId, request,
                (ad, error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    interstitialAd = ad;
                    
                    interstitialAd.OnAdFullScreenContentClosed += OnInterstitialAdFullScreenContentClosed;
                    interstitialAd.OnAdFullScreenContentFailed += OnInterstitialAdFullScreenContentFailed;
                });
        }

        private void OnInterstitialAdFullScreenContentClosed()
        {
            CreateAndLoadInterAd();
            InterVideoAdRewardedEvent();
        }

        private void OnInterstitialAdFullScreenContentFailed(AdError error)
        {
            Debug.LogError(error);
            CreateAndLoadRewardAd();
        }

        private void CreateAndLoadRewardAd()
        {
            if(Application.internetReachability == NetworkReachability.NotReachable)
                return;
            
            if (rewardedAd != null)
            {
                rewardedAd.OnAdFullScreenContentClosed -= OnRewardedAdFullScreenContentClosed;
                rewardedAd.OnAdFullScreenContentFailed -= OnRewardedAdFullScreenContentFailed;
                
                rewardedAd.Destroy();
                rewardedAd = null;
            }
            var request = new AdRequest();

            RewardedAd.Load(rewardedId, request,
                (ad, error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    rewardedAd = ad;
                    rewardedAd.OnAdFullScreenContentClosed += OnRewardedAdFullScreenContentClosed;
                    rewardedAd.OnAdFullScreenContentFailed += OnRewardedAdFullScreenContentFailed;
                });
        }

        private void OnRewardedAdFullScreenContentClosed()
        {
            CreateAndLoadRewardAd();
        }

        private void OnRewardedAdFullScreenContentFailed(AdError error)
        {
            Debug.LogError(error);
            CreateAndLoadRewardAd();
        }

        void RewardedVideoAdRewardedEvent(Reward args)
        {
            OnCompleteRewardVideo?.Invoke();
        }

        void InterVideoAdRewardedEvent()
        {
            OnCompleteInterVideo?.Invoke();
            OnCompleteInterVideo = null;
        }

        public static void RemoveAds()
        {
            removeAds = true;
        }
        
        public static bool RewardIsReady() => instance.rewardedAd != null && instance.rewardedAd.CanShowAd();
        public static bool InterIsReady() => instance.interstitialAd != null && instance.interstitialAd.CanShowAd();
        
        public static void ShowRewarded()
        {
            Debug.Log("Show reward video. Reward video is" + (RewardIsReady() ? "" : " not") + " ready");
            // if (removeAds)
            // {
            //     OnCompleteRewardVideo?.Invoke();
            //     return;
            // }
            if (RewardIsReady())
            {
                instance.rewardedAd.Show(instance.RewardedVideoAdRewardedEvent);
            }
            else
            {
                #if UNITY_EDITOR
                OnCompleteRewardVideo?.Invoke();
                #endif
                instance.CreateAndLoadRewardAd();
            }
        }

        private void ErrorShowingReward(object sender, AdErrorEventArgs args)
        {
            rewardedAd.Destroy();
            CreateAndLoadRewardAd();
        }
        
        public static void ShowInterstitial(System.Action ViewComplete = null)
        {
#if UNITY_EDITOR
            Debug.Log("Show Inter");
            ViewComplete?.Invoke();
            return;
#endif
            if (removeAds)
            {
                ViewComplete?.Invoke();
                return;
            }
            if (InterIsReady())
            {
                Debug.Log("unity-script: IronSource.Agent.isInterstitialReady - True");
                OnCompleteInterVideo = ViewComplete;
                instance.interstitialAd.Show();
            }
            else
            {
                Debug.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
                instance.LoadInterstitial();
                ViewComplete?.Invoke();
            }
        }
    }
}
