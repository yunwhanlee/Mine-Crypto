using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BestFloorDB
{
    [Header("광석 8종 및 시련의광산 최대 도달층")]
    [field:SerializeField] int[] oreArr; public int[] OreArr {
        get => oreArr;
    }
}
