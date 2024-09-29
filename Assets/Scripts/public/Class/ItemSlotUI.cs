using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// 아이템 슬롯 UI
/// </summary>
[System.Serializable]
public class ItemSlotUI
{
    public string name;         // 이름
    public GameObject obj;      // 보상 슬롯UI 오브젝트
    public TMP_Text cntTxt;     // 수량 텍스트
}

/// <summary>
///* 보상 아이템 슬롯UI
/// </summary>
[System.Serializable]
public class RewardSlotUI : ItemSlotUI
{
    public Enum.RWD rwdType;    // 보상 타입
    public DOTweenAnimation DOTAnim;    // 애니메이션
}

/// <summary>
///* 인벤토리 아이템 슬롯UI
/// </summary>
[System.Serializable]
public class InvSlotUI : ItemSlotUI
{
    public Enum.INV invType;    // 보상 타입
    public Sprite itemSpr;      // 아이템 이미지
    public string contentMsg;   // 아이템 정보

    /// <summary>
    /// 인벤토리 아이템 표시・비표시
    /// </summary>
    /// <param name="cnt">실제 수량</param>
    public void Active(int cnt) {
        bool isExist = cnt > 0;
        Debug.Log($"Active(cnt={cnt}):: name= {name}, isExist= {isExist}");

        obj.SetActive(isExist);

        if(isExist)
        {
            cntTxt.text = cnt.ToString();
        }
    }
}
