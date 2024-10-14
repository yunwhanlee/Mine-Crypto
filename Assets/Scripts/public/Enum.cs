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
        INC_TIMER, // int
        INC_CRISTAL, // int
        INC_POPULATION, // int
        ATK_PER,
        ATKSPD_PER,
        MOVSPD_PER,
        BAG_STG_PER,
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
    }

    /// <summary>
    /// 장식품 추가 능력치 종류
    /// </summary>
    public enum DECO_ABT {
        ATK_PER,
        ATKSPD_PER,
        MOVSPD_PER,
        INC_POPULATION, // int
        INC_FAME, // int
    }

    /// <summary>
    ///* 표시 재화 아이템 종류 (광석오브젝트 IDX용으로도 쓰이는데 이때 CRISTAL은 보물상자 오브젝트임)
    /// </summary>
    public enum RSC {
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL,
    }

    /// <summary>
    ///* 보상 아이템 종류
    //! (에디터) 보상팝업 아이템 순서와 서로같게 하기
    /// </summary>
    public enum RWD {
        // 재화 종류
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL,
        // 재료 아이템
        MAT1, MAT2, MAT3, MAT4, MAT5, MAT6, MAT7, MAT8,
        // 버섯 도감
        MUSH1, MUSH2, MUSH3, MUSH4, MUSH5, MUSH6, MUSH7, MUSH8,
        // 소비 아이템
        ORE_TICKET, RED_TICKET, ORE_CHEST, TREASURE_CHEST, MUSH_BOX1, MUSH_BOX2, MUSH_BOX3,
        //※ 여기에 추가

        // 포인트 (인벤토리 표기X)
        FAME,

    }

    /// <summary>
    ///* 인벤토리 아이템 종류
    //! (에디터) 인벤토리팝업 아이템 순서와 서로같게 하기
    /// </summary>
    public enum INV {
        // 재화 종류
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL,
        // 연금술 재료
        MAT1, MAT2, MAT3, MAT4, MAT5, MAT6, MAT7, MAT8,
        // 버섯 도감
        MUSH1, MUSH2, MUSH3, MUSH4, MUSH5, MUSH6, MUSH7, MUSH8,
        // 소비 아이템
        ORE_TICKET, RED_TICKET, ORE_CHEST, TREASURE_CHEST, MUSH_BOX1, MUSH_BOX2, MUSH_BOX3,
        //※ 여기에 추가
    }

    /// <summary>
    //* 위의 인벤토리 아이템 이름 및 정보
    /// <summary>
    //! (에디터) 인벤토리팝업 아이템 순서와 서로같게 하기
    public static InvItem_Info[] INV_ITEM_INFO = new InvItem_Info[9 + 8 + 8 + 7] {
        // (광석)재화
        new InvItem_Info("제1 광석", "제1 광석의 조각이다. (재화)"),
        new InvItem_Info("제2 광석", "제2 광석의 조각이다. (재화)"),
        new InvItem_Info("제3 광석", "제3 광석의 조각이다. (재화)"),
        new InvItem_Info("제4 광석", "제4 광석의 조각이다. (재화)"),
        new InvItem_Info("제5 광석", "제5 광석의 조각이다. (재화)"),
        new InvItem_Info("제6 광석", "제6 광석의 조각이다. (재화)"),
        new InvItem_Info("제7 광석", "제7 광석의 조각이다. (재화)"),
        new InvItem_Info("제8 광석", "제8 광석의 조각이다. (재화)"),
        new InvItem_Info("크리스탈", "크리스탈 조각이다. (재화)"),
        // (연금술) 재료
        new InvItem_Info("흙덩어리", "연금술에서 제작을위한 재료이다."),
        new InvItem_Info("통나무", "연금술에서 제작을위한 재료이다."),
        new InvItem_Info("천조각", "연금술에서 제작을위한 재료이다."),
        new InvItem_Info("유리", "연금술에서 제작을위한 재료이다."),
        new InvItem_Info("슬라임", "연금술에서 제작을위한 재료이다."),
        new InvItem_Info("황금덩어리", "연금술에서 제작을위한 재료이다."),
        new InvItem_Info("석탄", "연금술에서 제작을위한 재료이다."),
        new InvItem_Info("천연소금", "연금술에서 제작을위한 재료이다."),
        // (버섯도감) 버섯
        new InvItem_Info("1.버섯", "버섯도감에서 능력업그레이드가 가능하다."),
        new InvItem_Info("2.늪버섯", "버섯도감에서 능력업그레이드가 가능하다."),
        new InvItem_Info("3.능이버섯", "버섯도감에서 능력업그레이드가 가능하다."),
        new InvItem_Info("4.푸른버섯", "버섯도감에서 능력업그레이드가 가능하다."),
        new InvItem_Info("5.표고버섯", "버섯도감에서 능력업그레이드가 가능하다."),
        new InvItem_Info("6.붉은버섯", "버섯도감에서 능력업그레이드가 가능하다."),
        new InvItem_Info("7.포자버섯", "버섯도감에서 능력업그레이드가 가능하다."),
        new InvItem_Info("8.공포버섯", "버섯도감에서 능력업그레이드가 가능하다."),
        // (소비) 아이템
        new InvItem_Info("광산티켓", "광산에 입장가능한 티켓이다."),
        new InvItem_Info("붉은티켓", "시련의광산에 입장가능한 티켓이다."),
        new InvItem_Info("광석상자", "어떤 광석이 나올지 모릅니다. 최고층이 높을수록 많은 광석을 획득합니다."),
        new InvItem_Info("보물상자", "랜덤으로 티켓 또는 크리스탈을 획득할 수 있습니다."),
        new InvItem_Info("의문의 버섯상자", "일반적인 버섯이 들어있을 것 같다."),
        new InvItem_Info("신비한 버섯상자", "신비로운 버섯이 들어있을 것 같다."),
        new InvItem_Info("전설의 버섯상자", "전설적인 버섯이 들어있을 것 같다."),
        //※ 여기에 추가
    };

    /// <summary>
    /// (버섯도감) 버섯
    /// </summary>
    public enum MUSH {
        MUSH1, MUSH2, MUSH3, MUSH4, MUSH5, MUSH6, MUSH7, MUSH8
    }

    /// <summary>
    /// 연금술 카테고리
    /// </summary>
    public enum ALCHEMY_CATE {
        MATERIAL, CONSUME_ITEM, EXCHANGE, DECORATION
    }

    /// <summary>
    /// (연금술) 재료
    /// </summary>
    public enum MATE {
        MAT1, MAT2, MAT3, MAT4, MAT5, MAT6, MAT7, MAT8,
    }

    /// <summary>
    /// (연금술) 소모품
    /// </summary>
    public enum CONSUME {
        ORE_TICKET, RED_TICKET, ORE_CHEST, TREASURE_CHEST,
        MUSH_BOX1, MUSH_BOX2, MUSH_BOX3,
    }

    /// <summary>
    /// (연금술) 교환
    /// </summary>
    public enum EXCHANGE {
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8,
    }

    /// <summary>
    /// (연금술) 장식품
    /// </summary>
    public enum DECO {
        PURPLE_ORE_PILE_1, TREE_BRANCH_2, SNOWED_TREE_3, GORILLA_4, 
        PLATINUM_ORE_PILE_5, ICE_SHEET_6, DARK_CRISTAL_PILE_7, CANYON_ROCK_8,
    }

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
            case RWD.FAME: return "명예";
        }
        return "???";
    }


}
