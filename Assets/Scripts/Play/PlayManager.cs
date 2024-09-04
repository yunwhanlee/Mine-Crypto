using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enum;

public class PlayManager : MonoBehaviour
{
    
    const int CRISTAL_STAGE_PLAYTIME_SEC = 30;// 시련의광산 플레이타임

    //* Element
    public GameObject pausePopUp;
    public TMP_Text timerTxt;

    //* Value
    private Coroutine corTimerCowndownID;

    [Header("(인게임) 게임오버시 획득한 보상수량 결과표시 데이터")]
    public int[] playResRwdArr; // 정의는 StageManager:: StageStart()에서 매번 초기화

    [field:SerializeField] int timerMax;
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
        StopCorTimer();
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

    public void StopCorTimer() {
        if(corTimerCowndownID != null)
            StopCoroutine(corTimerCowndownID);
    }

    /// <summary>
    /// 시간 카운트 다운
    /// </summary>
    public void StartCowndownTimer()
        => corTimerCowndownID = StartCoroutine(CoStartCownDownTimer());

    private IEnumerator CoStartCownDownTimer() {
        // 타이머 최대시간
        if(GM._.stm.OreType == RSC.CRISTAL)
            timerVal = CRISTAL_STAGE_PLAYTIME_SEC;
        else
            timerVal = GM._.ugm.upgIncTimer.Val
                + (int)GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_TIMER);

        // 시작전에 최대타이머 시간 대입 -> 종료시, 채굴시간미션에 적용
        timerMax = timerVal;

        // 타이머 카운트 다운 
        while(timerVal > 0) {
            yield return Util.TIME1;
            TimerVal -= 1;
        }

        //! 타이머 0 -> 게임오버로 상태 변경
        GM._.gameState = GameState.TIMEOVER;
        timerTxt.text = "TIME OVER!";

        Timeover(); // 타임오버
    }

    /// <summary>
    ///* 타임오버 (제공 보상표시)
    /// </summary>
    public void Timeover() {
        // 입장티켓 1개 회수
        if(GM._.stm.OreType == RSC.CRISTAL)
        {
            playResRwdArr[(int)RWD.RED_TICKET]++;        // 결과수치 UI
            DM._.DB.statusDB.RedTicket++;               // 데이터
        }
        else
        {
            playResRwdArr[(int)RWD.ORE_TICKET]++;        // 결과수치 UI
            DM._.DB.statusDB.OreTicket++;               // 데이터
        }

        // 광석상자 획득 (매 층마다 +1)
        int oreChestCnt = GM._.stm.Floor - 1;
        playResRwdArr[(int)RWD.ORE_CHEST] = oreChestCnt; // 결과수치 UI
        DM._.DB.statusDB.OreChest = oreChestCnt;        // 데이터

        // 보상팝업 표시 (나머지는 게임 진행중 실시간으로 이미 제공됨)
        GM._.rwm.ShowGameoverReward(playResRwdArr);

        // 채굴시간 미션
        GM._.fm.missionArr[(int)MISSION.MINING_TIME].Exp += timerMax;

        // 광산 클리어 미션
        GM._.fm.missionArr[(int)MISSION.STAGE_CLEAR_CNT].Exp++;
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
        GM._.epm.employPopUp.SetActive(false);
        GM._.hm.HomeWindow.SetActive(true);

        //TODO 게임뷰 초기화
        InitPlayData();
    }
#endregion
}
