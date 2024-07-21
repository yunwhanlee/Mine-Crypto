using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {
    //* Object
    public GameObject[] orePrefs;

    public Transform oreAreaTopLeftTf;
    public Transform oreAreaBottomRightTf;
    private Vector2 topLeftPos;
    private Vector2 bottomRightPos;

    public TMP_Text stageTxt;
    public DOTweenAnimation cutOutMaskUIDOTAnim;

    //* Value
    [field:SerializeField] int stage;  public int Stage {
        get => stage;
        set {
            stage = value;
            stageTxt.text = $"광산 {stage}층";
        }
    }
    [field:SerializeField] List<Vector2> orePosList = new List<Vector2>();

    [field:SerializeField] int oreHp;       // 스테이지별 적용할 광석 JP
    [field:SerializeField] int oreCnt;      // 스테이지별 적용할 광석 수

    void Start() {
        Stage = 1;
        topLeftPos = oreAreaTopLeftTf.position;
        bottomRightPos = oreAreaBottomRightTf.position;

        cutOutMaskUIDOTAnim.DOPlay();
        StartCoroutine(CoUpdateAndCreateOre(interval: 2));
    }

    void Update() {
        //! TEST STAGE UP
        if(Input.GetKeyDown(KeyCode.A))
        {
            Stage++;
            cutOutMaskUIDOTAnim.DORestart();
            StartCoroutine(CoUpdateAndCreateOre(interval: 2));
        }
    }

    IEnumerator CoUpdateAndCreateOre(int interval) {
        yield return Util.TIME0_5;
        // RESET : 모든 광석 오브젝트 삭제
        for(int i = 0; i < GM._.mm.oreGroupTf.childCount; i++) {
            Destroy(GM._.mm.oreGroupTf.GetChild(i).gameObject);
        }

        UpdateOreValueByStage();
        InitOrePosList(interval);
        CreateOres(oreHp, oreCnt);
    }

    /// <summary>
    /// 스테이지에 따른 광석 적용값 업데이트
    /// </summary>
    private void UpdateOreValueByStage() {
        oreHp = 1000 + ((stage-1) * 100);
        oreCnt = (stage + 10) / 10;
    }
    
    /// <summary>
    /// 위치 리스트 초기화
    /// </summary>
    private void InitOrePosList(int interval) {
        for (float x = topLeftPos.x; x <= bottomRightPos.x; x += interval)
        {
            for (float y = topLeftPos.y; y >= bottomRightPos.y; y -= interval * 1.4f)
            {
                orePosList.Add(new Vector2(x, y));
            }
        }
    }

    /// <summary>
    /// 광석 생성
    /// </summary>
    public void CreateOres(int oreHp, int oreCnt) {
        // 생성개수가 리스트보다 많다면 리스트 최대치로 수정
        if(oreCnt > orePosList.Count)
            oreCnt = orePosList.Count;

        // 광석 생성 (랜덤위치)
        for(int i = 0; i < oreCnt; i++) {
            int rand = Random.Range(0, orePosList.Count);

            // 생성
            Ore ore = Instantiate(orePrefs[0], GM._.mm.oreGroupTf).GetComponent<Ore>();
            ore.transform.position = orePosList[rand]; // 랜덤위치 적용
            ore.MaxHp = oreHp;

            // 리스트 제거
            orePosList.RemoveAt(rand);
        }
    }
}
