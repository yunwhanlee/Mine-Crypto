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

    public WaitForSeconds waitSpawnToGoSec; // 캐릭터 생성후, Spawn에서 Go 상태변경 대기시간

    void Start() {
        waitSpawnToGoSec = Util.TIME0_2;
    }

    public void InitData()
    {
        // 여러개 광석에 분배위한 카운팅 데이터 초기화
        workerClearStageStatusCnt = 0;
        CurTotalMiningCnt = 0;
    }
}
