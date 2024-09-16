using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// (PUBLIC) 공통 UI 매니저 : 어디서든 접근가능
/// </summary>
public class UIManager : MonoBehaviour
{
    public Action OnClickConfirmBtnAction = () => {};    // 재확인용팝업 확인버튼 클릭액션

    [Header("TOP RSC CNT GROUP")]
    public GameObject topRscGroup;
    public TMP_Text[] topRscTxtArr;

    [Header("POP UP")]
    public GameObject menuPopUp;

    public GameObject warningMsgPopUp;
    public TMP_Text warningMsgTxt;

    public GameObject noticeMsgPopUp;
    public TMP_Text noticeMsgTxt;

    public GameObject confirmPopUp;                     // 재확인용 팝업 : 정말 하시겠습니까?
    public TMP_Text confirmMsgTxt;                      // 메세지 내용
    public TMP_Text confirmBtnTxt;                      // 확인버튼 텍스트
    public TMP_Text cancelBtnTxt;                       // 취소버튼 텍스트

    [Header("EFFECT")]
    public ParticleImage coinAttractionPtcImg;          // 광석조각 획득 UI-EF
    public ParticleImage treasureChestAttractionPtcImg; // 보물상자 획득 UI-EF

    void Start() {
        // TOP 재화표시창 끄기
        topRscGroup.SetActive(false);
        
        // 재화표시창 업데이트UI
        for(int i = 0; i < topRscTxtArr.Length; i++)
        {
            topRscTxtArr[i].text = $"{DM._.DB.statusDB.RscArr[i]}";
        }
    }

#region EVENT
    /// <summary>
    /// TOP 재화표시창 끄기
    /// </summary>
    public void OnCloseTopRscGroup() {
        topRscGroup.SetActive(false);
    }

    public void OnClickMenuIconBtn() {
        menuPopUp.SetActive(true);
    }

    public void OnClickInvIconBtn() {
        GM._.ivm.ShowInventory();
    }

    public void OnClickMenu_UpgradeBtn() {
        topRscGroup.SetActive(true);
        GM._.ugm.ShowPopUp();
    }

    public void OnClickMenu_FameBtn() {
        GM._.fm.windowObj.SetActive(true);
        GM._.fm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.fm.UpdateAll();
    }

    public void OnClickMenu_OreBlessBtn() {
        topRscGroup.SetActive(true);
        GM._.obm.windowObj.SetActive(true);
        GM._.obm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
    }

    public void OnClickMenu_AutoMiningBtn() {
        topRscGroup.SetActive(true);
        GM._.amm.windowObj.SetActive(true);
        GM._.amm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.amm.UpdateAll();
    }

    public void OnClickChallengeBtn() {
        topRscGroup.SetActive(true);
        GM._.clm.windowObj.SetActive(true);
        GM._.clm.UpdateUI();
    }

    public void OnClickMenu_OreProficiencyBtn() {
        GM._.pfm.windowObj.SetActive(true);
        GM._.pfm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.pfm.UpdateAll();
    }

    public void OnClickMenu_StatusBtn() {
        GM._.sttm.windowObj.SetActive(true);
        GM._.sttm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.sttm.UpdateMyStatus();
    }

    public void OnClickMenu_TranscendBtn() {
        topRscGroup.SetActive(true);
        GM._.tsm.ShowPopUp();
    }

    public void OnClickMenu_MushroomGuideBtn() {
        topRscGroup.SetActive(false);
        GM._.mrm.ShowPopUp();
    }

    public void OnClickMenu_AlchemyBtn() {
        topRscGroup.SetActive(true);
        GM._.acm.ShowPopUp();
    }

    public void OnClickMenu_SettingBtn() {
        GM._.stm.ShowPopUp();
    }

#endregion

#region FUNC
    /// <summary>
    /// 재확인용 팝업 표시 + OnClickConfirmBtnAction = () => { 처리할 내용 작성하기 }
    /// </summary>
    public void ShowConfirmPopUp(string msgTxt, string confirmTxt = "", string cancelTxt = "")
    {
        confirmPopUp.SetActive(true);
        confirmMsgTxt.text = msgTxt;
        confirmBtnTxt.text = (confirmTxt == "")? "네" : confirmTxt;
        cancelBtnTxt.text = (cancelTxt == "")? "아니오" : cancelTxt;
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
    /// 경고 메세지 팝업
    /// </summary>
    public void ShowWarningMsgPopUp(string msg)
        => StartCoroutine(CoShowWarningMsg(msg));

    private IEnumerator CoShowWarningMsg(string msg) {
        Debug.Log($"CoShowWarningMsg(msg= {msg})");
        warningMsgPopUp.SetActive(true);
        warningMsgTxt.text = msg.ToString();

        yield return Util.TIME1;
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

    public IEnumerator CoPlayCoinAttractionPtcUIEF(int playCnt, Enum.RSC oreType)
    {
        int time = 0;
        while(time < playCnt) {
            time++;
            coinAttractionPtcImg.sprite = GM._.RscSprArr[(int)oreType];
            coinAttractionPtcImg.Play();
            yield return Util.TIME0_1;
        }
    }

    public void PlayTreasureChestAttractionPtcUIEF()
    {
        treasureChestAttractionPtcImg.Play();
    }
#endregion
}