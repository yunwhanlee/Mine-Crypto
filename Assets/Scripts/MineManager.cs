using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManager : MonoBehaviour
{
    //* Object
    public GameObject[] goblinPrefs;
    public Transform homeTf;
    public Transform oreGroupTf;
    public Transform workerGroupTf;
    public List<GameObject> rockList;

    //* Value
    public int CurTotalMiningCnt; // 타겟광석을 순차적 배치하기위한 전체타겟 카운팅

    [field:SerializeField] public int Coin {
        get => DM._.DB.statusDB.Coin;
        set {
            DM._.DB.statusDB.Coin = value;
            GM._.ui.coinTxt.text = DM._.DB.statusDB.Coin.ToString();
        }
    }
}
