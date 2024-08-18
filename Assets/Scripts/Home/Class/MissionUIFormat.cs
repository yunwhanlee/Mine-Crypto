using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// (명성) 미션 UI
/// </summary>
[System.Serializable]
public class MissionUIFormat
{
    [field:SerializeField] public Slider ExpSlider {get; private set;}
    [field:SerializeField] public TMP_Text ExpTxt {get; private set;}

    [field:SerializeField] public TMP_Text Rwd1Txt {get; private set;}
    [field:SerializeField] public TMP_Text Rwd2Txt {get; private set;}

#region FUNC
    public void UpdateUI(MissionFormat mf) {
        // Slider UI
        ExpSlider.value = (float)mf.Exp / mf.MaxExp;
        ExpTxt.text = $"{mf.Exp} / {mf.MaxExp}";
    }

    //TODO 보상 ( 1. 명성, 2. 보물상자 )
#endregion
}
