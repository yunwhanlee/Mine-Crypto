using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///* 업그레이드 포멧 (최상위)
/// </summary>
public class UpgradeFormat
{
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int Price {get; set;}
    [field:SerializeField] public int PriceDef {get; protected set;}

    public void UpdatePrice()
    {
        Price = PriceDef + Lv * (Lv - 1) * 100 / 2;
    }
}

/// <summary>
/// 업그레이드 포멧 (int형 Val)
/// </summary>
[System.Serializable]
public class UpgradeFormatInt : UpgradeFormat
{
    [field:SerializeField] public int Val {get; set;}
    [field:SerializeField] public int Unit {get; private set;}

    public UpgradeFormatInt(int Lv, int Unit, int PriceDef) {
        this.Lv = Lv;
        this.Unit = Unit;
        this.PriceDef = PriceDef;
    }

    public void UpdateVal() => Val = Lv * Unit;
    public int GetNextVal() => (Lv + 1) * Unit;
}

/// <summary>
/// 업그레이드 포멧 (float형 Val)
/// </summary>
[System.Serializable]
public class UpgradeFormatFloat : UpgradeFormat
{
    [field:SerializeField] public float Val {get; set;}
    [field:SerializeField] public float Unit {get; private set;}

    public UpgradeFormatFloat(int Lv, float Unit, int PriceDef) {
        this.Lv = Lv;
        this.Unit = Unit;
        this.PriceDef = PriceDef;
    }

    public void UpdateVal() => Val = Lv * Unit;
    public float GetNextVal() => (Lv + 1) * Unit;
}
