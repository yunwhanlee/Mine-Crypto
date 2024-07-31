using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステータス
/// </summary>
[Serializable]
public class StatusDB
{
    [field:SerializeField] int coin; public int Coin {
        get {return coin;}
        set {
            coin = value;
            if(coin < 0) coin = 0;
            GM._.ui.coinTxt.text = coin.ToString();
        }
    }

    public StatusDB() {
        coin = 0;
    }

}
