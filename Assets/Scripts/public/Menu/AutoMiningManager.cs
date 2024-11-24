using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static Enum;

public class AutoMiningManager : MonoBehaviour
{
    int WAIT_TIME = 60; // 채굴 획득 대기시간
    const int ORE_INC_UNIT = 100; // 자동채굴 최대보관량 증가 단위

    [Header("자동채굴 수령버튼 알람아이콘 표시")]
    public GameObject[] autoMiningBtnAlertRedDotArr;

    [Header("자동채굴 팝업")]
    public GameObject windowObj;
    public GameObject alertRedDotObj;
    public GameObject acceptAllAlertRedDotObj;
    public TMP_Text timerTxt;
    public AutoMiningFormat[] autoMiningArr;
    
    private int time;

    [Header("시련의광산 자동채굴")]
    public GameObject cristalAlertRedDotObj;
    public TMP_Text cristalTimerTxt;
    private int cristalTime;

    StatusDB sttDB;
    StageDB stgDB;
    AutoMiningDB autoDB;

    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        sttDB = DM._.DB.statusDB;
        stgDB = DM._.DB.stageDB;
        autoDB = DM._.DB.autoMiningDB;

        //* 오프라인 자동채굴 결과처리
        yield return new WaitForSeconds(1); // 저장된 추가획득량 데이터가 로드안되는 문제가 있어 1초 대기

        OfflineAutoMining();
        UpdateAll();
        // StartCoroutine(CoTimerStart());
    }

    void Update() {
        //! TEST 자동채굴 대기시간 5초 <-> 1분
        if(Input.GetKeyDown(KeyCode.B))
        {
            if(WAIT_TIME == 60)
                WAIT_TIME = 5;
            else
                WAIT_TIME = 60;
            
            GM._.ui.ShowNoticeMsgPopUp($"(테스트모드) 자동채굴 및 시간조각 자동획득 대기시간 <color=red>{WAIT_TIME}</color>초로 변경");

            time = WAIT_TIME;
            cristalTime = WAIT_TIME;
        }
    }

#region EVENT
    /// <summary>
    /// 일괄수령 버튼
    /// </summary>
    public void OnClickAcceptAllBtn()
    {
        // 모인광석이 하나도 없을경우
        if(Array.TrueForAll(autoMiningArr, mine => mine.CurStorage <= 0))
        {   // 모인광석이 없습니다 메세지 표시
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NoOreYetMsg));
            return;
        }

        SoundManager._.PlaySfx(SoundManager.SFX.AutoMiningTakeSFX);

        //* 보상 획득
        GM._.rwm.ShowReward (
            new Dictionary<RWD, int> { 
                {RWD.ORE1, autoMiningArr[0].CurStorage},
                {RWD.ORE2, autoMiningArr[1].CurStorage},
                {RWD.ORE3, autoMiningArr[2].CurStorage},
                {RWD.ORE4, autoMiningArr[3].CurStorage},
                {RWD.ORE5, autoMiningArr[4].CurStorage},
                {RWD.ORE6, autoMiningArr[5].CurStorage},
                {RWD.ORE7, autoMiningArr[6].CurStorage},
                {RWD.ORE8, autoMiningArr[7].CurStorage},
            },
            isAutoMine: true
        );

        // 보관량 0으로 초기화
        for(int i = 0; i < autoMiningArr.Length; i++)
        {
            autoMiningArr[i].CurStorage = 0;
        }

        // 업데이트 UI
        UpdateUI();
    }
    /// <summary>
    /// 자동채굴 광석 수령 버튼 
    /// </summary>
    /// <param name="idx">광석:(0 ~ 7), 크리스탈 : 8</param>
    public void OnClickTakeStorageBtn(int idx)
    {
        AutoMiningFormat am = autoMiningArr[idx];

        if(am.CurStorage <= 0)
        {
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NoOreYetMsg));
            return;
        }

        SoundManager._.PlaySfx(SoundManager.SFX.AutoMiningTakeSFX);
        GM._.ui.ShowNoticeMsgPopUp(LM._.Localize(LM.ReceiptCompletedMsg));

        //* 보상 획득
        GM._.rwm.ShowReward (
            new Dictionary<RWD, int> { {(RWD)idx, am.CurStorage} },
            isAutoMine: true
        );
        
        // 보관량 초기화
        am.CurStorage = 0;

        // 업데이트 UI
        UpdateUI();
    }

    /// <summary>
    /// 최대보관량 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeBtn(int idx)
    {
        AutoMiningFormat am = autoMiningArr[idx];

        if(am.Type == RSC.CRISTAL)
        {
            // 크리스탈 광석 업그레이드에 필요한 광석 로테이션 
            idx = GetCristalUpgradeOreIdx(am.Lv);

            if(sttDB.RscArr[idx] < am.upgradePrice)
            {
                GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
                return;
            }
        }
        else
        {
            if(sttDB.RscArr[idx] < am.upgradePrice)
            {
                GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
                return;
            }
        }

        SoundManager._.PlaySfx(SoundManager.SFX.UpgradeSFX);

        GM._.ui.ShowNoticeMsgPopUp(LM._.Localize(LM.UpgradeMaxStorage) + "!");

        // 재화 감소
        sttDB.SetRscArr(idx, -am.upgradePrice);

        // 레벨 업
        am.Lv++;

        // 업데이트
        UpdateAll();
    }
#endregion

#region FUNC
    /// <summary>
    /// 오프라인 자동채굴 결과처리
    /// </summary>
    private void OfflineAutoMining()
    {
        Debug.Log("OfflineAutoMining():: 자동채굴 오프라인 처리");

        AutoMiningSaveData[] svDts = DM._.DB.autoMiningDB.saveDts;

        //* 어플시작시 이전까지 경과한시간
        int passedTime = DM._.DB.autoMiningDB.GetPassedSecData();

        // 데이터로드 : AutoMiningFormat클래스가 UI변수도 있어서 객체생성은 안되고, 저장된 데이터만 대입
        for(int i = 0; i < autoMiningArr.Length; i++)
        {
            // 잠금해제가 안됬다면 이하처리 안함
            if(!svDts[i].IsUnlock)
                continue;

            // Debug.Log($"자동채굴 광석{i+1} : 이전량= {autoMiningArr[i].CurStorage}");

            // 데이터 로드 최신화
            RSC oreType = autoMiningArr[i].Type;
            {
                // 잠금해제 상태
                autoMiningArr[i].IsUnlock = svDts[i].IsUnlock;
                // 레벨
                autoMiningArr[i].Lv = svDts[i].Lv;
                // 경과시간
                autoMiningArr[i].Time = svDts[i].Time;  
                // 현재보관량
                autoMiningArr[i].CurStorage = svDts[i].CurStorage; 
                // 최대보관량
                if(i == (int)RSC.CRISTAL)
                    autoMiningArr[i].maxStorage = CalcMaxCristalStorage(autoMiningArr[i].Lv);
                else
                    autoMiningArr[i].maxStorage = CalcMaxOreStorage(autoMiningArr[i].Lv);
            }

            // 자동채굴 획득량 계산
            int cnt = passedTime / WAIT_TIME; //(type == RSC.CRISTAL? HOUR : MINUTE);

            // 자동채굴 결과수량 //! 이미 수령할때 모두 적용된상태로 얻기때문에 현재 아래의 GetProductionVal가 있으면 중복이 되는 상태임.
            int resVal = autoMiningArr[i].CurStorage + cnt * GetProductionVal((RSC)i);

            // 최대수량보다 높다면 최대수량만큼으로 수정
            if(resVal > autoMiningArr[i].maxStorage)
                resVal = autoMiningArr[i].maxStorage;

            Debug.Log($"<color=white>자동채굴 오프라인 처리 {oreType}: 이전수량= {autoMiningArr[i].CurStorage} / {autoMiningArr[i].maxStorage}, 획득량: {resVal} (경과시간: {passedTime} / {WAIT_TIME} = {cnt})</color>");
            autoMiningArr[i].CurStorage = resVal;
        }

        time = WAIT_TIME;
        cristalTime = WAIT_TIME;
    }

    /// <summary>
    /// 광석 및 크리스탈 1분당 자동채굴량 계산 및 반환
    /// </summary>
    /// <param name="rscType"></param>
    /// <returns></returns>
    private int GetProductionVal(RSC rscType)
    {
        if(rscType == RSC.CRISTAL)
        {
            int extraVal = stgDB.BestFloorArr[(int)RSC.CRISTAL] + GM._.sttm.ExtraIncCristal;
            float extraPer = 1 + GM._.sttm.ExtraAutoCristalPer;
            Debug.Log($"자동채굴 생산량: GetProductionVal({rscType}):: extraVal({extraVal}) * extraPer({extraPer})");
            return Mathf.RoundToInt(extraVal * extraPer);
        }
        else
        {
            int extraVal = stgDB.BestFloorArr[(int)rscType] * ORE_INC_UNIT;
            float extraPer = 1 + GM._.sttm.ExtraAutoOrePer;
            Debug.Log($"자동채굴 생산량: GetProductionVal({rscType}):: extraVal({extraVal}) * extraPer({extraPer})");
            return Mathf.RoundToInt(extraVal * extraPer);
        }
    }

    /// <summary>
    /// 자동채굴 광석 증가 (광석타입)
    /// </summary>
    private void SetStorage(int idx)
    {
        AutoMiningFormat am = autoMiningArr[idx];

        // 잠금해제된 광석만
        if(am.IsUnlock)
        {
            // 광석 및 크리스탈 자동채굴량
            int val = GetProductionVal((RSC)idx);

            // 수량 증가
            am.CurStorage += val; 

            // 최대수량 다 채웠을 경우
            if(am.CurStorage >= am.maxStorage)
            {
                am.curStorageTxt.color = Color.red;
                am.CurStorage = am.maxStorage;
            }
        }
    }

    /// <summary>
    /// 광석 자동채굴 (1분)
    /// </summary>
    public void SetOreTimer()
    {
        time--;
        string timeFormat = Util.ConvertTimeFormat(time);
        timerTxt.text = timeFormat;

        // 리셋
        if(time < 1)
        {
            time = WAIT_TIME;

            // 자동채굴 처리
            int oreLenght = autoMiningArr.Length - 1; // 크리스탈은 제외
            for(int i = 0; i < oreLenght; i++)
            {
                SetStorage(i);
            }

            UpdateUI();
        }
    }
    /// <summary>
    /// 크리스탈 자동채굴 (1분)
    /// </summary>
    public void SetCristalTimer()
    {
        cristalTime--;
        string timeFormat = Util.ConvertTimeFormat(cristalTime);
        cristalTimerTxt.text = timeFormat;

        // 리셋
        if(cristalTime < 1)
        {
            Debug.Log($"SetCristalTimer():: cristalTime= {cristalTime}");
            cristalTime = WAIT_TIME;

            // 크리스탈 자동채굴 처리
            SetStorage((int)RSC.CRISTAL);

            UpdateUI();
        }
    }

    public void UpdateAll()
    {
        UpdateData();
        UpdateUI();
    }

    private void UpdateData()
    {
        // 강화비용 감소%
        float decreasePer = 1 - GM._.rbm.upgDecUpgradePricePer.Val;

        for(int i = 0; i < autoDB.saveDts.Length; i++)
        {
            AutoMiningFormat am = autoMiningArr[i];

            // 최대보관량
            if(i == (int)RSC.CRISTAL)
                am.maxStorage = CalcMaxCristalStorage(am.Lv);
            else
                am.maxStorage = CalcMaxOreStorage(am.Lv);

            // 채굴량
            am.productionVal = stgDB.BestFloorArr[i];

            // 가격
            if(i == (int)RSC.CRISTAL)
            {
                am.upgradePrice = Mathf.RoundToInt(CalcUpgradCristalPrice(am.Lv) * decreasePer);
            }
            else
            {
                am.upgradePrice = Mathf.RoundToInt(CalcUpgradeOrePrice(am.Lv) * decreasePer);
            }
        }
    }

    private void UpdateUI()
    {
        alertRedDotObj.SetActive(false);
        acceptAllAlertRedDotObj.SetActive(false);
        cristalAlertRedDotObj.SetActive(false);

        for(int i = 0; i < autoDB.saveDts.Length; i++)
        {
            AutoMiningSaveData saveDt = autoDB.saveDts[i];
            AutoMiningFormat am = autoMiningArr[i];

            // 잠금해제 화면 (setter: UI 업데이트)
            am.IsUnlock = saveDt.IsUnlock;

            // 타이틀
            if(i != (int)RSC.CRISTAL)
                am.titleTxt.text = $"{LM._.Localize($"UI_MineStage{i + 1}")} {stgDB.BestFloorArr[i]}{LM._.Localize(LM.Floor)}";

            // 현재수량이 최대수량만큼 쌓였는지에 따른 색깔태그
            string isFullcolorTag = am.CurStorage >= am.maxStorage? "red" : "white";

            // 현재수량
            am.curStorageTxt.text = $"<color={isFullcolorTag}><sprite name={am.Type}> {am.CurStorage} / {am.maxStorage}</color>";
            autoMiningBtnAlertRedDotArr[i].SetActive(am.CurStorage >= am.maxStorage);

            // 채굴량
            am.productionValTxt.text = $"{LM._.Localize(LM.MiningPerMin)} +{GetProductionVal((RSC)i)}";

            if(i == (int)RSC.CRISTAL)
            {
                // 가격
                int rotateOreIdx = GetCristalUpgradeOreIdx(am.Lv);
                am.UpgradePriceBtnTxt.text = $"<size=65%><sprite name=ORE{rotateOreIdx + 1}></size>{am.upgradePrice}";
            }
            else
            {
                // 가격
                am.UpgradePriceBtnTxt.text = $"{am.upgradePrice}";
            }

            // 다음 업그레이드 최대보관량 표시
            if(i == (int)RSC.CRISTAL)
                am.UpgradeInfoTxt.text = $"{am.maxStorage} => {CalcMaxCristalStorage(am.Lv + 1)}";
            else
                am.UpgradeInfoTxt.text = $"{am.maxStorage} => {CalcMaxOreStorage(am.Lv + 1)}";

            // 업데이트 알림UI 🔴
            UpdateAlertRedDotUI(i);
        }

        acceptAllAlertRedDotObj.SetActive(alertRedDotObj.activeSelf);
    }

    private void UpdateAlertRedDotUI(int idx)
    {
        AutoMiningFormat am = autoMiningArr[idx];

            if(idx == (int)RSC.CRISTAL)
            {
                if(am.CurStorage >= am.maxStorage && !cristalAlertRedDotObj.activeSelf)
                    cristalAlertRedDotObj.SetActive(true);
            }
            else
            {
                if(am.CurStorage >= am.maxStorage && !alertRedDotObj.activeSelf)
                    alertRedDotObj.SetActive(true);
            }
    }

    private int GetCristalUpgradeOreIdx(int lv)
    {
        return (lv - 1) % ((int)RSC.ORE8 + 1);
    }

    private int CalcMaxOreStorage(int lv)
    {
        const int DEF = 100, INC = 100;

        // 추가 수량
        float extraPer = 1 + GM._.sttm.ExtraAutoOreBagStoragePer; // (초월) 자동 광석 보관량%

        int val = DEF + (lv - 1) * INC;

        return Mathf.RoundToInt(val * extraPer * GM._.fm.CalcAllAutoMiningStorageMultiVal(GM._.fm.FameLv));
    }

    private int CalcMaxCristalStorage(int lv)
    {
        const int DEF = 10, INC = 10;

        // 추가 수량
        float extraPer = 1 + GM._.sttm.ExtraAutoCristalBagStoragePer; // (초월) 자동 크리스탈 보관량%

        int val = DEF + (lv - 1) * INC;

        return Mathf.RoundToInt(val * extraPer * GM._.fm.CalcAllAutoMiningStorageMultiVal(GM._.fm.FameLv));
    }

    /// <summary>
    /// 자동채굴 일반광석 최대보관량 업그레이드 가격
    /// </summary>
    private int CalcUpgradeOrePrice(int lv)
    {
        return 500 + (lv - 1) * 500;
    }

    /// <summary>
    /// 자동채굴 일반광석 최대보관량 업그레이드 가격
    /// </summary>
    private int CalcUpgradCristalPrice(int lv)
    {
        return 1000 + ( lv * ( lv - 1 ) * 1000 ) / 2;
    }
#endregion
}
