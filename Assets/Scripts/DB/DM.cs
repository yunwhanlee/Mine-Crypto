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
    public UpgradeDB upgradeDB;
    public MissionDB missionDB;
    public OreBlessDB oreBlessDB;
    public AutoMiningDB autoMiningDB;
    public OreProficiencyDB proficiencyDB;
    public StageDB stageDB;
    public DecoDB decoDB;
}

/// <summary>
///* 데이터베이스 매니저
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";

    //* ★데이터베이스
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

        // 데이터 로드
        DB = Load();

        // 로드할 데이터가 없는 경우 (게임 처음플레이시)
        if(DB == null)
        {
            Reset(); // 데이터 초기화
        }
    }

/// -----------------------------------------------------------------------------------------------------------------
#region QUIT GAME 
/// -----------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    public void OnApplicationQuit() {
        if(DB == null) return;

        Debug.Log("<color=yellow>QUIT APP(PC):: SAVE</color>");
        // DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
        Save();
    }
#elif UNITY_ANDROID
    public void OnApplicationPause(bool paused){
        if(DB == null) return;

        //* ゲームが開くとき（paused == true）にも起動されるので注意が必要。
        if(paused == true) {
            // DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
            Save();
        }
    }
#endif
#endregion

/// -----------------------------------------------------------------------------------------------------------------
#region SAVE
/// -----------------------------------------------------------------------------------------------------------------
    public void Save()
    {
        if(DB == null) return;

        //TODO 자동채굴 현재시간 저장 및 차이 계산
        
        //* Serialize DB To Json
        string jsonDB = JsonUtility.ToJson(DB, true);
        //* Save Data
        PlayerPrefs.SetString(DB_KEY, jsonDB);
        //* Print
        Debug.Log($"★SAVE:: The Key: {DB_KEY} Exists? {PlayerPrefs.HasKey(DB_KEY)}, jsonDB ={jsonDB}");
    }
#endregion

/// -----------------------------------------------------------------------------------------------------------------
#region LOAD
/// -----------------------------------------------------------------------------------------------------------------
#endregion
    public DB Load()
    {
        // Prefab KEY 존재여부 확인
        if(!PlayerPrefs.HasKey(DB_KEY)){
            GM._.ui.ShowWarningMsgPopUp("로드할 데이터가 없습니다.");
            return null;
        }

        // 데이터 불러오기 (Json형식으로 저장되어있음)
        string jsonDB = PlayerPrefs.GetString(DB_KEY);

        // JsonDB 존재여부 확인
        if (string.IsNullOrEmpty(jsonDB)) {
            GM._.ui.ShowWarningMsgPopUp("저장된 데이터(Json)가 없습니다.");
            return null;
        }

        // JsonDB 클래스화
        Debug.Log($"★LOAD:: PlayerPrefs.GetString({DB_KEY}) -> {jsonDB}");
        DB savedData = JsonUtility.FromJson<DB>(jsonDB);
        return savedData;
    }
/// -----------------------------------------------------------------------------------------------------------------
#region RESET
/// -----------------------------------------------------------------------------------------------------------------
    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log($"★RESET:: PlayerPrefs.HasKey({DB_KEY}) -> {PlayerPrefs.HasKey(DB_KEY)}");

        // 데이터 초기화
        DB.statusDB.Init();
        DB.upgradeDB.Init();
        DB.missionDB.Init();
        DB.oreBlessDB.Init();
        DB.autoMiningDB.Init();
        DB.proficiencyDB.Init();
        DB.stageDB.Init();
        DB.decoDB.Init();
    }
#endregion
}
