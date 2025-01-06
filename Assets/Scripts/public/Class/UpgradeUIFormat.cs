using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using static Enum;

[System.Serializable]
public class UpgradeUIFormat
{
    [field:SerializeField] public TMP_Text InfoTxt {get; private set;}
    [field:SerializeField] public TMP_Text PriceTxt {get; private set;}

    /// <summary>
    /// 업그레이드 가격 표시 및 구매여부에 따른 색깔반영
    /// </summary>
    private string GetNeedPriceTxtWithColor(INV needRscType, int price)
    {
        var sttDB = DM._.DB.statusDB;

        // 구매가능한지 여부 색깔태그
        PriceTxt.color = sttDB.GetInventoryItemVal(needRscType) >= price? Color.white : Color.gray;

        // 필요재화 가격 + 이미지 적용
        return $"<size=75%><sprite name={needRscType}></size> {price}";
    }

    /// <summary>
    /// UI 업데이트 (Int형)
    /// </summary>
    public void UpdateUI(UpgradeFormatInt upgFormat, string unitName = "", int defVal = 0) {
        Debug.Log($"UpdateUI():: {upgFormat.NeedRsc} => idx: {(int)upgFormat.NeedRsc}");

        // 최대레벨
        if(upgFormat.IsMaxLv)
        {
            PriceTxt.text = "<color=yellow>MAX</color>";
            InfoTxt.text = $"{upgFormat.Val}";
            return;
        }

        // 가격 표시
        PriceTxt.text = GetNeedPriceTxtWithColor(upgFormat.NeedRsc, upgFormat.Price);

        // 능력치 표시
        InfoTxt.text = $"{upgFormat.Val + defVal} => {upgFormat.GetNextVal() + defVal}{unitName}";
    }

    /// <summary>
    /// UI 업데이트 (Float형)
    /// </summary>
    public void UpdateUI(UpgradeFormatFloat upgFormat) {
        Debug.Log($"UpdateUI():: {upgFormat.NeedRsc} => idx: {(int)upgFormat.NeedRsc}");

        // 최대레벨
        if(upgFormat.IsMaxLv)
        {
            PriceTxt.text = "<color=yellow>MAX</color>";
            InfoTxt.text = $"{Util.FloatToStr(upgFormat.Val * 100)}%";
            return;
        }

        // 가격 표시
        PriceTxt.text = GetNeedPriceTxtWithColor(upgFormat.NeedRsc, upgFormat.Price);

        // 능력치 표시
        InfoTxt.text = $"{Util.FloatToStr(upgFormat.Val * 100)} => {Util.FloatToStr(upgFormat.GetNextVal() * 100)}%";
    }
}
