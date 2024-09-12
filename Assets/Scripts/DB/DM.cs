using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
///* 保存・読込のデータベース ★データはPlayerPrefsに保存するので、String、Float、Intのみ！！！★
/// </summary>
[Serializable]
public class DB {
    public StatusDB statusDB;
    public MissionDB missionDB;
    public OreBlessDB oreBlessDB;
    public AutoMiningDB autoMiningDB;
    public OreProficiencyDB proficiencyDB;
    public StageDB stageDB;
    public DecoDB decoDB;
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

        // 데이터 초기화
        DB.statusDB.Init();
        DB.missionDB.Init();
        DB.oreBlessDB.Init();
        DB.autoMiningDB.Init();
        DB.proficiencyDB.Init();
        DB.stageDB.Init();
        DB.decoDB.Init();
    }
}
