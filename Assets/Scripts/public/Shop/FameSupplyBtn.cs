using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// 명예보급 아이템 리스트 버튼
/// </summary>
public class FameSupplyBtn : MonoBehaviour
{
    public TMP_Text itemTxt;
    public TMP_Text unlockLvTxt;
    public GameObject alertRedDot;
    public GameObject lockedPanel;
    public Button button;

    public RWD rwdType;
    public int rwdCnt;
    public int unlockedLv;

    public void Init(int unlockedLv, RWD rwdType ,int rwdCnt)
    {
        this.unlockedLv = unlockedLv;
        this.rwdType = rwdType;
        this.rwdCnt = rwdCnt;

        UpdateUI();

        // 클릭 보상수령 이벤트
        button.onClick.AddListener(OnClickRewardItemBtn);
    }

#region FUNC
    public void UpdateUI()
    {
        unlockLvTxt.text = $"명예 레벨{unlockedLv}";
        itemTxt.text = $"<sprite name={rwdType}>\n{rwdCnt}";

        lockedPanel.SetActive(!(unlockedLv <= GM._.fm.FameLv));
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
