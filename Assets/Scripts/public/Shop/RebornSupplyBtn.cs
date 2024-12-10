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

    public RWD rwdType;
    public int rwdCnt;
    public int unlockedGoldPoint;

    public void Init(int unlockedGoldPoint, RWD rwdType ,int rwdCnt)
    {
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

        lockedPanel.SetActive(!(unlockedGoldPoint <= GM._.fm.FameLv));
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

        button.interactable = false;
    }
#endregion
}
