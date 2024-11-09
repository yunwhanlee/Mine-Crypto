using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// 두번째 스킬(공격형)
/// </summary>
[System.Serializable]
public class AttackSkillTree
{
    public SkillTree[] skillTreeArr;

    // 스킬레벨
    public int Lv {
        get => DM._.DB.skillTreeDB.attackSkillTreeLv;
        set => DM._.DB.skillTreeDB.attackSkillTreeLv = value;
    }
    // 올클리어 지진 발동횟수 (다음층마다 1번씩 발동)
    public int AllClearBonusCnt = 0;
    // 올클리어 지진 최대횟수
    public int AllClearBonusMax {
        get {
            if(Lv == 5) return 10;
            else if(Lv >= 3) return 3;
            else return 1;
        }
    }
    // 메테오 지속시간
    public int MeteoTime {
        get {
            if(Lv >= 4) return 10;
            else if(Lv >= 2) return 5;
            else return 0;
        }
    }
    // 지진 공격력
    public int EarthQuakeDmg {
        get {
            int[] dmgGradeArr = { 200, 400, 800, 1500, 3000 };
            int dmg = dmgGradeArr[Random.Range(0, dmgGradeArr.Length)];
            int extraVal = GM._.sttm.ExtraAtk;
            float extraPer = 1 + GM._.sttm.ExtraAtkPer;

            int result = Mathf.RoundToInt((dmg + extraVal) * extraPer);
            return result;
        }
    }
    // 메테오 공격력
    public int MeteoDmg {
        get {
            int[] dmgGradeArr = { 50, 100, 200, 400, 1000 };
            int dmg = Random.Range(0, dmgGradeArr.Length);
            int extraVal = GM._.sttm.ExtraAtk;
            float extraPer = 1 + GM._.sttm.ExtraAtkPer;

            int result = Mathf.RoundToInt((dmg + extraVal) * extraPer);
            return result;
        }
    }

    /// <summary>
    /// 스킬 상세설명
    /// </summary>
    public string GetDescription(int idx)
    {
        switch(idx)
        {
            default: return "전범위 지진을일으켜 (200/400/800/1500/3000) + 공격력증가 x 공격력증가%의 피해를 준다.";
            case 1: return "5초간 메테오를 내려 초당 (50/100/200/400/1000) + 공격력증가 x 공격력증가%의 피해를 준다.";
            case 2: return "지진으로 모든광석을 파괴하면 다음층 이동 후 1회 더 사용. (최대3회)";
            case 3: return "메테오 지속시간이 10초로 증가.";
            case 4: return "지진이 최대 10회까지 연속 발동.";
        }
    }
}
