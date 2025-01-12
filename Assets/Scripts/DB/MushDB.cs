using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Enum;

[Serializable]
public class MushDB
{
    [Header("업그레이드 데이터")]
    public bool isUnlock;
    public UpgradeMushFormatInt ms1_UpgAttack;
    public UpgradeMushFormatFloat ms2_UpgMovSpeedPer;
    public UpgradeMushFormatFloat ms3_UpgBagStoragePer;
    public UpgradeMushFormatFloat ms4_UpgNextStageSkipPer;
    public UpgradeMushFormatInt ms5_UpgIncTimer;
    public UpgradeMushFormatFloat ms6_UpgChestSpawnPer;
    public UpgradeMushFormatFloat ms7_UpgAtkSpeedPer;
    public UpgradeMushFormatInt ms8_IncPopulation;

    public void Init()
    {
        isUnlock = false;

        ms1_UpgAttack = new UpgradeMushFormatInt(
            Lv:0, Unit: 1, NeedMush: MUSH.MUSH1, PriceDef: 1, DefVal: 0, MaxLv: 1000
        );
        ms2_UpgMovSpeedPer = new UpgradeMushFormatFloat(
            Lv:0, Unit: 0.01f, NeedMush: MUSH.MUSH2, PriceDef: 1, DefVal: 0, MaxLv: 1000
        );
        ms3_UpgBagStoragePer = new UpgradeMushFormatFloat(
            Lv:0, Unit: 0.01f, NeedMush: MUSH.MUSH3, PriceDef: 1, DefVal: 0, MaxLv: 1000
        );
        ms4_UpgNextStageSkipPer = new UpgradeMushFormatFloat(
            Lv:0, Unit: 0.001f, NeedMush: MUSH.MUSH4, PriceDef: 1, DefVal: 0, MaxLv: 1000
        );
        ms5_UpgIncTimer = new UpgradeMushFormatInt(
            Lv:0, Unit: 30, NeedMush: MUSH.MUSH5, PriceDef: 1, DefVal: 0, MaxLv: 1000
        );
        ms6_UpgChestSpawnPer = new UpgradeMushFormatFloat(
            Lv:0, Unit: 0.005f, NeedMush: MUSH.MUSH6, PriceDef: 1, DefVal: 0, MaxLv: 1000
        );
        ms7_UpgAtkSpeedPer = new UpgradeMushFormatFloat(
            Lv:0, Unit: 0.05f, NeedMush: MUSH.MUSH7, PriceDef: 1, DefVal: 0, MaxLv: 1000
        );
        ms8_IncPopulation = new UpgradeMushFormatInt(
            Lv:0, Unit: 1, NeedMush: MUSH.MUSH8, PriceDef: 1, DefVal: 0, MaxLv: 20
        );
    }
}
