using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

/// <summary>
/// (子) 연금술 데이터SO : 소비 카테고리
/// </summary>
[CreateAssetMenu(fileName = "Consume", menuName = "AlchemySO/AlchemyDataSO_Consume")]
public class AlchemyDataSO_Consume : AlchemyDataSO
{
    public CONSUME type;                   // 소비타입
    public Sprite itemSpr;                 // 아이템 이미지
}
