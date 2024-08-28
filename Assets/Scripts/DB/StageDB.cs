using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StageDB
{
    [Header("스테이지 잠금해제")]
    [field:SerializeField] bool[] isUnlockArr; public bool[] IsUnlockArr {
        get => isUnlockArr;
    }

    [Header("광산 8종 및 시련의광산 최대도달층")]
    [field:SerializeField] int[] bestFloorArr; public int[] BestFloorArr {
        get => bestFloorArr;
    }

    public void Init()
    {
        isUnlockArr = new bool[8] {
            true,   // 제 1 광산
            false,  // 제 2 광산
            false,  // 제 3 광산
            false,  // 제 4 광산
            false,  // 제 5 광산
            false,  // 제 6 광산
            false,  // 제 7 광산
            false   // 제 8 광산
        };
        bestFloorArr = new int[9] {
            0,  // 제 1 광산
            0,  // 제 2 광산
            0,  // 제 3 광산
            0,  // 제 4 광산
            0,  // 제 5 광산
            0,  // 제 6 광산
            0,  // 제 7 광산
            0,  // 제 8 광산
            0   // 시련의 광산
        };
    }
}
