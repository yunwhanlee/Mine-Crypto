using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// 명예보급 아이템
/// </summary>
public class FameSupplyBtn : MonoBehaviour
{
    public TMP_Text itemTxt;        // 보상아이템아이콘 및 수량 텍스트
    public TMP_Text unlockLvTxt;    // 해금 명예필요레벨 텍스트
    public GameObject alertRedDot;
    public GameObject lockedPanel;  // 잠금패널
    public Button button;           // 버튼

    public int id;                  // 획득 트리거 데이터 저장 읽어오기 위한 배열확인용 ID
    public RWD rwdType;
    public int rwdCnt;
    public int unlockedLv;
    [field:SerializeField] private bool isAccept; public bool IsAccept {
        get => DM._.DB.shopDB.IsAcceptFameSupplyArr[id];
        set => DM._.DB.shopDB.IsAcceptFameSupplyArr[id] = value;
    }

    public void Init(int id, int unlockedLv, RWD rwdType ,int rwdCnt)
    {
        this.id = id;
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

        // 명예레벨 달성시 잠금표시 해제
        lockedPanel.SetActive(!(unlockedLv <= GM._.fm.FameLv));

        // 이미 수령했는지 확인 비활성화
        button.interactable = !IsAccept;
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
        button.interactable = false;
    }
#endregion
}
