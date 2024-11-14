using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///* 업그레이드 포멧 (최상위)
/// </summary>
public class UpgradeFormat
{
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int MaxLv {get; protected set;}
    [field:SerializeField] public Enum.INV NeedRsc {get; protected set;} // 업그레이드에 필요한 재화종류
    [field:SerializeField] public int Price {get; set;}                 // 업그레이드 가격
    [field:SerializeField] public int PriceDef {get; protected set;}   // 업그레이드 초기 가격
    public bool IsMaxLv {get => Lv >= MaxLv;}

    /// <summary>
    /// 가격 업데이트 (기본)
    /// </summary>
    public void UpdatePrice()
    {
        Price = PriceDef + Lv * (Lv) * PriceDef / 2;
    }

    public void UpdatePriceFreeSet(int[] priceArr)
    {
        Price = priceArr[Lv];
    }

    /// <summary>
    /// 가격 업데이트 (밸런스 조절방식)
    /// </summary>
    /// <param name="balanceVal">밸런스 조절 파라메터</param>
    public void UpdatePrice(int balanceVal)
    {
        Price = PriceDef + Lv * (Lv) * balanceVal / 2;
    }
}

/// <summary>
/// 업그레이드 포멧 (float형 Val)
/// </summary>
[System.Serializable]
public class UpgradeFormatFloat : UpgradeFormat
{
    [field:SerializeField] public float DefVal {get; private set;}
    [field:SerializeField] public float Val {get => DefVal + (Lv * Unit);}
    [field:SerializeField] public float Unit {get; private set;}

    public UpgradeFormatFloat(int Lv, int MaxLv, float Unit, Enum.INV NeedRsc, int PriceDef, float DefVal) {
        this.Lv = Lv;
        this.MaxLv = MaxLv;
        this.Unit = Unit;
        this.NeedRsc = NeedRsc;
        this.PriceDef = PriceDef;
        this.DefVal = DefVal;
    }

    public float GetNextVal() => DefVal + (Lv + 1) * Unit;
}

/// <summary>
/// 업그레이드 포멧 (int형 Val)
/// </summary>
[System.Serializable]
public class UpgradeFormatInt : UpgradeFormat
{
    [field:SerializeField] public int DefVal {get; private set;}
    [field:SerializeField] public int Val {get => DefVal + (Lv * Unit);}
    [field:SerializeField] public int Unit {get; private set;}

    public UpgradeFormatInt(int Lv, int Unit, Enum.INV NeedRsc, int PriceDef, int DefVal, int MaxLv) {
        this.Lv = Lv;
        this.MaxLv = MaxLv;
        this.Unit = Unit;
        this.NeedRsc = NeedRsc;
        this.PriceDef = PriceDef;
        this.DefVal = DefVal;
    }

    public int GetNextVal() => DefVal + (Lv + 1) * Unit;
}

#region MUSHRROM FORMAT
/// <summary>
///* 버섯 업그레이드 포멧 (최상위)
/// </summary>
public class UpgradeMushFormat
{
    [field:SerializeField] public int Lv {get; set;}
    [field:HideInInspector] public int MaxLv {get; protected set;}
    [field:SerializeField] public Enum.MUSH NeedMush {get; protected set;} // 업그레이드에 필요한 버섯종류
    [field:SerializeField] public int Price {get; set;}                 // 업그레이드 가격
    [field:HideInInspector] public int PriceDef {get; protected set;}   // 업그레이드 초기 가격

    public void UpdatePrice()
    {
        Price = 1 + (Lv * (Lv - 1) * 1) / 2;
    }
}

/// <summary>
/// 버섯 업그레이드 포멧 (float형 Val)
/// </summary>
[System.Serializable]
public class UpgradeMushFormatFloat : UpgradeMushFormat
{
    [field:SerializeField] public float DefVal {get; private set;}
    [field:SerializeField] public float Val {get => DefVal + (Lv * Unit);}
    [field:SerializeField] public float Unit {get; private set;}

    public UpgradeMushFormatFloat(int Lv, int MaxLv, float Unit, Enum.MUSH NeedMush, int PriceDef, float DefVal) {
        this.Lv = Lv;
        this.MaxLv = MaxLv;
        this.Unit = Unit;
        this.NeedMush = NeedMush;
        this.PriceDef = PriceDef;
        this.DefVal = DefVal;
    }

    public float GetNextVal() => DefVal + (Lv + 1) * Unit;
}

/// <summary>
/// 버섯 업그레이드 포멧 (int형 Val)
/// </summary>
[System.Serializable]
public class UpgradeMushFormatInt : UpgradeMushFormat
{
    [field:SerializeField] public int DefVal {get; private set;}
    [field:SerializeField] public int Val {get => DefVal + (Lv * Unit);}
    [field:SerializeField] public int Unit {get; private set;}

    public UpgradeMushFormatInt(int Lv, int Unit, Enum.MUSH NeedMush, int PriceDef, int DefVal, int MaxLv) {
        this.Lv = Lv;
        this.MaxLv = MaxLv;
        this.Unit = Unit;
        this.NeedMush = NeedMush;
        this.PriceDef = PriceDef;
        this.DefVal = DefVal;
    }

    public int GetNextVal() => DefVal + (Lv + 1) * Unit;
}
#endregion

