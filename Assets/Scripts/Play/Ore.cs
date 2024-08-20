using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ore : MonoBehaviour
{
    [field:SerializeField] public Enum.RSC OreType {get; private set;}
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int MaxHp {get; set;}
    [field:SerializeField] int Hp {get; set;}

    [field:SerializeField] public bool IsDestroied;

    /// <summary> 현재 자신을 채굴중인 캐릭터 수 </summary>
    [field:SerializeField] public int MiningCnt {get; set;}

    [field: Header("EFFECT")]
    [field: SerializeField] public ParticleSystem MiningHitPtcEF {get; private set;}

    [field: Header("鉱石 リソース：Large, Medium, Small")]
    const int OreLarge = 0, OreMedium = 1, OreSmall = 2;
    [field: SerializeField] public Sprite[] OreSprs {get; private set;}

    [field: SerializeField] public SpriteRenderer SprRdr {get; private set;}
    [field: SerializeField] public Slider HpSlider {get; private set;}
    [field: SerializeField] public TMP_Text HpSliderTxt {get; private set;}

    void Start()
    {
        // Ore スプライト
        SprRdr.sprite = OreSprs[OreLarge];

        // Sorting Layer
        SprRdr.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;

        // HpBar 非表示
        HpSlider.gameObject.SetActive(false);

        Hp = MaxHp;
        HpSlider.value = (float)Hp / MaxHp;
        HpSliderTxt.text = MaxHp.ToString();

        MiningCnt = 0;
    }

    public void DecreaseHp(int dmg) {
        // 破壊したら、同じ採掘をしている他のゴブリンはそのまま終了
        if(IsDestroied)
            return;

        Hp -= dmg;


        if(Hp > 0)
        {
            MiningHitPtcEF.Play();

            HpSlider.value = (float)Hp / MaxHp;
            HpSliderTxt.text = Hp.ToString();

            // Ore スプライト 設定
            float largeHpRatio = MaxHp * 0.6f;
            float mediumHpRatio = MaxHp * 0.3f;
            SprRdr.sprite = Hp > largeHpRatio? OreSprs[OreLarge]
                : Hp > mediumHpRatio? OreSprs[OreMedium]
                : OreSprs[OreSmall];
        }
        else
        {
            IsDestroied = true;
            GM._.mnm.CurTotalMiningCnt -= MiningCnt;
            Destroy(gameObject);
        }
    }
}
