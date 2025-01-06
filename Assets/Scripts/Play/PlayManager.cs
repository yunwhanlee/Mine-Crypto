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

    [Header("(인게임) 게임오버시 획득한 보상수량 결과표시용 (실제로는 수령안되므로 수령처리 해야됨)")]
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

        // 시간의조각 발동중이었다면, 정지
        if(GM._.tpm.isActive)
            GM._.tpm.ActiveProcess(false);

        Time.timeScale = 0;

        pausePopUp.SetActive(true);
    }
    public void OnClickContinueBtn() {
        SoundManager._.PauseBgm(isOn: false);
        SoundManager._.PlaySfx(SoundManager.SFX.Tap2SFX);
        GM._.gameState = GameState.PLAY;
        Time.timeScale = 1;

        // 시간의조각 발동중이었다면, 재발동처리
        if(GM._.tpm.isActive)
            GM._.tpm.ActiveProcess(true);

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
        Debug.Log($"Timeover():: isGiveUp= {isGiveUp}, stageType= {GM._.stgm.OreType}");
        var stageType = (int)GM._.stgm.OreType;
        var bestFloorArr = DM._.DB.stageDB.BestFloorArr;

        // 인게임 시간의결정 활성화버튼
        GM._.tpm.ingameActiveBtnObj.SetActive(false);

        // 스킬발동 종료
        GM._.skc.StopActiveSkill();

        Debug.Log($"Timeover():: floor= {GM._.stgm.Floor}, bestFloor= {bestFloorArr[stageType]}");

        // 일반광산 최고층 기록
        if(GM._.stgm.Floor > bestFloorArr[stageType])
        {
            bestFloorArr[stageType] = GM._.stgm.Floor;
            GM._.rwm.newBestFloorMsgTxt.text = $"{GM._.stgm.Floor}{LM._.Localize(LM.Floor)} {LM._.Localize(LM.BestRecord)}!"; // <제 {stageType+1}광산> 
            GM._.rwm.newBestFloorMsgTxt.gameObject.SetActive(true);

            // for(int i = 0; i < bestFloorArr.Length - 1; i++)
            //     totalFloor += bestFloorArr[i];
            int totalFloor = DM._.DB.stageDB.GetTotalBestFloor(); 

            //* 최대층수총합 기록갱신
            if(DM._.DB.bestTotalFloor < totalFloor)
            {
                DM._.DB.bestTotalFloor = totalFloor;
            }
        }

        //* 게임포기 안했을시 추가 처리
        if(isGiveUp == false)
        {
            // 시련의광산
            if(GM._.stgm.IsChallengeMode)
            {   
                // 돌파실패시
                if(GM._.mnm.oreGroupTf.childCount > 0)
                {
                    GM._.pm.timerTxt.text = "FAIL!";
                }
                // 돌파성공시
                else
                {
                    //* 시련의광산 기록갱신
                    if(DM._.DB.challengeBestFloor < GM._.clm.BestFloor)
                    {
                        DM._.DB.challengeBestFloor = GM._.clm.BestFloor;
                    }

                    GM._.clm.BestFloor++;
                    GM._.rwm.newBestFloorMsgTxt.text = $"{GM._.stgm.Floor}{LM._.Localize(LM.Floor)} {LM._.Localize(LM.BestRecord)}!"; // <제 {stageType+1}광산> 
                    GM._.rwm.newBestFloorMsgTxt.gameObject.SetActive(true);

                    GM._.pm.timerTxt.text = "CLEAR!";
                    GM._.fm.missionArr[(int)MISSION.CHALLENGE_CLEAR_CNT].Exp++; // 광산 클리어 미션
                    // 타임포션 1개 획득
                    playResRwdArr[(int)RWD.TIMEPOTION]++;
                    DM._.DB.statusDB.SetInventoryItemVal(INV.TIMEPOTION, 1);
                    // 황금코인 1개 획득
                    playResRwdArr[(int)RWD.GOLDCOIN]++;
                    DM._.DB.statusDB.SetInventoryItemVal(INV.GOLDCOIN, 1);
                    // 광석상자 획득 (매 층마다 +1)
                    int oreChestCnt = GM._.stgm.Floor - 1;
                    playResRwdArr[(int)RWD.ORE_CHEST] += oreChestCnt; // 결과수치 UI
                    DM._.DB.statusDB.OreChest += oreChestCnt;        // 데이터
                }
            }
            // 일반광산
            else
            {
                // 사용한 광석티켓 회수
                playResRwdArr[(int)RWD.ORE_TICKET]++;       // 결과수치 UI
                DM._.DB.statusDB.OreTicket++; // 데이터

                // 광석상자 획득 (매 층마다 +1)
                int oreChestCnt = GM._.stgm.Floor - 1;
                playResRwdArr[(int)RWD.ORE_CHEST] += oreChestCnt; // 결과수치 UI
                DM._.DB.statusDB.OreChest += oreChestCnt;        // 데이터

                // 광산 클리어 미션
                GM._.fm.missionArr[(int)MISSION.STAGE_CLEAR_CNT].Exp++; 
            }
        }

        // 보상팝업 표시 (나머지는 게임 진행중 실시간으로 이미 제공됨)
        SoundManager._.PlaySfx(SoundManager.SFX.OpenOreChestSFX);
        GM._.rwm.ShowGameoverReward(playResRwdArr);

        // 시간의조각 비활성화
        if(GM._.tpm.isActive)
        {
            GM._.tpm.isActive = false;
            GM._.tpm.ActiveProcess(GM._.tpm.isActive);
        }

        // 상점메뉴아이콘 버튼 비표시
        GM._.spm.shopMenuIconBtnObj.SetActive(false);

        // 업그레이드 가능알림🔴 최신화
        GM._.ugm.UpdateAlertRedDotUI();
        GM._.mrm.UpdateAlertRedDotUI();
    }
#endregion
}
