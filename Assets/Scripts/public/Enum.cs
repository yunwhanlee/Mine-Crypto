using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class Enum
{
    public enum GRADE {
        COMMON, UNCOMMON, RARE, UNIQUE, LEGEND, MYTH
    }

    public enum RSC {
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL
    }

    public enum RWD {
        // 재화 종류
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL,
        // 소비 아이템
        ORE_TICKET, RED_TICKET, TREASURE_CHEST, ORE_CHEST,
        // 포인트
        FAME,
    }

    /// <summary>
    /// 모든 재화타입 개수
    /// </summary>
    // public static int GetEnumRSCLenght() => System.Enum.GetValues(typeof(RSC)).Length;

    /// <summary>
    /// 모든 보상타입 개수
    /// </summary>
    public static int GetEnumRWDLenght() => GetEnumRWDArr().Length;

    /// <summary>
    /// 모든 보상타입 Enum 배열
    /// </summary>
    public static System.Array GetEnumRWDArr() => System.Enum.GetValues(typeof(RWD));

    /// <summary>
    /// 보상 이름 가져오기
    /// </summary>
    /// <param name="rwdType">RWD 인덱스</param>
    public static string GetRewardItemName(RWD rwdType) {
        switch(rwdType) {
            case RWD.ORE1: return "광석1";
            case RWD.ORE2: return "광석2";
            case RWD.ORE3: return "광석3";
            case RWD.ORE4: return "광석4";
            case RWD.ORE5: return "광석5";
            case RWD.ORE6: return "광석6";
            case RWD.ORE7: return "광석7";
            case RWD.ORE8: return "광석8";
            case RWD.CRISTAL: return "크리스탈";
            case RWD.ORE_TICKET: return "광석티켓";
            case RWD.RED_TICKET: return "붉은티켓";
            case RWD.TREASURE_CHEST: return "보물상자";
            case RWD.FAME: return "명성";
        }
        return "???";
    }

    public enum MISSION {
        MINING_ORE_CNT,
        MINING_TIME,
        UPGRADE_CNT,
        STAGE_CLEAR_CNT,
        MINING_CHEST_CNT,
        CHALLENGE_CLEAR_CNT
    }
}
