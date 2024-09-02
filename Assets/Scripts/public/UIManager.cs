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
    //* POP UP
    public GameObject menuPopUp;

    public GameObject warningMsgPopUp;
    public TMP_Text warningMsgTxt;

    public GameObject noticeMsgPopUp;
    public TMP_Text noticeMsgTxt;

    [Header("EFFECT")]
    public ParticleImage coinAttractionPtcImg;          // 광석조각 획득 UI-EF
    public ParticleImage treasureChestAttractionPtcImg; // 보물상자 획득 UI-EF

#region EVENT
    public void OnClickMenuIconBtn() {
        menuPopUp.SetActive(true);
    }

    public void OnClickChallengeBtn() {
        GM._.clm.windowObj.SetActive(true);
    }

    public void OnClickInvIconBtn() {
        GM._.ivm.ShowInventory();
    }

    public void OnClickMenu_UpgradeBtn() {
        GM._.ugm.ShowPopUp();
    }

    public void OnClickMenu_FameBtn() {
        GM._.fm.windowObj.SetActive(true);
        GM._.fm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.fm.UpdateAll();
    }

    public void OnClickMenu_OreBlessBtn() {
        GM._.obm.windowObj.SetActive(true);
        GM._.obm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
    }

    public void OnClickMenu_AutoMiningBtn() {
        GM._.amm.windowObj.SetActive(true);
        GM._.amm.windowObj.GetComponent<DOTweenAnimation>().DORestart();
        GM._.amm.UpdateAll();
    }

    public void OnClickMenu_KnowledgeBtnBtn() {
        //TODO
    }

    public void OnClickMenu_SettingBtn() {
        //TODO
    }

#endregion

#region FUNC
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