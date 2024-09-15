using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeUIFormat
{
    [field:SerializeField] public TMP_Text InfoTxt {get; private set;}
    [field:SerializeField] public Image PriceRscImg {get; private set;}
    [field:SerializeField] public TMP_Text PriceTxt {get; private set;}

    /// <summary>
    /// UI 업데이트 (Int형)
    /// </summary>
    public void UpdateUI(UpgradeFormatInt upgFormat, string unitName = "") {
        Debug.Log($"UpdateUI():: {upgFormat.NeedRsc} => idx: {(int)upgFormat.NeedRsc}");
        PriceRscImg.sprite = GM._.RscSprArr[(int)upgFormat.NeedRsc];
        PriceTxt.text = $"{upgFormat.Price}";
        InfoTxt.text = $"{upgFormat.Val} => {upgFormat.GetNextVal()}{unitName}";
    }

    /// <summary>
    /// UI 업데이트 (Float형)
    /// </summary>
    public void UpdateUI(UpgradeFormatFloat upgFormat) {
        Debug.Log($"UpdateUI():: {upgFormat.NeedRsc} => idx: {(int)upgFormat.NeedRsc}");
        PriceRscImg.sprite = GM._.RscSprArr[(int)upgFormat.NeedRsc];
        PriceTxt.text = $"{upgFormat.Price}";
        InfoTxt.text = $"{Mathf.Round(upgFormat.Val * 100)} => {Mathf.Round(upgFormat.GetNextVal() * 100)}%";
    }
}
