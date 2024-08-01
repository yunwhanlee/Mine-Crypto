using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    //* Component
    public DOTweenAnimation DOTAnim;

    //* Elements
    public GameObject windowObj;

    public TMP_Text upgAttackPriceTxt;
    public TMP_Text upgAttackSpeedPriceTxt;
    public TMP_Text upgMoveSpeedPriceTxt;
    public TMP_Text upgBagStoragePriceTxt;
    public TMP_Text upgExtraRebornPerPriceTxt;

    public TMP_Text upgAttackInfoTxt;
    public TMP_Text upgAttackSpeedInfoTxt;
    public TMP_Text upgMoveSpeedInfoTxt;
    public TMP_Text upgBagStorageInfoTxt;
    public TMP_Text upgExtraRebornPerInfoTxt;


    //* Value
    public UpgradeFormatInt upgAttack;
    public UpgradeFormatFloat upgAttackSpeed;
    public UpgradeFormatFloat upgMoveSpeed;
    public UpgradeFormatInt upgBagStorage;
    public UpgradeFormatFloat upgExtraRebornPer;

    void Start()
    {   // TODO Price값은 가짜값
        upgAttack = new UpgradeFormatInt(Lv: 1, Unit: 1, PriceDef: 200);
        upgAttackSpeed = new UpgradeFormatFloat(Lv: 1, Unit: 0.5f, PriceDef: 200);
        upgMoveSpeed = new UpgradeFormatFloat(Lv: 1, Unit: 0.5f, PriceDef: 200);
        upgBagStorage = new UpgradeFormatInt(Lv: 1, Unit: 10, PriceDef: 200);
        upgExtraRebornPer = new UpgradeFormatFloat(Lv: 1, Unit: 0.05f, PriceDef: 200);
    }

#region EVENT FUNC
    /// <summary>
    /// 팝업 열기
    /// </summary>
    public void OnClickPlusBtn()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        UpdateUIAndData();
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    public void OnClickDimScreen()
    {
        windowObj.SetActive(false);
    }

    /// <summary>
    /// (강화) 공격력 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeAttackBtn()
    {
        if(DM._.DB.statusDB.Coin >= upgAttack.Price)
        {
            GM._.ui.ShowNoticeMsgPopUp("공격력 업그레이드 완료!");
            DM._.DB.statusDB.Coin -= upgAttack.Price;
            upgAttack.Lv++;

            UpdateUIAndData();
        }
        else
            GM._.ui.ShowWarningMsgPopUp("돈이 부족합니다.");
    }

    /// <summary>
    /// (강화) 공격속도 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeAttackSpeedBtn()
    {
        if(DM._.DB.statusDB.Coin >= upgAttackSpeed.Price)
        {
            GM._.ui.ShowNoticeMsgPopUp("공격속도 업그레이드 완료!");
            DM._.DB.statusDB.Coin -= upgAttackSpeed.Price;
            upgAttackSpeed.Lv++;

            UpdateUIAndData();
        }
        else
            GM._.ui.ShowWarningMsgPopUp("돈이 부족합니다.");
    }

    /// <summary>
    /// (강화) 이동속도 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeMoveSpeedBtn()
    {
        if(DM._.DB.statusDB.Coin >= upgMoveSpeed.Price)
        {
            GM._.ui.ShowNoticeMsgPopUp("공격속도 업그레이드 완료!");
            DM._.DB.statusDB.Coin -= upgMoveSpeed.Price;
            upgMoveSpeed.Lv++;

            UpdateUIAndData();
        }
        else
            GM._.ui.ShowWarningMsgPopUp("돈이 부족합니다.");
    }

    /// <summary>
    /// (강화) 가방용량 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeBagStorageBtn()
    {
        if(DM._.DB.statusDB.Coin >= upgBagStorage.Price)
        {
            GM._.ui.ShowNoticeMsgPopUp("공격속도 업그레이드 완료!");
            DM._.DB.statusDB.Coin -= upgBagStorage.Price;
            upgBagStorage.Lv++;

            UpdateUIAndData();
        }
        else
            GM._.ui.ShowWarningMsgPopUp("돈이 부족합니다.");
    }

    /// <summary>
    /// (강화) 추가 환생의결정 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeExtraRebornBtn()
    {
        if(DM._.DB.statusDB.Coin >= upgExtraRebornPer.Price)
        {
            GM._.ui.ShowNoticeMsgPopUp("공격속도 업그레이드 완료!");
            DM._.DB.statusDB.Coin -= upgExtraRebornPer.Price;
            upgExtraRebornPer.Lv++;

            UpdateUIAndData();
        }
        else
            GM._.ui.ShowWarningMsgPopUp("돈이 부족합니다.");
    }
#endregion

#region FUNC
    private void UpdateUIAndData()
    {
        // TODO 여기도 UpgradeFormat만들었으니까 클래스 내부에서 메서드 만들어서 리펙토리 하기.

        // Data
        int nextUpgAttack = (upgAttack.Lv + 1) * upgAttack.Unit;
        float nextUpgAttackSpeed = (upgAttackSpeed.Lv + 1) * upgAttackSpeed.Unit;
        float nextUpgMoveSpeed = (upgMoveSpeed.Lv + 1) * upgMoveSpeed.Unit;
        int nextUpgBagStorage = (upgBagStorage.Lv + 1) * upgBagStorage.Unit;
        float nextUpgExtraRebornPer = (upgExtraRebornPer.Lv + 1) * upgExtraRebornPer.Unit;

        upgAttack.Val = upgAttack.Lv * upgAttack.Unit;
        upgAttackSpeed.Val = upgAttackSpeed.Lv * upgAttackSpeed.Unit;
        upgMoveSpeed.Val = upgMoveSpeed.Lv * upgMoveSpeed.Unit;
        upgBagStorage.Val = upgBagStorage.Lv * upgBagStorage.Unit;
        upgExtraRebornPer.Val = upgExtraRebornPer.Lv * upgExtraRebornPer.Unit;

        upgAttack.Price = upgAttack.PriceDef + upgAttack.Lv * (upgAttack.Lv - 1) * 100 / 2;
        upgAttackSpeed.Price = upgAttackSpeed.PriceDef + upgAttackSpeed.Lv * (upgAttackSpeed.Lv - 1) * 100 / 2;
        upgMoveSpeed.Price = upgMoveSpeed.PriceDef + upgMoveSpeed.Lv * (upgMoveSpeed.Lv - 1) * 100 / 2;
        upgBagStorage.Price = upgBagStorage.PriceDef + upgBagStorage.Lv * (upgBagStorage.Lv - 1) * 100 / 2;
        upgExtraRebornPer.Price = upgExtraRebornPer.PriceDef + upgExtraRebornPer.Lv * (upgExtraRebornPer.Lv - 1) * 100 / 2;

        // UI
        upgAttackPriceTxt.text = $"{upgAttack.Price}";
        upgAttackSpeedPriceTxt.text = $"{upgAttackSpeed.Price}";
        upgMoveSpeedPriceTxt.text = $"{upgMoveSpeed.Price}";
        upgBagStoragePriceTxt.text = $"{upgBagStorage.Price}";
        upgExtraRebornPerPriceTxt.text = $"{upgExtraRebornPer.Price}";

        upgAttackInfoTxt.text = $"{upgAttack.Val} => {nextUpgAttack}";
        upgAttackSpeedInfoTxt.text = $"{upgAttackSpeed.Val} => {nextUpgAttackSpeed}";
        upgMoveSpeedInfoTxt.text = $"{upgMoveSpeed.Val} => {nextUpgMoveSpeed}";
        upgBagStorageInfoTxt.text = $"{upgBagStorage.Val} => {nextUpgBagStorage}";
        upgExtraRebornPerInfoTxt.text = $"{upgExtraRebornPer.Val * 100} => {nextUpgExtraRebornPer * 100}%";
    }
#endregion
}
