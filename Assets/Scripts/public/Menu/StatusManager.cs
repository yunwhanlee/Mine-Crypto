using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enum;

public class StatusManager : MonoBehaviour
{
    const int DEF_POPULATION = 3;
    const int DEF_TIMER = 30;

    //* Element
    public GameObject windowObj;
    public TMP_Text fameLvTxt;
    public TMP_Text[] myStatusTxtArr; // [0]: LeftArea, [1]: RightArea

    //* Value
    // 공격력 +
    public int ExtraAtk { 
        get => GM._.ugm.upgAttack.Val
            + GM._.mrm.ms1_UpgAttack.Val;
    }
    // 공격력 %
    public float ExtraAtkPer { 
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.ATK_PER)
            + GM._.pfm.totalAttackPer
            + GM._.acm.decoItemData[(int)DECO.PURPLE_ORE_PILE_1].AbilityVal;
    }
    // 공격속도 %
    public float ExtraAtkSpdPer {
        get => GM._.ugm.upgAttackSpeed.Val
            + GM._.obm.GetAbilityValue(OREBLESS_ABT.ATKSPD_PER)
            + GM._.acm.decoItemData[(int)DECO.PLATINUM_ORE_PILE_5].AbilityVal
            + GM._.mrm.ms7_UpgAtkSpeedPer.Val;
    }
    // 이동속도 %
    public float ExtraMovSpdPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.MOVSPD_PER)
            + GM._.acm.decoItemData[(int)DECO.TREE_BRANCH_2].AbilityVal
            + GM._.mrm.ms2_UpgMovSpeedPer.Val;
    }
    // 가방용량 %
    public float ExtraBagStgPer {
        get => GM._.ugm.upgBagStorage.Val
            + GM._.obm.GetAbilityValue(OREBLESS_ABT.BAG_STG_PER)
            + GM._.mrm.ms3_UpgBagStoragePer.Val;
    }
    // 채굴시간 +
    public int ExtraIncTimer {
        get => GM._.ugm.upgIncTimer.Val
            + (int)GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_TIMER)
            + GM._.mrm.ms5_UpgIncTimer.Val;
    }
    // 다음층 스킵 %
    public float ExtraNextSkipPer {
        get => GM._.ugm.upgNextStageSkip.Val
            + GM._.obm.GetAbilityValue(OREBLESS_ABT.NEXT_STG_SKIP_PER)
            + GM._.mrm.ms4_UpgNextStageSkipPer.Val;
    }
    // 크리스탈 획득량 +
    public int ExtraIncCristal {
        get => GM._.ugm.upgIncCristal.Val
            + (int)GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_CRISTAL);
    }

    // 광석 1 ~ 8 보상 획득량 % 
    public float ExtraOre1RwdPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_ORE1_RWD_PER);
    }
    public float ExtraOre2RwdPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_ORE2_RWD_PER);
    }
    public float ExtraOre3RwdPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_ORE3_RWD_PER);
    }
    public float ExtraOre4RwdPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_ORE4_RWD_PER);
    }
    public float ExtraOre5RwdPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_ORE5_RWD_PER);
    }
    public float ExtraOre6RwdPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_ORE6_RWD_PER);
    }
    public float ExtraOre7RwdPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_ORE7_RWD_PER);
    }
    public float ExtraOre8RwdPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_ORE8_RWD_PER);
    }

    // 최종 소환캐릭터 수
    public int TotalPopulation { 
        get => GM._.ugm.upgIncPopulation.Val
            + (int)GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_POPULATION) // (축복)
            + GM._.tsm.upgIncPopulation.Val // (초월)
            + (int)GM._.acm.decoItemData[(int)DECO.SNOWED_TREE_3].AbilityVal // (연금술) 장식
            + (int)GM._.acm.decoItemData[(int)DECO.ICE_SHEET_6].AbilityVal // (연금술) 장식
            + (int)GM._.acm.decoItemData[(int)DECO.CANYON_ROCK_8].AbilityVal // (연금술) 장식
            + GM._.mrm.ms8_IncPopulation.Val;
    }

    // 보물상자 추가 랜덤생성 %
    public float ExtraChestSpawnPer {
        get => GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_CHEST_SPAWN_PER)
            + GM._.mrm.ms6_UpgChestSpawnPer.Val;
    }

    // 명성 추가 획득량
    public int IncFame { 
        get => GM._.tsm.upgIncFame.Val
            + (int)GM._.acm.decoItemData[(int)DECO.GORILLA_4].AbilityVal
            + (int)GM._.acm.decoItemData[(int)DECO.DARK_CRISTAL_PILE_7].AbilityVal;
    }
    // 자동채굴 광석 %
    public float ExtraAutoOrePer {
        get => GM._.tsm.upgIncAutoOrePer.Val;
    }
    // 자동채굴 크리스탈 %
    public float ExtraAutoCristalPer {
        get => GM._.tsm.upgIncAutoCristalPer.Val;
    }
    // 제작비용 감소 %
    public float DecAlchemyMaterialPer {
        get => GM._.tsm.upgDecAlchemyMaterialPer.Val;
    }
    // 보물상자 획득 +
    public int ExtraTreasureChest {
        get => GM._.tsm.upgIncTreasureChest.Val;
    }
    // 자동채굴 광석 수용량 %
    public float ExtraAutoOreBagStoragePer {
        get => GM._.tsm.upgIncAutoOreBagStoragePer.Val;
    }
    // 자동채굴 크리스탈 수용량 %
    public float ExtraAutoCristalBagStoragePer {
        get => GM._.tsm.upgIncAutoCristalBagStoragePer.Val;
    }

#region FUNC
    /// <summary>
    /// 현재 내 상태창 업데이트
    /// </summary>
    public void UpdateMyStatus()
    {
        var ugm = GM._.ugm; // 강화
        var obm = GM._.obm; // 축복
        var tsm = GM._.tsm; // 초월

        // 명성레벨 표시
        fameLvTxt.text = $"LV.{GM._.fm.FameLv}";

        // 공백으로 초기화
        myStatusTxtArr[0].text = "";
        myStatusTxtArr[1].text = "";

        // 텍스트 변환
        string ATK = ExtraAtk > 0? $"공격력 : +{ExtraAtk}\n" : "";
        string ATK_PER = ExtraAtkPer > 0? $"공격력 : +{ExtraAtkPer * 100}%\n" : "";
        string ATKSPD_PER = ExtraAtkSpdPer > 0? $"공격속도 : +{ExtraAtkSpdPer * 100}%\n" : "";
        string MOVSPD_PER = ExtraMovSpdPer > 0? $"이동속도 : +{ExtraMovSpdPer * 100}%\n" : "";
        string BAGSTG_PER = ExtraBagStgPer > 0? $"가방용량 : +{ExtraBagStgPer * 100}%\n" : "";
        string INC_TIMER = ExtraIncTimer > DEF_TIMER? $"채굴시간 : +{ExtraIncTimer - DEF_TIMER}초\n" : "";
        string NEXT_SKIP_PER = ExtraNextSkipPer > 0? $"다음층 스킵 : +{ExtraNextSkipPer * 100}%\n" : "";
        string INC_CRSITAL = ExtraIncCristal > 0? $"크리스탈 획득량 : +{ExtraIncCristal}\n" : "";
        string INC_POPULATION = TotalPopulation > DEF_POPULATION? $"소환캐릭 증가 : +{TotalPopulation - DEF_POPULATION}\n" : "";
        string CHEST_SPAWN_PER = ExtraChestSpawnPer > 0? $"상자 등장확률 : +{ExtraChestSpawnPer * 100}%\n" : "";
        string ORE1_RWD_PER = ExtraOre1RwdPer > 0? $"광석1 획득량 : +{ExtraOre1RwdPer * 100}%\n" : "";
        string ORE2_RWD_PER = ExtraOre2RwdPer > 0? $"광석2 획득량 : +{ExtraOre2RwdPer * 100}%\n" : "";
        string ORE3_RWD_PER = ExtraOre3RwdPer > 0? $"광석3 획득량 : +{ExtraOre3RwdPer * 100}%\n" : "";
        string ORE4_RWD_PER = ExtraOre4RwdPer > 0? $"광석4 획득량 : +{ExtraOre4RwdPer * 100}%\n" : "";
        string ORE5_RWD_PER = ExtraOre5RwdPer > 0? $"광석5 획득량 : +{ExtraOre5RwdPer * 100}%\n" : "";
        string ORE6_RWD_PER = ExtraOre6RwdPer > 0? $"광석6 획득량 : +{ExtraOre6RwdPer * 100}%\n" : "";
        string ORE7_RWD_PER = ExtraOre7RwdPer > 0? $"광석7 획득량 : +{ExtraOre7RwdPer * 100}%\n" : "";
        string ORE8_RWD_PER = ExtraOre8RwdPer > 0? $"광석8 획득량 : +{ExtraOre8RwdPer * 100}%\n" : "";
        string INC_AUTO_ORE_PER = ExtraAutoOrePer > 0? $"자동 광석 수량 : +{ExtraAutoOrePer * 100}%\n" : "";
        string INC_AUTO_CRISTAL_PER = ExtraAutoCristalPer > 0? $"자동 크리스탈 수량 : +{ExtraAutoCristalPer * 100}%\n" : "";
        string DEC_AUTO_PRODUCE_PER = DecAlchemyMaterialPer > 0? $"재료 제작비용 감소 : +{DecAlchemyMaterialPer * 100}%\n" : "";
        string INC_TREASURE_CHEST = ExtraTreasureChest > 0? $"보물상자 획득량 : +{ExtraTreasureChest}\n" : "";
        string INC_AUTO_ORE_BAGSTG_PER = ExtraAutoOreBagStoragePer > 0? $"자동 광석 보관량% : +{ExtraAutoOreBagStoragePer * 100}%\n" : "";
        string INC_AUTO_CRISTAL_BAGSTG_PER = ExtraAutoCristalBagStoragePer > 0? $"자동 크리스탈 보관량% : +{ExtraAutoCristalBagStoragePer * 100}%\n" : "";
        string INC_FAME = IncFame > 0? $"명예 획득량 : +{IncFame}\n" : "";

        // 결과 텍스트
        string resStr = ATK
            + ATK_PER
            + ATKSPD_PER
            + MOVSPD_PER
            + BAGSTG_PER
            + INC_TIMER
            + NEXT_SKIP_PER
            + INC_CRSITAL
            + INC_POPULATION
            + CHEST_SPAWN_PER
            + ORE1_RWD_PER
            + ORE2_RWD_PER
            + ORE3_RWD_PER
            + ORE4_RWD_PER
            + ORE5_RWD_PER
            + ORE6_RWD_PER
            + ORE7_RWD_PER
            + ORE8_RWD_PER
            + INC_AUTO_ORE_PER
            + INC_AUTO_CRISTAL_PER
            + DEC_AUTO_PRODUCE_PER
            + INC_TREASURE_CHEST
            + INC_AUTO_ORE_BAGSTG_PER
            + INC_AUTO_CRISTAL_BAGSTG_PER
            + INC_FAME;

        // 능력을 좌우 영역으로 나눠서 테이블형식으로 표시
        string[] abilityStrArr = resStr.Split("\n");

        for(int i = 0; i < abilityStrArr.Length; i++)
        {
            if(i % 2 == 0)
                myStatusTxtArr[0].text += abilityStrArr[i] + "\n";
            else
                myStatusTxtArr[1].text += abilityStrArr[i] + "\n";
        }
    }
#endregion
}
