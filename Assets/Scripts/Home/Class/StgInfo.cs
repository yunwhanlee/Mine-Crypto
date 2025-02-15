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
    // 바로 전단계 광석조각 필요
    [field:SerializeField] int unlockPrice; public int UnlockPrice {
        get {
            int res = unlockPrice;

            //* 비용감소 추가처리
            var rbm = GM._.rbm;
            // 강화비용 감소%
            if(rbm.upgDecUpgradePricePer.Lv > 0) {
                float decreasePer = 1 - rbm.upgDecUpgradePricePer.Val;
                res = Mathf.RoundToInt(res * decreasePer);
            }

            return res;
        }
    }           
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
        // 최대기록 표시
        BestFloorTxt.text = $"{LM._.Localize(LM.BestRecord)} : {DM._.DB.stageDB.BestFloorArr[id]} {LM._.Localize(LM.Floor)}";

        // 잠금상태에 따른 버튼표시
        EnterBtn.gameObject.SetActive(IsUnlocked);
        UnlockPriceBtn.gameObject.SetActive(!IsUnlocked);

        // 가격 표시
        UnlockPriceBtn.GetComponentInChildren<TMP_Text>().text = UnlockPrice.ToString();

        // 잠금패널 표시
        lockedPanel.SetActive(!IsUnlocked);
    }

    /// <summary>
    /// 이벤트 버튼 등록
    /// </summary>
    public void RegistEventHandler() {
        // 입장티켓 버튼 이벤트 등록
        EnterBtn.onClick.AddListener(() => {
            Debug.Log($"Click EnterBtn:: ");
            SoundManager._.PlaySfx(SoundManager.SFX.EnterSFX);

            // 선택한 광산종류 저장
            GM._.stgm.OreType = (Enum.RSC)id;

            // 캐릭터 가챠뽑기 UI준비
            GM._.stgm.SetGachaUI(GM._.ssm.selectStagePopUp);

            // 선택한 스테이지맵 표시
            GM._.stgm.SetSelectMap();
        });

        // 잠금해제 버튼 이벤트 등록
        UnlockPriceBtn.onClick.AddListener(() => {
            Debug.Log($"Click UnlockPriceBtn:: unlockPrice= {UnlockPrice}");

            var sttDB = DM._.DB.statusDB;

            int previousOreId = id - 1;

            if(sttDB.RscArr[previousOreId] >= UnlockPrice)
            {
                SoundManager._.PlaySfx(SoundManager.SFX.UnlockSFX);

                GM._.ui.ShowNoticeMsgPopUp(LM._.Localize(LM.UnlockOreMineMsg));

                // 재료 수량 감소
                sttDB.SetRscArr(previousOreId, -UnlockPrice);

                // 광산개방
                GM._.amm.autoMiningArr[id].IsUnlock = true;

                // 해금 UI
                IsUnlocked = true;
                EnterBtn.gameObject.SetActive(true);
                UnlockPriceBtn.gameObject.SetActive(false);
                lockedPanel.SetActive(false);
            }
            else
                GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
        });
    }


}
