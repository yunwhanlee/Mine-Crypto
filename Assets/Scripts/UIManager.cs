using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //* TOP
    public TMP_Text coinTxt;
    public TMP_Text workerInfoTxt;

    //* POP UP
    public GameObject warningMsgPopUp;
    public TMP_Text warningMsgTxt;

    public GameObject noticeMsgPopUp;
    public TMP_Text noticeMsgTxt;

    //* EFFECT
    public ParticleImage coinAttractionPtcImg;


    void Start()
    {
        coinTxt.text = DM._.DB.statusDB.Coin.ToString();
    }

    /// <summary>
    /// TOP 고블린수/인구 UI 최신화
    /// </summary>
    public void SetTopWorkerInfoTxt(int workerCnt, int population) {
        workerInfoTxt.text = $"{workerCnt} / {population}";
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

    public IEnumerator CoPlayCoinAttractionPtcUIEF(int playCnt = 1)
    {
        int time = 0;
        while(time < playCnt) {
            time++;
            coinAttractionPtcImg.Play();
            yield return Util.TIME0_1;
        }
    }
}
