using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using static Enum;

[Serializable]
public class ProficiencyFormat
{
    // UI
    [field:SerializeField] public TMP_Text LvTxt {get; private set;}
    [field:SerializeField] public Slider ExpSlider {get; private set;}
    [field:SerializeField] public TMP_Text ExpTxt {get; private set;}
    [field:SerializeField] public Button AcceptBtn {get; private set;}
    private Image acceptBtnFrameImg;
    private TMP_Text acceptBtnTxt;

    // Data
    [field:SerializeField] public PROFICIENCY Type {get; set;}      // 타입

    [field:SerializeField] public int Lv {                          // 레벨
        get => DM._.DB.proficiencyDB.saveDts[(int)Type].Lv;
        set => DM._.DB.proficiencyDB.saveDts[(int)Type].Lv = value;
    }

    [field:SerializeField] public bool IsAccept => Exp >= MaxExp;   // 수령가능 여부

    [field:SerializeField] public int MaxExp {get; set;}            // 필요경험치

    [field:SerializeField] public int Exp {                         // 현재경험치
        get {
            var pfcDB = DM._.DB.proficiencyDB;

            switch(Type)
            {
                case PROFICIENCY.ORE1: return pfcDB.saveDts[0].Exp;
                case PROFICIENCY.ORE2: return pfcDB.saveDts[1].Exp;
                case PROFICIENCY.ORE3: return pfcDB.saveDts[2].Exp;
                case PROFICIENCY.ORE4: return pfcDB.saveDts[3].Exp;
                case PROFICIENCY.ORE5: return pfcDB.saveDts[4].Exp;
                case PROFICIENCY.ORE6: return pfcDB.saveDts[5].Exp;
                case PROFICIENCY.ORE7: return pfcDB.saveDts[6].Exp;
                case PROFICIENCY.ORE8: return pfcDB.saveDts[7].Exp;
                case PROFICIENCY.ORE_CHEST: return pfcDB.saveDts[8].Exp;
                case PROFICIENCY.TREASURE_CHEST: return pfcDB.saveDts[9].Exp;
            }
            Debug.LogError("MissionFormat:: Type Error : 맞는 타입이 없습니다.");
            return -1;
        }
        set {
            var pfcDB = DM._.DB.proficiencyDB;

            switch(Type)
            {
                case PROFICIENCY.ORE1: pfcDB.saveDts[0].Exp = value; break;
                case PROFICIENCY.ORE2: pfcDB.saveDts[1].Exp = value; break;
                case PROFICIENCY.ORE3: pfcDB.saveDts[2].Exp = value; break;
                case PROFICIENCY.ORE4: pfcDB.saveDts[3].Exp = value; break;
                case PROFICIENCY.ORE5: pfcDB.saveDts[4].Exp = value; break;
                case PROFICIENCY.ORE6: pfcDB.saveDts[5].Exp = value; break;
                case PROFICIENCY.ORE7: pfcDB.saveDts[6].Exp = value; break;
                case PROFICIENCY.ORE8: pfcDB.saveDts[7].Exp = value; break;
                case PROFICIENCY.ORE_CHEST: pfcDB.saveDts[8].Exp = value; break;
                case PROFICIENCY.TREASURE_CHEST: pfcDB.saveDts[9].Exp = value; break;
            }

            // 객체생성 단계에서는 MaxExp가 0일때는 업데이트 알림UI 처리하지 않기
            if(MaxExp == 0)
                return;

            // 업데이트 알림UI 🔴
            UpdateAlertRedDot();
        }
    }

#region FUNC
    /// <summary>
    /// 객체 초기화
    /// </summary>
    public void Init()
    {
        acceptBtnFrameImg = AcceptBtn.GetComponent<Image>();
        acceptBtnTxt = AcceptBtn.GetComponentInChildren<TMP_Text>();
    }

    public void UpdateData()
    {
        switch(Type)
        {
            default: // ORE1 ~ 8
                MaxExp = GM._.pfm.oreMaxExpArr[Lv - 1];
                break;
            case PROFICIENCY.ORE_CHEST:
            case PROFICIENCY.TREASURE_CHEST:
                MaxExp = GM._.pfm.chestMaxExpArr[Lv - 1];
                break;
        }
    }

    public void UpdateUI()
    {
        // 레벨 텍스트
        LvTxt.text = $"Lv{Lv}";

        // 경험치 슬라이더
        ExpSlider.value = (float)Exp / MaxExp;
        ExpTxt.text = $"{Exp} / {MaxExp}";

        // 수령버튼
        acceptBtnTxt.text = IsAccept? "완료" : "진행중";
        acceptBtnFrameImg.sprite = IsAccept? GM._.pfm.yellowBtnSpr : GM._.pfm.grayBtnSpr;

        // 업데이트 알림UI 🔴
        UpdateAlertRedDot();
    }

    private void UpdateAlertRedDot()
    {
        Debug.Log($"UpdateAlertRedDot():: Type={Type}: IsAccept= {IsAccept}");

        if(IsAccept && !GM._.pfm.alertRedDotObj.activeSelf)
        {
            GM._.pfm.alertRedDotObj.SetActive(true);
        }
    }
#endregion
}
