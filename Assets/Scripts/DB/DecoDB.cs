using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DecoDB {
    [field:SerializeField] public bool[] IsBuyedArr {get; set;}

    public void Init()
    {
        IsBuyedArr = new bool[8] {
            false, false, false, false,
            false, false, false, false,
        };
    }
}
