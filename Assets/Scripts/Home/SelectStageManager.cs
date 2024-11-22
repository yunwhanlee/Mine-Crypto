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
    //* ELEMENT
    public GameObject selectStagePopUp;
    public TMP_Text stageTicketCntTxt;      // 광산입장 티켓수 
    public TMP_Text timerTxt;               // 티켓자동획득 시간표시

    //* VALUE
    const int ONE_MINUTE = 60;
    const int ORE_TICKET_MAX = 10;

    [Header("스테이지 정보 배열 (버튼 포함)")]
    public StgInfo[] stgInfoArr;

    private int time;

    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // 이벤트 버튼 등록
        for(int i = 0; i < stgInfoArr.Length; i++)
        {
            stgInfoArr[i].RegistEventHandler();
        }

        // 어플시작시 이전까지 경과한시간
        int passedTime = DM._.DB.autoMiningDB.GetPassedSecData();

        // 티켓자동획득량 계산
        int cnt = passedTime / ONE_MINUTE; // 획득수
        int remainTime = passedTime % ONE_MINUTE; // 남은시간
        Debug.Log($"<color=yellow>티켓자동획득:: 경과시간({passedTime}) / 대기시간({ONE_MINUTE})초, 획득량={cnt}, 남은시간={remainTime}</color>");

        // 대기시간 최신화 (30분에서 남은시간 뺌)
        time = ONE_MINUTE - remainTime;

        stageTicketCntTxt.text = $"({DM._.DB.statusDB.OreTicket} / {ORE_TICKET_MAX})";

        UpdateUI();
    }

#region FUNC
    public void ShowPopUp() {
        GM._.ui.topRscGroup.SetActive(true);
        selectStagePopUp.SetActive(true);
        selectStagePopUp.GetComponent<DOTweenAnimation>().DORestart();
        UpdateUI();
    }

    public void UpdateUI()
    {
        for(int i = 0; i < stgInfoArr.Length; i++) {
            Debug.Log($"{stgInfoArr[i].name}: EnterBtn= {stgInfoArr[i].EnterBtn}, UnlockPriceBtn= {stgInfoArr[i].UnlockPriceBtn}");
            stgInfoArr[i].InitUI();
        }
    }

    /// <summary>
    /// 광석입장티켓 자동획득 (1분)
    /// </summary>
    public void SetOreTicketTimer()
    {
        // 자동획득 최대치 체크
        if(DM._.DB.statusDB.OreTicket >= ORE_TICKET_MAX)
        {
            if(timerTxt.text != "00 : 00")
                timerTxt.text = "00 : 00";
            return;
        }

        time--;
        string timeFormat = Util.ConvertTimeFormat(time);
        timerTxt.text = timeFormat;

        // 리셋
        if(time < 1)
        {
            time = ONE_MINUTE;
            DM._.DB.statusDB.OreTicket++;
        }
    }
#endregion
}
