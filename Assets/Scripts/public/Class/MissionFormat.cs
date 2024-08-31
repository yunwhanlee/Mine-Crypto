using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Enum;

/// <summary>
/// (명성) 미션 데이터
/// </summary>
[System.Serializable]
public class MissionFormat
{
    [field:SerializeField] public MISSION Type {get; set;}      // 타입
    [field:SerializeField] public int Lv {get; set;}            // 레벨
    [field:SerializeField] public int Exp { // 현재경험치
        get {
            var missionDB = DM._.DB.missionDB;

            switch(Type)
            {
                case MISSION.MINING_ORE_CNT:
                    return missionDB.MiningOreCnt;
                case MISSION.MINING_TIME:
                    return missionDB.MiningTime;
                case MISSION.UPGRADE_CNT:
                    return missionDB.UpgradeCnt;
                case MISSION.STAGE_CLEAR_CNT: //* 고정
                    return missionDB.StageClearCnt;
                case MISSION.MINING_CHEST_CNT:
                    return missionDB.MiningChestCnt;
                case MISSION.CHALLENGE_CLEAR_CNT:
                    return missionDB.ChallengeClearCnt;
            }

            return -1;
        }
        set {
            var missionDB = DM._.DB.missionDB;

            switch(Type)
            {
                case MISSION.MINING_ORE_CNT:
                    missionDB.MiningOreCnt = value; break;
                case MISSION.MINING_TIME:
                    missionDB.MiningTime = value; break;
                case MISSION.UPGRADE_CNT:
                    missionDB.UpgradeCnt = value; break;
                case MISSION.STAGE_CLEAR_CNT: //* 고정
                    missionDB.StageClearCnt = value; break;
                case MISSION.MINING_CHEST_CNT:
                    missionDB.MiningChestCnt = value; break;
                case MISSION.CHALLENGE_CLEAR_CNT:
                    missionDB.ChallengeClearCnt = value; break;
            }

            // 업데이트 알림UI 🔴
            GM._.fm.UpdateAlertRedDot();
        }
    }
    [field:SerializeField] public int MaxExp {get; set;}        // 필요경험치

    public Dictionary<RWD, int> Reward {get; set;}              // 보상 Dictionary리스트

#region FUNC
    /// <summary>
    /// 레벨에 따른 필요경험치 업데이트
    /// </summary>
    public void UpdateData() {
        var missionDB = DM._.DB.missionDB;

        switch(Type) {
            case MISSION.MINING_ORE_CNT:
                MaxExp = 5 + (Lv * (Lv - 1) * 5 ) / 2;
                Reward = new Dictionary<RWD, int> {
                    { RWD.FAME, 1 },
                    { RWD.ORE_CHEST, Lv },
                };
                break;
            case MISSION.MINING_TIME:
                MaxExp = 30 + (Lv * (Lv - 1) * 30) / 2;
                Reward = new Dictionary<RWD, int> {
                    { RWD.FAME, 1 },
                    { RWD.ORE_CHEST, Lv },
                };
                break;
            case MISSION.UPGRADE_CNT:
                MaxExp = 5 + (Lv * (Lv - 1) * 5) / 2;
                Reward = new Dictionary<RWD, int> {
                    { RWD.FAME, 1 },
                    { RWD.ORE_CHEST, Lv },
                };
                break;
            case MISSION.STAGE_CLEAR_CNT: //* 고정
                MaxExp = 3 + (Lv - 1) * 3;
                Reward = new Dictionary<RWD, int> {
                    { RWD.FAME, 1 },
                    { RWD.TREASURE_CHEST, Lv },
                };
                break;
            case MISSION.MINING_CHEST_CNT:
                MaxExp = 1 + (Lv * (Lv - 1) * 2) / 2;
                Reward = new Dictionary<RWD, int> {
                    { RWD.FAME, 1 },
                    { RWD.TREASURE_CHEST, Lv },
                };
                break;
            case MISSION.CHALLENGE_CLEAR_CNT:
                MaxExp = 1 + (Lv - 1);
                Reward = new Dictionary<RWD, int> {
                    { RWD.FAME, 1 },
                    { RWD.TREASURE_CHEST, Lv },
                };
                break;
        }
    }
#endregion
}
