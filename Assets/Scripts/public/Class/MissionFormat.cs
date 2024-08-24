using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// (명성) 미션 데이터
/// </summary>
[System.Serializable]
public class MissionFormat
{
    [field:SerializeField] public Enum.MISSION Type {get; set;}
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int Exp {get; set;}
    [field:SerializeField] public int MaxExp {get; set;}

    //TODO 보상

    public MissionFormat(Enum.MISSION Type, int Lv, int Exp, int MaxExp) {
        this.Type = Type;
        this.Lv = Lv;
        this.Exp = Exp;
        this.MaxExp = MaxExp;
    }

#region FUNC
    /// <summary>
    /// 레벨에 따른 필요경험치 업데이트
    /// </summary>
    public void UpdateNeedExp() {
        switch(Type) {
            case Enum.MISSION.MINING_ORE_CNT:
                MaxExp = 5 + (Lv * (Lv - 1) * 5 ) / 2;
                break;
            case Enum.MISSION.MINING_TIME:
                MaxExp = 30 + (Lv * (Lv - 1) * 30) / 2;
                break;
            case Enum.MISSION.UPGRADE_CNT:
                MaxExp = 5 + (Lv * (Lv - 1) * 5) / 2;
                break;
            case Enum.MISSION.STAGE_CLEAR_CNT: //* 고정
                MaxExp = 3;
                break;
            case Enum.MISSION.MINING_CHEST_CNT:
                MaxExp = 1 + (Lv * (Lv - 1) * 2) / 2;
                break;
            case Enum.MISSION.CHALLENGE_CLEAR_CNT:
                MaxExp = 1 + (Lv - 1);
                break;
        }
    }

    //TODO 보상 ( 1. 명성, 2. 보물상자 )
#endregion
}
