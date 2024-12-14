using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

public class FameManager : MonoBehaviour
{
    const int FAME_MAXLV = 30;

    [Header("버튼 수령가능한지에 따른 색깔 스프라이트")]
    public Sprite grayBtnSpr;
    public Sprite yellowBtnSpr;

    [Space(15)]

    public GameObject windowObj;
    public GameObject alertRedDotObj;
    public Slider fameExpSlider;
    public TMP_Text fameLvTxt;
    public TMP_Text fameExpTxt;

    [Header("명예 레벨업 정보 패널 : 캐릭터 고용 랜덤테이블 확인용으로도 사용")]
    public GameObject fameLevelUpInfoPanel;
    public TMP_Text fameLevelUpInfoPanelTitleTxt;
    public TMP_Text fameLevelUpInfoPanelLvTxt;
    public TMP_Text employRandomTableValTxt;
    public TMP_Text allAutoMiningStorageMultiplyValTxt; // 명예 레벨에따른 모든 자동채굴보관량 곱하기값 텍스트

    public Toggle nextLvInfoToogleHandle;

    public int FameLv {
        get => DM._.DB.statusDB.FameLv;
        set => DM._.DB.statusDB.FameLv = value;
    }
    public int FameExp { 
        get => DM._.DB.statusDB.Fame;
        set => DM._.DB.statusDB.Fame = value;
    }
    public int fameMaxExp {
        get => CalcFameMaxExp(FameLv);
    }

    [field:Header("미션: 광석채굴, 채굴시간, 강화하기, 광산 클리어, 보물상자 채굴, 시련의광산 돌파")]

    [Header("미션 데이터 : Start()함수에서 초기화")]
    public MissionFormat[] missionArr;

    [Header("미션 UI")]
    public MissionUIFormat[] missionUIArr;

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        var svDts = DM._.DB.missionDB.saveDts;

        // 객체생성 및 데이터 로드 (MapExp는 UpdateAll에서 계산식으로 대입)
        missionArr = new MissionFormat[6] {
            new() { Type = svDts[0].Type, Lv = svDts[0].Lv, Exp = svDts[0].Exp, MaxExp = 0 },
            new() { Type = svDts[1].Type, Lv = svDts[1].Lv, Exp = svDts[1].Exp, MaxExp = 0 },
            new() { Type = svDts[2].Type, Lv = svDts[2].Lv, Exp = svDts[2].Exp, MaxExp = 0 },
            new() { Type = svDts[3].Type, Lv = svDts[3].Lv, Exp = svDts[3].Exp, MaxExp = 0 },
            new() { Type = svDts[4].Type, Lv = svDts[4].Lv, Exp = svDts[4].Exp, MaxExp = 0 },
            new() { Type = svDts[5].Type, Lv = svDts[5].Lv, Exp = svDts[5].Exp, MaxExp = 0 },
        };

        // UpdateAll();
        StartCoroutine(CoUpdateAllForSecond());
    }

    void Update()
    {
        //! TEST 미션 EXP 증가
        if(GM._.stm.testMode.activeSelf && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("TEST 미션 EXP 증가");
            missionArr[(int)MISSION.MINING_ORE_CNT].Exp += 50;
            missionArr[(int)MISSION.MINING_TIME].Exp += 50;
            missionArr[(int)MISSION.UPGRADE_CNT].Exp += 10;
            missionArr[(int)MISSION.STAGE_CLEAR_CNT].Exp += 100;
            missionArr[(int)MISSION.MINING_CHEST_CNT].Exp += 10;
            missionArr[(int)MISSION.CHALLENGE_CLEAR_CNT].Exp += 10;
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
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.MissionStillMsg));
            return;
        }

        SoundManager._.PlaySfx(SoundManager.SFX.FameCompleteSFX);

        // 미션 레벨 업
        mission.Lv++;

        // 미션 보상
        GM._.rwm.ShowReward(missionArr[idx].Reward);

        // 업데이트
        UpdateAll();

        // 업데이트 알림UI 🔴
        UpdateAlertRedDot();
    }

    /// <summary>
    /// 현재레벨 명예팝업 표시용 (i)정보버튼
    /// </summary>
    public void OnClickFameInfoIconBtn()
    {
        SoundManager._.PlaySfx(SoundManager.SFX.ItemDrop1SFX);
        SetFameLevelToogleUI(isOn: false);
        ShowFameLevelUpGradeTable(isLvUp: false);
    }

    /// <summary>
    /// 명예정보표시 토글버튼
    /// </summary>
    public void OnClickNextLvInfoToogleHandle()
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap2SFX);

        // 토글 UI
        SetFameLevelToogleUI(nextLvInfoToogleHandle.isOn);

        // 토글버튼에 따른 팝업 표시
        if(nextLvInfoToogleHandle.isOn)
        {
            ShowFameNextLevelUpGradeTable();
        }
        else
        {
            ShowFameLevelUpGradeTable(false);
        }
    }
#endregion

#region FUNC
    /// <summary>
    /// 게임진행시, 1초마다 채굴미션 증가 및 전체 데이터 업데이트
    /// </summary>
    /// <returns></returns>
    IEnumerator CoUpdateAllForSecond()
    {
        while(true)
        {
            if(GM._.gameState == GameState.PLAY)
            {
                Debug.Log("CoUpdateAllForSecond():: gameState=PLAY, 업데이트 명예 데이터 및 UI");
                // 채굴시간 미션
                GM._.fm.missionArr[(int)MISSION.MINING_TIME].Exp++;

                UpdateAll();
            }
            yield return Util.TIME1;
        }
    }

    /// <summary>
    /// 현재 명예 및 미션 데이터와 UI 업데이트
    /// </summary>
    public void UpdateAll()
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
        {
            missionUIArr[i].UpdateUI(missionArr[i]);
        }
    }

    /// <summary>
    /// 명예레벨 필요경험치 ( MAX 20LV )
    /// </summary>
    public void UpdateFameMapExp()
    {
        // 명예 레벨업인 경우
        if(FameExp >= fameMaxExp && FameLv < FAME_MAXLV)
        {
            // 업데이트
            FameExp = FameExp - fameMaxExp; // 레벨업후 남은량 적용
            FameLv++;
            GM._.spm.UpdateUI();

            GM._.ui.ShowNoticeMsgPopUp($"{LM._.Localize(LM.Fame)} {LM._.Localize(LM.Lv)} UP!");
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
    /// 명예경험치 슬라이더 값
    /// </summary>
    public float GetFameExpSliderVal() {
        // MAX 레벨이라면
        if(FameLv >= FAME_MAXLV)
            return 1; // FULL

        return (float)FameExp / fameMaxExp;
    } 

    /// <summary>
    /// 명예경험치 슬라이더 문자
    /// </summary>
    /// <returns></returns>
    public string GetFameExpSliderStr() {
        // MAX 레벨이라면
        if(FameLv >= FAME_MAXLV)
            return "MAX";

        return $"{FameExp} / {fameMaxExp}";
    } 

    /// <summary>
    /// 명예레벨 필요경험치 UI ( MAX 20LV )
    /// </summary>
    private void UpdateFameUI()
    {
        fameLvTxt.text = FameLv.ToString();
        fameExpSlider.value = GetFameExpSliderVal();
        fameExpTxt.text = GetFameExpSliderStr();
    }

    /// <summary>
    /// 명예레벨업 보상지급
    /// </summary>
    private void FameLevelUp()
    {
        SoundManager._.PlaySfx(SoundManager.SFX.FameLvUpSFX);

        ShowFameLevelUpGradeTable(isLvUp: true);

        GM._.rwm.ShowReward (
            new Dictionary<RWD, int> {
                { RWD.ORE_CHEST,      (FameLv - 1) * 5 },
                { RWD.TREASURE_CHEST, (FameLv - 1) * 2 },
                { RWD.ORE_TICKET,     (FameLv - 1) * 5 },
                { RWD.RED_TICKET,     (FameLv - 1) * 2 },
                { RWD.CRISTAL,        (FameLv - 1) * 10 },
            }
        );
    }

    /// <summary>
    /// 명예 레벨업 팝업 표시 (레벨업이 아닐경우, 현재 소환등급 표시용)
    /// </summary>
    /// <param name="isLvUp">레벨업인지 아닌지</param>
    private void ShowFameLevelUpGradeTable(bool isLvUp) {
        // 팝업 표시
        fameLevelUpInfoPanel.SetActive(true);
        // 타이틀
        fameLevelUpInfoPanelTitleTxt.text = isLvUp? $"{LM._.Localize(LM.Fame)} {LM._.Localize(LM.Lv)} UP!" : $"{LM._.Localize(LM.Fame)} {LM._.Localize(LM.Lv)}";
        // 레벨 텍스트
        fameLevelUpInfoPanelLvTxt.text = FameLv.ToString();

        // 모든 자동채굴보관량 곱하기값
        allAutoMiningStorageMultiplyValTxt.text = $"x {CalcAllAutoMiningStorageMultiVal(FameLv)}";

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
    /// 모든 자동채굴보관량 곱하기값 계산
    /// </summary>
    public int CalcAllAutoMiningStorageMultiVal(int fameLv)
    {
        return (int)Mathf.Pow(2, fameLv / 2);
    }

    /// <summary>
    /// 명예레벨업 토글UI 업데이트
    /// </summary>
    /// <param name="isOn"></param>
    public void SetFameLevelToogleUI(bool isOn)
    {
        nextLvInfoToogleHandle.isOn = isOn;
        var toggleTxt = nextLvInfoToogleHandle.GetComponentInChildren<TMP_Text>();

        toggleTxt.text = isOn? "ON" : "OFF";
        nextLvInfoToogleHandle.GetComponent<Image>().color = isOn? Color.green : Color.white;
    }

    /// <summary>
    /// 다음레벨 명예팝업 표시
    /// </summary>
    public void ShowFameNextLevelUpGradeTable() {
        // 팝업 표시
        fameLevelUpInfoPanel.SetActive(true);
        // 타이틀
        fameLevelUpInfoPanelTitleTxt.text = LM._.Localize(LM.NextFameLvGradeTable);
        // 레벨 텍스트
        fameLevelUpInfoPanelLvTxt.text = (FameLv + 1).ToString();

        // 모든 자동채굴보관량 곱하기값
        allAutoMiningStorageMultiplyValTxt.text = $"x {CalcAllAutoMiningStorageMultiVal(FameLv + 1)}";

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
    /// 명예에 따른 캐릭터 랜덤등급 배열 반환 (MAX 20LV)
    /// </summary>
    /// <returns>[일반 , 고급 , 희귀 , 유니크 , 전설 , 신화]</returns>
    public int[] GetRandomGradeArrByFame(int extraLv = 0) {
        int lv = FameLv + extraLv;
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

    /// <summary>
    /// 수령가능한 버튼이 있다면, 알림아이콘UI 🔴표시
    /// </summary>
    public void UpdateAlertRedDot()
    {
        Debug.Log($"명예 미션:: UpdateAlertRedDot()::");
        bool isAcceptable = Array.Exists(missionArr, msi => msi.Exp >= msi.MaxExp);
        alertRedDotObj.SetActive(isAcceptable);
    }
#endregion
}
