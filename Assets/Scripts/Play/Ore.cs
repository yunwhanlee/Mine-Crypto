using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;
using static SoundManager;
using static GameEffectManager;

public class Ore : MonoBehaviour
{
    [field:SerializeField] public RSC OreType {get; private set;} // CRISTAL은 보물상자 Prefab임
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
    [field: SerializeField] public Collider2D col {get; private set;}
    [field: SerializeField] public Slider HpSlider {get; private set;}
    [field: SerializeField] public TMP_Text HpSliderTxt {get; private set;}

    void Start()
    {
        col = GetComponent<Collider2D>();

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
        GM._.efm.ShowDmgTxtEF(transform.position, dmg);

        if(Hp > 0)
        {
            int randomIdx = Random.Range((int)SFX.Metal1SFX, (int)SFX.Metal3SFX + 1);
            SoundManager._.PlaySfx((SFX)randomIdx);

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

            // 보물상자인 경우
            if(OreType == RSC.CRISTAL) 
            {   
                SoundManager._.PlaySfx(SFX.TreasureChestOrePickSFX);

                // 수량 1 + (초월)보물상자 획득량
                int ammount = 1 + GM._.sttm.ExtraTreasureChest;

                GM._.pm.playResRwdArr[(int)RWD.TREASURE_CHEST] += ammount; // 결과수치 UI
                DM._.DB.statusDB.TreasureChest += ammount; // 데이터

                Debug.Log("보물상자 획득 EF표시!");
                GM._.ui.PlayTreasureChestAttractionPtcUIEF();

                // 보물상자캐기 미션 +
                GM._.fm.missionArr[(int)MISSION.MINING_CHEST_CNT].Exp++;
            }
            // 일반광석인 경우
            else 
            {
                SoundManager._.PlaySfx(SoundManager.SFX.BrokenOreSFX);
                // 광석캐기 미션 +
                GM._.fm.missionArr[(int)MISSION.MINING_ORE_CNT].Exp++;
                // 숙련도 경험치 UP
                GM._.pfm.proficiencyArr[(int)OreType].Exp++;
            }

            GM._.dim.DropBonusItem();
            
            GM._.efm.ShowEF((EFIDX)OreType, transform.position);
            SprRdr.enabled = false;
            col.enabled = false;
            HpSlider.gameObject.SetActive(false);

            Destroy(gameObject);
        }
    }
}
