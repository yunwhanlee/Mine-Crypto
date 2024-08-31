using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

/// <summary>
/// (명성) 미션 데이터
/// </summary>
[System.Serializable]
public class MissionFormat
{
    [field:SerializeField] public MISSION Type {get; set;} // 타입
    [field:SerializeField] public int Lv {get; set;}            // 레벨
    [field:SerializeField] public int Exp {get; set;}           // 현재경험치
    [field:SerializeField] public int MaxExp {get; set;}        // 필요경험치

    public Dictionary<RWD, int> Reward {get; set;}              // 보상 Dictionary리스트

#region FUNC
    /// <summary>
    /// 레벨에 따른 필요경험치 업데이트
    /// </summary>
    public void UpdateData() {
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
                MaxExp = 3;
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
