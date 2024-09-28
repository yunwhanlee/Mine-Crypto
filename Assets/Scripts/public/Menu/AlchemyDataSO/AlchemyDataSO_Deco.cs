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
    public int id;
    public Sprite itemSpr;                   // 아이템 이미지
    public DECO type;                        // 장식타입
    public DECO_ABT abilityType;             // 장식 추가능력 타입

    [field:SerializeField] float abilityVal; // 장식 추가능력치 값
    public float AbilityVal {
        get => IsBuyed? abilityVal : 0;
        private set => abilityVal = value;
    }

    public float AbilityVal_ShowTxt {
        get => abilityVal;
    }

    public GameObject Obj {                  // 장식 오브젝트
        get => GM._.acm.decoObjArr[id];
    }

    public bool IsBuyed {                     // 구매현황
        get => DM._.DB.decoDB.IsBuyedArr[id];
        set => DM._.DB.decoDB.IsBuyedArr[id] = value;
    }
}
