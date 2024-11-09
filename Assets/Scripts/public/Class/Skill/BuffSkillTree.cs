using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// 첫번째 스킬(버프형)
/// </summary>
[System.Serializable]
public class BuffSkillTree
{
    public SkillTree[] skillTreeArr;

    // 스킬레벨
    public int Lv {
        get => DM._.DB.skillTreeDB.buffSkillTreeLv;
        set => DM._.DB.skillTreeDB.buffSkillTreeLv = value;
    }
    // 지속시간
    public int Time {
        get {
            if(Lv == 5) return 30;
            else if(Lv >= 3) return 15;
            else return 10;
        }
    }
    // 추가 이동속도 %
    public float ExtraMoveSpeedPer {
        get {
            const float UNIT = 0.1f;
            return Random.Range((int)GRADE.COMMON, (int)GRADE.CNT) * UNIT;
        }
    }
    // 추가 공격속도 %
    public float ExtraAttackSpeedPer {
        get {
            const float UNIT = 0.1f;
            if(Lv >= 2) return Random.Range((int)GRADE.COMMON, (int)GRADE.CNT) * UNIT;
            else return 0;
        }
    }
    // 추가 공격력 %
    public float ExtraAttackPer {
        get {
            const float UNIT = 0.1f;
            if(Lv >= 4) return Random.Range((int)GRADE.COMMON, (int)GRADE.CNT) * UNIT;
            else return 0;
        }
    }

    /// <summary>
    /// 스킬 상세설명
    /// </summary>
    public string GetDescription(int idx)
    {
        switch(idx)
        {
            default: return "10초간 이동속도(10/20/30/40/50/60)(일반/고급/희귀/영웅/전설/신화)% 증가.";
            case 1: return "추가로 공격속도(10/20/30/40/50/60)% 증가.";
            case 2: return "지속시간이 15초로 증가.";
            case 3: return "추가로 공격력(10/20/30/40/50/60)% 증가.";
            case 4: return "지속시간이 30초로 증가.";
        }
    }
}