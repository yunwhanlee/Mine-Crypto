using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

/// <summary>
/// (子) 연금술 데이터SO : 교환 카테고리
/// </summary>
[CreateAssetMenu(fileName = "Exchange", menuName = "AlchemySO/AlchemyDataSO_Exchange")]
public class AlchemyDataSO_Exchange : AlchemyDataSO
{
    public EXCHANGE type;                  // 교환타입
    public Sprite itemSpr;                 // 아이템 이미지
}
