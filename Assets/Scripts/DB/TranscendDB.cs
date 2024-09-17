using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Enum;

[Serializable]
public class TranscendDB
{
[Header("업그레이드 데이터")]
    public UpgradeFormatFloat upgIncAutoOrePer;                 // 자동 광석 수량
    public UpgradeFormatFloat upgIncAutoCristalPer;             // 자동 크리스탈 수량
    public UpgradeFormatFloat upgDecAlchemyMaterialPer;         // 재료 제작비용 감소
    public UpgradeFormatInt upgIncTreasureChest;                // 보물상자 획득량 +
    public UpgradeFormatFloat upgIncAutoOreBagStoragePer;       // 자동 광석 보관량
    public UpgradeFormatFloat upgIncAutoCristalBagStoragePer;   // 자동 크리스탈 보관량
    public UpgradeFormatInt upgIncPopulation;                   // 소환캐릭 증가
    public UpgradeFormatInt upgIncFame;                         // 명예 획득량 +

    public void Init()
    {
        // 자동 광석 수량%
        upgIncAutoOrePer = new ( 
            Lv: 0, Unit: 0.1f, NeedRsc: RSC.CRISTAL, PriceDef: 100, DefVal: 0, MaxLv: 1000);
        // 자동 크리스탈 수량%
        upgIncAutoCristalPer = new ( 
            Lv: 0, Unit: 0.1f, NeedRsc: RSC.CRISTAL, PriceDef: 100, DefVal: 0, MaxLv: 1000);
        // 재료 제작비용 감소
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
}
