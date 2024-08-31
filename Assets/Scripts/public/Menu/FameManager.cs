using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

public class FameManager : MonoBehaviour
{
    const int MAX_FAME_LV = 20;

    public GameObject windowObj;
    public Slider fameExpSlider;
    public TMP_Text fameLvTxt;
    public TMP_Text fameExpTxt;

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

        GM._.rwm.ShowReward(missionArr[idx].Reward);
        UpdateAll();
    }
#endregion

#region FUNC
    /// <summary>
    /// 현재 명예 및 미션 데이터와 UI 업데이트
    /// </summary>
    private void UpdateAll()
    {
        // 명예 데이터 업데이트
        UpdateFameNeedExp();

        // 미션 데이터 업데이트
        for(int i = 0; i < missionArr.Length; i++)
            missionArr[i].UpdateData();

        // 명예 UI 업데이트
        UpdateFameUI();

        // 미션 UI 업데이트
        for(int i = 0; i < missionUIArr.Length; i++)
            missionUIArr[i].UpdateUI(missionArr[i]);
    }
    /// <summary>
    /// 명예레벨 필요경험치 ( MAX 20LV )
    /// </summary>
    private void UpdateFameNeedExp() {
        fameMaxExp = 10 + (fameLv * (fameLv - 1) * 10) / 2;

        if(FameExp >= fameMaxExp)
        {
            // 업데이트
            FameExp = 0;
            fameLv++;
            fameMaxExp = 10 + (fameLv * (fameLv - 1) * 10) / 2;

            GM._.ui.ShowNoticeMsgPopUp("명성 레벨업!");
            FameLevelUp();
        }
    }

    /// <summary>
    /// 명예레벨 필요경험치 UI ( MAX 20LV )
    /// </summary>
    private void UpdateFameUI() {
        fameExpSlider.value = (float)FameExp / fameMaxExp;
        fameLvTxt.text = $"{fameLv}";
        fameExpTxt.text = $"{FameExp} / {fameMaxExp}";
    }

    /// <summary>
    /// 명성레벨업 보상지급
    /// </summary>
    private void FameLevelUp() {
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
    /// 명성에 따른 캐릭터 랜덤등급 배열 반환 (MAX 20LV)
    /// </summary>
    /// <returns>[일반 , 고급 , 희귀 , 유니크 , 전설 , 신화]</returns>
    public int[] GetRandomGradeArrByFame(bool isNextLv = false) {
        int lv = isNextLv? fameLv + 1 : fameLv;
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
        }

        return null; // 해당 레벨이 아닌경우 에러: null반환
    }
#endregion
}
