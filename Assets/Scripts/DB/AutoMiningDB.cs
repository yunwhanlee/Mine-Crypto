using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct AutoMiningSaveData {
    [field:SerializeField] public bool IsUnlock {get; set;}
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int Time {get; set;}
    [field:SerializeField] public int CurStorage {get; set;}
}

[Serializable]
public class AutoMiningDB
{
    public AutoMiningSaveData[] saveDts;

    public void Init()
    {
        saveDts = new AutoMiningSaveData[9] {
            new() {IsUnlock= true,  Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
        };
    }
}