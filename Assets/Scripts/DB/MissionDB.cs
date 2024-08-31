using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MissionDB
{
    [field:SerializeField] public int MiningOreCnt {get; set;}
    [field:SerializeField] public int MiningTime {get; set;}
    [field:SerializeField] public int UpgradeCnt {get; set;}
    [field:SerializeField] public int StageClearCnt {get; set;}
    [field:SerializeField] public int MiningChestCnt {get; set;}
    [field:SerializeField] public int ChallengeClearCnt {get; set;}

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
