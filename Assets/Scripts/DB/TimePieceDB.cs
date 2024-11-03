using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

public class TimePieceDB
{
    [Header("업그레이드 데이터")]
    public UpgradeFormatInt upgFillVal;          // 1분당 회복 데이터
    public UpgradeFormatInt upgIncStorage;       // 보관량 증가 데이터
    public UpgradeFormatFloat upgIncTimeScale;   // 시간속도증가 데이터

    public void Init()
    {
        // 1분당 회복 데이터
        upgFillVal = new (
            Lv: 0, Unit: 10, NeedRsc: INV.ORE1, PriceDef: 10, DefVal: 100, MaxLv: 1000
        );
        // 보관량 증가 데이터
        upgIncStorage = new (
            Lv: 0, Unit: 100, NeedRsc: INV.ORE1, PriceDef: 10, DefVal: 1000, MaxLv: 1000
        );
        // 시간속도증가 데이터
        upgIncTimeScale = new (
            Lv: 0, Unit: 0.1f, NeedRsc: INV.ORE1, PriceDef: 30, DefVal: 1.2f, MaxLv: 28
        );
    }
}
