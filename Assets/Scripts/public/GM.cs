using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum GameState {
    HOME, PLAY, GAMEOVER, STOP
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
    
    //* PLAY
    [field:HideInInspector] public PlayManager pm;
    [field:HideInInspector] public MineManager mnm;
    [field:HideInInspector] public StageManager stm;
    [field:HideInInspector] public EmployManager epm;
    [field:HideInInspector] public UpgradeManager ugm;

    [field:SerializeField] public Sprite[] RscSprArr {get; private set;}

    void Awake()
    {
        gameState = GameState.HOME;
        _ = this;

        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        ivm = GameObject.Find("InventoryUIManager").GetComponent<InventoryUIManager>();
        idm = GameObject.Find("InventoryDescriptionManager").GetComponent<InventoryDescriptionManager>();
        rwm = GameObject.Find("RewardUIManager").GetComponent<RewardUIManager>();

        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
        ssm = GameObject.Find("SelectStageManager").GetComponent<SelectStageManager>();
        fm = GameObject.Find("FameManager").GetComponent<FameManager>();

        pm = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        mnm = GameObject.Find("MineManager").GetComponent<MineManager>();
        stm = GameObject.Find("StageManager").GetComponent<StageManager>();
        epm = GameObject.Find("EmployManager").GetComponent<EmployManager>();
        ugm = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
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
