using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

/// <summary>
/// 필요 아이템 정보 구조체
/// </summary>
[Serializable]
public struct NeedItemData {
    [SerializeField] INV type;
    public INV Type {get => type; set => type = value;}

    [SerializeField] int val;
    public int Val {get => val; set => val = value;}
}

/// <summary>
/// (親) 연금술 데이터 SO
/// </summary>
public class AlchemyDataSO : ScriptableObject
{
    public NeedItemData[] needItemDataArr; // (공통) 필요 아이템 정보배열
}