using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

public class UpgradeManager : MonoBehaviour
{
    //* COMPONENT
    public DOTweenAnimation DOTAnim;

    //* ELEMENT
    public GameObject windowObj;

    public GameObject alertRedDotObj;

    [Header("업그레이드 UI")]
    public UpgradeUIFormat upgAttackUI;
    public UpgradeUIFormat upgIncTimerUI;
    public UpgradeUIFormat upgAttackSpeedUI;
    public UpgradeUIFormat upgBagStorageUI;
    public UpgradeUIFormat upgNextStageSkipUI;
    public UpgradeUIFormat upgIncPopulationUI;
    public UpgradeUIFormat upgMoveSpeedUI;
    public UpgradeUIFormat upgIncCristalUI;

    //* VALUE
    [Header("업그레이드 데이터")]
    public UpgradeFormatInt upgAttack;
    public UpgradeFormatInt upgIncTimer;
    public UpgradeFormatFloat upgAttackSpeed;
    public UpgradeFormatFloat upgBagStorage;
    public UpgradeFormatFloat upgNextStageSkip;
    public UpgradeFormatInt upgIncPopulation;
    public UpgradeFormatFloat upgMoveSpeed;
    public UpgradeFormatInt upgIncCristal;

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // 데이터 로드
        upgAttack = DM._.DB.upgradeDB.upgAttack;
        upgIncTimer = DM._.DB.upgradeDB.upgIncTimer;
        upgAttackSpeed = DM._.DB.upgradeDB.upgAttackSpeed;
        upgBagStorage = DM._.DB.upgradeDB.upgBagStorage;
        upgNextStageSkip = DM._.DB.upgradeDB.upgNextStageSkip;
        upgIncPopulation = DM._.DB.upgradeDB.upgIncPopulation;
        upgMoveSpeed = DM._.DB.upgradeDB.upgMoveSpeed;
        upgIncCristal = DM._.DB.upgradeDB.upgIncCristal;

        UpdateDataAndUI();
    }

#region EVENT
    /// <summary>
    /// (강화) 공격력 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeAttackBtn() => Upgrade(upgAttack);

    /// <summary>
    /// (강화) 채굴시간 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeIncTimerBtn() => Upgrade(upgIncTimer);

    /// <summary>
    /// (강화) 공격속도 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeAttackSpeedBtn() => Upgrade(upgAttackSpeed);

    /// <summary>
    /// (강화) 가방용량 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeBagStorageBtn() => Upgrade(upgBagStorage);

    /// <summary>
    /// (강화) 다음 층 스킵 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeNextStageSkipBtn() => Upgrade(upgNextStageSkip);

    /// <summary>
    /// (강화) 고용 수 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeIncPopulationBtn() => Upgrade(upgIncPopulation);

    /// <summary>
    /// (강화) 이동속도 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeMoveSpeedBtn() => Upgrade(upgMoveSpeed);

    /// <summary>
    /// (강화) 크리스탈 획득량 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeIncCristalBtn() => Upgrade(upgIncCristal);
#endregion

#region FUNC
    /// <summary>
    /// 팝업 열기
    /// </summary>
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        UpdateDataAndUI();
    }

    /// <summary>
    /// 업그레이드 가능 알림UI 🔴
    /// </summary>
    public void UpdateAlertRedDotUI()
    {
        bool isAlertOn = (
            upgAttack.CheckPossibleUpgrade() ||          // upgAttackUI.PriceTxt.color == Color.white
            upgIncTimer.CheckPossibleUpgrade() ||        // || upgIncTimerUI.PriceTxt.color == Color.white
            upgAttackSpeed.CheckPossibleUpgrade() ||     // || upgAttackSpeedUI.PriceTxt.color == Color.white
            upgBagStorage.CheckPossibleUpgrade() ||      // || upgBagStorageUI.PriceTxt.color == Color.white
            upgNextStageSkip.CheckPossibleUpgrade() ||   // || upgNextStageSkipUI.PriceTxt.color == Color.white
            upgIncPopulation.CheckPossibleUpgrade() ||   // || upgIncPopulationUI.PriceTxt.color == Color.white
            upgMoveSpeed.CheckPossibleUpgrade() ||       // || upgMoveSpeedUI.PriceTxt.color == Color.white
            upgIncCristal.CheckPossibleUpgrade()         // || upgIncCristalUI.PriceTxt.color == Color.white
        );
        alertRedDotObj.SetActive(isAlertOn);
    }

    private void Upgrade(UpgradeFormat upgDt)
    {
        if(upgDt.IsMaxLv)
        {
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.MaxLvMsg));
            return;
        }

        if(DM._.DB.statusDB.RscArr[(int)upgDt.NeedRsc] >= upgDt.Price)
        {
            SoundManager._.PlaySfx(SoundManager.SFX.UpgradeSFX);
            GM._.ui.ShowNoticeMsgPopUp(LM._.Localize(LM.UpgradeCompleteMsg));
            DM._.DB.statusDB.SetRscArr((int)upgDt.NeedRsc, -upgDt.Price);
            GM._.fm.missionArr[(int)Enum.MISSION.UPGRADE_CNT].Exp++;
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
        upgAttack.UpdatePrice(DEC_UPG_TYPE.UPGRADE);
        upgIncTimer.UpdatePrice(DEC_UPG_TYPE.UPGRADE);
        upgAttackSpeed.UpdatePrice(DEC_UPG_TYPE.UPGRADE);
        upgBagStorage.UpdatePrice(DEC_UPG_TYPE.UPGRADE);
        upgNextStageSkip.UpdatePrice(DEC_UPG_TYPE.UPGRADE);
        upgIncPopulation.UpdatePrice(DEC_UPG_TYPE.UPGRADE);
        upgMoveSpeed.UpdatePrice(DEC_UPG_TYPE.UPGRADE);
        upgIncCristal.UpdatePrice(DEC_UPG_TYPE.UPGRADE);

        //* UI
        upgAttackUI.UpdateUI(upgAttack);
        upgIncTimerUI.UpdateUI(upgIncTimer, "sec");
        upgAttackSpeedUI.UpdateUI(upgAttackSpeed);
        upgBagStorageUI.UpdateUI(upgBagStorage);
        upgNextStageSkipUI.UpdateUI(upgNextStageSkip);
        upgIncPopulationUI.UpdateUI(upgIncPopulation);
        upgMoveSpeedUI.UpdateUI(upgMoveSpeed);
        upgIncCristalUI.UpdateUI(upgIncCristal);

        UpdateAlertRedDotUI();
    }
#endregion
}
