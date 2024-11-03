using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TimePieceManager : MonoBehaviour
{
    const int WAIT_TIME = 60; // 1분당 회복

    public DOTweenAnimation DOTAnim;

    //* ELEMENT
    public GameObject windowObj;
    public TMP_Text timerTxt;

    [Header("업그레이드 UI")]
    public UpgradeUIFormat upgFillValUI;         // 1분당 회복UI
    public UpgradeUIFormat upgIncStorageUI;      // 보관량 증가UI
    public UpgradeUIFormat upgIncTimeScaleUI;    // 시간속도증가UI

    //* VALUE
    private int time;                            // 회복 타이머시간
    public int maxStorage;                       // 시간의조각 최대보관량
    public int curStorage;                       // 시간의조각 현재보관량

    [Header("업그레이드 데이터")]
    public UpgradeFormatInt upgFillVal;          // 1분당 회복 데이터
    public UpgradeFormatInt upgIncStorage;       // 보관량 증가 데이터
    public UpgradeFormatFloat upgIncTimeScale;   // 시간속도증가 데이터

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);
        
        // timePieceDB가 null일 경우 새로 초기화
        if (DM._.DB.timePieceDB == null)
        {
            Debug.Log("<color=red>데이터가 없음으로 자체 초기화</color>");
            DM._.DB.timePieceDB = new TimePieceDB();
            DM._.DB.timePieceDB.Init();
        }

        // 1분당 회복 데이터
        upgFillVal = DM._.DB.timePieceDB.upgFillVal;
        // 보관량 증가 데이터
        upgIncStorage = DM._.DB.timePieceDB.upgIncStorage;
        // 시간속도증가 데이터  
        upgIncTimeScale = DM._.DB.timePieceDB.upgIncTimeScale;
    }

#region EVENT
    /// <summary>
    /// (업그레이드) 1분당 회복 버튼
    /// </summary>
    public void OnClickUpgradeFillValBtn() => Upgrade(upgFillVal);
    /// <summary>
    /// (업그레이드) 보관량 증가 버튼
    /// </summary>
    public void OnClickUpgradeIncStorageBtn() => Upgrade(upgIncStorage);
    /// <summary>
    /// (업그레이드) 시간속도 증가
    /// </summary>
    public void OnClickUpgradeIncTimeScaleBtn() => Upgrade(upgIncTimeScale);
#endregion

#region FUNC
    /// <summary>
    /// 팝업표시
    /// </summary>
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        UpdateDataAndUI();
    }

    /// <summary>
    /// 업그레이드 처리
    /// </summary>
    /// <param name="upgDt">업그레이드할 데이터</param>
    private void Upgrade(UpgradeFormat upgDt) {
        var sttDB = DM._.DB.statusDB;

        if(upgDt.IsMaxLv)
        {
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.MaxLvMsg));
            return;
        }

        if(sttDB.GetInventoryItemVal(upgDt.NeedRsc) >= upgDt.Price)
        {
            SoundManager._.PlaySfx(SoundManager.SFX.TranscendUpgradeSFX);
            GM._.ui.ShowNoticeMsgPopUp(LM._.Localize(LM.UpgradeCompleteMsg));

            // 제작에 필요한 아이템 수량 감소
            sttDB.SetInventoryItemVal(upgDt.NeedRsc, -upgDt.Price);
            upgDt.Lv++;

            UpdateDataAndUI();
        }
        else
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
    }

    /// <summary>
    /// 업그레이드 결과 최신화
    /// </summary>
    private void UpdateDataAndUI()
    {
        upgFillVal.UpdatePrice();
        upgIncStorage.UpdatePrice();
        upgIncTimeScale.UpdatePrice();

        upgFillValUI.UpdateUI(upgFillVal);
        upgIncStorageUI.UpdateUI(upgIncStorage);
        upgIncTimeScaleUI.UpdateUI(upgIncTimeScale);
    }
#endregion

}