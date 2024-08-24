using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FameManager : MonoBehaviour
{
    const int MAX_FAME_LV = 20;

    public GameObject windowObj;
    public Slider fameExpSlider;
    public TMP_Text fameLvTxt;
    public TMP_Text fameExpTxt;

    public int fameLv;
    public int fameExp;
    public int fameMaxExp;

    [Header("미션 데이터")]
    public MissionFormat m_MiningOreCnt; // 광석채굴
    public MissionFormat m_MiningTime; // 채굴시간
    public MissionFormat m_UpgradeCnt; // 강화하기
    public MissionFormat m_StageClearCnt; // 광산 클리어
    public MissionFormat m_MiningChestCnt; // 보물상자 채굴
    public MissionFormat m_ChallengeClearCnt; // 시련의광산 돌파

    [Header("미션 UI")]
    public MissionUIFormat m_MiningOreCntUI; // 광석채굴
    public MissionUIFormat m_MiningTimeUI; // 채굴시간
    public MissionUIFormat m_UpgradeCntUI; // 강화하기
    public MissionUIFormat m_StageClearCntUI; // 광산 클리어
    public MissionUIFormat m_MiningChestCntUI; // 보물상자 채굴
    public MissionUIFormat m_ChallengeClearCntUI; // 시련의광산 돌파

    void Start() {
        // 데이터 로드
        m_MiningOreCnt = new MissionFormat( Enum.MISSION.MINING_ORE_CNT, Lv: 1, Exp: 0, MaxExp: 10 );
        m_MiningTime = new MissionFormat( Enum.MISSION.MINING_TIME, Lv: 1, Exp: 0, MaxExp: 10 );
        m_UpgradeCnt = new MissionFormat( Enum.MISSION.UPGRADE_CNT, Lv: 1, Exp: 0, MaxExp: 10 );
        m_StageClearCnt = new MissionFormat( Enum.MISSION.STAGE_CLEAR_CNT, Lv: 1, Exp: 0, MaxExp: 10 );
        m_MiningChestCnt = new MissionFormat( Enum.MISSION.MINING_CHEST_CNT, Lv: 1, Exp: 0, MaxExp: 10 );
        m_ChallengeClearCnt = new MissionFormat( Enum.MISSION.CHALLENGE_CLEAR_CNT, Lv: 1, Exp: 0, MaxExp: 10 );

        // 데이터
        UpdateFameNeedExp();
        m_MiningOreCnt.UpdateNeedExp();
        m_MiningTime.UpdateNeedExp();
        m_UpgradeCnt.UpdateNeedExp();
        m_StageClearCnt.UpdateNeedExp();
        m_MiningChestCnt.UpdateNeedExp();
        m_ChallengeClearCnt.UpdateNeedExp();

        // UI
        UpdateFameUI();
        m_MiningOreCntUI.UpdateUI(m_MiningOreCnt);
        m_MiningTimeUI.UpdateUI(m_MiningTime);
        m_UpgradeCntUI.UpdateUI(m_UpgradeCnt);
        m_StageClearCntUI.UpdateUI(m_StageClearCnt);
        m_MiningChestCntUI.UpdateUI(m_MiningChestCnt);
        m_ChallengeClearCntUI.UpdateUI(m_ChallengeClearCnt);
    }

#region FUNC
    /// <summary>
    /// 명예레벨 필요경험치 ( MAX 20LV )
    /// </summary>
    private void UpdateFameNeedExp() {
        fameMaxExp = 10 + (fameLv * (fameLv - 1) * 10) / 2;
    }
    /// <summary>
    /// 명예레벨 필요경험치 UI ( MAX 20LV )
    /// </summary>
    private void UpdateFameUI() {
        fameExpSlider.value = (float)fameExp / fameMaxExp;
        fameLvTxt.text = $"{fameLv}";
        fameExpTxt.text = $"{fameExp} / {fameMaxExp}";
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
