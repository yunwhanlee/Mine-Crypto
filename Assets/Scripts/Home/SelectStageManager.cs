using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// 광산 스테이지 선택 매니저
/// </summary>
public class SelectStageManager : MonoBehaviour
{
    public GameObject selectStagePopUp;
    public TMP_Text stageTicketCntTxt;

    [Header("스테이지 정보 배열 (버튼 포함)")]
    public StgInfo[] stgInfoArr;

    void Start() {
        stageTicketCntTxt.text = $"{DM._.DB.statusDB.OreTicket} / 10";

        for(int i = 0; i < stgInfoArr.Length; i++) {
            Debug.Log($"{stgInfoArr[i].name}: EnterBtn= {stgInfoArr[i].EnterBtn}, UnlockPriceBtn= {stgInfoArr[i].UnlockPriceBtn}");
            stgInfoArr[i].InitUI();
        }
    }

#region FUNC
    public void ShowPopUp() {
        selectStagePopUp.SetActive(true);
        selectStagePopUp.GetComponent<DOTweenAnimation>().DORestart();
    }
#endregion
}
