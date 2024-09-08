using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static Enum;

[Serializable]
public class StatusDB
{
    [Header("광석재화 8종 및 크리스탈")]
    [field:SerializeField] int[] rscArr; public int[] RscArr {
        get => rscArr;
    }

    [Header("연금술 재료 8종")]
    [field:SerializeField] int[] matArr; public int[] MatArr {
        get => matArr;
    }

    [Header("광산 입장티켓")]
    [field:SerializeField] int oreTicket; public int OreTicket {
        get => oreTicket;
        set {
            oreTicket = value;
            if(oreTicket < 0) oreTicket = 0;

            //* (홈) 광산선택 팝업 텍스트UI 최신화
            GM._.ssm.stageTicketCntTxt.text = $"{oreTicket} / 10";
        }
    }

    [Header("시련의광산 입장티켓")]
    [field:SerializeField] int redTicket; public int RedTicket {
        get {return redTicket;}
        set {
            redTicket = value;
            if(redTicket < 0) redTicket = 0;
        }
    }

    [Header("보물상자")]
    [field:SerializeField] int treasureChest; public int TreasureChest {
        get {return treasureChest;}
        set {
            treasureChest = value;
            if(treasureChest < 0) treasureChest = 0;
        }
    }

    [Header("광석상자")]
    [field:SerializeField] int oreChest; public int OreChest {
        get {return oreChest;}
        set {
            oreChest = value;
            if(oreChest < 0) oreChest = 0;
        }
    }

    [Header("명예 포인트")]
    [field:SerializeField] int fame; public int Fame {
        get {return fame;}
        set {
            fame = value;
            if(fame < 0) fame = 0;
        }
    }

#region FUNC
    public void Init()
    {
        // 재화 (광석 및 크리스탈 )
        rscArr = new int[9] {
            100000, 9000, 100000, 100000, // 광석 1,2,3,4
            100000, 100000, 100000, 100000, // 광석 5,6,7,8
            10000, // 크리스탈
        };

        // 연금술 재료
        matArr = new int[8] {
            10, 10, 10, 10, 
            10, 10, 10, 10,
        };

        // 아이템 수
        oreTicket = 5;     // 광산 입장티켓
        redTicket = 3;     // 시련의광산 입장티켓
        treasureChest = 5; // 보물상자
        oreChest = 5;      // 광석상자
        fame = 1;          // 명성포인트

    }

    public int GetInvItemVal(INV type)
    {
        switch(type)
        {
            case INV.ORE1:
            case INV.ORE2:
            case INV.ORE3:
            case INV.ORE4:
            case INV.ORE5:
            case INV.ORE6:
            case INV.ORE7:
            case INV.ORE8:
            case INV.CRISTAL:
                return rscArr[(int)type];
            case INV.MAT1:
            case INV.MAT2:
            case INV.MAT3:
            case INV.MAT4:
            case INV.MAT5:
            case INV.MAT6:
            case INV.MAT7:
            case INV.MAT8:
                return matArr[(int)type - (int)INV.MAT1];
        }

        return -9999999; // ERROR
    }

    /// <summary>
    /// 재화량 추가 및 UI업데이트
    /// </summary>
    /// <param name="idx">재화 타입(인덱스)</param>
    /// <param name="val">증가량</param>
    public int SetRscArr(int idx, int val) //? 배열 setter는 요소가 바뀌어도 호출이 안되므로, 메서드 자체 제작
    {
        OreBlessManager obm = GM._.obm;
        float extraPer = 1;

        // 수량이 추가되는 경우
        if(val > 0)
        {
            // 추가획득량% 적용
            switch(idx)
            {
                case (int)RWD.ORE1:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE1_RWD_PER);
                    if(val < 0) extraPer = 1;
                    val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE2:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE2_RWD_PER);
                    if(val < 0) extraPer = 1;
                    val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE3:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE3_RWD_PER);
                    if(val < 0) extraPer = 1;
                    val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE4:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE4_RWD_PER);
                    if(val < 0) extraPer = 1;
                    val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE5:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE5_RWD_PER);
                    if(val < 0) extraPer = 1;
                    val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE6:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE6_RWD_PER);
                    if(val < 0) extraPer = 1;
                    val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE7:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE7_RWD_PER);
                    if(val < 0) extraPer = 1;
                    val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE8:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE8_RWD_PER);
                    if(val < 0) extraPer = 1;
                    val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.CRISTAL: // (Int형)
                    int extraVal = GM._.ugm.upgIncCristal.Val + (int)obm.GetAbilityValue(OREBLESS_ABT.INC_CRISTAL);
                    val += extraVal;
                    break;
            }
        }

        rscArr[idx] += val;

        if(rscArr[idx] < 0)
            rscArr[idx] = 0;

        // TOP RSC 재화표시 업데이트UI
        GM._.hm.topRscTxtArr[idx].text = $"{rscArr[idx]}";

        // 게임플레이 경우 인벤토리 업데이트UI
        if(GM._.gameState == GameState.PLAY)
            GM._.ivm.UpdateSlotUI();

        return val;
    }

    /// <summary>
    /// 연금술 재료 수량 추가 및 UI업데이트
    /// </summary>
    /// <param name="idx">재료 타입(인덱스)</param>
    /// <param name="val">증가량</param>
    public int SetMatArr(int idx, int val) //? 배열 setter는 요소가 바뀌어도 호출이 안되므로, 메서드 자체 제작
    {
        matArr[idx] += val;

        if(matArr[idx] < 0)
            matArr[idx] = 0;

        // 게임플레이 경우 인벤토리 업데이트UI
        if(GM._.gameState == GameState.PLAY)
            GM._.ivm.UpdateSlotUI();

        return val;
    }

    /// <summary>
    /// (홈) 모든재화 텍스트UI 업데이트
    /// </summary>
    public void UpdateAllRscUIAtHome() {
        for(int i = 0; i < rscArr.Length; i++){
            GM._.hm.topRscTxtArr[i].text = $"{rscArr[i]}";
        }
    }
#endregion
}
