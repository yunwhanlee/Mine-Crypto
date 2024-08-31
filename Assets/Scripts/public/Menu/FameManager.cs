using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

public class FameManager : MonoBehaviour
{
    const int FAME_MAXLV = 2;

    [Header("버튼 수령가능한지에 따른 색깔 스프라이트")]
    public Sprite grayBtnSpr;
    public Sprite yellowBtnSpr;

    [Space(15)]

    public GameObject windowObj;
    public Slider fameExpSlider;
    public TMP_Text fameLvTxt;
    public TMP_Text fameExpTxt;

    [Header("명성 레벨업 정보 패널 : 캐릭터 고용 랜덤테이블 확인용으로도 사용")]
    public GameObject fameLevelUpInfoPanel;
    public TMP_Text fameLevelUpInfoPanelTitleTxt;
    public TMP_Text fameLevelUpInfoPanelLvTxt;
    public TMP_Text employRandomTableValTxt;

    public int fameLv;
    public int FameExp { 
        get => DM._.DB.statusDB.Fame;
        set => DM._.DB.statusDB.Fame = value;
    }
    public int fameMaxExp;

    [field:Header("미션: 광석채굴, 채굴시간, 강화하기, 광산 클리어, 보물상자 채굴, 시련의광산 돌파")]

    [Header("미션 데이터 : Start()함수에서 초기화")]
    public MissionFormat[] missionArr;

    [Header("미션 UI")]
    public MissionUIFormat[] missionUIArr;

    void Start() {
        // 데이터 로드
        missionArr = new MissionFormat[6] {
            new() { Type = MISSION.MINING_ORE_CNT, Lv = 1, Exp = 0, MaxExp = 10 },
            new() { Type = MISSION.MINING_TIME, Lv = 1, Exp = 0, MaxExp = 10 },
            new() { Type = MISSION.UPGRADE_CNT, Lv = 1, Exp = 0, MaxExp = 10 },
            new() { Type = MISSION.STAGE_CLEAR_CNT, Lv = 1, Exp = 0, MaxExp = 10 },
            new() { Type = MISSION.MINING_CHEST_CNT, Lv = 1, Exp = 0, MaxExp = 10 },
            new() { Type = MISSION.CHALLENGE_CLEAR_CNT, Lv = 1, Exp = 0, MaxExp = 10 },
        };

        UpdateAll();
    }

    void Update() {
        //! TEST 미션 EXP 증가
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("TEST 미션 EXP 증가");
            DM._.DB.missionDB.MiningOreCnt += 50;
            DM._.DB.missionDB.MiningTime += 50;
            DM._.DB.missionDB.UpgradeCnt += 10;
            DM._.DB.missionDB.StageClearCnt += 100;
            DM._.DB.missionDB.MiningChestCnt += 10;
            DM._.DB.missionDB.ChallengeClearCnt += 10;

            UpdateAll();
        }
    }

#region EVENT
    /// <summary>
    /// 미션 보상획득 버튼
    /// </summary>
    /// <param name="idx"></param>
    public void OnClickAcceptRewardBtn(int idx) {
        var mission = missionArr[idx];

        if(mission.Exp < mission.MaxExp)
        {
            GM._.ui.ShowWarningMsgPopUp("미션을 아직 수행중입니다!");
            return;
        }

        // 미션 레벨 업
        mission.Lv++;

        // 미션 보상
        GM._.rwm.ShowReward(missionArr[idx].Reward);

        // 업데이트
        UpdateAll();
    }

    /// <summary>
    /// 현재레벨 명성팝업 표시용 (i)정보버튼
    /// </summary>
    public void OnClickFameInfoIconBtn()
    {
        ShowFameLevelUpGradeTable(isLvUp: false);
    }
#endregion

#region FUNC
    /// <summary>
    /// 현재 명예 및 미션 데이터와 UI 업데이트
    /// </summary>
    private void UpdateAll()
    {
        // 명예 데이터 업데이트
        UpdateFameMapExp();

        // 명예 UI 업데이트
        UpdateFameUI();

        // 미션 데이터 업데이트
        for(int i = 0; i < missionArr.Length; i++)
            missionArr[i].UpdateData();

        // 미션 UI 업데이트
        for(int i = 0; i < missionUIArr.Length; i++)
            missionUIArr[i].UpdateUI(missionArr[i]);
    }

    /// <summary>
    /// 명예레벨 필요경험치 ( MAX 20LV )
    /// </summary>
    private void UpdateFameMapExp()
    {
        fameMaxExp = CalcFameMaxExp(fameLv);

        // 명성 레벨업인 경우
        if(FameExp >= fameMaxExp && fameLv < FAME_MAXLV)
        {
            // 업데이트
            fameLv++;
            FameExp = 0;
            fameMaxExp = CalcFameMaxExp(fameLv);

            GM._.ui.ShowNoticeMsgPopUp("명성 레벨업!");
            FameLevelUp();
        }
    }

    /// <summary>
    /// 명예레벨 필요경험치 계산식
    /// </summary>
    private int CalcFameMaxExp(int fameLv)
    {
        return 10 + fameLv * (fameLv - 1) * 10 / 2;
    }

    /// <summary>
    /// 명예레벨 필요경험치 UI ( MAX 20LV )
    /// </summary>
    private void UpdateFameUI()
    {
        fameLvTxt.text = fameLv.ToString();
        fameExpSlider.value = (float)FameExp / fameMaxExp;
        fameExpTxt.text = $"{FameExp} / {fameMaxExp}";

        // MAX 레벨이라면
        if(fameLv >= FAME_MAXLV)
        {
            fameExpSlider.value = 1; // FULL
            fameExpTxt.text = "MAX";
        }
    }

    /// <summary>
    /// 명성레벨업 보상지급
    /// </summary>
    private void FameLevelUp()
    {
        ShowFameLevelUpGradeTable(isLvUp: true);

        GM._.rwm.ShowReward (
            new Dictionary<RWD, int> {
                { RWD.ORE_CHEST,      (fameLv - 1) * 5 },
                { RWD.TREASURE_CHEST, (fameLv - 1) * 2 },
                { RWD.ORE_TICKET,     (fameLv - 1) * 5 },
                { RWD.RED_TICKET,     (fameLv - 1) * 2 },
                { RWD.CRISTAL,        (fameLv - 1) * 10 },
            }
        );
    }



    /// <summary>
    /// 명성 레벨업 팝업 표시 (레벨업이 아닐경우, 현재 소환등급 표시용)
    /// </summary>
    /// <param name="isLvUp">레벨업인지 아닌지</param>
    private void ShowFameLevelUpGradeTable(bool isLvUp) {
        // 팝업 표시
        fameLevelUpInfoPanel.SetActive(true);
        // 타이틀
        fameLevelUpInfoPanelTitleTxt.text = isLvUp? "명성 레벨업!" : "명성 레벨";
        // 레벨 텍스트
        fameLevelUpInfoPanelLvTxt.text = fameLv.ToString();

        // 등급표 데이터
        int[] curLvGrdValTb = GM._.fm.GetRandomGradeArrByFame(); // 레벨업한 현재 레벨확률
        int[] befLvGrdValTb = GM._.fm.GetRandomGradeArrByFame(extraLv: -1); // 이전 레벨확률

        // 각 등급 문자열
        string commonGrade = $"{curLvGrdValTb[0]}%";
        string unCommonGrade = $"\n<color=green>{curLvGrdValTb[1]}%</color>";
        string rareGrade = $"\n<color=blue>{curLvGrdValTb[2]}%</color>";
        string uniqueGrade = $"\n<color=purple>{curLvGrdValTb[3]}%</color>";
        string legendGrade = $"\n<color=yellow>{curLvGrdValTb[4]}%</color>";
        string mythGrade = $"\n<color=red>{curLvGrdValTb[5]}%</color>";

        // 레벨업 경우, "<= 이전 레벨%" 우측에 표시
        if(isLvUp)
        {
            commonGrade += $"<color=grey> <= {befLvGrdValTb[0]}%</color>";
            unCommonGrade += $"<color=grey> <= {befLvGrdValTb[1]}%</color>";
            rareGrade += $"<color=grey> <= {befLvGrdValTb[2]}%</color>";
            uniqueGrade += $"<color=grey> <= {befLvGrdValTb[3]}%</color>";
            legendGrade += $"<color=grey> <= {befLvGrdValTb[4]}%</color>";
            mythGrade += $"<color=grey> <= {befLvGrdValTb[5]}%</color>";
        }

        // 등급표 작성
        employRandomTableValTxt.text = commonGrade + unCommonGrade + rareGrade + uniqueGrade + legendGrade + mythGrade;
    }

    /// <summary>
    /// 다음레벨 명성팝업 표시
    /// </summary>
    public void ShowFameNextLevelUpGradeTable() {
        // 팝업 표시
        fameLevelUpInfoPanel.SetActive(true);
        // 타이틀
        fameLevelUpInfoPanelTitleTxt.text = "다음명성 레벨등급표";
        // 레벨 텍스트
        fameLevelUpInfoPanelLvTxt.text = (fameLv + 1).ToString();

        // 등급표 데이터
        int[] curLvGrdValTb = GM._.fm.GetRandomGradeArrByFame(); // 현재 레벨확률
        int[] nextLvGrdValTb = GM._.fm.GetRandomGradeArrByFame(extraLv: +1); // 다음 레벨확률

        // 각 등급 문자열
        string commonGrade = $"<color=grey>{curLvGrdValTb[0]}%";
        string unCommonGrade = $"\n<color=grey>{curLvGrdValTb[1]}%";
        string rareGrade = $"\n<color=grey>{curLvGrdValTb[2]}%";
        string uniqueGrade = $"\n<color=grey>{curLvGrdValTb[3]}%";
        string legendGrade = $"\n<color=grey>{curLvGrdValTb[4]}%";
        string mythGrade = $"\n<color=grey>{curLvGrdValTb[5]}%";

        // 다음레벨 등급확률 문자열
        commonGrade += $" => </color>{nextLvGrdValTb[0]}%</color=grey>";
        unCommonGrade += $" => </color><color=green>{nextLvGrdValTb[1]}%</color>";
        rareGrade += $" => </color><color=blue>{nextLvGrdValTb[2]}%</color>";
        uniqueGrade += $" => </color><color=purple>{nextLvGrdValTb[3]}%</color>";
        legendGrade += $" => </color><color=yellow>{nextLvGrdValTb[4]}%</color>";
        mythGrade += $" => </color><color=red>{nextLvGrdValTb[5]}%</color>";

        // 등급표 작성
        employRandomTableValTxt.text = commonGrade + unCommonGrade + rareGrade + uniqueGrade + legendGrade + mythGrade;
    }

    /// <summary>
    /// 명성에 따른 캐릭터 랜덤등급 배열 반환 (MAX 20LV)
    /// </summary>
    /// <returns>[일반 , 고급 , 희귀 , 유니크 , 전설 , 신화]</returns>
    public int[] GetRandomGradeArrByFame(int extraLv = 0) {
        int lv = fameLv + extraLv;
        switch(lv) {
            case 1: return new int[] {70, 25, 3, 2, 0, 0};
            case 2: return new int[] {65, 30, 3, 2, 0, 0};
            case 3: return new int[] {60, 30, 6, 3, 1, 0};
            case 4: return new int[] {55, 35, 6, 3, 1, 0};
            case 5: return new int[] {50, 35, 9, 5, 1, 0};
            case 6: return new int[] {45, 40, 8, 5, 2, 0};
            case 7: return new int[] {40, 40, 12, 6, 2, 0};
            case 8: return new int[] {35, 45, 12, 6, 2, 0};
            case 9: return new int[] {30, 50, 12, 6, 2, 0};
            case 10: return new int[] {30, 50, 10, 7, 3, 0};
            case 11: return new int[] {25, 45, 15, 10, 4, 1};
            case 12: return new int[] {20, 45, 20, 10, 4, 1};
            case 13: return new int[] {15, 40, 25, 13, 6, 1};
            case 14: return new int[] {10, 40, 30, 13, 6, 1};
            case 15: return new int[] {5, 40, 35, 15, 9, 1};
            case 16: return new int[] {0, 35, 35, 20, 9, 1};
            case 17: return new int[] {0, 30, 40, 20, 9, 1};
            case 18: return new int[] {0, 20, 40, 25, 12, 3};
            case 19: return new int[] {0, 10, 30, 45, 12, 3};
            case 20: return new int[] {0, 0, 20, 60, 15, 5};
            case 21: return new int[] {0, 0, 20, 58, 17, 5};
            case 22: return new int[] {0, 0, 18, 57, 20, 5};
            case 23: return new int[] {0, 0, 18, 55, 21, 6};
            case 24: return new int[] {0, 0, 16, 55, 23, 6};
            case 25: return new int[] {0, 0, 14, 53, 26, 7};
            case 26: return new int[] {0, 0, 14, 50, 29, 7};
            case 27: return new int[] {0, 0, 12, 48, 32, 8};
            case 28: return new int[] {0, 0, 10, 45, 36, 9};
            case 29: return new int[] {0, 0, 5, 40, 45, 10};
            case 30: return new int[] {0, 0, 0, 35, 50, 15};
        }

        return null; // 해당 레벨이 아닌경우 에러: null반환
    }
#endregion
}
