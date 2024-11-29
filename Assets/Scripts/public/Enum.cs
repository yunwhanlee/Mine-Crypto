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
        COMMON, UNCOMMON, RARE, UNIQUE, LEGEND, MYTH, CNT
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

    /// <summary>
    /// (환생) 업그레이드 가격감소% 적용할 업그레이드 가격대상
    /// </summary>
    public enum DEC_UPG_TYPE {
        NONE, UPGRADE, TRANSCEND,
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
    //! 인벤토리 추가의경우, 인벤토리랑 보상팝업 전부 Hierarchy와 데이터를 추가해줘야 됨!
    /// </summary>
    public enum RWD {
        NONE = -1,
        // 재화 종류
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL,
        // 재료 아이템
        MAT1, MAT2, MAT3, MAT4, MAT5, MAT6, MAT7, MAT8,
        // 버섯 도감
        MUSH1, MUSH2, MUSH3, MUSH4, MUSH5, MUSH6, MUSH7, MUSH8,
        // 소비 아이템
        ORE_TICKET, RED_TICKET,
        MUSH_BOX1, MUSH_BOX2, MUSH_BOX3,
        ORE_CHEST, TREASURE_CHEST,
        SKILLPOTION, LIGHTSTONE, TIMEPOTION,
        //※ 여기 위에 추가 => 이 키워드로 다른 스크립트에 추가할곳 다 찾기

        // 포인트 (인벤토리 표기X)
        FAME,
    }

    /// <summary>
    ///* 인벤토리 아이템 종류
    //! (에디터) 인벤토리팝업 아이템 순서와 서로같게 하기
    //! 인벤토리 추가의경우, 인벤토리랑 보상팝업 전부 Hierarchy와 데이터를 추가해줘야 됨!
    /// </summary>
    public enum INV {
        // 재화 종류
        ORE1, ORE2, ORE3, ORE4, ORE5, ORE6, ORE7, ORE8, CRISTAL,
        // 연금술 재료
        MAT1, MAT2, MAT3, MAT4, MAT5, MAT6, MAT7, MAT8,
        // 버섯 도감
        MUSH1, MUSH2, MUSH3, MUSH4, MUSH5, MUSH6, MUSH7, MUSH8,
        // 소비 아이템
        ORE_TICKET, RED_TICKET,
        MUSH_BOX1, MUSH_BOX2, MUSH_BOX3,
        ORE_CHEST, TREASURE_CHEST,
        SKILLPOTION, LIGHTSTONE, TIMEPOTION,
        //※ 여기 위에 추가 => 이 키워드로 다른 스크립트에 추가할곳 다 찾기
    }

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
        ORE_TICKET,
        RED_TICKET,
        MUSH_BOX1,
        MUSH_BOX2,
        MUSH_BOX3,
        ORE_CHEST,
        TREASURE_CHEST,
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
            case RWD.ORE1: return LM._.Localize(LM.Ore1);
            case RWD.ORE2: return LM._.Localize(LM.Ore2);
            case RWD.ORE3: return LM._.Localize(LM.Ore3);
            case RWD.ORE4: return LM._.Localize(LM.Ore4);
            case RWD.ORE5: return LM._.Localize(LM.Ore5);
            case RWD.ORE6: return LM._.Localize(LM.Ore6);
            case RWD.ORE7: return LM._.Localize(LM.Ore7);
            case RWD.ORE8: return LM._.Localize(LM.Ore8);
            case RWD.CRISTAL: return LM._.Localize(LM.Cristal);
            case RWD.ORE_TICKET: return LM._.Localize(LM.OreTicket);
            case RWD.RED_TICKET: return LM._.Localize(LM.RedTicket);
            case RWD.TREASURE_CHEST: return LM._.Localize(LM.TreasureChest);
            case RWD.SKILLPOTION: return LM._.Localize(LM.SkillPotion);
            case RWD.LIGHTSTONE: return LM._.Localize(LM.LightStone);
            case RWD.TIMEPOTION: return LM._.Localize(LM.TimePotion);
            case RWD.FAME: return LM._.Localize(LM.Fame);
            //※여기 위에 추가
        }
        return "???";
    }


}
