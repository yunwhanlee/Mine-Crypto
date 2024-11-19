using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public int skillGrade;

    // 스킬레벨
    public int Lv {
        get => DM._.DB.skillTreeDB.buffSkillTreeLv;
        set => DM._.DB.skillTreeDB.buffSkillTreeLv = value;
    }
    // 지속시간
    public WaitForSeconds Time {
        get {
            if(Lv == 5) return Util.TIME30;
            else if(Lv >= 3) return Util.TIME15;
            else return Util.TIME10;
        }
    }
    // 추가 이동속도 %
    public float ExtraMoveSpeedPer {
        get {
            if(!GM._.skc.isActiveBuff)
                return 0;

            const float UNIT = 0.1f;
            return Random.Range((int)GRADE.COMMON, (int)GRADE.CNT) * UNIT;
        }
    }
    // 추가 공격속도 %
    public float ExtraAttackSpeedPer {
        get {
            if(!GM._.skc.isActiveBuff)
                return 0;

            const float UNIT = 0.1f;
            if(Lv >= 2) return Random.Range((int)GRADE.COMMON, (int)GRADE.CNT) * UNIT;
            else return 0;
        }
    }
    // 추가 공격력 %
    public float ExtraAttackPer {
        get {
            if(!GM._.skc.isActiveBuff)
                return 0;

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
            default: return "10초간 이동속도(10/<color=green>20</color>/<color=blue>30</color>/<color=purple>40</color>/<color=orange>50</color>/<color=red>60</color>)% 증가.";
            case 1: return "추가로 공격속도(10/<color=green>20</color>/<color=blue>30</color>/<color=purple>40</color>/<color=orange>50</color>/<color=red>60</color>)% 증가.";
            case 2: return "지속시간이 15초로 증가.";
            case 3: return "추가로 공격력(10/<color=green>20</color>/<color=blue>30</color>/<color=purple>40</color>/<color=orange>50</color>/<color=red>60</color>)% 증가.";
            case 4: return "지속시간이 30초로 증가.";
        }
    }
}