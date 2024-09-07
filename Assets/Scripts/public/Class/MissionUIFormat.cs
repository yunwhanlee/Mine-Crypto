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
    [field:SerializeField] public TMP_Text LvTxt {get; private set;}

    [field:SerializeField] public Slider ExpSlider {get; private set;}
    [field:SerializeField] public TMP_Text ExpTxt {get; private set;}

    [field:SerializeField] public Image BtnImg {get; private set;}
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
        // 레벨 텍스트
        LvTxt.text = mission.Lv.ToString();

        // 경험치 슬라이더
        ExpSlider.value = (float)mission.Exp / mission.MaxExp;
        ExpTxt.text = $"{mission.Exp} / {mission.MaxExp}";

        // 보상 수령버튼
        var rewardKeys = mission.Reward.Keys.ToList();
        var rewardValues = mission.Reward.Values.ToList();

        // 수령가능에 따른 버튼 색깔
        if(mission.Exp >= mission.MaxExp)
        {
            BtnImg.sprite = GM._.fm.yellowBtnSpr;
        }
        else
            BtnImg.sprite = GM._.fm.grayBtnSpr;

        if(rewardValues.Count > 1)
        {
            // 보상1 FAME
            int val = rewardValues[0];
            if(rewardKeys[0] == RWD.FAME) // (초월)명예 획득량
                val += GM._.tsm.upgIncFame.Val;
            Rwd1Txt.text = val.ToString();
            Rwd1Img.sprite = GM._.rwm.GetRewardItemSprite(rewardKeys[0]);
            // 보상2
            Rwd2Txt.text = rewardValues[1].ToString();
            Rwd2Img.sprite = GM._.rwm.GetRewardItemSprite(rewardKeys[1]);
        }
    }
#endregion
}
