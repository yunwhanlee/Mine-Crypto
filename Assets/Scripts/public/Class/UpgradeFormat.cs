using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

/// <summary>
///* 업그레이드 포멧 (최상위)
/// </summary>
public class UpgradeFormat
{
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int MaxLv {get; set;}
    [field:SerializeField] public Enum.INV NeedRsc {get; protected set;} // 업그레이드에 필요한 재화종류
    [field:SerializeField] public int Price {get; set;}                 // 업그레이드 가격
    [field:SerializeField] public int PriceDef {get; protected set;}   // 업그레이드 초기 가격
    public bool IsMaxLv {get => Lv >= MaxLv;}

    /// <summary>
    /// 업그레이드 가능한지 여부 확인
    /// </summary>
    /// <returns>업그레이드 가능하다면 true, 아니면 false</returns>
    public bool CheckPossibleUpgrade()
    {
        return DM._.DB.statusDB.GetInventoryItemVal(NeedRsc) >= Price;
    }

    /// <summary>
    /// 가격 업데이트 (기본)
    /// </summary>
    public void UpdatePrice(DEC_UPG_TYPE decUpgType = DEC_UPG_TYPE.NONE)
    {
        Price = PriceDef + Lv * (Lv) * PriceDef / 2;

        //* 비용감소 추가처리
        var rbm = GM._.rbm;

        // 강화비용 감소%
        if(rbm.upgDecUpgradePricePer.Lv > 0 && decUpgType == DEC_UPG_TYPE.UPGRADE) {
            float decreasePer = 1 - rbm.upgDecUpgradePricePer.Val;
            Price = Mathf.RoundToInt(Price * decreasePer);
        }
        // 초월 강화비용 감소%
        else if(rbm.upgDecTranscendPircePer.Lv > 0 && decUpgType == DEC_UPG_TYPE.TRANSCEND) {
            float decreasePer = 1 - rbm.upgDecTranscendPircePer.Val;
            Price = Mathf.RoundToInt(Price * decreasePer);
        }
    }

    /// <summary>
    /// 가격 업데이트 (등차수열)
    /// </summary>
    public void UpdatePriceArithmetic(DEC_UPG_TYPE decUpgType = DEC_UPG_TYPE.NONE)
    {
        Price = PriceDef + Lv * PriceDef;

        //* 비용감소 추가처리
        var rbm = GM._.rbm;

        // 강화비용 감소%
        if(rbm.upgDecUpgradePricePer.Lv > 0 && decUpgType == DEC_UPG_TYPE.UPGRADE) {
            float decreasePer = 1 - rbm.upgDecUpgradePricePer.Val;
            Price = Mathf.RoundToInt(Price * decreasePer);
        }
    }

    /// <summary>
    /// 가격 업데이트 배열설정 프리셋 
    /// </summary>
    public void UpdatePriceFreeSet(int[] priceArr)
    {
        if(Lv >= priceArr.Length)
            Price = 9999; // MAX LV
        else
            Price = priceArr[Lv];
    }

    /// <summary>
    /// 가격 업데이트 (밸런스 조절방식)
    /// </summary>
    /// <param name="balanceVal">밸런스 조절 파라메터</param>
    public void UpdatePrice(int balanceVal)
    {
        Price = PriceDef + (Lv * Lv * balanceVal) / 2;
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
    [field:SerializeField] public int MaxLv {get; set;}
    [field:SerializeField] public Enum.MUSH NeedMush {get; protected set;} // 업그레이드에 필요한 버섯종류
    [field:SerializeField] public int Price {get; set;}                 // 업그레이드 가격
    [field:HideInInspector] public int PriceDef {get; protected set;}   // 업그레이드 초기 가격

    /// <summary>
    /// 버섯 가격 업데이트
    /// </summary>
    public void UpdatePrice()
    {
        // 소수점 올림처리를 위해 Lv를 float로 형변환
        Price = 1 + Mathf.CeilToInt((float)Lv * Lv / 2);
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

    public UpgradeMushFormatFloat(int Lv, float Unit, MUSH NeedMush, int PriceDef, float DefVal, int MaxLv) {
        this.Lv = Lv;
        this.Unit = Unit;
        this.NeedMush = NeedMush;
        this.PriceDef = PriceDef;
        this.DefVal = DefVal;
        this.MaxLv = MaxLv;
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

    public UpgradeMushFormatInt(int Lv, int Unit, MUSH NeedMush, int PriceDef, int DefVal, int MaxLv) {
        this.Lv = Lv;
        this.Unit = Unit;
        this.NeedMush = NeedMush;
        this.PriceDef = PriceDef;
        this.DefVal = DefVal;
        this.MaxLv = MaxLv;
    }

    public int GetNextVal() => DefVal + (Lv + 1) * Unit;
}
#endregion

