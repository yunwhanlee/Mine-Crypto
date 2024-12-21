using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGS : MonoBehaviour
{
    public static GPGS _;

    public void Start() {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

#region FUNC
    private void ProcessAuthentication(SignInStatus status) {
        if (status == SignInStatus.Success) {
            GM._.ui.ShowNoticeMsgPopUp("GOOGLE LOGIN SUCCESS");
            // Continue with Play Games Services
        } else {
            GM._.ui.ShowNoticeMsgPopUp("GOOGLE LOGIN FAIL");
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }

    public void ShowLeaderBoard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    /// <summary>
    /// 베스트 일반광산 전체층수 총합
    /// </summary>
    public void UpdateBestTotalFloor(int val)
    {
        // 리더보드 최신화
        PlayGamesPlatform.Instance.ReportScore(val, GPGSIds.leaderboard_besttotalfloor, (bool success) => {});
    }

    /// <summary>
    /// 베스트 시련의광산 총합
    /// </summary>
    public void UpdateBestChallengeFloor(int val)
    {
        PlayGamesPlatform.Instance.ReportScore(val, GPGSIds.leaderboard_bestchallengefloor, (bool success) => {});
    }

#endregion
}
