using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MissionDB
{
    [field:SerializeField] int MiningOreCnt {get; set;}
    [field:SerializeField] int MiningTime {get; set;}
    [field:SerializeField] int UpgradeCnt {get; set;}
    [field:SerializeField] int StageClearCnt {get; set;}
    [field:SerializeField] int MiningChestCnt {get; set;}
    [field:SerializeField] int ChallengeClearCnt {get; set;}
}
