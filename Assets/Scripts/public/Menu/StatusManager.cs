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
    public int TotalPopulation { // 최종 소환캐릭터 수
        get => GM._.ugm.upgIncPopulation.Val
            + (int)GM._.obm.GetAbilityValue(OREBLESS_ABT.INC_POPULATION) // (축복)
            + GM._.tsm.upgIncPopulation.Val // (초월)
            + (int)GM._.acm.decoItemData[(int)DECO.SNOWED_TREE_3].AbilityVal // (연금술) 장식
            + (int)GM._.acm.decoItemData[(int)DECO.ICE_SHEET_6].AbilityVal // (연금술) 장식
            + (int)GM._.acm.decoItemData[(int)DECO.CANYON_ROCK_8].AbilityVal; // (연금술) 장식
    }
    public int IncFame { // 명성 추가 획득량
        get => GM._.tsm.upgIncFame.Val
            + (int)GM._.acm.decoItemData[(int)DECO.GORILLA_4].AbilityVal
            + (int)GM._.acm.decoItemData[(int)DECO.DARK_CRISTAL_PILE_7].AbilityVal;
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
        fameLvTxt.text = $"LV.{GM._.fm.fameLv}";

        // 공백으로 초기화
        myStatusTxtArr[0].text = "";
        myStatusTxtArr[1].text = "";

        //* 추가능력치 계산
        // 공격력 +
        int Atk = ugm.upgAttack.Val;
        // 공격력 %
        float AtkPer = obm.GetAbilityValue(OREBLESS_ABT.ATK_PER)
            + GM._.pfm.totalAttackPer
            + GM._.acm.decoItemData[(int)DECO.PURPLE_ORE_PILE_1].AbilityVal;
        // 공격속도 %
        float AtkSpdPer = ugm.upgAttackSpeed.Val
            + obm.GetAbilityValue(OREBLESS_ABT.ATKSPD_PER)
            + GM._.acm.decoItemData[(int)DECO.PLATINUM_ORE_PILE_5].AbilityVal;
        // 이동속도 %
        float MovSpdPer = ugm.upgMoveSpeed.Val
            + obm.GetAbilityValue(OREBLESS_ABT.MOVSPD_PER)
            + GM._.acm.decoItemData[(int)DECO.TREE_BRANCH_2].AbilityVal;
        // 가방용량 %
        float BagStgPer = ugm.upgBagStorage.Val
            + obm.GetAbilityValue(OREBLESS_ABT.BAG_STG_PER);
        // 제한시간 +
        int IncTimer = ugm.upgIncTimer.Val
            + (int)obm.GetAbilityValue(OREBLESS_ABT.INC_TIMER);
        // 다음층 스킵 %
        float NextSkipPer = ugm.upgNextStageSkip.Val
            + obm.GetAbilityValue(OREBLESS_ABT.NEXT_STG_SKIP_PER);
        // 크리스탈 획득량 +
        int IncCristal = ugm.upgIncCristal.Val
            + (int)obm.GetAbilityValue(OREBLESS_ABT.INC_CRISTAL);
        // 소환캐릭터 + //! 그냥 변수로 선언
        // Population = ugm.upgIncPopulation.Val
        //     + (int)obm.GetAbilityValue(OREBLESS_ABT.INC_POPULATION)
        //     + tsm.upgIncPopulation.Val
        //     + (int)GM._.acm.decoItemData[(int)DECO.SNOWED_TREE_3].abilityVal
        //     + (int)GM._.acm.decoItemData[(int)DECO.ICE_SHEET_6].abilityVal
        //     + (int)GM._.acm.decoItemData[(int)DECO.CANYON_ROCK_8].abilityVal;
        // 보물상자 소환확률 %
        float ChestSpawnPer = obm.GetAbilityValue(OREBLESS_ABT.INC_CHEST_SPAWN_PER);
        // 광석 보상 획득량 %
        float Ore1RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE1_RWD_PER);
        float Ore2RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE2_RWD_PER);
        float Ore3RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE3_RWD_PER);
        float Ore4RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE4_RWD_PER);
        float Ore5RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE5_RWD_PER);
        float Ore6RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE6_RWD_PER);
        float Ore7RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE7_RWD_PER);
        float Ore8RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE8_RWD_PER);
        // 자동채굴 광석 %
        float IncAutoOrePer = tsm.upgIncAutoOrePer.Val;
        // 자동채굴 크리스탈 %
        float IncAutoCristalPer = tsm.upgIncAutoCristalPer.Val;
        // 제작비용 감소 %
        float DecAutoProducePer = tsm.upgDecAutoProducePer.Val;
        // 보물상자 획득 +
        int IncTreasureChest = tsm.upgIncTreasureChest.Val;
        // 자동채굴 광석 수용량 %
        float IncAutoOreBagStoragePer = tsm.upgIncAutoOreBagStoragePer.Val;
        // 자동채굴 크리스탈 수용량 %
        float IncAutoCristalBagStoragePer = tsm.upgIncAutoCristalBagStoragePer.Val;
        // 명성 획득 + //! 그냥 변수로 선언
        // int IncFame = tsm.upgIncFame.Val
        //     + (int)GM._.acm.decoItemData[(int)DECO.GORILLA_4].abilityVal
        //     + (int)GM._.acm.decoItemData[(int)DECO.DARK_CRISTAL_PILE_7].abilityVal;

        // 텍스트 변환
        string ATK = Atk > 0? $"공격력 : +{Atk}\n" : "";
        string ATK_PER = AtkPer > 0? $"공격력 : +{AtkPer * 100}%\n" : "";
        string ATKSPD_PER = AtkSpdPer > 0? $"공격속도 : +{AtkSpdPer * 100}%\n" : "";
        string MOVSPD_PER = MovSpdPer > 0? $"이동속도 : +{MovSpdPer * 100}%\n" : "";
        string BAGSTG_PER = BagStgPer > 0? $"가방용량 : +{BagStgPer * 100}%\n" : "";
        string INC_TIMER = IncTimer > DEF_TIMER? $"채굴시간 : +{IncTimer - DEF_TIMER}초\n" : "";
        string NEXT_SKIP_PER = NextSkipPer > 0? $"다음층 스킵 : +{NextSkipPer * 100}%\n" : "";
        string INC_CRSITAL = IncCristal > 0? $"크리스탈 획득량 : +{IncCristal}\n" : "";
        string INC_POPULATION = TotalPopulation > DEF_POPULATION? $"소환캐릭 증가 : +{TotalPopulation - DEF_POPULATION}\n" : "";
        string CHEST_SPAWN_PER = ChestSpawnPer > 0? $"상자 등장확률 : +{ChestSpawnPer * 100}%\n" : "";
        string ORE1_RWD_PER = Ore1RwdPer > 0? $"광석1 획득량 : +{Ore1RwdPer * 100}%\n" : "";
        string ORE2_RWD_PER = Ore2RwdPer > 0? $"광석2 획득량 : +{Ore2RwdPer * 100}%\n" : "";
        string ORE3_RWD_PER = Ore3RwdPer > 0? $"광석3 획득량 : +{Ore3RwdPer * 100}%\n" : "";
        string ORE4_RWD_PER = Ore4RwdPer > 0? $"광석4 획득량 : +{Ore4RwdPer * 100}%\n" : "";
        string ORE5_RWD_PER = Ore5RwdPer > 0? $"광석5 획득량 : +{Ore5RwdPer * 100}%\n" : "";
        string ORE6_RWD_PER = Ore6RwdPer > 0? $"광석6 획득량 : +{Ore6RwdPer * 100}%\n" : "";
        string ORE7_RWD_PER = Ore7RwdPer > 0? $"광석7 획득량 : +{Ore7RwdPer * 100}%\n" : "";
        string ORE8_RWD_PER = Ore8RwdPer > 0? $"광석8 획득량 : +{Ore8RwdPer * 100}%\n" : "";
        string INC_AUTO_ORE_PER = IncAutoOrePer > 0? $"자동 광석 수량 : +{IncAutoOrePer * 100}%\n" : "";
        string INC_AUTO_CRISTAL_PER = IncAutoCristalPer > 0? $"자동 크리스탈 수량 : +{IncAutoCristalPer * 100}%\n" : "";
        string DEC_AUTO_PRODUCE_PER = DecAutoProducePer > 0? $"재료 제작비용 감소 : +{DecAutoProducePer * 100}%\n" : "";
        string INC_TREASURE_CHEST = IncTreasureChest > 0? $"보물상자 획득량 : +{IncTreasureChest}\n" : "";
        string INC_AUTO_ORE_BAGSTG_PER = IncAutoOreBagStoragePer > 0? $"자동 광석 보관량% : +{IncAutoOreBagStoragePer * 100}%\n" : "";
        string INC_AUTO_CRISTAL_BAGSTG_PER = IncAutoCristalBagStoragePer > 0? $"자동 크리스탈 보관량% : +{IncAutoCristalBagStoragePer * 100}%\n" : "";
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
