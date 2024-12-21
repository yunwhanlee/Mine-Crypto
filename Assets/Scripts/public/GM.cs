using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Assets.PixelFantasy.Common.Scripts;
using UnityEngine;

public enum GameState {
    HOME, PLAY, TIMEOVER, STOP
}

public class GM : MonoBehaviour
{
    public GameState gameState;
    public static GM _;

    //* PUBLIC
    [field:HideInInspector] public UIManager ui;
    [field:HideInInspector] public InventoryUIManager ivm;
    [field:HideInInspector] public InventoryDescriptionManager idm;
    [field:HideInInspector] public RewardUIManager rwm;
    [field:HideInInspector] public FameManager fm; // 명예 및 미션
    [field:HideInInspector] public OreBlessManager obm;
    [field:HideInInspector] public AutoMiningManager amm;
    [field:HideInInspector] public OreProficiencyManager pfm;
    [field:HideInInspector] public ChallengeManager clm; // 시련의 광산
    [field:HideInInspector] public StatusManager sttm;
    [field:HideInInspector] public TranscendManager tsm; // 초월
    [field:HideInInspector] public MushroomManager mrm; // 버섯도감
    [field:HideInInspector] public AlchemyManager acm; // 연금술
    [field:HideInInspector] public TimePieceManager tpm; // 시간의조각
    [field:HideInInspector] public SkillManager skm; // 스킬
    [field:HideInInspector] public SkillController skc; // 스킬발동 컨트롤러
    [field:HideInInspector] public RebornManager rbm; // 환생
    [field:HideInInspector] public ShopManager spm; // 상점

    //* HOME
    [field:HideInInspector] public HomeManager hm;
    [field:HideInInspector] public SelectStageManager ssm;
    [field:HideInInspector] public SettingManager stm;
    
    //* PLAY
    [field:HideInInspector] public PlayManager pm;
    [field:HideInInspector] public GameEffectManager efm;
    [field:HideInInspector] public MineManager mnm;
    [field:HideInInspector] public StageManager stgm;
    [field:HideInInspector] public EmployManager epm;
    [field:HideInInspector] public UpgradeManager ugm;
    [field:HideInInspector] public DropItemManager dim;

    [field:Header("컨텐츠개방팝업 이미지")]
    [field:SerializeField] public Sprite oreBlessIconSpr;    // 광산축복 아이콘
    [field:SerializeField] public Sprite transcendIconSpr;   // 초월 아이콘
    [field:SerializeField] public Sprite mushroomIconSpr;   // 버섯도감 아이콘


    [field:Header("광석재화 이미지배열")]
    [field:SerializeField] public Sprite[] RscSprArr {get; private set;}

    [field:Header("버섯도감 이미지배열")]
    [field:SerializeField] public Sprite[] MushSprArr {get; private set;}

    void Awake()
    {
        Application.targetFrameRate = 60;

        gameState = GameState.HOME;
        _ = this;

        // PUBLIC
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        ivm = GameObject.Find("InventoryUIManager").GetComponent<InventoryUIManager>();
        idm = GameObject.Find("InventoryDescriptionManager").GetComponent<InventoryDescriptionManager>();
        rwm = GameObject.Find("RewardUIManager").GetComponent<RewardUIManager>();

        // PUBLIC MENU
        ugm = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
        fm = GameObject.Find("FameManager").GetComponent<FameManager>();
        obm = GameObject.Find("OreBlessManager").GetComponent<OreBlessManager>();
        amm = GameObject.Find("AutoMiningManager").GetComponent<AutoMiningManager>();
        pfm = GameObject.Find("OreProficiencyManager").GetComponent<OreProficiencyManager>();
        clm = GameObject.Find("ChallengeManager").GetComponent<ChallengeManager>();
        sttm = GameObject.Find("StatusManager").GetComponent<StatusManager>();
        tsm = GameObject.Find("TranscendManager").GetComponent<TranscendManager>();
        mrm = GameObject.Find("MushroomManager").GetComponent<MushroomManager>();
        acm = GameObject.Find("AlchemyManager").GetComponent<AlchemyManager>();
        tpm = GameObject.Find("TimePieceManager").GetComponent<TimePieceManager>();
        skm = GameObject.Find("SkillManager").GetComponent<SkillManager>();
        skc = GameObject.Find("SkillController").GetComponent<SkillController>();
        rbm = GameObject.Find("RebornManager").GetComponent<RebornManager>();
        spm = GameObject.Find("ShopManager").GetComponent<ShopManager>();

        // HOME
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
        stm = GameObject.Find("SettingManager").GetComponent<SettingManager>();
        ssm = GameObject.Find("SelectStageManager").GetComponent<SelectStageManager>();

        // PLAY
        pm = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        efm = GameObject.Find("GameEffectManager").GetComponent<GameEffectManager>();
        mnm = GameObject.Find("MineManager").GetComponent<MineManager>();
        stgm = GameObject.Find("StageManager").GetComponent<StageManager>();
        epm = GameObject.Find("EmployManager").GetComponent<EmployManager>();
        dim = GameObject.Find("DropItemManager").GetComponent<DropItemManager>();
    }

    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        SoundManager._.PlayBgm(SoundManager.BGM.Home);
        StartCoroutine(CoTimerStart());
        StartCoroutine(CoRealTimeTimerStart());
    }

#region FUNC
    /// <summary>
    /// 게임속도 적용 타이머
    /// </summary>
    private IEnumerator CoTimerStart()
    {
        // 데이터가 다 로드 될때까지 1초 대기
        yield return Util.TIME1;

        while(true)
        {
            // 티켓 자동획득 
            ssm.SetOreTicketTimer();

            // 광석 자동채굴
            amm.SetOreTimer();
            amm.SetCristalTimer();

            // 시간의조각 자동회복
            tpm.SetTimer();

            yield return Util.TIME1;
        }
    }

    /// <summary>
    /// 현실타이머 
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoRealTimeTimerStart()
    {
        // 데이터가 다 로드 될때까지 1초 대기
        yield return Util.RT_TIME1;

        while(true)
        {
            // 상점 명예보급 타이머
            spm.SetFameSupplyTimer();

            yield return Util.RT_TIME1;
        }
    }
#endregion
}
