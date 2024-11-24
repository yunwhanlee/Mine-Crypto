using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Enum;

[Serializable]
public class RebornDB
{
    [Header("업그레이드 저장용 데이터")]
    public UpgradeFormatFloat upgIncLightStonePer;            // 빛의돌 획득량%
    public UpgradeFormatFloat upgIncOrePer;                   // 모든광석 획득량%
    public UpgradeFormatInt upgDecSkillTime;                  // (int) 스킬쿨타임 감소
    public UpgradeFormatInt upgMinOreBlessCnt;                // (int) 축복옵션 최소개수
    public UpgradeFormatFloat upgDecUpgradePricePer;          // 강화비용 감소%
    public UpgradeFormatFloat upgDecConsumePricePer;          // 소모품 제작 비용감소%
    public UpgradeFormatFloat upgDecDecoPricePer;             // 장식 제작 비용감소%
    public UpgradeFormatFloat upgDecTranscendPircePer;        // 초월 강화비용감소%

    public void Init()
    {
        // 빛의돌 획득량%
        upgIncLightStonePer = new (
            Lv: 0, Unit: 0.1f, NeedRsc: INV.CRISTAL, PriceDef: 1000, DefVal: 0, MaxLv: 9999);
        // 모든광석 획득량%
        upgIncOrePer = new (
            Lv: 0, Unit: 0.1f, NeedRsc: INV.LIGHTSTONE, PriceDef: 10, DefVal: 0, MaxLv: 9999);
        // (int) 스킬쿨타임 감소
        upgDecSkillTime = new (
            Lv: 0, Unit: 1, NeedRsc: INV.LIGHTSTONE, PriceDef: 10, DefVal: 0, MaxLv: 30);
        // (int) 축복옵션 최소개수
        upgMinOreBlessCnt = new (
            Lv: 0, Unit: 1, NeedRsc: INV.LIGHTSTONE, PriceDef: 10, DefVal: 1, MaxLv: 2);
        // 강화비용 감소%  
        upgDecUpgradePricePer = new (
            Lv: 0, Unit: 0.01f, NeedRsc: INV.LIGHTSTONE, PriceDef: 20, DefVal: 0, MaxLv: 99);
        // 소모품 제작 비용감소%
        upgDecConsumePricePer = new (
            Lv: 0, Unit: 0.1f, NeedRsc: INV.LIGHTSTONE, PriceDef: 100, DefVal: 0, MaxLv: 9);
        // 장식 제작 비용감소%
        upgDecDecoPricePer = new (
            Lv: 0, Unit: 0.1f, NeedRsc: INV.LIGHTSTONE, PriceDef: 100, DefVal: 0, MaxLv: 9);
        // 초월 강화비용감소%
        upgDecTranscendPircePer = new (
            Lv: 0, Unit: 0.1f, NeedRsc: INV.LIGHTSTONE, PriceDef: 200, DefVal: 0, MaxLv: 9);
    }
}
