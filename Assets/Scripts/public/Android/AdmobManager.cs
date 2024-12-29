using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_ANDROID
using GoogleMobileAds.Api;
#endif

#if UNITY_ANDROID
public class AdmobManager : MonoBehaviour {
    public static AdmobManager _;
    public Action OnGetRewardAd = () => {}; // 리워드 광고보기 액션

    const string RewardAdTestID = "ca-app-pub-3940256099942544/5224354917";
    const string RewardAdID = "ca-app-pub-3908204064369314/8676974201";

    [SerializeField] public bool isTestMode;

    public RewardedAd rewardAd;

    void Awake() {
        // SINGLETON
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(_);
        }
        else
            Destroy(gameObject);
    }

    void Start() {
        var requestConfiguration = new RequestConfiguration.Builder().build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // 리워드광고 로드
        LoadRewardAd();
    }

    /// <summary>
    /// 리워드광고 로드
    /// </summary> <summary>
    /// 
    /// </summary>
    void LoadRewardAd()
    {
        rewardAd = new RewardedAd(isTestMode ? RewardAdTestID : RewardAdID);
        rewardAd.LoadAd(GetAdRequest());
        rewardAd.OnUserEarnedReward += (sender, e) => 
        {
            // 리워드 보상수령 (액션함수 구독)
            OnGetRewardAd?.Invoke();
            OnGetRewardAd = () => {}; // 초기화
        };
        rewardAd.OnAdFailedToLoad +=  (sender, e) =>
        {
            // 리워도 광고로드 실패
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.AdFailedToLoad));
        };
    }

    /// <summary>
    /// 리워드광고 요청
    /// </summary>
    AdRequest GetAdRequest() 
    {
        return new AdRequest.Builder().Build();
    }

    /// <summary>
    /// 리워드가 로드됬는지 확인
    /// </summary>
    public bool CheckIsLoadedAd()
    {
        if(!rewardAd.IsLoaded())
        {
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.LoadAdPlsRetryMsg));
            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// 리워드광고 표시
    /// </summary>
    public void ShowRewardAd()
    {
        rewardAd.Show();
        LoadRewardAd();
    }
}
#endif