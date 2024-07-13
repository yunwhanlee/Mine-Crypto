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

    [field:SerializeField] public Slider HpSlider {get; set;}

    void Start()
    {
        // HpBar 非表示
        HpSlider.gameObject.SetActive(false);

        // MaxHp = 100;
        Hp = MaxHp;
        HpSlider.value = (float)Hp / MaxHp;

        IsMining = false;
        MaxMiningCnt = 5;
        MiningCnt = 0;
    }

    public bool DecreaseHp(int dmg) {
        bool isDestory;

        Hp -= dmg;

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
