using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static Enum;

public class AutoMiningManager : MonoBehaviour
{
    const int MINUTE = 60;
    const int HOUR = MINUTE * 60;

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

    void Start() {
        time = MINUTE;
        cristalTime = HOUR;

        sttDB = DM._.DB.statusDB;
        stgDB = DM._.DB.stageDB;
        autoDB = DM._.DB.autoMiningDB;

        UpdateAll();
        StartCoroutine(CoTimerStart());
    }

#region EVENT
    /// <summary>
    /// 자동채굴 광석 수령 버튼 
    /// </summary>
    /// <param name="idx">광석:(0 ~ 7), 크리스탈 : 8</param>
    public void OnClickTakeStorageBtn(int idx)
    {
        AutoMiningFormat am = autoMiningArr[idx];

        if(am.curStorage <= 0)
        {
            GM._.ui.ShowWarningMsgPopUp("현재 모인 광석이 없습니다.");
            return;
        }

        GM._.ui.ShowNoticeMsgPopUp("수령 완료!");

        // 재화 증가
        sttDB.SetRscArr(idx, am.curStorage);
        
        // 보관량 초기화
        am.curStorage = 0;

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
    private IEnumerator CoTimerStart()
    {
        while(true)
        {
            SetOreTimer();
            SetCristalTimer();

            yield return Util.TIME1;
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
            int val = 0;

            // 추가 수량 계산
            switch(idx)
            {
                // (초월) 자동 광석 수량%
                default: {
                    float extraPer = 1 + GM._.sttm.ExtraAutoOrePer;
                    val = Mathf.RoundToInt(stgDB.BestFloorArr[idx] * extraPer);
                    break;
                }
                // (초월) 자동 크리스탈 수량%
                case (int)RSC.CRISTAL: {
                    float extraPer = 1 + GM._.sttm.ExtraAutoCristalPer;
                    val = Mathf.RoundToInt(stgDB.BestFloorArr[idx] * extraPer);
                    break;
                }
            }

            // 수량 증가
            am.curStorage += val; 

            // 최대수량 다 채웠을 경우
            if(am.curStorage >= am.maxStorage)
            {
                am.curStorageTxt.color = Color.red;
                am.curStorage = am.maxStorage;
            }
        }
    }

    /// <summary>
    /// 광석 자동채굴 (1분)
    /// </summary>
    private void SetOreTimer()
    {
        time--;
        string timeFormat = Util.ConvertTimeFormat(time);
        timerTxt.text = timeFormat;

        // 리셋
        if(time < 1)
        {
            time = MINUTE;

            // 자동채굴 처리
            for(int i = 0; i < autoMiningArr.Length; i++)
            {
                SetStorage(i);
            }

            UpdateUI();
        }
    }
    /// <summary>
    /// 크리스탈 자동채굴 (1시간)
    /// </summary>
    private void SetCristalTimer()
    {
        cristalTime--;
        string timeFormat = Util.ConvertTimeFormat(cristalTime);
        cristalTimerTxt.text = timeFormat;

        // 리셋
        if(cristalTime < 1)
        {
            cristalTime = HOUR;

            // 자동채굴 처리
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
            if(i == (int)RSC.CRISTAL)
                am.titleTxt.text = $"제 {i + 1} 광산 {stgDB.BestFloorArr[i]}층";
            else
                am.titleTxt.text = $"크리스탈 광산 {stgDB.BestFloorArr[i]}층";

            // 현재수량
            am.curStorageTxt.text = $"<sprite name={am.Type}> {am.curStorage} / {am.maxStorage}";

            // 채굴량
            am.productionValTxt.text = $"채굴량 {am.productionVal}";

            // 가격
            if(i == (int)RSC.CRISTAL)
            {
                int rotateOreIdx = GetCristalUpgradeOreIdx(am.Lv);
                am.UpgradePriceBtnTxt.text = $"<size=65%><sprite name=ORE{rotateOreIdx + 1}></size>{am.upgradePrice}";
            }
                
            else
                am.UpgradePriceBtnTxt.text = $"{am.upgradePrice}";

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
        float extraPer = 1
            + GM._.sttm.ExtraAutoOreBagStoragePer; // (초월) 자동 광석 보관량%

        int val = DEF + (lv - 1) * INC;

        return Mathf.RoundToInt(val * extraPer);
    }

    private int CalcMaxCristalStorage(int lv)
    {
        const int DEF = 5, INC = 1;

        // 추가 수량
        float extraPer = 1
            + GM._.sttm.ExtraAutoCristalBagStoragePer; // (초월) 자동 크리스탈 보관량%

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
