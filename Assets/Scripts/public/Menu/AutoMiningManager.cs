using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static Enum;

public class AutoMiningManager : MonoBehaviour
{
    int WAIT_TIME = 60; // 채굴 획득 대기시간
    const int ORE_INC_UNIT = 100;

    [Header("자동채굴 팝업")]
    public GameObject windowObj;
    public TMP_Text timerTxt;
    public AutoMiningFormat[] autoMiningArr;
    private int time;

    [Header("시련의광산 자동채굴")]
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

        AutoMiningSaveData[] svDts = DM._.DB.autoMiningDB.saveDts;
        int[] autoMiningRwdArr = new int[9];
        var rewardList = new Dictionary<RWD, int>();

        //* 어플시작시 이전까지 경과한시간
        int passedTime = DM._.DB.autoMiningDB.GetPassedSecData();

        // 데이터로드 : AutoMiningFormat클래스가 UI변수도 있어서 객체생성은 안되고, 저장된 데이터만 대입
        for(int i = 0; i < autoMiningArr.Length; i++)
        {
            // 자동채굴이 해금안됬다면 이하처리 안함
            if(!svDts[i].IsUnlock)
                continue;

            // Debug.Log($"자동채굴 광석{i+1} : 이전량= {autoMiningArr[i].CurStorage}");

            var type = autoMiningArr[i].Type;

            autoMiningArr[i].IsUnlock = svDts[i].IsUnlock;
            autoMiningArr[i].Lv = svDts[i].Lv;
            autoMiningArr[i].Time = svDts[i].Time;
            autoMiningArr[i].CurStorage = svDts[i].CurStorage;

            // 자동채굴 획득량 계산
            int cnt = passedTime / WAIT_TIME; //(type == RSC.CRISTAL? HOUR : MINUTE);

            // 자동채굴 결과수량
            int resVal = cnt * GetProductionVal((RSC)i);

            // 최대수량보다 높다면 최대수량만큼으로 수정
            if(resVal > autoMiningArr[i].maxStorage)
                resVal = autoMiningArr[i].maxStorage;

            Debug.Log($"자동채굴 광석{i+1} : 이전량= {autoMiningArr[i].CurStorage}, 획득량= {resVal}");
            autoMiningArr[i].CurStorage += resVal;
        }

        time = WAIT_TIME;
        cristalTime = WAIT_TIME;

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
            
            GM._.ui.ShowNoticeMsgPopUp($"(테스트모드) 자동채굴 대기시간 <color=red>{WAIT_TIME}</color>초로 변경");

            time = WAIT_TIME;
            cristalTime = WAIT_TIME;
        }
    }

#region EVENT
    /// <summary>
    /// 자동채굴 광석 수령 버튼 
    /// </summary>
    /// <param name="idx">광석:(0 ~ 7), 크리스탈 : 8</param>
    public void OnClickTakeStorageBtn(int idx)
    {
        AutoMiningFormat am = autoMiningArr[idx];

        if(am.CurStorage <= 0)
        {
            GM._.ui.ShowWarningMsgPopUp("현재 모인 광석이 없습니다.");
            return;
        }

        GM._.ui.ShowNoticeMsgPopUp("수령 완료!");

        // 재화 증가
        sttDB.SetRscArr(idx, am.CurStorage);
        
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
                GM._.ui.ShowWarningMsgPopUp("재화가 부족합니다.");
                return;
            }
        }
        else
        {
            if(sttDB.RscArr[idx] < am.upgradePrice)
            {
                GM._.ui.ShowWarningMsgPopUp("재화가 부족합니다.");
                return;
            }
        }

        GM._.ui.ShowNoticeMsgPopUp("최대보관량 업그레이드 성공!");

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
    /// 광석 및 크리스탈 자동채굴량 반환
    /// </summary>
    /// <param name="rscType"></param>
    /// <returns></returns>
    private int GetProductionVal(RSC rscType)
    {
        if(rscType != RSC.CRISTAL)
        {
            float extraPer = 1 + GM._.sttm.ExtraAutoOrePer;
            return Mathf.RoundToInt(stgDB.BestFloorArr[(int)rscType] * ORE_INC_UNIT * extraPer);
        }
        else
        {
            float extraPer = 1 + GM._.sttm.ExtraAutoCristalPer;
            return Mathf.RoundToInt(stgDB.BestFloorArr[(int)rscType] * extraPer);
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
    /// 크리스탈 자동채굴 (1시간)
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
                am.upgradePrice = CalcUpgradCristalPrice(am.Lv);
            else
                am.upgradePrice = CalcUpgradeOrePrice(am.Lv);
        }
    }

    private void UpdateUI()
    {
        for(int i = 0; i < autoDB.saveDts.Length; i++)
        {
            AutoMiningSaveData saveDt = autoDB.saveDts[i];
            AutoMiningFormat am = autoMiningArr[i];

            // 잠금해제 화면 (setter: UI 업데이트)
            am.IsUnlock = saveDt.IsUnlock;

            // 타이틀
            if(i != (int)RSC.CRISTAL)
                am.titleTxt.text = $"제 {i + 1} 광산 {stgDB.BestFloorArr[i]}층";

            // 현재수량이 최대수량만큼 쌓였는지에 따른 색깔태그
            string isFullcolorTag = am.CurStorage >= am.maxStorage? "red" : "white";

            // 현재수량
            am.curStorageTxt.text = $"<color={isFullcolorTag}><sprite name={am.Type}> {am.CurStorage} / {am.maxStorage}</color>";

            // 채굴량
            am.productionValTxt.text = $"+채굴량 {GetProductionVal((RSC)i)}";

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

        return Mathf.RoundToInt(val * extraPer);
    }

    private int CalcMaxCristalStorage(int lv)
    {
        const int DEF = 5, INC = 1;

        // 추가 수량
        float extraPer = 1 + GM._.sttm.ExtraAutoCristalBagStoragePer; // (초월) 자동 크리스탈 보관량%

        int val = DEF + (lv - 1) * INC;

        return Mathf.RoundToInt(val * extraPer);
    }

    private int CalcUpgradeOrePrice(int lv)
    {
        return 1000 + (lv - 1) * 1000;
    }

    private int CalcUpgradCristalPrice(int lv)
    {
        return 1000 + ( lv * ( lv - 1 ) * 1000 ) / 2;
    }
#endregion
}
