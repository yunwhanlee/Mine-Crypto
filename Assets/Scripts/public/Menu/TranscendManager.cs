using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static Enum;

public class TranscendManager : MonoBehaviour
{
    //* COMPONENT
    public DOTweenAnimation DOTAnim;

    //* ELEMENT
    public GameObject windowObj;

    [Header("업그레이드 UI")]
    public UpgradeUIFormat upgIncAutoOrePerUI;
    public UpgradeUIFormat upgIncAutoCristalPerUI;
    public UpgradeUIFormat upgDecAutoProducePerUI;
    public UpgradeUIFormat upgIncTreasureChestUI;
    public UpgradeUIFormat upgIncAutoOreBagStoragePerUI;
    public UpgradeUIFormat upgIncAutoCristalBagStoragePerUI;
    public UpgradeUIFormat upgIncPopulationUI;
    public UpgradeUIFormat upgIncFameUI;

    //* VALUE
    [Header("업그레이드 데이터")]
    public UpgradeFormatFloat upgIncAutoOrePer;                 // 자동 광석 수량
    public UpgradeFormatFloat upgIncAutoCristalPer;             // 자동 크리스탈 수량
    public UpgradeFormatFloat upgDecAlchemyMaterialPer;         //TODO 재료 제작비용 감소
    public UpgradeFormatInt upgIncTreasureChest;                // 보물상자 획득량 +
    public UpgradeFormatFloat upgIncAutoOreBagStoragePer;       // 자동 광석 보관량
    public UpgradeFormatFloat upgIncAutoCristalBagStoragePer;   // 자동 크리스탈 보관량
    public UpgradeFormatInt upgIncPopulation;                   // 소환캐릭 증가
    public UpgradeFormatInt upgIncFame;                         // 명예 획득량 +

    void Start()
    {
        // 자동 광석 수량%
        upgIncAutoOrePer = new ( 
            Lv: 0, Unit: 0.1f, NeedRsc: RSC.CRISTAL, PriceDef: 100, DefVal: 0, MaxLv: 1000);
        // 자동 크리스탈 수량%
        upgIncAutoCristalPer = new ( 
            Lv: 0, Unit: 0.1f, NeedRsc: RSC.CRISTAL, PriceDef: 100, DefVal: 0, MaxLv: 1000);
        //TODO 재료 제작비용 감소
        upgDecAlchemyMaterialPer = new ( 
            Lv: 0, Unit: 0.01f, NeedRsc: RSC.CRISTAL, PriceDef: 100, DefVal: 0, MaxLv: 90);
        // 보물상자 획득량 (int)
        upgIncTreasureChest = new ( 
            Lv: 0, Unit: 1, NeedRsc: RSC.CRISTAL, PriceDef: 1000, DefVal: 0, MaxLv: 1000);
        // 자동 광석 보관량%
        upgIncAutoOreBagStoragePer = new ( 
            Lv: 0, Unit: 0.1f, NeedRsc: RSC.CRISTAL, PriceDef: 100, DefVal: 0, MaxLv: 1000);
        // 자동 크리스탈 보관량%
        upgIncAutoCristalBagStoragePer = new ( 
            Lv: 0, Unit: 0.1f, NeedRsc: RSC.CRISTAL, PriceDef: 100, DefVal: 0, MaxLv: 1000);
        // 소환캐릭 증가 (int)
        upgIncPopulation = new ( 
            Lv: 0, Unit: 1, NeedRsc: RSC.CRISTAL, PriceDef: 1000, DefVal: 0, MaxLv: 30);
        // 명예 획득량 (int)
        upgIncFame = new ( 
            Lv: 0, Unit: 1, NeedRsc: RSC.CRISTAL, PriceDef: 5000, DefVal: 0, MaxLv: 1000);
    }

#region EVENT
    /// <summary>
    /// (강화) 자동 광석 수량
    /// </summary>
    public void OnClickUpgradeIncAutoOrePerBtn()=> Upgrade(upgIncAutoOrePer);
    /// <summary>
    /// (강화) 자동 크리스탈 수량
    /// </summary>
    public void OnClickUpgradeIncAutoCristalPerBtn() => Upgrade(upgIncAutoCristalPer);
    /// <summary>
    /// (강화) 재료 제작비용 감소
    /// </summary>
    public void OnClickUpgradeDecAutoProducePerBtn() => Upgrade(upgDecAlchemyMaterialPer);
    /// <summary>
    /// (강화) 보물상자 획득량 +
    /// </summary>
    public void OnClickUpgradeIncTreasureChestBtn() => Upgrade(upgIncTreasureChest);
    /// <summary>
    /// (강화) 자동 광석 보관량
    /// </summary>
    public void OnClickUpgradeIncAutoOreBagStoragePerBtn() => Upgrade(upgIncAutoOreBagStoragePer);
    /// <summary>
    /// (강화) 자동 크리스탈 보관량
    /// </summary>
    public void OnClickUpgradeIncAutoCristalBagStoragePerBtn() => Upgrade(upgIncAutoCristalBagStoragePer);
    /// <summary>
    /// (강화) 소환캐릭 증가
    /// </summary>
    public void OnClickUpgradeIncPopulationBtn() => Upgrade(upgIncPopulation);
    /// <summary>
    /// (강화) 명예 획득량 +
    /// </summary>
    public void OnClickUpgradeIncFameBtn() => Upgrade(upgIncFame);
#endregion

#region FUNC
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        UpdateDataAndUI();
    }

    private void Upgrade(UpgradeFormat upgDt) {
        if(DM._.DB.statusDB.RscArr[(int)upgDt.NeedRsc] >= upgDt.Price)
        {
            GM._.ui.ShowNoticeMsgPopUp("업그레이드 성공!");
            DM._.DB.statusDB.SetRscArr((int)upgDt.NeedRsc, -upgDt.Price);
            GM._.fm.missionArr[(int)MISSION.UPGRADE_CNT].Exp++;
            upgDt.Lv++;

            UpdateDataAndUI();
        }
        else
            GM._.ui.ShowWarningMsgPopUp("해당 재화가 부족합니다!");
    }

    /// <summary>
    /// 업그레이드 결과 최신화
    /// </summary>
    private void UpdateDataAndUI()
    {
        //* Data Price
        upgIncAutoOrePer.UpdatePrice();
        upgIncAutoCristalPer.UpdatePrice();
        upgDecAlchemyMaterialPer.UpdatePrice();
        upgIncTreasureChest.UpdatePrice();
        upgIncAutoOreBagStoragePer.UpdatePrice();
        upgIncAutoCristalBagStoragePer.UpdatePrice();
        upgIncPopulation.UpdatePrice();
        upgIncFame.UpdatePrice();

        //* UI
        upgIncAutoOrePerUI.UpdateUI(upgIncAutoOrePer);
        upgIncAutoCristalPerUI.UpdateUI(upgIncAutoCristalPer);
        upgDecAutoProducePerUI.UpdateUI(upgDecAlchemyMaterialPer);
        upgIncTreasureChestUI.UpdateUI(upgIncTreasureChest);
        upgIncAutoOreBagStoragePerUI.UpdateUI(upgIncAutoOreBagStoragePer);
        upgIncAutoCristalBagStoragePerUI.UpdateUI(upgIncAutoCristalBagStoragePer);
        upgIncPopulationUI.UpdateUI(upgIncPopulation);
        upgIncFameUI.UpdateUI(upgIncFame);
    }
#endregion
}
