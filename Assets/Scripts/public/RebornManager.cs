using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static Enum;

public class RebornManager : MonoBehaviour
{
    //* COMPONENT
    public DOTweenAnimation DOTAnim;

    //* ELEMENT
    public GameObject windowObj;

    [Header("업그레이드 UI")]
    public UpgradeUIFormat upgIncLightStonePerUI;             // 빛의돌 획득량%
    public UpgradeUIFormat upgIncOrePerUI;                    // 모든광석 획득량%
    public UpgradeUIFormat upgDecSkillTimeUI;                 // (int) 스킬쿨타임 감소
    public UpgradeUIFormat upgMinOreBlessCntUI;               // (int) 축복옵션 최소개수
    public UpgradeUIFormat upgDecUpgradePricePerUI;           // 강화비용 감소%
    public UpgradeUIFormat upgDecConsumePricePerUI;           // 소모품 제작 비용감소%
    public UpgradeUIFormat upgDecDecoPricePerUI;              // 장식 제작 비용감소%
    public UpgradeUIFormat upgDecTranscendPircePerUI;         // 초월 강화비용감소%

    //* VALUE
    [Header("업그레이드 데이터")]
    public UpgradeFormatFloat upgIncLightStonePer;            // 빛의돌 획득량%
    public UpgradeFormatFloat upgIncOrePer;                   // 모든광석 획득량%
    public UpgradeFormatInt upgDecSkillTime;                  // (int) 스킬쿨타임 감소
    public UpgradeFormatInt upgMinOreBlessCnt;                // (int) 축복옵션 최소개수
    public UpgradeFormatFloat upgDecUpgradePricePer;          // 강화비용 감소%
    public UpgradeFormatFloat upgDecConsumePricePer;          // 소모품 제작 비용감소%
    public UpgradeFormatFloat upgDecDecoPricePer;             // 장식 제작 비용감소%
    public UpgradeFormatFloat upgDecTranscendPircePer;        // 초월 강화비용감소%

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // DB가 null일 경우 새로 초기화
        if (DM._.DB.rebornDB == null)
        {
            Debug.Log($"<color=red>{this.name}DB데이터가 없음으로 자체 초기화</color>");
            DM._.DB.rebornDB = new RebornDB();
            DM._.DB.rebornDB.Init();
        }

        //* 데이터 최신화
        // 빛의돌 획득량%
        upgIncLightStonePer = DM._.DB.rebornDB.upgIncLightStonePer;
        // 모든광석 획득량%
        upgIncOrePer = DM._.DB.rebornDB.upgIncOrePer;
        // (int) 스킬쿨타임 감소
        upgDecSkillTime = DM._.DB.rebornDB.upgDecSkillTime;
        // (int) 축복옵션 최소개수
        upgMinOreBlessCnt = DM._.DB.rebornDB.upgMinOreBlessCnt;
        // 강화비용 감소%
        upgDecUpgradePricePer = DM._.DB.rebornDB.upgDecUpgradePricePer;
        // 소모품 제작 비용감소%
        upgDecConsumePricePer = DM._.DB.rebornDB.upgDecConsumePricePer;
        // 장식 제작 비용감소%
        upgDecDecoPricePer = DM._.DB.rebornDB.upgDecDecoPricePer;
        // 초월 강화비용감소%
        upgDecTranscendPircePer = DM._.DB.rebornDB.upgDecTranscendPircePer;
    }

#region EVENT
    // 빛의돌 획득량% 버튼
    public void OnClickUpgIncLightStonePerBtn() => Upgrade(upgIncLightStonePer);
    // 모든광석 획득량% 버튼
    public void OnClickUpgIncOrePerBtn() => Upgrade(upgIncOrePer);
    // (int) 스킬쿨타임 감소 버튼
    public void OnClickUpgDecSkillTimeBtn() => Upgrade(upgDecSkillTime);
    // (int) 축복옵션 최소개수 버튼
    public void OnClickUpgMinOreBlessCntBtn() => Upgrade(upgMinOreBlessCnt);
    // 강화비용 감소% 버튼
    public void OnClickUpgDecUpgradePricePerBtn() => Upgrade(upgDecUpgradePricePer);
    // 소모품 제작 비용감소% 버튼
    public void OnClickUpgDecConsumePricePerBtn() => Upgrade(upgDecConsumePricePer);
    // 장식 제작 비용감소% 버튼
    public void OnClickUpgDecDecoPricePerBtn() => Upgrade(upgDecDecoPricePer);
    // 초월 강화비용감소% 버튼
    public void OnClickUpgDecTranscendPircePerBtn() => Upgrade(upgDecTranscendPircePer);
    // 환생 버튼
    public void OnClickRebornBtn() => Reborn();
#endregion

#region FUNC
    /// <summary>
    /// 환생하기
    /// </summary>
    public void Reborn()
    {

    }

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
    private void Upgrade(UpgradeFormat upgDt)
    {
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

            GM._.fm.missionArr[(int)MISSION.UPGRADE_CNT].Exp++;
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
        //* Data Price
        upgIncLightStonePer.UpdatePrice();
        upgIncOrePer.UpdatePrice();
        upgDecSkillTime.UpdatePrice();
        upgMinOreBlessCnt.UpdatePriceFreeSet(new int[] {500, 1000});
        upgDecUpgradePricePer.UpdatePrice();
        upgDecConsumePricePer.UpdatePrice();
        upgDecDecoPricePer.UpdatePrice();
        upgDecTranscendPircePer.UpdatePrice();

        //* UI
        upgIncLightStonePerUI.UpdateUI(upgIncLightStonePer);
        upgIncOrePerUI.UpdateUI(upgIncOrePer);
        upgDecSkillTimeUI.UpdateUI(upgDecSkillTime);
        upgMinOreBlessCntUI.UpdateUI(upgMinOreBlessCnt);
        upgDecUpgradePricePerUI.UpdateUI(upgDecUpgradePricePer);
        upgDecConsumePricePerUI.UpdateUI(upgDecConsumePricePer);
        upgDecDecoPricePerUI.UpdateUI(upgDecDecoPricePer);
        upgDecTranscendPircePerUI.UpdateUI(upgDecTranscendPircePer);
    }
#endregion
}
