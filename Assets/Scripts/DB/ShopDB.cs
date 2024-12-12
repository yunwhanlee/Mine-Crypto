using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Enum;

[Serializable]
public class ShopDB
{
    public int fameSupplyTime;              // 명예보급 시간
    public bool[] IsAcceptFameSupplyArr;    // 명예보급 획득 트리거 배열

    public void Init()
    {
        fameSupplyTime = 60;
        IsAcceptFameSupplyArr = new bool[15] {
            false, false, false, false, false,
            false, false, false, false, false,
            false, false, false, false, false,
        };
    }
}
