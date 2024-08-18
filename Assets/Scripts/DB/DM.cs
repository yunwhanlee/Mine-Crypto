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
}

/// <summary>
///* データマネジャー
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";

    //* ★データベース
    [field: SerializeField] public DB DB {get; private set;}

    private void Awake() {
        //* SINGLETON
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(_);
        }
        else {
            Destroy(gameObject);
            return;
        }
    }
}
