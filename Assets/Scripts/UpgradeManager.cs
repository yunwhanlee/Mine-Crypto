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


    //* VALUE
    public UpgradeFormatInt upgAttack;
    public UpgradeFormatFloat upgAttackSpeed;
    public UpgradeFormatFloat upgMoveSpeed;
    public UpgradeFormatInt upgBagStorage;
    public UpgradeFormatFloat upgExtraRebornPer;

    void Start()
    {
        upgAttack = new UpgradeFormatInt(Lv: 1, Unit: 1, PriceDef: 200);
        upgAttackSpeed = new UpgradeFormatFloat(Lv: 1, Unit: 0.2f, PriceDef: 200);
        upgMoveSpeed = new UpgradeFormatFloat(Lv: 1, Unit: 2, PriceDef: 200);
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
    public void OnClickUpgradeAttackBtn() => Upgrade(upgAttack);

    /// <summary>
    /// (강화) 공격속도 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeAttackSpeedBtn() => Upgrade(upgAttackSpeed);

    /// <summary>
    /// (강화) 이동속도 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeMoveSpeedBtn() => Upgrade(upgMoveSpeed);

    /// <summary>
    /// (강화) 가방용량 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeBagStorageBtn() => Upgrade(upgBagStorage);

    /// <summary>
    /// (강화) 추가 환생의결정 업그레이드 버튼
    /// </summary>
    public void OnClickUpgradeExtraRebornBtn() => Upgrade(upgExtraRebornPer);
#endregion

#region FUNC
    private void Upgrade(UpgradeFormat upgradeDt) {
        if(DM._.DB.statusDB.Coin >= upgradeDt.Price)
        {
            GM._.ui.ShowNoticeMsgPopUp("업그레이드 성공!");
            DM._.DB.statusDB.Coin -= upgradeDt.Price;
            upgradeDt.Lv++;

            UpdateUIAndData();
        }
        else
            GM._.ui.ShowWarningMsgPopUp("돈이 부족합니다.");
    }

    private void UpdateUIAndData()
    {
        // TODO 여기도 UpgradeFormat만들었으니까 클래스 내부에서 메서드 만들어서 리펙토리 하기.

        // Data
        upgAttack.UpdateVal();
        upgAttackSpeed.UpdateVal();
        upgMoveSpeed.UpdateVal();
        upgBagStorage.UpdateVal();
        upgExtraRebornPer.UpdateVal();

        upgAttack.UpdatePrice();
        upgAttackSpeed.UpdatePrice();
        upgMoveSpeed.UpdatePrice();
        upgBagStorage.UpdatePrice();
        upgExtraRebornPer.UpdatePrice();

        // UI
        upgAttackPriceTxt.text = $"{upgAttack.Price}";
        upgAttackSpeedPriceTxt.text = $"{upgAttackSpeed.Price}";
        upgMoveSpeedPriceTxt.text = $"{upgMoveSpeed.Price}";
        upgBagStoragePriceTxt.text = $"{upgBagStorage.Price}";
        upgExtraRebornPerPriceTxt.text = $"{upgExtraRebornPer.Price}";

        upgAttackInfoTxt.text = $"{upgAttack.Val} => {upgAttack.GetNextVal()}";
        upgAttackSpeedInfoTxt.text = $"{upgAttackSpeed.Val} => {upgAttackSpeed.GetNextVal()}";
        upgMoveSpeedInfoTxt.text = $"{upgMoveSpeed.Val} => {upgMoveSpeed.GetNextVal()}";
        upgBagStorageInfoTxt.text = $"{upgBagStorage.Val} => {upgBagStorage.GetNextVal()}";
        upgExtraRebornPerInfoTxt.text = $"{upgExtraRebornPer.Val * 100} => {upgExtraRebornPer.GetNextVal() * 100}%";
    }
#endregion
}
