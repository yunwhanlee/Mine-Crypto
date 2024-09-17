using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UpgradeDB
{
    [Header("업그레이드 데이터")]
    public UpgradeFormatInt upgAttack;
    public UpgradeFormatInt upgIncTimer;
    public UpgradeFormatFloat upgAttackSpeed;
    public UpgradeFormatFloat upgBagStorage;
    public UpgradeFormatFloat upgNextStageSkip;
    public UpgradeFormatInt upgIncPopulation;
    public UpgradeFormatFloat upgMoveSpeed;
    public UpgradeFormatInt upgIncCristal;

    public void Init()
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
}
