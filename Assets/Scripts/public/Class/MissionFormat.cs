using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Enum;

/// <summary>
/// (명예) 미션 데이터
/// </summary>
[System.Serializable]
public class MissionFormat
{
    [field:SerializeField] public MISSION Type {get; set;}      // 타입
    [field:SerializeField] public int Lv {                      // 레벨
        get {
            var msDB = DM._.DB.missionDB;
            switch(Type)
            {
                case MISSION.MINING_ORE_CNT: return msDB.saveDts[0].Lv;
                case MISSION.MINING_TIME: return msDB.saveDts[1].Lv;
                case MISSION.UPGRADE_CNT: return msDB.saveDts[2].Lv;
                case MISSION.STAGE_CLEAR_CNT:  return msDB.saveDts[3].Lv; //* 고정
                case MISSION.MINING_CHEST_CNT: return msDB.saveDts[4].Lv;
                case MISSION.CHALLENGE_CLEAR_CNT: return msDB.saveDts[5].Lv;
            }
            Debug.LogError("MissionFormat:: Type Error : 맞는 타입이 없습니다.");
            return -1;
        }
        set {
            var msDB = DM._.DB.missionDB;
            switch(Type)
            {
                case MISSION.MINING_ORE_CNT: msDB.saveDts[0].Lv = value; break;
                case MISSION.MINING_TIME: msDB.saveDts[1].Lv = value; break;
                case MISSION.UPGRADE_CNT: msDB.saveDts[2].Lv = value; break;
                case MISSION.STAGE_CLEAR_CNT:  msDB.saveDts[3].Lv = value; break;
                case MISSION.MINING_CHEST_CNT: msDB.saveDts[4].Lv = value; break;
                case MISSION.CHALLENGE_CLEAR_CNT: msDB.saveDts[5].Lv = value; break;
            }
        }
    }            
    [field:SerializeField] public int MaxExp {get; set;}        // 필요경험치
    [field:SerializeField] public int Exp {                     // 현재경험치
        get {
            var missionDB = DM._.DB.missionDB;

            switch(Type)
            {
                case MISSION.MINING_ORE_CNT:
                    return missionDB.saveDts[0].Exp;
                case MISSION.MINING_TIME:
                    return missionDB.saveDts[1].Exp;
                case MISSION.UPGRADE_CNT:
                    return missionDB.saveDts[2].Exp;
                case MISSION.STAGE_CLEAR_CNT: //* 고정
                    return missionDB.saveDts[3].Exp;
                case MISSION.MINING_CHEST_CNT:
                    return missionDB.saveDts[4].Exp;
                case MISSION.CHALLENGE_CLEAR_CNT:
                    return missionDB.saveDts[5].Exp;
            }
            Debug.LogError("MissionFormat:: Type Error : 맞는 타입이 없습니다.");
            return -1;
        }
        set {
            var missionDB = DM._.DB.missionDB;

            switch(Type)
            {
                case MISSION.MINING_ORE_CNT:
                    missionDB.saveDts[0].Exp = value;
                    break;
                case MISSION.MINING_TIME:
                    missionDB.saveDts[1].Exp = value; 
                    break;
                case MISSION.UPGRADE_CNT:
                    missionDB.saveDts[2].Exp = value; 
                    break;
                case MISSION.STAGE_CLEAR_CNT: //* 고정
                    missionDB.saveDts[3].Exp = value; 
                    break;
                case MISSION.MINING_CHEST_CNT:
                    missionDB.saveDts[4].Exp = value; 
                    break;
                case MISSION.CHALLENGE_CLEAR_CNT:
                    missionDB.saveDts[5].Exp = value; 
                    break;
            }

            // 객체생성 단계에서는 MaxExp가 0일때는 업데이트 알림UI 처리하지 않기
            if(MaxExp == 0)
                return;

            Debug.Log($"MissionFormat:: Type= {Type}, Lv= {Lv}, Exp= {value}, MaxExp= {MaxExp}");

            // 업데이트 알림UI 🔴
            GM._.fm.UpdateAlertRedDot();
        }
    }

    public Dictionary<RWD, int> Reward {get; set;}              // 보상 Dictionary리스트


#region FUNC
    /// <summary>
    /// 레벨에 따른 필요경험치 업데이트
    /// </summary>
    public void UpdateData() {
        switch(Type)
        {
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
                    { RWD.SKILLPOTION, 1 },
                };
                break;
        }
    }
#endregion
}
