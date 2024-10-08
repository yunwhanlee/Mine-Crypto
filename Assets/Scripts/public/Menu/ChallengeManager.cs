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

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        UpdateUI();
    }

#region EVENT
    /// <summary>
    /// 시련의광산 입장 버튼
    /// </summary>
    public void OnClickEnterBtn()
    {
        // 선택한 광산타입
        GM._.stgm.OreType = RSC.CRISTAL;

        // 캐릭터 가챠뽑기 UI준비
        GM._.stgm.SetGachaUI(windowObj);

        UpdateUI();
    }
#endregion

#region FUNC
    /// <summary>
    /// 시련의광산 팝업 표시
    /// </summary>
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        GM._.ui.topRscGroup.SetActive(true);
        UpdateUI();
        GM._.amm.UpdateAll(); // 시련의광산 자동채굴처리를 AutoMiningManager에서 관리함으로 호출
    }

    public void UpdateUI()
    {
        redTicketCntTxt.text = $"{CurRedTicketCnt}";
        bestFloorTxt.text = $"{BestFloor}";
    }
#endregion
}
