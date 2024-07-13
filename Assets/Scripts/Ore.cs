using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ore : MonoBehaviour
{

    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int MaxHp {get; set;}
    [field:SerializeField] public int Hp {get; set;}

    [field:SerializeField] public bool IsMining;
    [field:SerializeField] public int MaxMiningCnt {get; set;}
    [field:SerializeField] public int MiningCnt {get; set;}

    [field: Header("鉱石 リソース：Large, Medium, Small")]
    const int OreLarge = 0, OreMedium = 1, OreSmall = 2;
    [field: SerializeField] public Sprite[] OreSprs {get; private set;}

    [field: SerializeField] public SpriteRenderer SprRdr {get; private set;}
    [field: SerializeField] public Slider HpSlider {get; private set;}

    void Start()
    {
        // Ore スプライト
        SprRdr.sprite = OreSprs[OreLarge];

        // HpBar 非表示
        HpSlider.gameObject.SetActive(false);

        Hp = MaxHp;
        HpSlider.value = (float)Hp / MaxHp;

        IsMining = false;
        MaxMiningCnt = 5;
        MiningCnt = 0;
    }

    public bool DecreaseHp(int dmg) {
        bool isDestory;

        Hp -= dmg;

        // Ore スプライト 設定
        float largeHpRatio = MaxHp * 0.6f;
        float mediumHpRatio = MaxHp * 0.3f;
        SprRdr.sprite = Hp > largeHpRatio? OreSprs[OreLarge]
            : Hp > mediumHpRatio? OreSprs[OreMedium]
            : OreSprs[OreSmall];

        if(Hp > 0)
        {
            HpSlider.value = (float)Hp / MaxHp;
            isDestory = false;
        }
        else
        {
            isDestory = true;
            Destroy(gameObject);
        }

        return isDestory;
    }
}
