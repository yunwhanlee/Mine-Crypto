using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Enum;

[Serializable]
public struct MissionSaveData {
    [field:SerializeField] public MISSION Type {get; private set;}      // 타입
    [field:SerializeField] public int Lv {get; set;}                    // 레벨
    [field:SerializeField] public int Exp {get; set;}

    public MissionSaveData(MISSION Type, int Lv, int Exp)
    {
        this.Type = Type;
        this.Lv = Lv;
        this.Exp = Exp;
    }
}

[Serializable]
public class MissionDB
{
    public MissionSaveData[] saveDts;

    public void Init()
    {
        saveDts = new MissionSaveData[6] {
            new MissionSaveData(MISSION.MINING_ORE_CNT, Lv: 1, Exp: 0),         // 광석채굴
            new MissionSaveData(MISSION.MINING_TIME, Lv: 1, Exp: 0),            // 채굴시간
            new MissionSaveData(MISSION.UPGRADE_CNT, Lv: 1, Exp: 0),            // 강화하기
            new MissionSaveData(MISSION.STAGE_CLEAR_CNT, Lv: 1, Exp: 0),        // 광산 클리어
            new MissionSaveData(MISSION.MINING_CHEST_CNT, Lv: 1, Exp: 0),       // 보물상자 획득
            new MissionSaveData(MISSION.CHALLENGE_CLEAR_CNT, Lv: 1, Exp: 0),    // 시련의광산 돌파
        };
    }
}
