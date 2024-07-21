using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManager : MonoBehaviour
{
    public GameObject[] goblinPrefs;

    public Transform homeTf;
    public Transform oreGroupTf;
    public Transform workerGroupTf;
    public List<GameObject> rockList;

    [field: SerializeField] public int Coin {
        get => DM._.DB.statusDB.Coin;
        set {
            DM._.DB.statusDB.Coin = value;
            GM._.ui.coinTxt.text = DM._.DB.statusDB.Coin.ToString();
        }
    }

    private int workerCnt;
    [field: SerializeField] public int WorkerCnt {
        get {
            workerCnt = workerGroupTf.childCount; // 값을 가져올 때 최신화
            return workerCnt;
        }
        set {
            workerCnt = workerGroupTf.childCount; // 값을 설정할 때 최신화
            GM._.ui.workerCntTxt.text = workerGroupTf.childCount.ToString();
        }
    }

    void Start() {
        WorkerCnt = workerGroupTf.childCount;
    }

    void Update()
    {
        //! TEST GOBLIN
        if(Input.GetKeyDown(KeyCode.Q))
        {
            var ins = Instantiate(goblinPrefs[0], workerGroupTf);
            ins.transform.position = GM._.mm.homeTf.position;
            WorkerCnt++;
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            var ins = Instantiate(goblinPrefs[1], workerGroupTf);
            ins.transform.position = GM._.mm.homeTf.position;
            WorkerCnt++;
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            var ins = Instantiate(goblinPrefs[2], workerGroupTf);
            ins.transform.position = GM._.mm.homeTf.position;
            WorkerCnt++;
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            var ins = Instantiate(goblinPrefs[3], workerGroupTf);
            ins.transform.position = GM._.mm.homeTf.position;
            WorkerCnt++;
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            var ins = Instantiate(goblinPrefs[4], workerGroupTf);
            ins.transform.position = GM._.mm.homeTf.position;
            WorkerCnt++;
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            var ins = Instantiate(goblinPrefs[5], workerGroupTf);
            ins.transform.position = GM._.mm.homeTf.position;
            WorkerCnt++;
        }
    }
}
