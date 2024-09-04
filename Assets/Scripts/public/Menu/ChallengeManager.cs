using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enum;

/// <summary>
/// 시련의 광산
/// </summary>
public class ChallengeManager : MonoBehaviour
{
    // Elements
    public GameObject windowObj;
    public TMP_Text redTicketCntTxt; // 붉은티켓 수량 텍스트
    public TMP_Text bestFloorTxt;    // 최대 도달 층수

    // Values
    [SerializeField] public int CurRedTicketCnt {
        get => DM._.DB.statusDB.RedTicket;
        set => DM._.DB.statusDB.RedTicket = value;
    }
    [SerializeField] public int BestFloor {
        get => DM._.DB.stageDB.BestFloorArr[(int)RSC.CRISTAL];
        set => DM._.DB.stageDB.BestFloorArr[(int)RSC.CRISTAL] = value;
    }

    void Start()
    {
        UpdateUI();
    }

#region EVENT
    public void OnClickEnterBtn()
    {
        if(CurRedTicketCnt > 1)
        {
            CurRedTicketCnt--;

            
        }
        else
            GM._.ui.ShowWarningMsgPopUp("입장티켓이 부족합니다.");
    }
#endregion

#region FUNC
    private void UpdateUI()
    {
        redTicketCntTxt.text = $"{CurRedTicketCnt}";
        bestFloorTxt.text = $"{BestFloor}";
    }

    private void UpdateData()
    {

    }
#endregion
}
