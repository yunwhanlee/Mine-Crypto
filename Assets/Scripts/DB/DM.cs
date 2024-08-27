using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///* 保存・読込のデータベース ★データはPlayerPrefsに保存するので、String、Float、Intのみ！！！★
/// </summary>
[Serializable]
public class DB {
    public StatusDB statusDB;
    public MissionDB missionDB;
    public OreBlessDB oreBlessDB;

    [Header("광산 8종 및 시련의광산 최대도달층")]
    public int[] bestFloorArr;
}

/// <summary>
///* データマネジャー
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";

    //* ★データベース
    [field: SerializeField] public DB DB {get; private set;}

    void Awake() {
        //* SINGLETON
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(_);
        }
        else {
            Destroy(gameObject);
            return;
        }

        // Reset
        DB.oreBlessDB = new OreBlessDB {IsUnlockArr = new bool[8]};
        for(int i = 0; i < DB.oreBlessDB.IsUnlockArr.Length; i++)
        {   
            // [0]빼고 전부 false
            if(i == 0) DB.oreBlessDB.IsUnlockArr[i] = true;
            else       DB.oreBlessDB.IsUnlockArr[i] = false;
        }
    }
}
