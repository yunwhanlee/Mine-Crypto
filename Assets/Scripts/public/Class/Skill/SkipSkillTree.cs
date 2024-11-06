using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 세번째 스킬(스킵형)
/// </summary>
[System.Serializable]
public class SkipSkillTree
{
    public SkillTree[] skillTreeArr;

    // 스킬레벨
    public int Lv {get => DM._.DB.skillTreeDB.skipSkillTreeLv;}
    // 다음층 이동
    public int MoveNextFloor {
        get {
            int[] skipGradeFloorArr = {3, 4, 5, 6, 7, 8};
            return skipGradeFloorArr[Random.Range(0, skipGradeFloorArr.Length)];
        }
    }
    // 광산 남은시간 감소%
    public float DecTimerPer {
        get {
            if(Lv == 5) return 1 - 0.2f;        // -20% 감소
            else if(Lv >= 3) return 1 - 0.35f;  // -35% 감소
            else return 0.5f;                   // -50% 감소 (기본)
        }
    }
    // 스킬쿨타임 감소%
    public float DecSkillCoolTimePer {
        get {
            if(Lv == 5) return 1 - 0.2f; // -20% 감소
            else if(Lv >= 2) return 1 - 0.1f; // -10% 감소
            else return 1.0f; // 감소 없음
        }
    }
    // 보너스상자 (광석상자 및 보물상자 획득)
    public bool IsBonusChest { get => Lv >= 4;}

    /// <summary>
    /// 스킬 상세설명
    /// </summary>
    public string GetDescription(int idx)
    {
        switch(idx)
        {
            default: return "광산의 남은시간이 50% 감소하고 (3/4/5/6/7/8)층 이동.";
            case 1: return "남은 스킬쿨타임이 10% 감소.";
            case 2: return "광산의 남은시간 감소수치가 35%로 줄어듦.";
            case 3: return "발동시 광석상자 30개와 보물상자 10개를 획득.";
            case 4: return "남은 스킬쿨타임이 20%로 감소, 광산의 남은시간 감소수치가 20%로 줄어듦.";
        }
    }
}
