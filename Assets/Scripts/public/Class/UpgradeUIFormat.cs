using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private string GetNeedPriceTxtWithColor(RSC needRscType, int price)
    {
        // 구매가능한지 여부 색깔태그
        string colorTag = DM._.DB.statusDB.RscArr[(int)needRscType] >= price? "white" : "grey";

        // 필요재화 가격 + 이미지 적용
        return $"<size=75%><sprite name={needRscType}><color={colorTag}></size> {price}</color>";
    }

    /// <summary>
    /// UI 업데이트 (Int형)
    /// </summary>
    public void UpdateUI(UpgradeFormatInt upgFormat, string unitName = "") {
        Debug.Log($"UpdateUI():: {upgFormat.NeedRsc} => idx: {(int)upgFormat.NeedRsc}");

        // 가격 표시
        PriceTxt.text = GetNeedPriceTxtWithColor(upgFormat.NeedRsc, upgFormat.Price);

        // 능력치 표시
        InfoTxt.text = $"{upgFormat.Val} => {upgFormat.GetNextVal()}{unitName}";
    }

    /// <summary>
    /// UI 업데이트 (Float형)
    /// </summary>
    public void UpdateUI(UpgradeFormatFloat upgFormat) {
        Debug.Log($"UpdateUI():: {upgFormat.NeedRsc} => idx: {(int)upgFormat.NeedRsc}");

        // 가격 표시
        PriceTxt.text = GetNeedPriceTxtWithColor(upgFormat.NeedRsc, upgFormat.Price);

        // 능력치 표시
        InfoTxt.text = $"{upgFormat.Val * 100} => {upgFormat.GetNextVal() * 100}%";
    }
}
