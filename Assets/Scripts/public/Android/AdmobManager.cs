using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

//* TEST REWARD AD ID
// Android : ca-app-pub-3940256099942544/5224354917
// iOS : ca-app-pub-3940256099942544/1712485313

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
            OnGetRewardAd.Invoke();
            OnGetRewardAd = () => {}; // 초기화
        };
        rewardAd.OnAdFailedToLoad +=  (sender, e) =>
        {
            // 리워도 광고로드 실패
            GM._.ui.ShowWarningMsgPopUp("(언어변역하기)광고로드 실패");
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
    /// 리워드광고 표시
    /// </summary>
    public bool ShowRewardAd()
    {
        if(!rewardAd.IsLoaded())
        {
            GM._.ui.ShowWarningMsgPopUp("(언어변역하기)광고로드중.. 잠시후 다시 실행해주세요.");
            return false;
        }

        rewardAd.Show();
        LoadRewardAd();

        return true;
    }
}
