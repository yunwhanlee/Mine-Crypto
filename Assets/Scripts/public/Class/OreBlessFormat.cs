using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// 재설정에 필요한 아이템
/// </summary>
[Serializable]
public struct ResetNeedItem {
    public INV type;
    public int val;
}

/// <summary>
/// 광석의축복 실제 적용데이터
/// </summary>
[Serializable]
public struct OreBlessAbilityData {
    public OREBLESS_ABT type; // 능력타입
    public GRADE grade;
    public float val; // int형 능력치일 경우 (int)로 형변환 할 것!
}

/// <summary>
///* 광석의 축복 포멧 (위의 클래스를 모아서 실제 객체를 만드는 클래스)
/// </summary>
[Serializable]
public class OreBlessFormat
{
    //* Elements
    [field:SerializeField] public GameObject LockedPanel;
    [field:SerializeField] public TMP_Text AbilityTxt;
    [field:SerializeField] public Image ResetBtnImg;
    [field:SerializeField] public TMP_Text ResetNeedItemTxt;

    //* Value
    public int id;
    [field:SerializeField] public RSC Type {get; set;} // 축복 광석타입
    [field:SerializeField] public ResetNeedItem ResetNeedItem {get; private set;} // 재설정에 필요한 아이템목록

    [field:SerializeField] public bool IsUnlock {
        get => DM._.DB.oreBlessDB.saveDts[id].IsUnlock;
        set {
            DM._.DB.oreBlessDB.saveDts[id].IsUnlock = value;
            LockedPanel.SetActive(!value); // 잠금화면UI 업데이트
        }
    }

    [field:SerializeField] public int AbilityCnt {
        get => DM._.DB.oreBlessDB.saveDts[id].AbilityCnt;
        set => DM._.DB.oreBlessDB.saveDts[id].AbilityCnt = value;
    }
    [field:SerializeField] public List<OreBlessAbilityData> AbilityList {
        get => DM._.DB.oreBlessDB.saveDts[id].AbilityList; 
        set => DM._.DB.oreBlessDB.saveDts[id].AbilityList = value;
    }

#region FUNC
    /// <summary>
    /// 재설정에 필요한 아이템목록 텍스트 표시
    /// </summary>
    public void SetResetNeedItemTxtUI()
    {
        ResetNeedItemTxt.text = $"<size=75%><sprite name={ResetNeedItem.type}></size> {ResetNeedItem.val}";
    }

    /// <summary>
    /// 재설정 가능여부에 따라 버튼색상 변경
    /// </summary>
    public void SetResetBtnColorUI()
    {
        if(IsUnlock)
        {
            bool isPossible = DM._.DB.statusDB.GetInventoryItemVal(ResetNeedItem.type) >= ResetNeedItem.val;

            ResetBtnImg.sprite = isPossible? GM._.obm.YellowBtnSpr : GM._.obm.GrayBtnSpr;
        }
    }
#endregion
}