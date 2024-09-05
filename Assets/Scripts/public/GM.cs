using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    //* HOME
    [field:HideInInspector] public HomeManager hm;
    [field:HideInInspector] public SelectStageManager ssm;
    [field:HideInInspector] public FameManager fm; // 명성 및 미션
    [field:HideInInspector] public OreBlessManager obm;
    [field:HideInInspector] public AutoMiningManager amm;
    [field:HideInInspector] public OreProficiencyManager pfm;
    [field:HideInInspector] public ChallengeManager clm; // 시련의 광산
    
    //* PLAY
    [field:HideInInspector] public PlayManager pm;
    [field:HideInInspector] public MineManager mnm;
    [field:HideInInspector] public StageManager stm;
    [field:HideInInspector] public EmployManager epm;
    [field:HideInInspector] public UpgradeManager ugm;

    // 광석조각 이미지배열
    [field:SerializeField] public Sprite[] RscSprArr {get; private set;}

    void Awake()
    {
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

        // HOME
        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
        ssm = GameObject.Find("SelectStageManager").GetComponent<SelectStageManager>();

        // PLAY
        pm = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        mnm = GameObject.Find("MineManager").GetComponent<MineManager>();
        stm = GameObject.Find("StageManager").GetComponent<StageManager>();
        epm = GameObject.Find("EmployManager").GetComponent<EmployManager>();
    }

    void Update()
    {
        //! TEST NEXT STAGE
        if(Input.GetKeyDown(KeyCode.B))
        {
            mnm.workerClearStageStatusCnt = 0;
            StartCoroutine(stm.CoNextStage());
        }
    }
}
