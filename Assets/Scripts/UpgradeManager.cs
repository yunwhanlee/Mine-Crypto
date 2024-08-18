using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    //* COMPONENT
    public DOTweenAnimation DOTAnim;

    //* ELEMENT
    public GameObject windowObj;

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

    void Start()
    {
        upgAttack = new UpgradeFormatInt(
            Lv: 0, Unit: 1, NeedRsc: Enum.RSC.ORE1, PriceDef: 200, DefVal: 0, MaxLv: 1000
        );
        upgIncTimer = new UpgradeFormatInt(
            Lv: 0, Unit: 10, NeedRsc: Enum.RSC.ORE2, PriceDef: 200, DefVal: 30, MaxLv: 3000
        );
        upgAttackSpeed = new UpgradeFormatFloat(
            Lv: 0, Unit: 0.05f, NeedRsc: Enum.RSC.ORE3, PriceDef: 200, DefVal: 0, MaxLv: 20
        );
        upgBagStorage = new UpgradeFormatFloat(
            Lv: 0, Unit: 0.05f, NeedRsc: Enum.RSC.ORE4, PriceDef: 200, DefVal: 0, MaxLv: 40
        );
        upgNextStageSkip = new UpgradeFormatFloat(
            Lv: 0, Unit: 0.002f, NeedRsc: Enum.RSC.ORE5, PriceDef: 200, DefVal: 0, MaxLv: 50
        );
        upgIncPopulation = new UpgradeFormatInt(
            Lv: 0, Unit: 1, NeedRsc: Enum.RSC.ORE6, PriceDef: 200, DefVal: 3, MaxLv: 7
        );
        upgMoveSpeed = new UpgradeFormatFloat(
            Lv: 0, Unit: 0.05f, NeedRsc: Enum.RSC.ORE7, PriceDef: 200, DefVal: 0, MaxLv: 20
        );
        upgIncCristal = new UpgradeFormatInt(
            Lv: 0, Unit: 1, NeedRsc: Enum.RSC.ORE8, PriceDef: 200, DefVal: 0, MaxLv: 100
        );
    }

#region EVENT FUNC
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
    public void ShowPopUp() {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        UpdateUIAndData();
    }

    private void Upgrade(UpgradeFormat upgDt) {
        if(DM._.DB.statusDB.RscArr[(int)upgDt.NeedRsc] >= upgDt.Price)
        {
            GM._.ui.ShowNoticeMsgPopUp("업그레이드 성공!");
            DM._.DB.statusDB.SetRscArr((int)upgDt.NeedRsc, -upgDt.Price);
            upgDt.Lv++;

            UpdateUIAndData();
        }
        else
            GM._.ui.ShowWarningMsgPopUp("돈이 부족합니다.");
    }

    /// <summary>
    /// 업그레이드 결과 최신화
    /// </summary>
    private void UpdateUIAndData()
    {
        //* Data Val
        upgAttack.UpdateVal();
        upgIncTimer.UpdateVal();
        upgAttackSpeed.UpdateVal();
        upgBagStorage.UpdateVal();
        upgNextStageSkip.UpdateVal();
        upgIncPopulation.UpdateVal();
        upgMoveSpeed.UpdateVal();
        upgIncCristal.UpdateVal();

        //* Data Price
        upgAttack.UpdatePrice();
        upgIncTimer.UpdatePrice();
        upgAttackSpeed.UpdatePrice();
        upgBagStorage.UpdatePrice();
        upgNextStageSkip.UpdatePrice();
        upgIncPopulation.UpdatePrice();
        upgMoveSpeed.UpdatePrice();
        upgIncCristal.UpdatePrice();

        //* UI
        upgAttackUI.UpdateUI(upgAttack);
        upgIncTimerUI.UpdateUI(upgIncTimer, "sec");
        upgAttackSpeedUI.UpdateUI(upgAttackSpeed);
        upgBagStorageUI.UpdateUI(upgBagStorage);
        upgNextStageSkipUI.UpdateUI(upgNextStageSkip);
        upgIncPopulationUI.UpdateUI(upgIncPopulation);
        upgMoveSpeedUI.UpdateUI(upgMoveSpeed);
        upgIncCristalUI.UpdateUI(upgIncCristal);
    }
#endregion
}
