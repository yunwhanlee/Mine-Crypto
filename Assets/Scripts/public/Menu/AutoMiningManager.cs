using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class AutoMiningManager : MonoBehaviour
{
    const int ONE_MINUTE = 15;

    public GameObject windowObj;
    public TMP_Text timerTxt;
    public AutoMiningFormat[] autoMiningArr;

    private int time;

    private StatusDB sttDB;
    private StageDB stgDB;
    private AutoMiningDB autoDB;

    void Start() {
        time = ONE_MINUTE;
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

        if(sttDB.RscArr[idx] < am.upgradePrice)
        {
            GM._.ui.ShowWarningMsgPopUp("재화가 부족합니다.");
            return;
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
            time--;
            string timeFormat = Util.ConvertTimeFormat(time);
            timerTxt.text = timeFormat;

            // 리셋 타임
            if(time < 1)
            {
                time = ONE_MINUTE;

                // 자동채굴 처리
                for(int i = 0; i < autoDB.saveDts.Length; i++)
                {
                    AutoMiningFormat am = autoMiningArr[i];

                    // 잠금해제된 광석만
                    if(am.IsUnlock)
                    {
                        am.curStorage += 60; //stgDB.BestFloorArr[i]; // 수량 증가

                        // 최대수량 다 채웠을 경우
                        if(am.curStorage >= am.maxStorage)
                        {
                            am.curStorageTxt.color = Color.red;
                            am.curStorage = am.maxStorage;
                        }
                    }
                }

                UpdateUI();
            }

            yield return Util.TIME1;
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
            am.maxStorage = CalcMaxStorage(am.Lv);

            // 채굴량
            am.productionVal = stgDB.BestFloorArr[i];

            // 가격
            am.upgradePrice = CalcUpgradePrice(am.Lv);
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
            am.titleTxt.text = $"제 {i + 1} 광산 {stgDB.BestFloorArr[i]}층";

            // 현재수량
            am.curStorageTxt.text = $"<sprite name={am.Type}> {am.curStorage} / {am.maxStorage}";

            // 채굴량
            am.productionValTxt.text = $"채굴량 {am.productionVal}";

            // 다음 업그레이드 최대보관량 표시
            am.UpgradeInfoTxt.text = $"{am.maxStorage} => {CalcMaxStorage(am.Lv + 1)}";
        }
    }

    private int CalcMaxStorage(int lv)
    {
        return 100 + (lv - 1) * 100;
    }

    private int CalcUpgradePrice(int lv) 
    {
        return 1000 + (lv - 1) * 1000;
    }
#endregion
}
