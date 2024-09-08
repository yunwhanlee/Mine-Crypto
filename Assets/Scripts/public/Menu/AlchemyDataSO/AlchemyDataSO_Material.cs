using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

/// <summary>
/// (子) 연금술 데이터SO : 재료 카테고리
/// </summary>
[CreateAssetMenu(fileName = "Mat", menuName = "AlchemySO/AlchemyDataSO_Material")]
public class AlchemyDataSO_Material : AlchemyDataSO
{
    public MATE type;                      // 재료타입
    public Sprite itemSpr;                 // 아이템 이미지
}
