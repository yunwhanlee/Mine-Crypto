using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using static Enum;

public class RebornManager : MonoBehaviour
{
    //* COMPONENT
    public DOTweenAnimation DOTAnim;

    //* ELEMENT
    public GameObject windowObj;
    public TMP_Text myLightStoneCntTxt;                       // 현재 빛의돌 수량 텍스트
    public TMP_Text myTotleBestFloorTxt;                      // 나의 최고점수 합 텍스트
    public DOTweenAnimation whiteFadeInOutEFAnim;             // 환생 페이드인아웃 이펙트 애니메이션

    [Header("업그레이드 UI")]
    public UpgradeUIFormat upgIncLightStonePerUI;             // 빛의돌 획득량%
    public UpgradeUIFormat upgIncOrePerUI;                    // 모든광석 획득량%
    public UpgradeUIFormat upgDecSkillTimeUI;                 // (int) 스킬쿨타임 감소
    public UpgradeUIFormat upgMinOreBlessCntUI;               // (int) 축복옵션 최소개수
    public UpgradeUIFormat upgDecUpgradePricePerUI;           // 강화비용 감소%
    public UpgradeUIFormat upgDecConsumePricePerUI;           // 소모품 제작 비용감소%
    public UpgradeUIFormat upgDecDecoPricePerUI;              // 장식 제작 비용감소%
    public UpgradeUIFormat upgDecTranscendPircePerUI;         // 초월 강화비용감소%

    //* VALUE
    const int REBORN_MIN_FLOOR = 100;                         // 환생 최소가능 최고층 합 수치
    public bool isRebornTrigger;                              // 환생 게임재시작 트리거

    [Header("업그레이드 데이터")]
    public UpgradeFormatFloat upgIncLightStonePer;            // 빛의돌 획득량%
    public UpgradeFormatFloat upgIncOrePer;                   // 모든광석 획득량%
    public UpgradeFormatInt upgDecSkillTime;                  // (int) 스킬쿨타임 감소
    public UpgradeFormatInt upgMinOreBlessCnt;                // (int) 축복옵션 최소개수
    public UpgradeFormatFloat upgDecUpgradePricePer;          // 강화비용 감소% -> 「모든 업그레이드강화, 자동채굴강화, 시련의광산 크리스탈보관량강화」
    public UpgradeFormatFloat upgDecConsumePricePer;          // 소모품 제작 비용감소%
    public UpgradeFormatFloat upgDecDecoPricePer;             // 장식 제작 비용감소%
    public UpgradeFormatFloat upgDecTranscendPircePer;        // 초월 강화비용감소%

    IEnumerator Start()
    {
        whiteFadeInOutEFAnim.gameObject.SetActive(false);

        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // DB가 null일 경우 새로 초기화
        if (DM._.DB.rebornDB == null)
        {
            Debug.Log($"<color=red>{this.name}DB데이터가 없음으로 자체 초기화</color>");
            DM._.DB.rebornDB = new RebornDB();
            DM._.DB.rebornDB.Init();
        }

        //* 데이터 최신화
        // 빛의돌 획득량%
        upgIncLightStonePer = DM._.DB.rebornDB.upgIncLightStonePer;
        // 모든광석 획득량%
        upgIncOrePer = DM._.DB.rebornDB.upgIncOrePer;
        // (int) 스킬쿨타임 감소
        upgDecSkillTime = DM._.DB.rebornDB.upgDecSkillTime;
        // (int) 축복옵션 최소개수
        upgMinOreBlessCnt = DM._.DB.rebornDB.upgMinOreBlessCnt;
        // 강화비용 감소%
        upgDecUpgradePricePer = DM._.DB.rebornDB.upgDecUpgradePricePer;
        // 소모품 제작 비용감소%
        upgDecConsumePricePer = DM._.DB.rebornDB.upgDecConsumePricePer;
        // 장식 제작 비용감소%
        upgDecDecoPricePer = DM._.DB.rebornDB.upgDecDecoPricePer;
        // 초월 강화비용감소%
        upgDecTranscendPircePer = DM._.DB.rebornDB.upgDecTranscendPircePer;
    }

#region EVENT
    // 빛의돌 획득량% 버튼
    public void OnClickUpgIncLightStonePerBtn() => Upgrade(upgIncLightStonePer);
    // 모든광석 획득량% 버튼
    public void OnClickUpgIncOrePerBtn() => Upgrade(upgIncOrePer);
    // (int) 스킬쿨타임 감소 버튼
    public void OnClickUpgDecSkillTimeBtn() => Upgrade(upgDecSkillTime);
    // (int) 축복옵션 최소개수 버튼
    public void OnClickUpgMinOreBlessCntBtn() => Upgrade(upgMinOreBlessCnt);
    // 강화비용 감소% 버튼
    public void OnClickUpgDecUpgradePricePerBtn() => Upgrade(upgDecUpgradePricePer);
    // 소모품 제작 비용감소% 버튼
    public void OnClickUpgDecConsumePricePerBtn() => Upgrade(upgDecConsumePricePer);
    // 장식 제작 비용감소% 버튼
    public void OnClickUpgDecDecoPricePerBtn() => Upgrade(upgDecDecoPricePer);
    // 초월 강화비용감소% 버튼
    public void OnClickUpgDecTranscendPircePerBtn() => Upgrade(upgDecTranscendPircePer);
    // 환생 버튼
    public void OnClickRebornBtn() {
        if(DM._.DB.stageDB.GetTotalBestFloor() < REBORN_MIN_FLOOR)
        {
            GM._.ui.ShowWarningMsgPopUp("최고층 합이 100층 이상에서만 가능합니다.");
            return;
        }

        AskConfirmReborn();
    } 
#endregion

#region FUNC
    /// <summary>
    /// 환생하시겠습니까?
    /// </summary>
    private void AskConfirmReborn()
    {
        int val = DM._.DB.stageDB.GetTotalBestFloor() / 5;

        string title = "모든 기억을 잃고 환생하시겠습니까?";
        string content = "숙련도, 명예레벨, 환생강화, 버섯도감을 제외한 모든것이 초기화됩니다.";
        string extraRwd = upgIncLightStonePer.Lv > 0? $"(+ {GetExtraRwdVal()})" : "";
        string reward = $"보상 : <sprite name=LIGHTSTONE> {GetRwdVal()} {extraRwd}";

        GM._.ui.ShowConfirmPopUp($"{title}\n{content}\n{reward}");
        GM._.ui.OnClickConfirmBtnAction = () => {
            if(GM._.gameState != GameState.HOME)
            {   // 플레이중에는 불가능합니다 메세지 표시
                GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotAvailableInPlayMsg));
                return;
            }

            StartCoroutine(CoReborn());
        };
    }

    /// <summary>
    /// 환생하기
    /// </summary>
    IEnumerator CoReborn()
    {
        Debug.Log("CoReborn():: 환생!");
        isRebornTrigger = true;

        DB db = DM._.DB;

        //* 화이트 페이드인아웃 이펙트
        SoundManager._.PlaySfx(SoundManager.SFX.Reborn_SFX);
        whiteFadeInOutEFAnim.gameObject.SetActive(true);
        whiteFadeInOutEFAnim.DORestart();

        yield return Util.TIME2;
        windowObj.SetActive(false);

        //* 데이터 초기화전 유지될 데이터 사전준비
        var prevlanguageIdx = db.languageIdx; // 언어
        var prevRebornCnt = db.rebornCnt; // 환생횟수
        var prevBgmVolume = GM._.stm.bgmSlider.value; // 배경음
        var prevSfxVolume = GM._.stm.sfxSlider.value; // 효과음
        var prevFameLv = db.statusDB.FameLv; // 명예레벨
        var prevFame = db.statusDB.Fame; // 명예경험치
        int prevLightStone = db.statusDB.LightStone; // 빛의돌
        var prevProficiencyDB = db.proficiencyDB; // 숙련도 데이터
        var prevRebornDB = db.rebornDB; // 환생강화 데이터
        var prevTimePieceDB = db.timePieceDB; // 시간의결정 데이터
        var prevMushDB = db.mushDB; // 버섯도감 데이터

        // 빛의돌 보상수량
        int rewardCnt = GetRwdVal() + GetExtraRwdVal();

        // 모든 데이터 초기화
        db.Init();

        //* 환생이후도 유지될 데이터 반영
        db.languageIdx = prevlanguageIdx;
        db.rebornCnt = prevRebornCnt;
        db.bgmVolume = prevBgmVolume;
        db.sfxVolume = prevSfxVolume;
        db.statusDB.FameLv = prevFameLv;            // 명예레벨
        db.statusDB.Fame = prevFame;                // 명예포인트
        db.statusDB.LightStone = prevLightStone;    // 빛의돌
        db.proficiencyDB = prevProficiencyDB;       // 숙련도 데이터
        db.timePieceDB = prevTimePieceDB;           // 시간의결정 데이터
        db.rebornDB = prevRebornDB;                 // 환생강화 데이터
        db.mushDB = prevMushDB;                     // 버섯도감 데이터

        //* 환생횟수 증가
        DM._.DB.rebornCnt++;

        //* 빛의돌 보상획득
        GM._.rwm.ShowReward (
            new Dictionary<RWD, int> {
                { RWD.LIGHTSTONE, rewardCnt },
            }
        );

        yield return Util.TIME3;
        whiteFadeInOutEFAnim.gameObject.SetActive(false);
        UpdateDataAndUI();
    }

    /// <summary>
    /// 빛의돌 획득량 반환
    /// </summary>
    private int GetRwdVal()
    {
        float pow = Mathf.Pow(DM._.DB.stageDB.GetTotalBestFloor(), 2);
        return Mathf.RoundToInt(pow / 100);
    }

    /// <summary>
    /// 빛의돌 추가획득량 반환
    /// </summary>
    /// <returns></returns>
    private int GetExtraRwdVal()
    {
        return Mathf.RoundToInt(GetRwdVal() * upgIncLightStonePer.Val);
    }

    /// <summary>
    /// 팝업표시
    /// </summary>
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        UpdateDataAndUI();
    }

    /// <summary>
    /// 업그레이드 처리
    /// </summary>
    /// <param name="upgDt">업그레이드할 데이터</param>
    private void Upgrade(UpgradeFormat upgDt)
    {
        var sttDB = DM._.DB.statusDB;

        if(upgDt.IsMaxLv)
        {
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.MaxLvMsg));
            return;
        }

        if(sttDB.GetInventoryItemVal(upgDt.NeedRsc) >= upgDt.Price)
        {
            SoundManager._.PlaySfx(SoundManager.SFX.TranscendUpgradeSFX);
            GM._.ui.ShowNoticeMsgPopUp(LM._.Localize(LM.UpgradeCompleteMsg));

            // 제작에 필요한 아이템 수량 감소
            sttDB.SetInventoryItemVal(upgDt.NeedRsc, -upgDt.Price);

            GM._.fm.missionArr[(int)MISSION.UPGRADE_CNT].Exp++;
            upgDt.Lv++;

            UpdateDataAndUI();
        }
        else
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
    }

    /// <summary>
    /// 업그레이드 결과 최신화
    /// </summary>
    private void UpdateDataAndUI()
    {
        myLightStoneCntTxt.text = DM._.DB.statusDB.GetInventoryItemVal(INV.LIGHTSTONE).ToString();
        myTotleBestFloorTxt.text = $"{LM._.Localize(LM.MyTotleBestFloor)} : {DM._.DB.stageDB.GetTotalBestFloor()}{LM._.Localize(LM.Floor)}";

        //* Data Price
        upgIncLightStonePer.UpdatePriceArithmetic();
        upgIncOrePer.UpdatePrice();
        upgDecSkillTime.UpdatePrice();
        upgMinOreBlessCnt.UpdatePriceFreeSet(new int[] {500, 1000});
        upgDecUpgradePricePer.UpdatePriceArithmetic();
        upgDecConsumePricePer.UpdatePrice();
        upgDecDecoPricePer.UpdatePrice();
        upgDecTranscendPircePer.UpdatePrice();

        //* UI
        upgIncLightStonePerUI.UpdateUI(upgIncLightStonePer);
        upgIncOrePerUI.UpdateUI(upgIncOrePer);
        upgDecSkillTimeUI.UpdateUI(upgDecSkillTime, "sec");
        upgMinOreBlessCntUI.UpdateUI(upgMinOreBlessCnt);
        upgDecUpgradePricePerUI.UpdateUI(upgDecUpgradePricePer);
        upgDecConsumePricePerUI.UpdateUI(upgDecConsumePricePer);
        upgDecDecoPricePerUI.UpdateUI(upgDecDecoPricePer);
        upgDecTranscendPircePerUI.UpdateUI(upgDecTranscendPircePer);
    }
#endregion
}
