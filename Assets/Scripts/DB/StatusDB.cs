using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static Enum;

[Serializable]
public class StatusDB
{
    [Header("(광석) 재화 8종 및 크리스탈")]
    [field:SerializeField] int[] rscArr; public int[] RscArr {
        get => rscArr;
    }

    [Header("(연금술) 재료 8종")]
    [field:SerializeField] int[] matArr; public int[] MatArr {
        get => matArr;
    }
    [Header("(버섯도감) 버섯 8종")]
    [field:SerializeField] int[] msrArr; public int[] MsrArr {
        get => msrArr;
    }

    [Header("(소비) 광산 입장티켓")]
    [field:SerializeField] int oreTicket; public int OreTicket {
        get => oreTicket;
        set {
            oreTicket = value;
            if(oreTicket < 0) oreTicket = 0;

            //* (홈) 광산선택 팝업 텍스트UI 최신화
            GM._.ssm.stageTicketCntTxt.text = $"{oreTicket} / 10";
        }
    }

    [Header("(소비) 시련의광산 입장티켓")]
    [field:SerializeField] int redTicket; public int RedTicket {
        get {return redTicket;}
        set {
            redTicket = value;
            if(redTicket < 0) redTicket = 0;
        }
    }

    [Header("(소비) 광석상자")]
    [field:SerializeField] int oreChest; public int OreChest {
        get {return oreChest;}
        set {
            oreChest = value;
            if(oreChest < 0) oreChest = 0;
            GM._.ivm.UpdateAlertRedDot();
        }
    }

    [Header("(소비) 보물상자")]
    [field:SerializeField] int treasureChest; public int TreasureChest {
        get {return treasureChest;}
        set {
            treasureChest = value;
            if(treasureChest < 0) treasureChest = 0;
            GM._.ivm.UpdateAlertRedDot();
        }
    }

    [Header("(소비) 버섯상자1")]
    [field:SerializeField] int mushBox1; public int MushBox1 {
        get {return mushBox1;}
        set {
            mushBox1 = value;
            if(mushBox1 < 0) mushBox1 = 0;
            GM._.ivm.UpdateAlertRedDot();
        }
    }

    [Header("(소비) 버섯상자2")]
    [field:SerializeField] int mushBox2; public int MushBox2 {
        get {return mushBox2;}
        set {
            mushBox2 = value;
            if(mushBox2 < 0) mushBox2 = 0;
            GM._.ivm.UpdateAlertRedDot();
        }
    }

    [Header("(소비) 버섯상자3")]
    [field:SerializeField] int mushBox3; public int MushBox3 {
        get {return mushBox3;}
        set {
            mushBox3 = value;
            if(mushBox3 < 0) mushBox3 = 0;
            GM._.ivm.UpdateAlertRedDot();
        }
    }

    [Header("(소비) 스킬포션")]
    [field:SerializeField] int skillPotion; public int SkillPotion {
        get {return skillPotion;}
        set {
            skillPotion = value;
            if(skillPotion < 0) skillPotion = 0;
        }
    }

    [Header("(소비) 빛나는돌")]
    [field:SerializeField] int lightStone; public int LightStone {
        get {return lightStone;}
        set {
            lightStone = value;
            if(lightStone < 0) lightStone = 0;
        }
    }

    [Header("(소비) 시간의포션")]
    [field:SerializeField] int timePotion; public int TimePotion {
        get {return timePotion;}
        set {
            timePotion = value;
            if(timePotion < 0) timePotion = 0;
        }
    }
    [Header("(소비) 황금동전")]
    [field:SerializeField] int goldCoin; public int GoldCoin {
        get {return goldCoin;}
        set {
            goldCoin = value;
            if(goldCoin < 0) goldCoin = 0;
        }
    }

    [Header("명예 레벨")]
    [field:SerializeField] int fameLv; public int FameLv {
        get => fameLv;
        set => fameLv = value;
    }

    [Header("명예 포인트")]
    [field:SerializeField] int fame; public int Fame {
        get {return fame;}
        set {
            fame = value;
            if(fame < 0) fame = 0;
        }
    }

    [Header("황금 포인트")]
    [field:SerializeField] int goldPoint; public int GoldPoint {
        get {return goldPoint;}
        set {
            goldPoint = value;
            if(goldPoint < 0) goldPoint = 0;
        }
    }

#region FUNC
    public void Init()
    {
        // 재화 (광석 및 크리스탈 )
        rscArr = new int[9] {
            0, 0, 0, 0, // 광석 1,2,3,4
            0, 0, 0, 0, // 광석 5,6,7,8
            0, // 크리스탈
        };

        // (연금술) 재료 8종
        matArr = new int[8] {
            0, 0, 0, 0, 
            0, 0, 0, 0,
        };

        // (버섯도감) 버섯 8종
        msrArr = new int[8] {
            0, 0, 0, 0,
            0, 0, 0, 0,
        };

        // (소비) 아이템
        oreTicket = 5;     // 광산 입장티켓
        redTicket = 0;     // 시련의광산 입장티켓
        mushBox1 = 0;      // 의문의 버섯상자
        mushBox2 = 0;      // 신비한 버섯상자
        mushBox3 = 0;      // 전설의 버섯상자
        oreChest = 0;      // 광석상자
        treasureChest = 5; // 보물상자
        skillPotion = 0;   // 스킬포션
        goldCoin = 0;      // 황금코인
        lightStone = 0;    // 빛나는돌
        timePotion = 0;    // 시간의포션
        goldPoint = 0;     // 황금포인트

        fameLv = 1;        // 명예 레벨
        fame = 0;          // 명예경험치 포인트
    }

    /// <summary>
    /// 모든 인벤토리 아이템수량 가져오기
    /// </summary>
    public int GetInventoryItemVal(INV type)
    {
        switch(type)
        {
            // (광석) 재화
            case INV.ORE1: case INV.ORE2: case INV.ORE3: case INV.ORE4:
            case INV.ORE5: case INV.ORE6: case INV.ORE7: case INV.ORE8:
            case INV.CRISTAL:
                return rscArr[(int)type];
            // (연금술) 재료
            case INV.MAT1: case INV.MAT2: case INV.MAT3: case INV.MAT4:
            case INV.MAT5: case INV.MAT6: case INV.MAT7: case INV.MAT8:
                return matArr[(int)type - (int)INV.MAT1];
            // (버섯도감) 버섯
            case INV.MUSH1: case INV.MUSH2: case INV.MUSH3: case INV.MUSH4:
            case INV.MUSH5: case INV.MUSH6: case INV.MUSH7: case INV.MUSH8:
                return msrArr[(int)type - (int)INV.MUSH1];
            // (소비) 아이템
            case INV.ORE_TICKET:     return oreTicket;
            case INV.RED_TICKET:     return redTicket;
            case INV.MUSH_BOX1:      return mushBox1;
            case INV.MUSH_BOX2:      return mushBox2;
            case INV.MUSH_BOX3:      return mushBox3;
            case INV.ORE_CHEST:      return oreChest;
            case INV.TREASURE_CHEST: return treasureChest;
            case INV.SKILLPOTION:    return skillPotion;
            case INV.LIGHTSTONE:     return lightStone;
            case INV.TIMEPOTION:     return timePotion;
            case INV.GOLDCOIN:     return goldCoin;
            //※ 여기위에 추가
        }

        Debug.LogError("해당하는 아이템 타입이 없어서 찾을수가 없습니다. 소스코드에서 case 타입을 추가하세요!!!");
        return -9999999; // ERROR
    }

    /// <summary>
    /// 모든 인벤토리 아이템수량 설정
    /// </summary>
    /// <param name="itemType">아이템 타입(인벤토리 Enum)</param>
    /// <param name="val">증가 또는 감소시킬 수량</param>
    public void SetInventoryItemVal(INV itemType, int val)
    {
        const int matIdxOffset = (int)INV.MAT1;
        const int mushIdxOffset = (int)INV.MUSH1;

        switch(itemType)
        {
            case INV.ORE1: case INV.ORE2: case INV.ORE3: case INV.ORE4:
            case INV.ORE5: case INV.ORE6: case INV.ORE7: case INV.ORE8:
            case INV.CRISTAL:
                SetRscArr((int)itemType, val);
                break;
            case INV.MAT1: case INV.MAT2: case INV.MAT3: case INV.MAT4:
            case INV.MAT5: case INV.MAT6: case INV.MAT7: case INV.MAT8: 
                SetMatArr((int)itemType - matIdxOffset, val);
                break;
            case INV.MUSH1: case INV.MUSH2: case INV.MUSH3: case INV.MUSH4:
            case INV.MUSH5: case INV.MUSH6: case INV.MUSH7: case INV.MUSH8:
                SetMsrArr((int)itemType - mushIdxOffset, val);
                break;
            case INV.ORE_TICKET:
                OreTicket += val;
                break;
            case INV.RED_TICKET:
                RedTicket += val;
                break;
            case INV.ORE_CHEST: 
                OreChest += val;
                break;
            case INV.TREASURE_CHEST:
                TreasureChest += val;
                break;
            case INV.SKILLPOTION:
                skillPotion += val;
                break;
            case INV.LIGHTSTONE:
                lightStone += val;
                break;
            case INV.TIMEPOTION:
                timePotion += val;
                break;
            case INV.GOLDCOIN:
                goldCoin += val;

                // 골드코인을 사용시 황금포인트 추가
                if(val < 0)
                {
                    goldPoint += Mathf.Abs(val);
                }
                break;

            //※ 여기위에 추가
        }
    }

    /// <summary>
    /// 재화량 추가 및 UI업데이트
    /// </summary>
    /// <param name="idx">재화 타입(인덱스)</param>
    /// <param name="val">증가량</param>
    /// <param name="isAutoMine">자동채굴 체크트리거 : 자동채굴전용 : 이미 합산이 되었음으로 보상얻을시에는 추가증가 안함</param>
    public int SetRscArr(int idx, int val, bool isAutoMine = false) //? 배열 setter는 요소가 바뀌어도 호출이 안되므로, 메서드 자체 제작
    {
        OreBlessManager obm = GM._.obm;
        RebornManager rbm = GM._.rbm;
        float extraPer = 1;

        // 수량이 추가되는 경우
        if(val > 0)
        {
            // 추가획득량% 적용
            switch(idx)
            {
                case (int)RWD.ORE1:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE1_RWD_PER);
                    extraPer += rbm.upgIncOrePer.Val;
                    if(val < 0) extraPer = 1;
                    // 자동채굴체크의경우 이미 추가값이 계산되었음으로 처리안함
                    if(!isAutoMine)
                        val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE2:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE2_RWD_PER);
                    extraPer += rbm.upgIncOrePer.Val;
                    if(val < 0) extraPer = 1;
                    // 자동채굴체크의경우 이미 추가값이 계산되었음으로 처리안함
                    if(!isAutoMine)
                        val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE3:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE3_RWD_PER);
                    extraPer += rbm.upgIncOrePer.Val;
                    if(val < 0) extraPer = 1;
                    // 자동채굴체크의경우 이미 추가값이 계산되었음으로 처리안함
                    if(!isAutoMine)
                        val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE4:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE4_RWD_PER);
                    extraPer += rbm.upgIncOrePer.Val;
                    if(val < 0) extraPer = 1;
                    // 자동채굴체크의경우 이미 추가값이 계산되었음으로 처리안함
                    if(!isAutoMine)
                        val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE5:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE5_RWD_PER);
                    extraPer += rbm.upgIncOrePer.Val;
                    if(val < 0) extraPer = 1;
                    // 자동채굴체크의경우 이미 추가값이 계산되었음으로 처리안함
                    if(!isAutoMine)
                        val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE6:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE6_RWD_PER);
                    extraPer += rbm.upgIncOrePer.Val;
                    if(val < 0) extraPer = 1;
                    // 자동채굴체크의경우 이미 추가값이 계산되었음으로 처리안함
                    if(!isAutoMine)
                        val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE7:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE7_RWD_PER);
                    extraPer += rbm.upgIncOrePer.Val;
                    if(val < 0) extraPer = 1;
                    // 자동채굴체크의경우 이미 추가값이 계산되었음으로 처리안함
                    if(!isAutoMine)
                        val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.ORE8:
                    extraPer += obm.GetAbilityValue(OREBLESS_ABT.INC_ORE8_RWD_PER);
                    extraPer += rbm.upgIncOrePer.Val;
                    if(val < 0) extraPer = 1;
                    // 자동채굴체크의경우 이미 추가값이 계산되었음으로 처리안함
                    if(!isAutoMine)
                        val = Mathf.RoundToInt(val * extraPer);
                    break;
                case (int)RWD.CRISTAL: // (Int형)
                    int extraVal = GM._.sttm.ExtraIncCristal;
                    extraPer += rbm.upgIncOrePer.Val;
                    // 자동채굴체크의경우 이미 추가값이 계산되었음으로 처리안함
                    if(!isAutoMine) {
                        val = Mathf.RoundToInt((val + extraVal) * extraPer);
                    }
                    break;
            }
        }

        rscArr[idx] += val;

        if(rscArr[idx] < 0)
            rscArr[idx] = 0;

        // TOP RSC 재화표시 업데이트UI
        GM._.ui.topRscTxtArr[idx].text = $"{rscArr[idx]}";

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
    /// (버섯도감) 버섯 추가 및 UI업데이트
    /// </summary>
    /// <param name="idx">버섯 타입(인덱스)</param>
    /// <param name="val">증가량</param>
    public int SetMsrArr(int idx, int val) //? 배열 setter는 요소가 바뀌어도 호출이 안되므로, 메서드 자체 제작
    {
        msrArr[idx] += val;

        if(msrArr[idx] < 0)
            msrArr[idx] = 0;

        // TOP 버섯표시 업데이트UI
        GM._.ui.topMushTxtArr[idx].text = $"{msrArr[idx]}";

        // 게임플레이 경우 인벤토리 업데이트UI
        if(GM._.gameState == GameState.PLAY)
            GM._.ivm.UpdateSlotUI();

        return val;
    }

    /// <summary>
    /// (홈) 모든재화 텍스트UI 업데이트
    /// </summary>
    public void UpdateAllTopUIAtHome() {
        for(int i = 0; i < rscArr.Length; i++){
            GM._.ui.topRscTxtArr[i].text = $"{rscArr[i]}";
        }
        for(int i = 0; i < msrArr.Length; i++){
            GM._.ui.topMushTxtArr[i].text = $"{msrArr[i]}";
        }
    }
#endregion
}