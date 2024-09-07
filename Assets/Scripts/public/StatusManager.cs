using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enum;

public class StatusManager : MonoBehaviour
{
    const int DEF_POPULATION = 3;
    const int DEF_TIMER = 30;

    // Element
    public GameObject windowObj;
    public TMP_Text fameLvTxt;
    public TMP_Text[] myStatusTxtArr; // [0]: LeftArea, [1]: RightArea

    // Value

#region FUNC
    /// <summary>
    /// 현재 내 상태창 업데이트
    /// </summary>
    public void UpdateMyStatus()
    {
        var ugm = GM._.ugm;
        var obm = GM._.obm;

        // 명성레벨 표시
        fameLvTxt.text = $"LV.{GM._.fm.fameLv}";

        // 공백으로 초기화
        myStatusTxtArr[0].text = "";
        myStatusTxtArr[1].text = "";

        // 추가능력치 계산
        int Atk = ugm.upgAttack.Val;
        float AtkPer = obm.GetAbilityValue(OREBLESS_ABT.ATK_PER) + GM._.pfm.totalAttackPer;
        float AtkSpdPer = ugm.upgAttackSpeed.Val + obm.GetAbilityValue(OREBLESS_ABT.ATKSPD_PER);
        float MovSpdPer = ugm.upgMoveSpeed.Val + obm.GetAbilityValue(OREBLESS_ABT.MOVSPD_PER);
        float BagStgPer = ugm.upgBagStorage.Val + obm.GetAbilityValue(OREBLESS_ABT.BAG_STG_PER);
        int IncTimer = ugm.upgIncTimer.Val + (int)obm.GetAbilityValue(OREBLESS_ABT.INC_TIMER);
        float NextSkipPer = ugm.upgNextStageSkip.Val + obm.GetAbilityValue(OREBLESS_ABT.NEXT_STG_SKIP_PER);
        int IncCristal = ugm.upgIncCristal.Val + (int)obm.GetAbilityValue(OREBLESS_ABT.INC_CRISTAL);
        int IncPopulation = ugm.upgIncPopulation.Val + (int)obm.GetAbilityValue(OREBLESS_ABT.INC_POPULATION);
        float ChestSpawnPer = obm.GetAbilityValue(OREBLESS_ABT.INC_CHEST_SPAWN_PER);
        float Ore1RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE1_RWD_PER);
        float Ore2RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE2_RWD_PER);
        float Ore3RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE3_RWD_PER);
        float Ore4RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE4_RWD_PER);
        float Ore5RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE5_RWD_PER);
        float Ore6RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE6_RWD_PER);
        float Ore7RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE7_RWD_PER);
        float Ore8RwdPer = obm.GetAbilityValue(OREBLESS_ABT.INC_ORE8_RWD_PER);

        // 텍스트 변환
        string ATK = Atk > 0? $"공격력 : +{Atk}\n" : "";
        string ATK_PER = AtkPer > 0? $"공격력 : +{AtkPer * 100}%\n" : "";
        string ATKSPD_PER = AtkSpdPer > 0? $"공격속도 : +{AtkSpdPer * 100}%\n" : "";
        string MOVSPD_PER = MovSpdPer > 0? $"이동속도 : +{MovSpdPer * 100}%\n" : "";
        string BAGSTG_PER = BagStgPer > 0? $"가방용량 : +{BagStgPer * 100}%\n" : "";
        string INC_TIMER = IncTimer > DEF_TIMER? $"채굴시간 : +{IncTimer - DEF_TIMER}초\n" : "";
        string NEXT_SKIP_PER = NextSkipPer > 0? $"다음층 스킵 : +{NextSkipPer * 100}%\n" : "";
        string INC_CRSITAL = IncCristal > 0? $"크리스탈 획득량 : +{IncCristal}\n" : "";
        string INC_POPULATION = IncPopulation > DEF_POPULATION? $"고용 수 : +{IncPopulation - DEF_POPULATION}\n" : "";
        string CHEST_SPAWN_PER = ChestSpawnPer > 0? $"상자 등장확률 : +{ChestSpawnPer * 100}%\n" : "";
        string ORE1_RWD_PER = Ore1RwdPer > 0? $"광석1 획득량 : +{Ore1RwdPer * 100}%\n" : "";
        string ORE2_RWD_PER = Ore2RwdPer > 0? $"광석2 획득량 : +{Ore2RwdPer * 100}%\n" : "";
        string ORE3_RWD_PER = Ore3RwdPer > 0? $"광석3 획득량 : +{Ore3RwdPer * 100}%\n" : "";
        string ORE4_RWD_PER = Ore4RwdPer > 0? $"광석4 획득량 : +{Ore4RwdPer * 100}%\n" : "";
        string ORE5_RWD_PER = Ore5RwdPer > 0? $"광석5 획득량 : +{Ore5RwdPer * 100}%\n" : "";
        string ORE6_RWD_PER = Ore6RwdPer > 0? $"광석6 획득량 : +{Ore6RwdPer * 100}%\n" : "";
        string ORE7_RWD_PER = Ore7RwdPer > 0? $"광석7 획득량 : +{Ore7RwdPer * 100}%\n" : "";
        string ORE8_RWD_PER = Ore8RwdPer > 0? $"광석8 획득량 : +{Ore8RwdPer * 100}%\n" : "";

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
            + ORE8_RWD_PER;

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
