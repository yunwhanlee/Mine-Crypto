using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM _;

    //* Outside
    public UIManager ui;
    public MineManager mnm;
    public StageManager stm;
    public EmployManager epm;
    public UpgradeManager ugm;
    public RebornManager rbm;


    void Awake()
    {
        _ = this;

        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        mnm = GameObject.Find("MineManager").GetComponent<MineManager>();
        stm = GameObject.Find("StageManager").GetComponent<StageManager>();
        epm = GameObject.Find("EmployManager").GetComponent<EmployManager>();
        ugm = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
        rbm = GameObject.Find("RebornManager").GetComponent<RebornManager>();
    }
}
