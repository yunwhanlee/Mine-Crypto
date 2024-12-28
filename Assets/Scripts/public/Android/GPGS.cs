using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using System;


#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

#if UNITY_ANDROID
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
        Debug.Log($"ShowLeaderBoard():: 기록갱신: DB.bestTotalFloor({DM._.DB.bestTotalFloor}) < CurTotalBestFloor({DM._.DB.stageDB.GetTotalBestFloor()})");
        Debug.Log($"ShowLeaderBoard():: 기록갱신: DB.challengeBestFloor({DM._.DB.challengeBestFloor}) < CurChallangeBestFloor({GM._.clm.BestFloor})");

        if(!isLoginSucceed)
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        else
            DisplayLeaderBoard();
    }

    private void ProcessAuthentication(SignInStatus status) {
        if (status == SignInStatus.Success) {
            isLoginSucceed = true;
            GM._.ui.ShowNoticeMsgPopUp("GOOGLE LOGIN SUCCESS");
            DisplayLeaderBoard();

        } else {
            GM._.ui.ShowNoticeMsgPopUp("GOOGLE LOGIN FAIL");
        }
    }
    
    /// <summary>
    /// 콜백함수 파라메터를 사용해, 순차적으로 리더보드 갱신후에 열람
    /// </summary>
    private void DisplayLeaderBoard()
    {
        //* #1.최대층수총합 기록갱신
        ReportBestTotalFloor(DM._.DB.bestTotalFloor, () => {
            //* #2.시련의광산 기록갱신
            ReportBestChallengeFloor(DM._.DB.challengeBestFloor, () => {
                //* #3.리더보드 열람 (모든 리더보드 최신화가 완료된 후)
                PlayGamesPlatform.Instance.ShowLeaderboardUI();
            });
        });
    }

    /// <summary>
    /// 베스트 일반광산 전체층수 총합
    /// </summary>
    private void ReportBestTotalFloor(int myBestScore, Action onComplete)
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_best_totalfloor,    // ID
            LeaderboardStart.PlayerCentered,        // 플레이어 중심
            rowCount: 1,                            // 가져올 점수개수 (플레이어 1개)
            LeaderboardCollection.Public,           // 공개된 리더보드
            LeaderboardTimeSpan.AllTime,            // 전체 시간 기준
            (LeaderboardScoreData data) => {
                // 플레이어 리더보드 점수가 존재하고
                if(data.Valid && data.PlayerScore != null && data.Scores.Length > 0)
                {
                    // GM._.ui.ShowNoticeMsgPopUp($"TOTALFLOORS:: plScore= {data.PlayerScore.value} < myBestScore={myBestScore}");

                    // 리더보드점수가 최신 베스트점수보다 낮다면
                    if(data.PlayerScore.value < myBestScore)
                    {
                        // 리더보드 최신화
                        PlayGamesPlatform.Instance.ReportScore(myBestScore, GPGSIds.leaderboard_best_totalfloor, (bool success) => {
                            onComplete?.Invoke();
                        });
                    }
                    else
                        onComplete?.Invoke(); // 콜백 호출
                }
                else
                    onComplete?.Invoke(); // 콜백 호출
            }
        );
    }

    /// <summary>
    /// 베스트 시련의광산 총합
    /// </summary>
    private void ReportBestChallengeFloor(int myBestScore, Action onComplete)
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_best_challengefloor,    // ID
            LeaderboardStart.PlayerCentered,            // 플레이어 중심
            rowCount: 1,                                // 가져올 점수개수 (플레이어 1개)
            LeaderboardCollection.Public,               // 공개된 리더보드
            LeaderboardTimeSpan.AllTime,                // 전체 시간 기준
            (LeaderboardScoreData data) => {
                // 플레이어 리더보드 점수가 존재하고
                if(data.Valid && data.PlayerScore != null && data.Scores.Length > 0)
                {
                    // GM._.ui.ShowNoticeMsgPopUp($"CHALLANGE:: plScore= {data.PlayerScore.value} < myBestScore={myBestScore}");

                    // 리더보드점수가 최신 베스트점수보다 낮다면
                    if(data.PlayerScore.value < myBestScore)
                    {
                        // 리더보드 최신화
                        PlayGamesPlatform.Instance.ReportScore(myBestScore, GPGSIds.leaderboard_best_challengefloor, (bool success) => {
                            // 최신화 완료 후 콜백 호출
                            onComplete?.Invoke();
                        });
                    }
                    else
                        onComplete?.Invoke(); // 콜백 호출
                }
                else
                    onComplete?.Invoke(); // 콜백 호출
            }
        );
    }

#endregion
}
#endif