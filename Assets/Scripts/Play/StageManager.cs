using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using static Enum;
using Random = UnityEngine.Random;

public class StageManager : MonoBehaviour {
    [Header("DOT ANIM")]
    public DOTweenAnimation cutOutMaskUIDOTAnim;
    public DOTweenAnimation stageTitleUIDOTAnim;
    public TMP_Text stageTitleTxt;

    [Header("TOP")]
    public TMP_Text stageTxt;

    //* Ore Object
    [Header("보물상자 및 광석 Prefs")]
    public GameObject treasureChestPref;
    public GameObject[] orePrefs;



    [Header("광석생성 바깥영역")]
    public Transform outOreAreaTopLeftTf;
    public Transform outOreAreaBottomRightTf;
    [Header("광석생성 안영역")]
    public Transform inOreAreaTopLeftTf;
    public Transform inOreAreaBottomRightTf;

    //* Value
    [Header("생성시 보물상자로 랜덤변경 확률%")]
    [Range(1, 1000)] public int treasureChestSpawnDefPer;

    [field:SerializeField] RSC oreType;  public RSC OreType {
        get => oreType;
        set => oreType = value;
    }

    // 시련의광산 모드
    public bool IsChallengeMode {get => oreType == RSC.CRISTAL;}

    [field:SerializeField] int floor;  public int Floor {
        get => floor;
        set {
            floor = value;
            stageTxt.text = GetStageName();
        }
    }

    [Header("광석 배치영역 광석 사이 간격")]
    [field:SerializeField] int oreAreaInterval;         // (작을수록 더 많은 위치리스트 생성)

    [field:SerializeField] List<Vector2> orePosList = new List<Vector2>();

    [field:SerializeField] int oreHp;                   // 스테이지별 적용할 광석 JP
    [field:SerializeField] int oreCnt;                  // 스테이지별 적용할 광석 수

    void Start() {
        cutOutMaskUIDOTAnim.gameObject.SetActive(true);
        stageTitleUIDOTAnim.gameObject.SetActive(true);
    }

#region FUNC
    /// <summary>
    /// 선택한 스테이지 시작
    /// </summary>
    public void StartStage()
    {
        Debug.Log("StartStage()::");
        GM._.gameState = GameState.PLAY;
        Floor = IsChallengeMode? GM._.clm.BestFloor : 1;

        // 스테이지층
        stageTxt.text = GetStageName();

        // 타이머 카운트다운 시작
        GM._.pm.StartCowndownTimer();

        // 게임결과 획득한 재화배열 초기화
        GM._.pm.playResRwdArr = new int[GetEnumRWDLenght()];

        // 스테이지 타이틀UI 애니메이션
        stageTitleTxt.text =  IsChallengeMode? "시련의광산" : $"제{(int)oreType + 1} 광산";
        stageTitleUIDOTAnim.DORestart();

        // 광석 오브젝트 생성
        StartCoroutine(CoUpdateAndCreateOre(oreAreaInterval));
    }

    /// <summary>
    /// 다음 스테이지 이동
    /// </summary>
    public IEnumerator CoNextStage()
    {
        Floor++;

        // 2층 이동확률% 적용 (소수점 3자리까지)
        int skipFloorPer = Mathf.RoundToInt(GM._.sttm.ExtraNextSkipPer * 1000);
        int randPer = Random.Range(0, 1000);
        Debug.Log($"2층 이동확률%:: skipFloorPer(randPer({randPer}) <= {skipFloorPer})");

        if(randPer <= skipFloorPer)
        {
            Floor++; // 현재층 스킵
            GM._.ui.ShowNoticeMsgPopUp($"2층 이동확률 발동! 다음층으로 바로 이동합니다. 랜덤%({randPer}) <= 스킵%({skipFloorPer})");
        }

        yield return Util.TIME0_5;

        cutOutMaskUIDOTAnim.DORestart();
        yield return CoUpdateAndCreateOre(oreAreaInterval);
        yield return Util.TIME1;

        // 고블린 채광이동 시작!
        Transform workerGroup = GM._.mnm.workerGroupTf;
        for(int i = 0; i < workerGroup.childCount; i++)
        {
            yield return Util.TIME0_1; // 이동시 겹치지 않게 대기
            var worker = workerGroup.GetChild(i).GetComponent<MiningController>();
            StartCoroutine(worker.CoInitStatus());
        }
    }

    /// <summary>
    /// 캐릭터 가챠뽑기 UI준비
    /// </summary>
    /// <param name="closeWindowObj">비표시할 이전 팝업</param>
    public void SetGachaUI(GameObject closeWindowObj)
    {
        // 이전 팝업 닫기
        closeWindowObj.SetActive(false);
        GM._.ui.topRscGroup.SetActive(false);
        GM._.hm.HomeWindow.SetActive(false);

        // 게임시작전 보이는 텍스트 공백으로 정리
        GM._.pm.timerTxt.text = "";
        GM._.stgm.stageTxt.text = "캐릭터를 뽑아주세요!";

        // 캐릭터 고용(소환) 팝업 열기
        GM._.epm.ShowPopUp();
    }

    private string GetStageName()
    {
        if(IsChallengeMode)
            return $"시련의광산{floor}층";
        else
            return $"{floor}층";
    }

    /// <summary>
    /// 광석 오브젝트 생성
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    IEnumerator CoUpdateAndCreateOre(int interval)
    {
        // CutOutMask UI 애니메이션 대기
        yield return GM._.epm.waitCreateCharaSec;
        yield return GM._.mnm.waitSpawnToGoSec;

        // RESET : 모든 광석 오브젝트 삭제
        for(int i = 0; i < GM._.mnm.oreGroupTf.childCount; i++)
            Destroy(GM._.mnm.oreGroupTf.GetChild(i).gameObject);

        UpdateOreValueByStage();
        InitOrePosList(interval);
        CreateOres(oreHp, oreCnt);
    }

    /// <summary>
    /// 스테이지에 따른 광석 적용값 업데이트
    /// </summary>
    private void UpdateOreValueByStage()
    {
        int hp = 100;

        // 101층부터 체력 100씩 증가
        if(floor > 100)
            hp += (floor - 100) * 100; 

        // 광석 체력
        oreHp = hp + ((floor-1) * 100);

        // 광석 숫자
        if(floor < 100)
            oreCnt = floor;
        else
            oreCnt = 100;
    }
    
    /// <summary>
    /// 위치 리스트 초기화
    /// </summary>
    private void InitOrePosList(int interval)
    {
        Vector2 outTLPos = outOreAreaTopLeftTf.position;
        Vector2 outBRPos = outOreAreaBottomRightTf.position;
        Vector2 inTLPos = inOreAreaTopLeftTf.position;
        Vector2 inBRPos = inOreAreaBottomRightTf.position;


        for (float x = outTLPos.x; x <= outBRPos.x; x += interval)
        {
            for (float y = outTLPos.y; y >= outBRPos.y; y -= interval * 1.5f)
            {
                if (!(x >= inTLPos.x && x <= inBRPos.x && y <= inTLPos.y && y >= inBRPos.y))
                {
                    orePosList.Add(new Vector2(x, y));
                }
            }
        }
    }

    /// <summary>
    /// 광석 생성
    /// </summary>
    private void CreateOres(int oreHp, int oreCnt)
    {
        GameObject obj = null;

        // 생성개수가 리스트보다 많다면 리스트 최대치로 수정
        if(oreCnt > orePosList.Count)
            oreCnt = orePosList.Count;

        // 광석 생성 (랜덤위치)
        for(int i = 0; i < oreCnt; i++) {
            int rand = Random.Range(0, orePosList.Count);

            //* 시련의광산
            if(IsChallengeMode)
            {
                // 모든 광석 랜덤 (보물상자 변경 X)
                int randOreIdx = Random.Range(0, orePrefs.Length);
                obj = orePrefs[randOreIdx];
            }
            //* 일반광산
            else
            {
                // 낮은확률 광석 -> 보물상자로 랜덤변경
                int randPer = Random.Range(0, 1000);
                int spawnPer = treasureChestSpawnDefPer + Mathf.RoundToInt(GM._.sttm.ExtraChestSpawnPer * 1000);
                obj = (randPer <= spawnPer)? treasureChestPref : orePrefs[(int)oreType];
                Debug.Log($"CreateOres():: 보물상자 랜덤변경: randPer({randPer}) <= sapwnPer({spawnPer})");
            }

            // 생성
            Ore ore = Instantiate(obj, GM._.mnm.oreGroupTf).GetComponent<Ore>();
            ore.transform.position = orePosList[rand]; // 랜덤위치 적용
            ore.MaxHp = oreHp;

            // 리스트 제거
            orePosList.RemoveAt(rand);
        }

        // (TEST) 광석 모두 생성해보기
        // for(int i = 0; i < orePosList.Count; i++) {
        //     // 생성
        //     obj = orePrefs[0];
        //     Ore ore = Instantiate(obj, GM._.mnm.oreGroupTf).GetComponent<Ore>();
        //     ore.transform.position = orePosList[i]; // 랜덤위치 적용
        //     ore.MaxHp = oreHp;
        // }
    }
#endregion
}
