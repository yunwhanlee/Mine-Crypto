using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// (명성) 미션 UI
/// </summary>
[System.Serializable]
public class MissionUIFormat
{
    [field:SerializeField] public Slider ExpSlider {get; private set;}
    [field:SerializeField] public TMP_Text ExpTxt {get; private set;}

    [field:SerializeField] public TMP_Text Rwd1Txt {get; private set;}
    [field:SerializeField] public Image Rwd1Img {get; private set;}
    [field:SerializeField] public TMP_Text Rwd2Txt {get; private set;}
    [field:SerializeField] public Image Rwd2Img {get; private set;}

#region FUNC
    /// <summary>
    /// 미션 업데이트 UI
    /// </summary>
    /// <param name="mission">미션타입</param> <summary>
    public void UpdateUI(MissionFormat mission) {
        // 경험치 슬라이더
        ExpSlider.value = (float)mission.Exp / mission.MaxExp;
        ExpTxt.text = $"{mission.Exp} / {mission.MaxExp}";

        // 보상 수령버튼
        var rewardKeys = mission.Reward.Keys.ToList();
        var rewardValues = mission.Reward.Values.ToList();
        if(rewardValues.Count > 1)
        {
            // 보상1
            Rwd1Txt.text = rewardValues[0].ToString();
            Rwd1Img.sprite = GM._.rwm.GetRewardItemSprite(rewardKeys[0]);
            // 보상2
            Rwd2Txt.text = rewardValues[1].ToString();
            Rwd2Img.sprite = GM._.rwm.GetRewardItemSprite(rewardKeys[1]);
        }
    }
#endregion
}
