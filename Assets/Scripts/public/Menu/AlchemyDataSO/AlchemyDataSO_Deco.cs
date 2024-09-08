using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

/// <summary>
/// (子) 연금술 데이터SO : 교환 카테고리
/// </summary>
[CreateAssetMenu(fileName = "Deco", menuName = "AlchemySO/AlchemyDataSO_Deco")]
public class AlchemyDataSO_Deco : AlchemyDataSO
{
    public DECO type;                      // 장식타입
    public Sprite itemSpr;                 // 아이템 이미지
    public float ability;                  // 추가능력치
}
