using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

public class MushroomManager : MonoBehaviour
{
    [Header("아이템 버튼 이미지")]
    public Sprite itemBtnSelectedSpr;           // 아이템 버튼 선택 색깔
    public Sprite itemBtnUnSelectedSpr;         // 아이템 버튼 미선택 색깔

    [Header("UI 요소")]
    // Element
    public GameObject windowObj;
    public GameObject LockFrameObj;
    public DOTweenAnimation DOTAnim;

    public Image targetImg;
    public Image[] btnImgArr;
    public TMP_Text targetValueTxt;
    public TMP_Text targetNameTxt;
    public TMP_Text targetLvTxt;
    public TMP_Text targetAbilityTxt;
    public TMP_Text upgradeBtnTxt;
    public TMP_Text totalAbilityTxt;

    // Value
    public int mushIdx;

    [Header("업그레이드 데이터")]
    public UpgradeMushFormatInt ms1_UpgAttack;
    public UpgradeMushFormatFloat ms2_UpgMovSpeedPer;
    public UpgradeMushFormatFloat ms3_UpgBagStoragePer;
    public UpgradeMushFormatFloat ms4_UpgNextStageSkipPer;
    public UpgradeMushFormatInt ms5_UpgIncTimer;
    public UpgradeMushFormatFloat ms6_UpgChestSpawnPer;
    public UpgradeMushFormatFloat ms7_UpgAtkSpeedPer;
    public UpgradeMushFormatInt ms8_IncPopulation;

    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // 버섯도감 잠금상태
        LockFrameObj.SetActive(!DM._.DB.mushDB.isUnlock);

        mushIdx = 0;

        ms1_UpgAttack = DM._.DB.mushDB.ms1_UpgAttack;
        ms2_UpgMovSpeedPer = DM._.DB.mushDB.ms2_UpgMovSpeedPer;
        ms3_UpgBagStoragePer = DM._.DB.mushDB.ms3_UpgBagStoragePer;
        ms4_UpgNextStageSkipPer = DM._.DB.mushDB.ms4_UpgNextStageSkipPer;
        ms5_UpgIncTimer = DM._.DB.mushDB.ms5_UpgIncTimer;
        ms6_UpgChestSpawnPer = DM._.DB.mushDB.ms6_UpgChestSpawnPer;
        ms7_UpgAtkSpeedPer = DM._.DB.mushDB.ms7_UpgAtkSpeedPer;
        ms8_IncPopulation = DM._.DB.mushDB.ms8_IncPopulation;

        UpdateDataAndUI();
    }

#region EVENT
    /// <summary>
    /// 잠금 아이콘 버튼
    /// </summary>
    public void OnClickLockFrameBtn()
        => GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.UnlockMushroomDicMsg));

    public void OnClickMushroomBtn(int idx)
    {
        mushIdx = idx;
        UpdateDataAndUI();
    }

    public void OnClickUpgradeBtn()
    {
        switch(mushIdx)
        {
            case (int)MUSH.MUSH1: Upgrade(ms1_UpgAttack); break;
            case (int)MUSH.MUSH2: Upgrade(ms2_UpgMovSpeedPer); break;
            case (int)MUSH.MUSH3: Upgrade(ms3_UpgBagStoragePer); break;
            case (int)MUSH.MUSH4: Upgrade(ms4_UpgNextStageSkipPer); break;
            case (int)MUSH.MUSH5: Upgrade(ms5_UpgIncTimer); break;
            case (int)MUSH.MUSH6: Upgrade(ms6_UpgChestSpawnPer); break;
            case (int)MUSH.MUSH7: Upgrade(ms7_UpgAtkSpeedPer); break;
            case (int)MUSH.MUSH8: Upgrade(ms8_IncPopulation); break;
        }

        UpdateDataAndUI();
    }
#endregion

#region FUNC
    /// <summary>
    /// (버섯도감) 시스템 잠금해제
    /// </summary>
    public void Unlock()
    {
        LockFrameObj.SetActive(false);
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        GM._.ui.topMushGroup.SetActive(true);
        OnClickMushroomBtn(idx: 0); // 버섯1로 선택 초기화
    }

    private void Upgrade(UpgradeMushFormat upgDt)
    {
        if(DM._.DB.statusDB.MsrArr[(int)upgDt.NeedMush] >= upgDt.Price)
        {
            SoundManager._.PlaySfx(SoundManager.SFX.UpgradeMushSFX);
            GM._.ui.ShowNoticeMsgPopUp(LM._.Localize(LM.UpgradeCompleteMsg));
            DM._.DB.statusDB.SetMsrArr((int)upgDt.NeedMush, -upgDt.Price);
            upgDt.Lv++;

            UpdateDataAndUI();
        }
        else
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
    }

    private void UpdateData()
    {
        ms1_UpgAttack.UpdatePrice();
        ms2_UpgMovSpeedPer.UpdatePrice();
        ms3_UpgBagStoragePer.UpdatePrice();
        ms4_UpgNextStageSkipPer.UpdatePrice();
        ms5_UpgIncTimer.UpdatePrice();
        ms6_UpgChestSpawnPer.UpdatePrice();
        ms7_UpgAtkSpeedPer.UpdatePrice();
        ms8_IncPopulation.UpdatePrice();
    }

    private void UpdateUI()
    {
        const int offset = 9 + 8;
        UpgradeMushFormat mushFormat = null;
        targetValueTxt.text = $"{LM._.Localize(LM.Ammount)} : {DM._.DB.statusDB.MsrArr[mushIdx]}";
        totalAbilityTxt.text = "";

        // 선택한 버튼 색상
        for(int i = 0; i < btnImgArr.Length; i++)
        {
            btnImgArr[i].sprite = (i == mushIdx)? itemBtnSelectedSpr : itemBtnUnSelectedSpr;
        }

        // 현재 버섯아이템 능력 표시
        switch(mushIdx)
        {
            case (int)MUSH.MUSH1:
                mushFormat = ms1_UpgAttack;
                targetAbilityTxt.text = $"{LM._.Localize(LM.Attack)} +{ms1_UpgAttack.Val} => +{ms1_UpgAttack.GetNextVal()}";
                break;
            case (int)MUSH.MUSH2:
                mushFormat = ms2_UpgMovSpeedPer;
                targetAbilityTxt.text = $"{LM._.Localize(LM.MoveSpeed)} +{Util.FloatToStr(ms2_UpgMovSpeedPer.Val * 100)} => +{Util.FloatToStr(ms2_UpgMovSpeedPer.GetNextVal() * 100)}%";
                break;
            case (int)MUSH.MUSH3:
                mushFormat = ms3_UpgBagStoragePer;
                targetAbilityTxt.text = $"{LM._.Localize(LM.BagStorage)} +{Util.FloatToStr(ms3_UpgBagStoragePer.Val * 100)} => +{Util.FloatToStr(ms3_UpgBagStoragePer.GetNextVal() * 100)}%";
                break;
            case (int)MUSH.MUSH4:
                mushFormat = ms4_UpgNextStageSkipPer;
                targetAbilityTxt.text = $"{LM._.Localize(LM.NextStageSkip)} {Util.FloatToStr(ms4_UpgNextStageSkipPer.Val * 100)} => +{Util.FloatToStr(ms4_UpgNextStageSkipPer.GetNextVal() * 100)}%";
                break;
            case (int)MUSH.MUSH5:
                mushFormat = ms5_UpgIncTimer;
                targetAbilityTxt.text = $"{LM._.Localize(LM.MiningTime)} +{ms5_UpgIncTimer.Val} => +{ms5_UpgIncTimer.GetNextVal()}초";
                break;
            case (int)MUSH.MUSH6:
                mushFormat =ms6_UpgChestSpawnPer;
                targetAbilityTxt.text = $"{LM._.Localize(LM.IncChestSpawnPer)} +{Util.FloatToStr(ms6_UpgChestSpawnPer.Val * 100)} => +{Util.FloatToStr(ms6_UpgChestSpawnPer.GetNextVal() * 100)}%";
                break;
            case (int)MUSH.MUSH7:
                mushFormat = ms7_UpgAtkSpeedPer;
                targetAbilityTxt.text = $"{LM._.Localize(LM.AttackSpeed)} +{Util.FloatToStr(ms7_UpgAtkSpeedPer.Val * 100)} => +{Util.FloatToStr(ms7_UpgAtkSpeedPer.GetNextVal() * 100)}%";
                break;
            case (int)MUSH.MUSH8:
                mushFormat = ms8_IncPopulation;
                targetAbilityTxt.text = $"{LM._.Localize(LM.IncPopulation)} +{ms8_IncPopulation.Val} => +{ms8_IncPopulation.GetNextVal()}";
                break;
        }

        // 총 획득능력치 표시
        if(ms1_UpgAttack.Lv > 0) totalAbilityTxt.text += $"{LM._.Localize(LM.Attack)} +{ms1_UpgAttack.Val}\n";
        if(ms2_UpgMovSpeedPer.Lv > 0) totalAbilityTxt.text += $"{LM._.Localize(LM.MoveSpeed)} +{Util.FloatToStr(ms2_UpgMovSpeedPer.Val * 100)}%\n";
        if(ms3_UpgBagStoragePer.Val > 0) totalAbilityTxt.text += $"{LM._.Localize(LM.BagStorage)} +{Util.FloatToStr(ms3_UpgBagStoragePer.Val * 100)}%\n";
        if(ms4_UpgNextStageSkipPer.Val > 0) totalAbilityTxt.text += $"{LM._.Localize(LM.NextStageSkip)} {Util.FloatToStr(ms4_UpgNextStageSkipPer.Val * 100)}%\n";
        if(ms5_UpgIncTimer.Val > 0) totalAbilityTxt.text += $"{LM._.Localize(LM.MiningTime)} +{ms5_UpgIncTimer.Val}초\n";
        if(ms6_UpgChestSpawnPer.Val > 0) totalAbilityTxt.text += $"{LM._.Localize(LM.IncChestSpawnPer)} +{Util.FloatToStr(ms6_UpgChestSpawnPer.Val * 100)}%\n";
        if(ms7_UpgAtkSpeedPer.Val > 0) totalAbilityTxt.text += $"{LM._.Localize(LM.AttackSpeed)} +{Util.FloatToStr(ms7_UpgAtkSpeedPer.Val * 100)}%\n";
        if(ms8_IncPopulation.Val > 0) totalAbilityTxt.text += $"{LM._.Localize(LM.IncPopulation)} +{ms8_IncPopulation.Val}\n";

        targetImg.sprite = GM._.MushSprArr[mushIdx];
        targetNameTxt.text = $"{GM._.idm.INV_ITEM_INFO[offset + mushIdx].name}";
        targetLvTxt.text = $"LV. {mushFormat.Lv}";

        string colorTag = DM._.DB.statusDB.MsrArr[(int)mushFormat.NeedMush] >= mushFormat.Price? "white" : "red";
        upgradeBtnTxt.text = $"{LM._.Localize(LM.Upgrade)}\n<sprite name={mushFormat.NeedMush}> <color={colorTag}>{mushFormat.Price}</color>";
    }

    /// <summary>
    /// 업그레이드 결과 최신화
    /// </summary>
    private void UpdateDataAndUI()
    {
        UpdateData();
        UpdateUI();
    }
#endregion
}
