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
    // Element
    public GameObject windowObj;
    public DOTweenAnimation DOTAnim;

    public Image targetImg;
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
    /// 팝업 열기
    /// </summary>
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        GM._.ui.topMushGroup.SetActive(true);
    }

    private void Upgrade(UpgradeMushFormat upgDt)
    {
        if(DM._.DB.statusDB.MsrArr[(int)upgDt.NeedMush] >= upgDt.Price)
        {
            GM._.ui.ShowNoticeMsgPopUp("업그레이드 성공!");
            DM._.DB.statusDB.SetMsrArr((int)upgDt.NeedMush, -upgDt.Price);
            upgDt.Lv++;

            UpdateDataAndUI();
        }
        else
            GM._.ui.ShowWarningMsgPopUp("해당 버섯이 부족합니다!");
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

    private void UpgradeUI()
    {
        const int offset = 9 + 8;
        UpgradeMushFormat mushFormat = null;
        targetValueTxt.text = $"보유량 : {DM._.DB.statusDB.MsrArr[mushIdx]}";
        totalAbilityTxt.text = "";

        // 현재 버섯아이템 능력 표시
        switch(mushIdx)
        {
            case (int)MUSH.MUSH1:
                mushFormat = ms1_UpgAttack;
                targetAbilityTxt.text = $"공격력 +{ms1_UpgAttack.Val} => +{ms1_UpgAttack.GetNextVal()}";
                break;
            case (int)MUSH.MUSH2:
                mushFormat = ms2_UpgMovSpeedPer;
                targetAbilityTxt.text = $"이동속도 +{ms2_UpgMovSpeedPer.Val * 100} => +{ms2_UpgMovSpeedPer.GetNextVal() * 100}%";
                break;
            case (int)MUSH.MUSH3:
                mushFormat = ms3_UpgBagStoragePer;
                targetAbilityTxt.text = $"가방용량 +{ms3_UpgBagStoragePer.Val * 100} => +{ms3_UpgBagStoragePer.GetNextVal() * 100}%";
                break;
            case (int)MUSH.MUSH4:
                mushFormat = ms4_UpgNextStageSkipPer;
                targetAbilityTxt.text = $"다음 층 스킵 {ms4_UpgNextStageSkipPer.Val * 100} => +{ms4_UpgNextStageSkipPer.GetNextVal() * 100}%";
                break;
            case (int)MUSH.MUSH5:
                mushFormat = ms5_UpgIncTimer;
                targetAbilityTxt.text = $"채굴시간 +{ms5_UpgIncTimer.Val} => +{ms5_UpgIncTimer.GetNextVal()}초";
                break;
            case (int)MUSH.MUSH6:
                mushFormat =ms6_UpgChestSpawnPer;
                targetAbilityTxt.text = $"상자 등장확률 +{ms6_UpgChestSpawnPer.Val * 100} => +{ms6_UpgChestSpawnPer.GetNextVal() * 100}%";
                break;
            case (int)MUSH.MUSH7:
                mushFormat = ms7_UpgAtkSpeedPer;
                targetAbilityTxt.text = $"공격속도 +{ms7_UpgAtkSpeedPer.Val * 100} => +{ms7_UpgAtkSpeedPer.GetNextVal() * 100}%";
                break;
            case (int)MUSH.MUSH8:
                mushFormat = ms8_IncPopulation;
                targetAbilityTxt.text = $"소환캐릭터 증가 +{ms8_IncPopulation.Val} => +{ms8_IncPopulation.GetNextVal()}";
                break;
        }

        // 총 획득능력치 표시
        if(ms1_UpgAttack.Lv > 0) totalAbilityTxt.text += $"공격력 +{ms1_UpgAttack.Val}\n";
        if(ms2_UpgMovSpeedPer.Lv > 0) totalAbilityTxt.text += $"이동속도 +{ms2_UpgMovSpeedPer.Val * 100}%\n";
        if(ms3_UpgBagStoragePer.Val > 0) totalAbilityTxt.text += $"가방용량 +{ms3_UpgBagStoragePer.Val * 100}%\n";
        if(ms4_UpgNextStageSkipPer.Val > 0) totalAbilityTxt.text += $"다음 층 스킵 {ms4_UpgNextStageSkipPer.Val * 100}%\n";
        if(ms5_UpgIncTimer.Val > 0) totalAbilityTxt.text += $"채굴시간 +{ms5_UpgIncTimer.Val}초\n";
        if(ms6_UpgChestSpawnPer.Val > 0) totalAbilityTxt.text += $"상자 등장확률 +{ms6_UpgChestSpawnPer.Val * 100}%\n";
        if(ms7_UpgAtkSpeedPer.Val > 0) totalAbilityTxt.text += $"공격속도 +{ms7_UpgAtkSpeedPer.Val * 100}%\n";
        if(ms8_IncPopulation.Val > 0) totalAbilityTxt.text += $"소환캐릭터 증가 +{ms8_IncPopulation.Val}\n";

        targetImg.sprite = GM._.MushSprArr[mushIdx];
        targetNameTxt.text = $"{INV_ITEM_INFO[offset + mushIdx].name}";
        targetLvTxt.text = $"LV. {mushFormat.Lv}";

        string colorTag = DM._.DB.statusDB.MsrArr[(int)mushFormat.NeedMush] >= mushFormat.Price? "white" : "red";
        upgradeBtnTxt.text = $"업그레이드\n<sprite name={mushFormat.NeedMush}> <color={colorTag}>{mushFormat.Price}</color>";
    }

    /// <summary>
    /// 업그레이드 결과 최신화
    /// </summary>
    private void UpdateDataAndUI()
    {
        UpdateData();
        UpgradeUI();
    }
#endregion
}
