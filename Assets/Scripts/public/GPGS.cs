using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGS : MonoBehaviour
{
    public static GPGS _;
    private bool isLoginSucceed;

    public void Start() {
        _ = this;

        // 자동 로그인 시작
        // PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

#region FUNC
    public void ShowLeaderBoard()
    {
        if(!isLoginSucceed)
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        else
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    private void ProcessAuthentication(SignInStatus status) {
        if (status == SignInStatus.Success) {
            isLoginSucceed = true;
            GM._.ui.ShowNoticeMsgPopUp("GOOGLE LOGIN SUCCESS");

            //* 최대층수총합 기록갱신
            if(DM._.DB.bestTotalFloor < DM._.DB.stageDB.GetTotalBestFloor())
                SetBestTotalFloor(DM._.DB.stageDB.GetTotalBestFloor());

            //* 시련의광산 기록갱신
            if(DM._.DB.challengeBestFloor < GM._.clm.BestFloor)
                SetBestChallengeFloor(GM._.clm.BestFloor);

            // 리더보드 열람
            PlayGamesPlatform.Instance.ShowLeaderboardUI();

        } else {
            GM._.ui.ShowNoticeMsgPopUp("GOOGLE LOGIN FAIL");
        }
    }

    /// <summary>
    /// 베스트 일반광산 전체층수 총합
    /// </summary>
    public void SetBestTotalFloor(int val)
    {
        // 리더보드 최신화
        PlayGamesPlatform.Instance.ReportScore(val, GPGSIds.leaderboard_best_totalfloor, (bool success) => {});
    }

    /// <summary>
    /// 베스트 시련의광산 총합
    /// </summary>
    public void SetBestChallengeFloor(int val)
    {
        PlayGamesPlatform.Instance.ReportScore(val, GPGSIds.leaderboard_best_challengefloor, (bool success) => {});
    }

#endregion
}
