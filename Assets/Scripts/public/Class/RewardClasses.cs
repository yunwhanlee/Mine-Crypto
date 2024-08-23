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
