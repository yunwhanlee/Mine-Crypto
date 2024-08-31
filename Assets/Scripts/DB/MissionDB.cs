using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MissionDB
{
    /// <summary> 광석채굴 </summary>
    [field:SerializeField] public int MiningOreCnt {get; set;}
    /// <summary> 채굴시간 </summary>
    [field:SerializeField] public int MiningTime {get; set;}
    /// <summary> 강화하기 </summary>
    [field:SerializeField] public int UpgradeCnt {get; set;}
    /// <summary> 광산 클리어 </summary>
    [field:SerializeField] public int StageClearCnt {get; set;}
    /// <summary> 보물상자 획득 </summary>
    [field:SerializeField] public int MiningChestCnt {get; set;}
    /// <summary> 시련의광산 돌파 </summary>
    [field:SerializeField] public int ChallengeClearCnt {get; set;} //TODO

    public void Init()
    {
        MiningOreCnt = 0;
        MiningTime = 0;
        UpgradeCnt = 0;
        StageClearCnt = 0;
        MiningChestCnt = 0;
        ChallengeClearCnt = 0;
    }
}
