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
    public GameObject LockFrameObj;

    [Header("업그레이드 UI")]
    public UpgradeUIFormat upgIncAutoOrePerUI;
    public UpgradeUIFormat upgIncAutoCristalPerUI;
    public UpgradeUIFormat upgDecAutoProducePerUI;
    public UpgradeUIFormat upgIncTreasureChestUI;
    public UpgradeUIFormat upgIncAutoOreBagStoragePerUI;
    public UpgradeUIFormat upgIncAutoCristalBagStoragePerUI;
    public UpgradeUIFormat upgIncStartFloorUI;
    public UpgradeUIFormat upgIncFameUI;

    //* VALUE
    [Header("업그레이드 데이터")]
    public UpgradeFormatFloat upgIncAutoOrePer;                 // 자동 광석 수량
    public UpgradeFormatFloat upgIncAutoCristalPer;             // 자동 크리스탈 수량
    public UpgradeFormatFloat upgDecAlchemyMaterialPer;         // 재료 제작비용 감소
    public UpgradeFormatInt upgIncTreasureChest;                // 보물상자 획득량 +
    public UpgradeFormatFloat upgIncAutoOreBagStoragePer;       // 자동 광석 보관량
    public UpgradeFormatFloat upgIncAutoCristalBagStoragePer;   // 자동 크리스탈 보관량
    public UpgradeFormatInt upgIncStartFloor;                   // 시작층수 증가
    public UpgradeFormatInt upgIncFame;                         // 명예 획득량 +

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // 자동 광석 수량%
        upgIncAutoOrePer = DM._.DB.transcendDB.upgIncAutoOrePer;
        // 자동 크리스탈 수량%
        upgIncAutoCristalPer = DM._.DB.transcendDB.upgIncAutoCristalPer;
        // 재료 제작비용 감소
        upgDecAlchemyMaterialPer = DM._.DB.transcendDB.upgDecAlchemyMaterialPer;
        // 보물상자 획득량 (int)
        upgIncTreasureChest = DM._.DB.transcendDB.upgIncTreasureChest;
        // 자동 광석 보관량%
        upgIncAutoOreBagStoragePer = DM._.DB.transcendDB.upgIncAutoOreBagStoragePer;
        // 자동 크리스탈 보관량%
        upgIncAutoCristalBagStoragePer = DM._.DB.transcendDB.upgIncAutoCristalBagStoragePer;
        // 시작층수 증가 (int)
        upgIncStartFloor = DM._.DB.transcendDB.upgIncStartFloor;
        // 명예 획득량 (int)
        upgIncFame = DM._.DB.transcendDB.upgIncFame;
    }

#region EVENT
    /// <summary>
    /// 잠금 아이콘 버튼
    /// </summary>
    public void OnClickLockFrameBtn()
        => GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.UnlockTranscendMsg));
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
    /// (강화) 시작층수 증가
    /// </summary>
    public void OnClickUpgradeIncPopulationBtn() => Upgrade(upgIncStartFloor);
    /// <summary>
    /// (강화) 명예 획득량 +
    /// </summary>
    public void OnClickUpgradeIncFameBtn() => Upgrade(upgIncFame);
#endregion

#region FUNC
    /// <summary>
    /// (초월) 시스템 잠금해제
    /// </summary>
    public void Unlock()
    {
        LockFrameObj.SetActive(false);
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
        upgIncAutoOrePer.UpdatePrice();
        upgIncAutoCristalPer.UpdatePrice();
        upgDecAlchemyMaterialPer.UpdatePrice();
        upgIncTreasureChest.UpdatePrice();
        upgIncAutoOreBagStoragePer.UpdatePrice();
        upgIncAutoCristalBagStoragePer.UpdatePrice();
        upgIncStartFloor.UpdatePrice();
        upgIncFame.UpdatePrice();

        //* UI
        upgIncAutoOrePerUI.UpdateUI(upgIncAutoOrePer);
        upgIncAutoCristalPerUI.UpdateUI(upgIncAutoCristalPer);
        upgDecAutoProducePerUI.UpdateUI(upgDecAlchemyMaterialPer);
        upgIncTreasureChestUI.UpdateUI(upgIncTreasureChest);
        upgIncAutoOreBagStoragePerUI.UpdateUI(upgIncAutoOreBagStoragePer);
        upgIncAutoCristalBagStoragePerUI.UpdateUI(upgIncAutoCristalBagStoragePer);
        upgIncStartFloorUI.UpdateUI(upgIncStartFloor, unitName: "", defVal: 1);
        upgIncFameUI.UpdateUI(upgIncFame);
    }
#endregion
}
