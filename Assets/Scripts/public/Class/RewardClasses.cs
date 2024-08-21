using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
///* (PUBLIC) 보상 슬롯UI 객체
/// </summary>
[System.Serializable]
public class RewardSlotUI {
    public string name;         // 이름
    public Enum.RWD rwdType;    // 보상 타입
    public GameObject obj;      // 보상 슬롯UI 오브젝트
    public TMP_Text cntTxt;     // 수량 텍스트
}

/// <summary>
///* (PUBLIC) 보상 결과 데이터관리 => 이 데이터로 보상타입 및 수량 확인
/// </summary>
[System.Serializable]
public class RewardResultData {
    // public Dictionary<Enum.RWD, int> rwdResultDic;

    public RewardResultData() {
        // Dictionary 객체 생성
        // rwdResultDic = new Dictionary<Enum.RWD, int>();
        // Dic 초기화
        // InitDic();
    }

    /// <summary>
    /// 모든 Enum.RWD 항목을 돌며 Dictionary데이터 생성 (1회)
    /// </summary>
    // private void InitDic() {
    //     foreach (Enum.RWD rwdType in Enum.GetEnumRWDArr())
    //     {
    //         Debug.Log($"RewardResultData Class:: rwdResultDic.Add({rwdType}, {0})");
    //         rwdResultDic.Add(rwdType, 0);
    //     }
    // }

    /// <summary>
    /// 보상 수령
    /// </summary>
    /// <param name="rwdDic">수령할 보상 Dictionary 데이터</param>
    public void ShowResultReward(Dictionary<Enum.RWD, int> rwdDic) {
        if(rwdDic.Count == 0) {
            GM._.ui.ShowWarningMsgPopUp("수령할 보상 아이템이 없습니다.");
            return;
        }

        // 보상 슬롯UI 팝업 표시
        GM._.rwm.rewardUIPopUp.SetActive(true);

        // 보상슬롯UI 리셋 초기화
        GM._.rwm.ResetAllSlotUI();

        // 획득할 보상아이템 처리
        foreach(var rwd in rwdDic)
        {
            Enum.RWD rwdType = rwd.Key;     // 타입
            int cnt = rwd.Value;            // 획득량

            // rwdResultDic[rwdType] = cnt;    // Dic데이터 최신화

            // 해당 보상슬롯UI 표시
            GM._.rwm.rewardSlotUIArr[(int)rwdType].obj.SetActive(true);
            GM._.rwm.rewardSlotUIArr[(int)rwdType].cntTxt.text = cnt.ToString();

            switch(rwdType)
            {
                case Enum.RWD.ORE1:
                case Enum.RWD.ORE2:
                case Enum.RWD.ORE3:
                case Enum.RWD.ORE4:
                case Enum.RWD.ORE5:
                case Enum.RWD.ORE6:
                case Enum.RWD.ORE7:
                case Enum.RWD.ORE8:
                case Enum.RWD.CRISTAL:
                    // 타겟재화 증가
                    DM._.DB.statusDB.SetRscArr((int)rwdType, cnt);
                    break;
                case Enum.RWD.ORE_TICKET:
                    //TODO
                    break;
                case Enum.RWD.RED_TICKET:
                    //TODO
                    break;
                case Enum.RWD.CHEST:
                    //TODO
                    break;
                case Enum.RWD.FAME:
                    //TODO
                    break;
            }
            
        }
    }
}
