using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// (PUBLIC) 공통 UI 매니저 : 어디서든 접근가능
/// </summary>
public class UIManager : MonoBehaviour
{
    public Action OnClickConfirmBtnAction = () => {};       // 재확인용팝업 확인버튼 클릭액션
    public Action OnClickConfirmAdsBtnAction = () => {};    // 재확인용팝업 광고버튼 클릭액션
    public Action OnClickUnlockPopUpDimScreen = null;       // 해금팝업 닫기 스크린 클릭시 추가액션

    [Header("TOP RSC CNT GROUP")]
    public GameObject topRscGroup;
    public TMP_Text[] topRscTxtArr;

    public GameObject topMushGroup;
    public TMP_Text[] topMushTxtArr;

    [Header("POP UP")]
    public GameObject menuPopUp;

    public GameObject warningMsgPopUp;
    public TMP_Text warningMsgTxt;

    public GameObject noticeMsgPopUp;
    public TMP_Text noticeMsgTxt;

    public GameObject confirmPopUp;                     // 재확인용 팝업 : 정말 하시겠습니까?
    public GameObject adsBtnObj;                        // 광고시청 버튼 오브젝트
    public TMP_Text confirmTitleTxt;                    // 타이틀 내용
    public TMP_Text confirmMsgTxt;                      // 메세지 내용
    public TMP_Text confirmBtnTxt;                      // 확인버튼 텍스트
    public TMP_Text cancelBtnTxt;                       // 취소버튼 텍스트

    public GameObject unlockContentPopUp;               // 컨텐츠개방 팝업
    public Image unlockContentPopUpLogoImg;             // 컨텐츠개방 로고이미지
    public TMP_Text unlockContentPopUpTitleTxt;         // 컨텐츠개방 타이틀 텍스트
    public TMP_Text unlockContentPopUpMsgTxt;           // 컨텐츠개방 메세지 텍스트

    [Header("EFFECT")]
    public ParticleImage[] coinAttractionPtcImgArr;       // 광석조각 획득 UI-EF
    public ParticleImage treasureChestAttractionPtcImg; // 보물상자 획득 UI-EF

    void Start() {
        // TOP 표시창 끄기
        topRscGroup.SetActive(false);
        topMushGroup.SetActive(false);
        
        // TOP 재화표시창 업데이트UI
        for(int i = 0; i < topRscTxtArr.Length; i++)
        {
            topRscTxtArr[i].text = $"{DM._.DB.statusDB.RscArr[i]}";
        }
        // TOP 버섯표시창 업데이트UI
        for(int i = 0; i < topMushTxtArr.Length; i++)
        {
            topMushTxtArr[i].text = $"{DM._.DB.statusDB.MsrArr[i]}";
        }
    }

#region EVENT
    /// <summary>
    /// TOP 재화표시창 끄기
    /// </summary>
    public void OnCloseTopRscGroup() {
        //* (BUG) 인게임종료후, 다른메뉴팝업 닫을경우 재화그룹사라지는 문제대응
        if(GM._.clm.windowObj.activeSelf)
            return;

        topRscGroup.SetActive(false);
    }
    public void OnCloseTopMushGroup() {
        topMushGroup.SetActive(false);
        // 업그레이드 가능알림🔴 최신화
        GM._.ugm.UpdateAlertRedDotUI();
        GM._.mrm.UpdateAlertRedDotUI();
    }

    public void OnClickMenuIconBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        menuPopUp.SetActive(true);
    }

    public void OnClickInvIconBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.ClickBagSFX);
        GM._.ivm.ShowInventory();
    }

    public void OnClickTimePieceIconBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        GM._.tpm.ShowPopUp();
    }

    public void OnClickMenu_UpgradeBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        topRscGroup.SetActive(true);
        GM._.ugm.ShowPopUp();
    }

    public void OnClickMenu_FameBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        GM._.fm.windowObj.SetActive(true);
        GM._.fm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.fm.UpdateAll();
    }

    public void OnClickMenu_OreBlessBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        topRscGroup.SetActive(true);
        GM._.obm.ShowPopUp();
    }

    public void OnClickMenu_AutoMiningBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        topRscGroup.SetActive(true);
        GM._.amm.windowObj.SetActive(true);
        GM._.amm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.amm.UpdateAll();
    }

    public void OnClickChallengeBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        topRscGroup.SetActive(true);
        GM._.clm.ShowPopUp();
    }

    public void OnClickMenu_OreProficiencyBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        GM._.pfm.windowObj.SetActive(true);
        GM._.pfm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.pfm.UpdateAll();
    }

    public void OnClickMenu_StatusBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        GM._.sttm.windowObj.SetActive(true);
        GM._.sttm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.sttm.UpdateMyStatus();
    }

    public void OnClickMenu_TranscendBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        topRscGroup.SetActive(true);
        GM._.tsm.ShowPopUp();
    }

    public void OnClickMenu_MushroomGuideBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        topRscGroup.SetActive(false);
        GM._.mrm.ShowPopUp();
    }

    public void OnClickMenu_AlchemyBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        topRscGroup.SetActive(true);
        GM._.acm.ShowPopUp();
    }

    public void OnClickMenu_SkillBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        GM._.skm.ShowPopUp();
    }

    public void OnClickMenu_RebornBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        GM._.rbm.ShowPopUp();
    }

    public void OnClickMenu_SettingBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        GM._.stm.ShowPopUp();
    }

    public void OnClickMenu_RankBtn() {
        if(DM._.isPC)
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.ComingSoonMsg));
        else
        {
            GPGS._.ShowLeaderBoard(); // 구글 리더보드
        }
    }

    public void OnClickShopBtn() {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        GM._.spm.ShowPopUp();
    }

#endregion

#region FUNC
    /// <summary>
    /// 재확인용 팝업 표시 + OnClickConfirmBtnAction = () => { 처리할 내용 작성하기 }
    /// </summary>
    public void ShowConfirmPopUp(string title, string msgTxt, bool isActiveAdsBtn = false)//string confirmTxt = "", string cancelTxt = "")
    {
        confirmPopUp.SetActive(true);
        confirmTitleTxt.text = title;
        confirmMsgTxt.text = msgTxt;
        confirmBtnTxt.text = LM._.Localize(LM.Yes); //(confirmTxt == "")? LM._.Localize(LM.Yes) : confirmTxt;
        cancelBtnTxt.text = LM._.Localize(LM.No); //(cancelTxt == "")? LM._.Localize(LM.No) : cancelTxt;
        adsBtnObj.SetActive(isActiveAdsBtn);
    }

    /// <summary>
    /// 재확인용 팝업 확인버튼 클릭
    /// </summary>
    public void OnClickConfirmPopUp_ConfirmBtn()
    {
        confirmPopUp.SetActive(false);
        OnClickConfirmBtnAction.Invoke(); // 구독한 확인버튼 액션실행
    }

    /// <summary>
    /// 재확인용 팝업 광고버튼 클릭
    /// </summary>
    public void OnClickConfirmPopUp_AdsBtn()
    {
        confirmPopUp.SetActive(false);
        OnClickConfirmAdsBtnAction.Invoke();
    }

    /// <summary>
    /// 경고 메세지 팝업
    /// </summary>
    public void ShowWarningMsgPopUp(string msg)
        => StartCoroutine(CoShowWarningMsg(msg));

    private IEnumerator CoShowWarningMsg(string msg) {
        Debug.Log($"CoShowWarningMsg(msg= {msg})");
        SoundManager._.PlaySfx(SoundManager.SFX.ErrorSFX);
        warningMsgPopUp.SetActive(true);
        warningMsgTxt.text = msg.ToString();

        yield return Util.TIME3;
        warningMsgPopUp.SetActive(false);
    }

    /// <summary>
    /// 알림 메세지 팝업
    /// </summary>
    public void ShowNoticeMsgPopUp(string msg)
        => StartCoroutine(CoShowNoticeMsg(msg));

    private IEnumerator CoShowNoticeMsg(string msg) {
        Debug.Log($"CoShowNoticeMsg(msg= {msg})");
        noticeMsgPopUp.SetActive(true);
        noticeMsgTxt.text = msg.ToString();

        yield return Util.TIME1;
        noticeMsgPopUp.SetActive(false);
    }

    /// <summary>
    /// 광석 가방으로 파티클 효과
    /// </summary>
    /// <param name="oreType">광석타입</param>
    public void PlayOreAttractionPtcUIEF(Enum.RSC oreType)
    {
        GM._.ui.coinAttractionPtcImgArr[(int)oreType].Play();
    }

    public void PlayTreasureChestAttractionPtcUIEF()
    {
        treasureChestAttractionPtcImg.Play();
    }

    /// <summary>
    /// 잠금해제 팝업 표시
    /// </summary>
    /// <param name="sprite">이미지</param>
    /// <param name="titleTxt">타이틀</param>
    /// <param name="msgTxt">메세지</param>
    public void ShowUnlockContentPopUp(Sprite sprite, string titleTxt, string msgTxt)
    {
        unlockContentPopUp.SetActive(true);
        unlockContentPopUp.GetComponent<DOTweenAnimation>().DORestart();
        unlockContentPopUpLogoImg.sprite = sprite;
        unlockContentPopUpTitleTxt.text = titleTxt;
        unlockContentPopUpMsgTxt.text = msgTxt;
    }

    /// <summary>
    /// 잠금해제 팝업 스크린 클릭 닫기
    /// </summary>
    public void OnClickUnlockContentPopUpDimScreenAction()
    {
        // 구독된 액션이벤트가 있다면 실행
        if(OnClickUnlockPopUpDimScreen != null)
        {
            OnClickUnlockPopUpDimScreen.Invoke();
            OnClickUnlockPopUpDimScreen = null;
        }
    }
#endregion
}