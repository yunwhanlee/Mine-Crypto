using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Enum;

[Serializable]
public struct OreBlessSaveData {
    [Header("광산의축복 잠금해제")]
    [field:SerializeField] bool isUnlock; public bool IsUnlock {
        get => isUnlock;
        set => isUnlock = value;
    }

    [Header("능력치 수")]
    [field:SerializeField] int abilityCnt; public int AbilityCnt {
        get => abilityCnt;
        set => abilityCnt = value;
    }

    [Header("능력치 리스트")]
    [field:SerializeField] List<OreBlessAbilityData> abilityList; public List<OreBlessAbilityData> AbilityList {
        get => abilityList;
        set => abilityList = value;
    }

    public OreBlessSaveData(bool isUnlock, int abilityCnt, List<OreBlessAbilityData> abilityList)
    {
        this.isUnlock = isUnlock;
        this.abilityCnt = abilityCnt;
        this.abilityList = abilityList;
    }
}

[Serializable]
public class OreBlessDB
{
    public OreBlessSaveData[] saveDts;

    public void Init()
    {
        // 초기 능력치 값
        List<OreBlessAbilityData> defAbilityList = new ()
        {
            new OreBlessAbilityData() { 
                grade = GRADE.COMMON,
                type = OREBLESS_ABT.ATK_PER,
                val = 0.01f
            },
        };

        saveDts = new OreBlessSaveData[8] {
            new OreBlessSaveData(false, 0, defAbilityList),
            new OreBlessSaveData(false, 0, null),
            new OreBlessSaveData(false, 0, null),
            new OreBlessSaveData(false, 0, null),
            new OreBlessSaveData(false, 0, null),
            new OreBlessSaveData(false, 0, null),
            new OreBlessSaveData(false, 0, null),
            new OreBlessSaveData(false, 0, null),
        };
    }
}
