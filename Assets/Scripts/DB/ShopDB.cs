using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Enum;

[Serializable]
public class ShopDB
{
    // 명예보급 획득 트리거 배열수량 상수
    public const int FAME_SUPPLY_ARRCNT = 15;
    public const int REBORN_SUPPLY_ARRCNT = 30;

    public int fameSupplyTime;              // 명예보급 시간
    public bool[] IsAcceptFameSupplyArr;    // 명예보급 획득 트리거 배열
    public bool[] IsAcceptRebornSupplyArr;  // 환생보급 획득 트리거 배열

    public void Init()
    {
        fameSupplyTime = ShopManager.FAME_SUPPLY_RESET_TIME_SEC;

        InitIsAcceptFameSupplyArr();
        InitIsAcceptRebornSupplyArr();
    }

    /// <summary>
    /// 처음시작 및 배열수량 수정시 Out of Index 에러 방지처리
    /// </summary>
    public void CheckNewDataErr()
    {
        if(IsAcceptFameSupplyArr.Length != FAME_SUPPLY_ARRCNT)
            InitIsAcceptFameSupplyArr();
        if(IsAcceptRebornSupplyArr.Length != REBORN_SUPPLY_ARRCNT)
            InitIsAcceptRebornSupplyArr();
    }

    /// <summary>
    /// 명예보급 획득트리거 배열초기화
    /// </summary>
    public void InitIsAcceptFameSupplyArr()
    {
        IsAcceptFameSupplyArr = new bool[FAME_SUPPLY_ARRCNT] {
            false, false, false, false, false,
            false, false, false, false, false,
            false, false, false, false, false,
        };
    }

    /// <summary>
    /// 환생보급 획득트리거 배열초기화
    /// </summary>
    public void InitIsAcceptRebornSupplyArr()
    {
        IsAcceptRebornSupplyArr = new bool[REBORN_SUPPLY_ARRCNT] {
            false, false, false, false, false,
            false, false, false, false, false,
            false, false, false, false, false,
            false, false, false, false, false,
            false, false, false, false, false,
            false, false, false, false, false,
        };
    }
}
