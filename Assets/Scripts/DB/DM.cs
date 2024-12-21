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
    public int languageIdx;                 // 언어설정 인덱스
    public int rebornCnt;                   // 환생횟수
    public int bestTotalFloor;              // 최대층수 총합기록
    public int challengeBestFloor;          // 시련광산 최대층수
    public float bgmVolume;                 // 배경음 볼륨
    public float sfxVolume;                 // 효과음 볼륨

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
    public TimePieceDB timePieceDB;         // 시간의조각
    public SkillTreeDB skillTreeDB;         // 스킬
    public RebornDB rebornDB;               // 환생
    public ShopDB shopDB;                   // 상점

    public void Init()
    {
        languageIdx = -1;
        rebornCnt = 0;
        bestTotalFloor = 0;
        challengeBestFloor = 0;
        bgmVolume = 0.5f;
        sfxVolume = 0.4f;
        statusDB = new StatusDB();
        statusDB.Init();
        upgradeDB = new UpgradeDB();
        upgradeDB.Init();
        missionDB = new MissionDB();
        missionDB.Init();
        oreBlessDB = new OreBlessDB();
        oreBlessDB.Init();
        proficiencyDB = new OreProficiencyDB();
        proficiencyDB.Init();
        autoMiningDB = new AutoMiningDB();
        autoMiningDB.Init();
        transcendDB = new TranscendDB();
        transcendDB.Init();
        mushDB = new MushDB();
        mushDB.Init();
        decoDB = new DecoDB();
        decoDB.Init();
        stageDB = new StageDB();
        stageDB.Init();
        timePieceDB = new TimePieceDB();
        timePieceDB.Init();
        skillTreeDB = new SkillTreeDB();
        skillTreeDB.Init();
        rebornDB = new RebornDB();
        rebornDB.Init();
        shopDB =  new ShopDB();
        shopDB.Init();
    }
}

/// <summary>
///* 데이터베이스 매니저
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";
    public const string PASSEDTIME_KEY = "PASSEDTIME";
    public bool IsDebugMode; // 디버그로그 체크트리거
    public bool isPC; // PC모드인지 아닌지 체크트리거

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

#if UNITY_ANDROID
    public void OnApplicationPause(bool paused){
        if(DB == null) return;

        //* ゲームが開くとき（paused == true）にも起動されるので注意が必要。
        if(paused == true) {
            // DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
            Save();
        }
    }
#else
    public void OnApplicationQuit() {
        if(DB == null) return;

        Debug.Log("<color=yellow>QUIT APP(PC):: SAVE</color>");
        // DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
        Save();
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
        
        // 일반단위 데이터 저장
        DB.bgmVolume = GM._.stm.bgmSlider.value;
        DB.sfxVolume = GM._.stm.sfxSlider.value;
        DB.timePieceDB.curStorage = GM._.tpm.curStorage;

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

        // 데이터 클래스변수 객체 생성 및 초기화
        DB.Init();
    }
#endregion
}
