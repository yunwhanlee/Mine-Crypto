using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
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

    [Header("DEBUG")]
    public TMP_Text workerInfoDebugTxt;


    void Start()
    {
        coinTxt.text = DM._.DB.statusDB.Coin.ToString();
    }

    void Update() {
        if(GM._.mnm.workerGroupTf.childCount > 0)
        {
            var worker = GM._.mnm.workerGroupTf.GetChild(0).GetComponent<MiningController>();
            workerInfoDebugTxt.text =
                $"[고블린1 업글] 공: {worker.AttackVal}"
                + $", 공속: {worker.AttackSpeed}({(1 / worker.AttackSpeed).ToString("F2")}초)"
                + $", 이속: {worker.MoveSpeed}"
                + $", 가방: {worker.BagStorageSize}";
        }
    }

#region EVENT
    public void OnClickTopCoinPlusIcon() {
        //! Add Coin TEST
        DM._.DB.statusDB.Coin += 100000;
    }
#endregion

#region FUNC
    /// <summary>
    /// TOP 고블린수/인구 UI 최신화
    /// </summary>
    public void SetTopWorkerInfoTxt(int workerCnt, int population) {
        workerInfoTxt.text = $"{workerCnt} / {population}";
        workerInfoTxt.color = (workerCnt == population)? Color.red : Color.green;
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
#endregion
}
