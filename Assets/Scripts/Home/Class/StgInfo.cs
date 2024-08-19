using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 광산 스테이지 정보
/// </summary>
[System.Serializable]
public class StgInfo
{
    public string name;
    public int id;
    public int needTicketCnt = 1;       // 필요 입장티켓
    public int unlockPrice;             // 바로 전단계 광석조각 필요
    public bool isUnlocked;             // 잠금상태 (TRUE: 열림, FALSE: 잠김)

    public GameObject lockedPanel;      // 잠금패널
    public Button EnterBtn;             // 입장버튼
    public Button UnlockPriceBtn;       // 잠금해제 가격버튼

    /// <summary>
    /// 스테이지 UI 초기화
    /// </summary>
    public void InitUI() {
        // 이벤트 버튼 등록
        RegistEventHandler();

        // 잠금상태에 따른 버튼표시
        EnterBtn.gameObject.SetActive(isUnlocked);
        UnlockPriceBtn.gameObject.SetActive(!isUnlocked);

        // 가격 표시
        EnterBtn.GetComponentInChildren<TMP_Text>().text = needTicketCnt.ToString();
        UnlockPriceBtn.GetComponentInChildren<TMP_Text>().text = unlockPrice.ToString();

        // 잠금패널 표시
        lockedPanel.SetActive(!isUnlocked);
    }

    /// <summary>
    /// 이벤트 버튼 등록
    /// </summary>
    private void RegistEventHandler() {
        // 입장티켓 버튼 이벤트 등록
        EnterBtn.onClick.AddListener(() => {
            Debug.Log($"Click EnterBtn:: needTicketCnt= {needTicketCnt}");
            if(DM._.DB.statusDB.StageTicket >= needTicketCnt)
            {
                GM._.ui.ShowNoticeMsgPopUp("광산진입");
                DM._.DB.statusDB.StageTicket -= needTicketCnt;

                // 선택한 광산종류 저장
                GM._.stm.OreType = (Enum.RSC)id;

                // 이전 팝업 닫기
                GM._.hm.HomeWindow.SetActive(false);
                GM._.ssm.selectStagePopUp.SetActive(false);

                // 캐릭터 고용(소환) 팝업 열기
                GM._.epm.ShowPopUp();
            }
            else
                GM._.ui.ShowWarningMsgPopUp("입장티켓이 부족합니다.");
        });

        // 잠금해제 버튼 이벤트 등록
        UnlockPriceBtn.onClick.AddListener(() => {
            Debug.Log($"Click UnlockPriceBtn:: unlockPrice= {unlockPrice}");
            if(DM._.DB.statusDB.RscArr[id] >= unlockPrice)
            {
                GM._.ui.ShowNoticeMsgPopUp("광산해금 완료!");
                DM._.DB.statusDB.RscArr[id] -= unlockPrice;

                // 해금 UI
                isUnlocked = true;
                EnterBtn.gameObject.SetActive(true);
                UnlockPriceBtn.gameObject.SetActive(false);
                lockedPanel.SetActive(false);
            }
            else
                GM._.ui.ShowWarningMsgPopUp("광석이 부족합니다.");
        });
    }


}
