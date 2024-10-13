using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enum;

public class PlayManager : MonoBehaviour
{
    const int CHANLLENGE_PLAYTIME_SEC = 60; // 시련의광산 플레이타임

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


#region EVENT
    public void OnClickPauseIconBtn() {
        SoundManager._.PauseBgm(isOn: true);
        SoundManager._.PlaySfx(SoundManager.SFX.Tap2SFX);
        GM._.gameState = GameState.STOP;
        Time.timeScale = 0;

        pausePopUp.SetActive(true);
    }
    public void OnClickContinueBtn() {
        SoundManager._.PauseBgm(isOn: false);
        SoundManager._.PlaySfx(SoundManager.SFX.Tap2SFX);
        GM._.gameState = GameState.PLAY;
        Time.timeScale = 1;

        pausePopUp.SetActive(false);
    }

    /// <summary>
    /// 게임포기 버튼 (지금까지 획득 데이터는 그대로 반영)
    /// </summary>
    public void OnClickGiveUpBtn() {
        Time.timeScale = 1;
        pausePopUp.SetActive(false);

        // GM._.gameState = GameState.HOME;
        // GM._.epm.employPopUp.SetActive(true);
        GM._.hm.HomeWindow.SetActive(false);

        // 현재까지 진행한 데이터 획득
        GM._.gameState = GameState.TIMEOVER;
        timerTxt.text = "GIVE UP!";
        Timeover(isGiveUp: true);

        InitPlayData();
    }
#endregion

#region FUNC
    public void InitPlayData() {
        // 캐릭터 생성시 등급표시 메세지 정지
        GM._.epm.StopCorCreateRandomCharaID();
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
        DM._.DB.statusDB.UpdateAllTopUIAtHome();
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
        if(GM._.stgm.IsChallengeMode)
            timerVal = CHANLLENGE_PLAYTIME_SEC;
        else
            timerVal = GM._.sttm.ExtraIncTimer;

        // 시작전에 최대타이머 시간 대입 -> 종료시, 채굴시간미션에 적용
        timerMax = timerVal;

        // 타이머 카운트 다운 
        while(timerVal > 0) {
            yield return Util.TIME1;
            TimerVal -= 1;
        }

        //* 타이머0 게임종료
        GM._.gameState = GameState.TIMEOVER;
        timerTxt.text = "TIME OVER!";

        Timeover();
    }

    /// <summary>
    ///* 타임오버 (제공 보상표시)
    /// </summary>
    public void Timeover(bool isGiveUp = false) {
        var stageType = (int)GM._.stgm.OreType;
        var bestFloorArr = DM._.DB.stageDB.BestFloorArr;

        // 광산 최고층 기록
        if(GM._.stgm.Floor > bestFloorArr[stageType])
        {
            bestFloorArr[stageType] = GM._.stgm.Floor;
            GM._.rwm.newBestFloorMsgTxt.text = $"{GM._.stgm.Floor}층 최고기록 달성!"; // <제 {stageType+1}광산> 
            GM._.rwm.newBestFloorMsgTxt.gameObject.SetActive(true);
        }

        // 게임포기 안했을때
        if(isGiveUp == false)
        {
            //* 시련의광산
            if(GM._.stgm.IsChallengeMode)
            {
                // playResRwdArr[(int)RWD.RED_TICKET]++;        // 결과수치 UI
                // DM._.DB.statusDB.RedTicket++;               // 데이터
                GM._.fm.missionArr[(int)MISSION.CHALLENGE_CLEAR_CNT].Exp++; // 광산 클리어 미션
            }
            //* 일반광산
            else
            {
                playResRwdArr[(int)RWD.ORE_TICKET]++;        // 결과수치 UI
                DM._.DB.statusDB.OreTicket++;               // 데이터
                GM._.fm.missionArr[(int)MISSION.STAGE_CLEAR_CNT].Exp++; // 광산 클리어 미션
            }
        }

        //* 시련의광산
        if(GM._.stgm.IsChallengeMode)
        {
            // 게임포기 안했을때 (달성한 층수만큼 광석상자 획득)
            if(isGiveUp == false)
            {
                // 광석상자 획득 (매 층마다 +1)
                int oreChestCnt = GM._.stgm.Floor - 1;
                playResRwdArr[(int)RWD.ORE_CHEST] = oreChestCnt; // 결과수치 UI
                DM._.DB.statusDB.OreChest = oreChestCnt;        // 데이터
            }
        }
        //* 일반광산
        else
        {
            // 광석상자 획득 (매 층마다 +1)
            int oreChestCnt = GM._.stgm.Floor - 1;
            playResRwdArr[(int)RWD.ORE_CHEST] = oreChestCnt; // 결과수치 UI
            DM._.DB.statusDB.OreChest = oreChestCnt;        // 데이터
        }

        // 보상팝업 표시 (나머지는 게임 진행중 실시간으로 이미 제공됨)
        SoundManager._.PlaySfx(SoundManager.SFX.OpenOreChestSFX);
        GM._.rwm.ShowGameoverReward(playResRwdArr);
    }
#endregion
}
