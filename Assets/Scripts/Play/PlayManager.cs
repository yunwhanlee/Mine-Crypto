using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    //* Element
    public GameObject pausePopUp;
    public TMP_Text timerTxt;

    //* Value
    private Coroutine corTimerCowndownID;

    [Header("인게임 채굴한 광석재화 수량 표시용 데이터")]
    public int[] resultRscArr;

    [field:SerializeField] int timerVal;  public int TimerVal {
        get => timerVal;
        set {
            timerVal = value;

            int min = timerVal / 60;
            int sec = timerVal % 60;
            timerTxt.text = $"{min:00} : {sec:00}";
        }
    }

#region FUNC
    public void InitPlayData() {
        // 타이머 카운트 정지
        if(corTimerCowndownID != null)
            StopCoroutine(corTimerCowndownID);
        // 광석 삭제
        for(int i = 0; i < GM._.mnm.oreGroupTf.childCount; i++) {
            Destroy(GM._.mnm.oreGroupTf.GetChild(i).gameObject); 
        }
        // 캐릭터 삭제
        for(int i = 0; i < GM._.mnm.workerGroupTf.childCount; i++){
            Destroy(GM._.mnm.workerGroupTf.GetChild(i).gameObject); 
        }
        // 모든 재화 UI 업데이트
        DM._.DB.statusDB.UpdateAllRscUIAtHome();
    }

    /// <summary>
    /// 시간 카운트 다운
    /// </summary>
    public void StartCowndownTimer()
        => corTimerCowndownID = StartCoroutine(CoStartCownDownTimer());

    private IEnumerator CoStartCownDownTimer() {
        timerVal = GM._.ugm.upgIncTimer.Val;

        // 타이머 카운트 다운 
        while(timerVal > 0) {
            yield return Util.TIME1;
            TimerVal -= 1;
        }

        //* 게임오버 (타이머 0이면)
        GM._.gameState = GameState.GAMEOVER;
        timerTxt.text = GameState.GAMEOVER.ToString();

        //* 보상팝업 표시
        GM._.rwm.ShowResultRscUI(resultRscArr);
    }
#endregion

#region EVENT
    public void OnClickPauseIconBtn() {
        GM._.gameState = GameState.STOP;
        Time.timeScale = 0;

        pausePopUp.SetActive(true);
    }
    public void OnClickContinueBtn() {
        GM._.gameState = GameState.PLAY;
        Time.timeScale = 1;

        pausePopUp.SetActive(false);
    }
    public void OnClickGiveUpBtn() {
        GM._.gameState = GameState.HOME;
        Time.timeScale = 1;

        pausePopUp.SetActive(false);
        GM._.hm.HomeWindow.SetActive(true);

        //TODO 게임뷰 초기화
        InitPlayData();
    }
#endregion
}
