using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
///* 保存・読込のデータベース ★データはPlayerPrefsに保存するので、String、Float、Intのみ！！！★
/// </summary>
[Serializable]
public class DB {
    public StatusDB statusDB;
    public UpgradeDB upgradeDB;             // 강화
    public MissionDB missionDB;             // 명예미션
    public OreBlessDB oreBlessDB;           // 축복
    public OreProficiencyDB proficiencyDB;  // 숙련도
    public AutoMiningDB autoMiningDB;       // 자동채굴
    public TranscendDB transcendDB;         // 초월
    public MushDB mushDB;                   // 버섯도감
    public DecoDB decoDB;                   // 장식품 오브젝트
    public StageDB stageDB;                 // 스테이지 (잠금, 최고층)

    public void Init()
    {
        statusDB = new StatusDB();
        upgradeDB = new UpgradeDB();
        missionDB = new MissionDB();
        oreBlessDB = new OreBlessDB();
        proficiencyDB = new OreProficiencyDB();
        autoMiningDB = new AutoMiningDB();
        transcendDB = new TranscendDB();
        mushDB = new MushDB();
        decoDB = new DecoDB();
        stageDB = new StageDB();
    }
}

/// <summary>
///* 데이터베이스 매니저
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";
    public const string PASSEDTIME_KEY = "PASSEDTIME";

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

        //* 어플종료시 종료한시간을 저장
        // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
        TimeSpan curTimeStamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        // 시간 차이를 정수형으로 변환하여 저장
        PlayerPrefs.SetInt(PASSEDTIME_KEY, (int)curTimeStamp.TotalSeconds);
        
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
            return null;
        }

        // 데이터 불러오기 (Json형식으로 저장되어있음)
        string jsonDB = PlayerPrefs.GetString(DB_KEY);

        // JsonDB 존재여부 확인
        if (string.IsNullOrEmpty(jsonDB)) {
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

        // 데이터 객체 생성
        DB = new DB();

        // 데이터 클래스변수 객체 생성
        DB.Init();

        // 데이터 초기화
        DB.statusDB.Init();
        DB.upgradeDB.Init();
        DB.missionDB.Init();
        DB.oreBlessDB.Init();
        DB.proficiencyDB.Init();
        DB.autoMiningDB.Init();
        DB.transcendDB.Init();
        DB.mushDB.Init();
        DB.decoDB.Init();
        DB.stageDB.Init();
    }
#endregion
}
