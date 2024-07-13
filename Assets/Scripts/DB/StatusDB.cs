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
    [field:SerializeField] public int Coin {get; set;}

    public StatusDB() {
        Coin = 0;
    }

}
