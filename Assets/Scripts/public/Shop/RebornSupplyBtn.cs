using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// 환생보급 아이템
/// </summary>
public class RebornSupplyBtn : MonoBehaviour
{
    public TMP_Text itemTxt;            // 보상아이템아이콘 및 수량 텍스트
    public TMP_Text UnlockGoldPointTxt; // 해금 필요 황금포인트
    public GameObject alertRedDot;
    public GameObject lockedPanel;  // 잠금패널
    public Button button;           // 버튼

    public int id;                  // 획득 트리거 데이터 저장 읽어오기 위한 배열확인용 ID
    public RWD rwdType;
    public int rwdCnt;
    public int unlockedGoldPoint;
    [field:SerializeField] private bool isAccept; public bool IsAccept {
        get => DM._.DB.shopDB.IsAcceptRebornSupplyArr[id];
        set => DM._.DB.shopDB.IsAcceptRebornSupplyArr[id] = value;
    }

    public void Init(int id, int unlockedGoldPoint, RWD rwdType ,int rwdCnt)
    {
        this.id = id;
        this.unlockedGoldPoint = unlockedGoldPoint;
        this.rwdType = rwdType;
        this.rwdCnt = rwdCnt;

        UpdateUI();

        // 클릭 보상수령 이벤트
        button.onClick.AddListener(OnClickRewardItemBtn);
    }

#region FUNC
    public void UpdateUI()
    {
        UnlockGoldPointTxt.text = $"<sprite name=GOLDCOIN>{unlockedGoldPoint}";
        itemTxt.text = $"<sprite name={rwdType}>\n{rwdCnt}";

        // 황금포인트 달성시 잠금표시 해제
        lockedPanel.SetActive(!(unlockedGoldPoint <= GM._.spm.GoldPoint));

        // 이미 수령했는지 확인 비활성화
        button.interactable = !IsAccept;

        // 🔴알람표시
        alertRedDot.SetActive(!lockedPanel.activeSelf && button.interactable);

        // 카테고리UI 업데이트
        GM._.spm.UpdateCatetory();
    }
#endregion
#region EVENT
    /// <summary>
    /// 클릭 아이템수령 버튼 이벤트 
    /// </summary>
    private void OnClickRewardItemBtn()
    {
        SoundManager._.PlaySfx(SoundManager.SFX.FameCompleteSFX);
        GM._.rwm.ShowReward(
            new Dictionary<RWD, int> {
                {rwdType, rwdCnt},
            }
        );

        IsAccept = true;

        UpdateUI();
    }
#endregion
}
