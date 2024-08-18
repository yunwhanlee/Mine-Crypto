using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM _;

    //* Outside
    [field:HideInInspector] public UIManager ui;

    [field:HideInInspector] public HomeManager hm;
    [field:HideInInspector] public SelectStageManager ssm;

    [field:HideInInspector] public PlayManager pm;
    [field:HideInInspector] public MineManager mnm;
    [field:HideInInspector] public StageManager stm;
    [field:HideInInspector] public EmployManager epm;
    [field:HideInInspector] public UpgradeManager ugm;
    [field:HideInInspector] public RebornManager rbm;

    [field:SerializeField] public Sprite[] RscSprArr {get; private set;}

    void Awake()
    {
        _ = this;

        ui = GameObject.Find("UIManager").GetComponent<UIManager>();

        hm = GameObject.Find("HomeManager").GetComponent<HomeManager>();
        ssm = GameObject.Find("SelectStageManager").GetComponent<SelectStageManager>();

        pm = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        mnm = GameObject.Find("MineManager").GetComponent<MineManager>();
        stm = GameObject.Find("StageManager").GetComponent<StageManager>();
        epm = GameObject.Find("EmployManager").GetComponent<EmployManager>();
        ugm = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
        rbm = GameObject.Find("RebornManager").GetComponent<RebornManager>();

    }

    void Update()
    {
        //! TEST NEXT STAGE
        if(Input.GetKeyDown(KeyCode.A))
        {
            mnm.workerClearStageStatusCnt = 0;
            StartCoroutine(stm.CoNextStage());
        }
    }
}
