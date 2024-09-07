using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// 인벤토리 아이템 이름 및 내용 데이터
/// </summary>
public struct InvItem_Info {
    public string name;
    public string content;

    public InvItem_Info(string name, string content) 
    {
        this.name = name;
        this.content = content;
    }
}

public static class Enum
{
    public enum GRADE {
        COMMON, UNCOMMON, RARE, UNIQUE, LEGEND, MYTH
    }

    public static string GetGradeTagColor(GRADE grade)
    {
        //* 등급 텍스트 색상
        string colorTag = (grade == GRADE.COMMON)? "white"
            : (grade == GRADE.UNCOMMON)? "green"
            : (grade == GRADE.RARE)? "blue"
            : (grade == GRADE.UNIQUE)? "purple"
            : (grade == GRADE.LEGEND)? "yellow"
            : "red";
        
        return colorTag;
    }

    public enum MISSION {
        MINING_ORE_CNT,
        MINING_TIME,
        UPGRADE_CNT,
        STAGE_CLEAR_CNT,
        MINING_CHEST_CNT,
        CHALLENGE_CLEAR_CNT,
        NULL,
    }

    /// <summary>
    /// 숙련도 미션 항목
    /// </summary>
    public enum PROFICIENCY {
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8,
        ORE_CHEST, TREASURE_CHEST
    }

    /// <summary>
    /// 축복 능력치 (int OR Float)
    /// </summary>
    public enum OREBLESS_ABT {
        ATK_PER,
        ATKSPD_PER,
        MOVSPD_PER,
        BAG_STG_PER,
        INC_TIMER, // int
        INC_CRISTAL, // int
        NEXT_STG_SKIP_PER,
        INC_CHEST_SPAWN_PER,
        INC_ORE1_RWD_PER,
        INC_ORE2_RWD_PER,
        INC_ORE3_RWD_PER,
        INC_ORE4_RWD_PER,
        INC_ORE5_RWD_PER,
        INC_ORE6_RWD_PER,
        INC_ORE7_RWD_PER,
        INC_ORE8_RWD_PER,
        INC_POPULATION // int
    }

    /// <summary>
    ///* 재화 아이템 종류
    /// </summary>
    public enum RSC {
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL
    }

    /// <summary>
    ///* 보상 아이템 종류
    //! (에디터) 보상팝업 아이템 순서와 서로같게 하기
    /// </summary>
    public enum RWD {
        // 재화 종류
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL,
        // 소비 아이템
        ORE_TICKET, RED_TICKET, ORE_CHEST, TREASURE_CHEST,
        // 포인트
        FAME,
        //※ 여기에 추가
    }

    /// <summary>
    ///* 인벤토리 아이템 종류
    //! (에디터) 인벤토리팝업 아이템 순서와 서로같게 하기
    /// </summary>
    public enum INV {
        // 재화 종류
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL,
        // 아이템 종류
        ORE_TICKET, RED_TICKET, ORE_CHEST, TREASURE_CHEST,
        //※ 여기에 추가
    }

    /// <summary>
    //* 위의 인벤토리 아이템 이름 및 정보
    /// <summary>
    //! (에디터) 인벤토리팝업 아이템 순서와 서로같게 하기
    public static InvItem_Info[] INV_ITEM_INFO = new InvItem_Info[13] {
        new InvItem_Info("제1 광석", "제1 광석의 조각이다. (재화)"),
        new InvItem_Info("제2 광석", "제2 광석의 조각이다. (재화)"),
        new InvItem_Info("제3 광석", "제3 광석의 조각이다. (재화)"),
        new InvItem_Info("제4 광석", "제4 광석의 조각이다. (재화)"),
        new InvItem_Info("제5 광석", "제5 광석의 조각이다. (재화)"),
        new InvItem_Info("제6 광석", "제6 광석의 조각이다. (재화)"),
        new InvItem_Info("제7 광석", "제7 광석의 조각이다. (재화)"),
        new InvItem_Info("제8 광석", "제8 광석의 조각이다. (재화)"),
        new InvItem_Info("크리스탈", "크리스탈 조각이다. (재화)"),
        new InvItem_Info("광산티켓", "광산에 입장가능한 티켓이다."),
        new InvItem_Info("붉은티켓", "시련의광산에 입장가능한 티켓이다."),
        new InvItem_Info("광석상자", "어떤 광석이 나올지 알 수 없다."),
        new InvItem_Info("보물상자", "랜덤으로 티켓 또는 크리스탈을 획득할 수 있다."),
        // 여기에 추가
    };

    /// <summary>모든 재화 개수</summary>
    public static int GetEnumRSCLenght() => System.Enum.GetValues(typeof(RSC)).Length;

    /// <summary>모든 보상아이템 개수</summary>
    public static int GetEnumRWDLenght() => GetEnumRWDArr().Length;

    /// <summary>모든 인벤토리 아이템 개수</summary>
    public static int GetEnumINVLenght() => GetEnumINVArr().Length;

    /// <summary>모든 보상아이템 Enum 배열</summary>
    public static System.Array GetEnumRWDArr() => System.Enum.GetValues(typeof(RWD));

    /// <summary>모든 인벤토리 아이템 Enum 배열</summary>
    public static System.Array GetEnumINVArr() => System.Enum.GetValues(typeof(INV));

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


}
