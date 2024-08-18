using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FameManager : MonoBehaviour
{
    const int MAX_FAME_LV = 20;

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
    private void UpdateFameUI() {
        fameExpSlider.value = (float)fameExp / fameMaxExp;
        fameLvTxt.text = $"{fameLv}";
        fameExpTxt.text = $"{fameExp} / {fameMaxExp}";
    }
#endregion
}
