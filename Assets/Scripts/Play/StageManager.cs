using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {
    [Header("TOP")]
    public Image curRscIconImg;
    public TMP_Text curRscCntTxt;
    public TMP_Text timerTxt;
    public TMP_Text stageTxt;

    //* Ore Object
    public GameObject[] orePrefs;

    public Transform oreAreaTopLeftTf;
    public Transform oreAreaBottomRightTf;
    private Vector2 topLeftPos;
    private Vector2 bottomRightPos;

    public DOTweenAnimation cutOutMaskUIDOTAnim;

    //* Value

    [field:SerializeField] int timerVal;  public int TimerVal {
        get => timerVal;
        set {
            timerVal = value;

            int min = timerVal / 60;
            int sec = timerVal % 60;
            timerTxt.text = $"{min:00} : {sec:00}";
        }
    }

    [field:SerializeField] Enum.RSC oreType;  public Enum.RSC OreType {
        get => oreType;
        set => oreType = value;
    }

    [field:SerializeField] int floor;  public int Floor {
        get => floor;
        set {
            floor = value;
            stageTxt.text = $"{(int)oreType + 1}광산 {floor}층";
        }
    }

    [field:SerializeField] int oreAreaInterval;       // 광석 배치영역 광석 사이 간격 (작을수록 더 많은 위치리스트 생성)
    [field:SerializeField] List<Vector2> orePosList = new List<Vector2>();

    [field:SerializeField] int oreHp;                   // 스테이지별 적용할 광석 JP
    [field:SerializeField] int oreCnt;                  // 스테이지별 적용할 광석 수

    /// <summary>
    /// 선택한 스테이지 시작
    /// </summary>
    public void StartStage() {
        Debug.Log("StartStage()::");
        GM._.gameState = GameState.PLAY;
        Floor = 1;

        // 선택한 광산타입으로 초기화
        curRscIconImg.sprite = GM._.RscSprArr[(int)oreType]; // 재화 아이콘
        curRscCntTxt.text = $"{DM._.DB.statusDB.RscArr[(int)oreType]}"; // 재화수량

        // 타이머 초기화 및 카운트다운 시작
        TimerVal = GM._.ugm.upgIncTimer.Val;
        StartCoroutine(CoCownDownTimer());

        // 광석 생성 영역
        topLeftPos = oreAreaTopLeftTf.position;
        bottomRightPos = oreAreaBottomRightTf.position;

        // 컷아웃 마스크 UI효과
        cutOutMaskUIDOTAnim.DOPlay();

        // 광석 오브젝트 생성
        StartCoroutine(CoUpdateAndCreateOre(oreAreaInterval));
    }

    /// <summary>
    /// 다음 스테이지 이동
    /// </summary>
    public IEnumerator CoNextStage() {
        Floor++;
        yield return Util.TIME0_5;

        cutOutMaskUIDOTAnim.DORestart();
        yield return CoUpdateAndCreateOre(oreAreaInterval);
        yield return Util.TIME1;

        // 고블린 채광이동 시작!
        Transform workerGroup = GM._.mnm.workerGroupTf;
        for(int i = 0; i < workerGroup.childCount; i++)
        {
            yield return Util.TIME0_2; // 이동시 겹치지 않게 0.2초씩 대기
            var worker = workerGroup.GetChild(i).GetComponent<MiningController>();
            StartCoroutine(worker.CoInitStatus());
        }
    }

    /// <summary>
    /// 시간 카운트 다운
    /// </summary>
    IEnumerator CoCownDownTimer() {
        while(timerVal > 0) {
            yield return Util.TIME1;
            TimerVal -= 1;
        }

        GM._.gameState = GameState.GAMEOVER;
        timerTxt.text = GameState.GAMEOVER.ToString();
    }

    IEnumerator CoUpdateAndCreateOre(int interval) {
        yield return Util.TIME0_5;
        // RESET : 모든 광석 오브젝트 삭제
        for(int i = 0; i < GM._.mnm.oreGroupTf.childCount; i++) {
            Destroy(GM._.mnm.oreGroupTf.GetChild(i).gameObject);
        }

        UpdateOreValueByStage();
        InitOrePosList(interval);
        CreateOres(oreHp, oreCnt);
    }

    /// <summary>
    /// 스테이지에 따른 광석 적용값 업데이트
    /// </summary>
    private void UpdateOreValueByStage() {
        const int DEF_HP = 1000;
        oreHp = DEF_HP + ((floor-1) * 100);
        oreCnt = (floor + 10) / 10;
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
    private void CreateOres(int oreHp, int oreCnt) {
        // 생성개수가 리스트보다 많다면 리스트 최대치로 수정
        if(oreCnt > orePosList.Count)
            oreCnt = orePosList.Count;

        // 광석 생성 (랜덤위치)
        for(int i = 0; i < oreCnt; i++) {
            int rand = Random.Range(0, orePosList.Count);

            // 생성
            Ore ore = Instantiate(orePrefs[(int)oreType], GM._.mnm.oreGroupTf).GetComponent<Ore>();
            ore.transform.position = orePosList[rand]; // 랜덤위치 적용
            ore.MaxHp = oreHp;

            // 리스트 제거
            orePosList.RemoveAt(rand);
        }
    }
}
