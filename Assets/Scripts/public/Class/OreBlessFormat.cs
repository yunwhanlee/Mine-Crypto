using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// 광석의축복 실제 적용데이터
/// </summary>
[Serializable]
public struct OreBlessAbilityData {
    public Enum.OREBLESS_ABT type;
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

    //* Value
    [field:SerializeField] public Enum.RSC Type {get; set;}
    [field:SerializeField] public bool IsUnlock {get; set;}

    [field:SerializeField] public int AbilityCnt {get; set;}
    [field:SerializeField] public List<OreBlessAbilityData> AbilityList {get; set;}
}