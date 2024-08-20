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
    public int workerClearStageStatusCnt; // 광석을 다캐고 다음스테이지로 가기위한 카운팅
    public int CurTotalMiningCnt; // 타겟광석을 순차적 배치하기위한 전체타겟 카운팅
}
