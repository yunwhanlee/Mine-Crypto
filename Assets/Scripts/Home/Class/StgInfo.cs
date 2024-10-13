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
    public int unlockPrice;             // 바로 전단계 광석조각 필요
    public bool IsUnlocked              // 잠금상태 (TRUE: 열림, FALSE: 잠김)
    {            
        get => DM._.DB.stageDB.IsUnlockArr[id];
        set => DM._.DB.stageDB.IsUnlockArr[id] = value;
    }

    public GameObject lockedPanel;      // 잠금패널
    public TMP_Text BestFloorTxt;       // 최대기록
    public Button EnterBtn;             // 입장버튼
    public Button UnlockPriceBtn;       // 잠금해제 가격버튼

    /// <summary>
    /// 스테이지 UI 초기화
    /// </summary>
    public void InitUI() {
        // 이벤트 버튼 등록
        RegistEventHandler();

        // 최대기록 표시
        BestFloorTxt.text = $"최대기록 : {DM._.DB.stageDB.BestFloorArr[id]}층";

        // 잠금상태에 따른 버튼표시
        EnterBtn.gameObject.SetActive(IsUnlocked);
        UnlockPriceBtn.gameObject.SetActive(!IsUnlocked);

        // 가격 표시
        UnlockPriceBtn.GetComponentInChildren<TMP_Text>().text = unlockPrice.ToString();

        // 잠금패널 표시
        lockedPanel.SetActive(!IsUnlocked);
    }

    /// <summary>
    /// 이벤트 버튼 등록
    /// </summary>
    private void RegistEventHandler() {
        // 입장티켓 버튼 이벤트 등록
        EnterBtn.onClick.AddListener(() => {
            Debug.Log($"Click EnterBtn:: ");
            SoundManager._.PlaySfx(SoundManager.SFX.EnterSFX);

            // 선택한 광산종류 저장
            GM._.stgm.OreType = (Enum.RSC)id;

            // 캐릭터 가챠뽑기 UI준비
            GM._.stgm.SetGachaUI(GM._.ssm.selectStagePopUp);
        });

        // 잠금해제 버튼 이벤트 등록
        UnlockPriceBtn.onClick.AddListener(() => {
            Debug.Log($"Click UnlockPriceBtn:: unlockPrice= {unlockPrice}");

            var sttDB = DM._.DB.statusDB;

            int previousOreId = id - 1;

            if(sttDB.RscArr[previousOreId] >= unlockPrice)
            {
                SoundManager._.PlaySfx(SoundManager.SFX.UnlockSFX);

                GM._.ui.ShowNoticeMsgPopUp("광산해금 완료!");

                // 재료 수량 감소
                sttDB.SetRscArr(previousOreId, -unlockPrice);

                // 광산개방
                GM._.amm.autoMiningArr[id].IsUnlock = true;

                // 광산의 축복 개방
                // GM._.obm.oreBlessFormatArr[id].IsUnlock = true;

                // 광산의 축복 능력치 설정 (초기)
                GM._.obm.ResetAbilities(id);

                // 해금 UI
                IsUnlocked = true;
                EnterBtn.gameObject.SetActive(true);
                UnlockPriceBtn.gameObject.SetActive(false);
                lockedPanel.SetActive(false);
            }
            else
                GM._.ui.ShowWarningMsgPopUp("광석이 부족합니다.");
        });
    }


}
