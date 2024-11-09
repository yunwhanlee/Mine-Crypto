using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using AssetKits.ParticleImage;

/// <summary>
/// 스킬 카테고리
/// </summary>
public enum SkillCate {Buff, Attack, Skip}

public class SkillManager : MonoBehaviour
{
    public DOTweenAnimation DOTAnim;

    //* ELEMENT
    public GameObject windowObj;
    public Image skillImg;
    public TMP_Text mySkillPotionCntTxt;   // 현재 스킬포인트 보유량
    public TMP_Text titleTxt;
    public TMP_Text descriptionTxt;
    public TMP_Text priceTxt;
    public ParticleImage learnBtnParticleEF;

    //* VALUE
    public SkillCate cate;
    public SkillCate curSelectCate;  // 현재 선택된 스킬 카테고리
    public int curSelectIdx; // 현재 선택된 스킬 LV인덱스
    string[] skillNameArr = { "힘내라 친구여", "우주 대폭발", "시공을 거슬러" };
    int[] skillPriceArr = { 0, 10, 20, 30 ,40 }; // 스킬가격 LV1은 기본적용 되어있음
    

    // Skill UI & Data
    public BuffSkillTree buffSkill;
    public AttackSkillTree attackSkill;
    public SkipSkillTree skipSkill;

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // skillTreeDB가 null일 경우 새로 초기화
        if (DM._.DB.skillTreeDB == null)
        {
            Debug.Log("<color=red>데이터가 없음으로 자체 초기화</color>");
            DM._.DB.skillTreeDB = new SkillTreeDB();
            DM._.DB.skillTreeDB.Init();
        }

        UpdateUI();
    }

#region EVENT
    /// <summary>
    /// 스킬아이콘 클릭 버프형, 공격형, 스킵형 3가지 함수
    /// </summary>
    public void OnClickBuffSkillIconBtn(int idx)
    {
        DisplaySkillDetail(idx, SkillCate.Buff);
    }
    public void OnClickAttackSkillIconBtn(int idx)
    {
        DisplaySkillDetail(idx, SkillCate.Attack);
    }
    public void OnClickSkipSkillIconBtn(int idx)
    {
        DisplaySkillDetail(idx, SkillCate.Skip);
    }

    /// <summary>
    /// 선택스킬 배우기 버튼
    /// </summary>
    public void OnClickLearnSkill()
    {
        switch(curSelectCate) {
            case SkillCate.Buff:
                buffSkill.Lv += LearnSkill(buffSkill.Lv);
                break;
            case SkillCate.Attack:
                attackSkill.Lv += LearnSkill(attackSkill.Lv);
                break;
            case SkillCate.Skip:
                skipSkill.Lv += LearnSkill(skipSkill.Lv);
                break;
        }

        UpdateUI();
    }

    /// <summary>
    /// 스킬포인트 리셋 버튼
    /// </summary>
    public void OnClickResetBtn()
    {
        GM._.ui.ShowConfirmPopUp("스킬을 전부 리셋하시겠습니까?\n스킬포인트 물약은 전부 반환됩니다.");
        GM._.ui.OnClickConfirmBtnAction = () => {
            SoundManager._.PlaySfx(SoundManager.SFX.BlessResetSFX);
            GM._.ui.ShowNoticeMsgPopUp("스킬 초기화 완료!");

            // 스킬레벨 데이터 초기화
            DM._.DB.skillTreeDB.Init();
            // 스킬포인트 물약 반환

            UpdateUI();
        };
    }
#endregion

#region FUNC
    /// <summary>
    /// 팝업표시
    /// </summary>
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        UpdateUI();
    }

    private void UpdateUI()
    {
        mySkillPotionCntTxt.text = DM._.DB.statusDB.SkillPotion.ToString();

        // 스킬 잠김상태
        Array.ForEach(buffSkill.skillTreeArr, skillTree => skillTree.UpdateDimUI());
        Array.ForEach(attackSkill.skillTreeArr, skillTree => skillTree.UpdateDimUI());
        Array.ForEach(skipSkill.skillTreeArr, skillTree => skillTree.UpdateDimUI());
    }

    /// <summary>
    /// 스킬아이콘 테두리 전부 미선택으로 초기화
    /// </summary>
    private void InitSkillIconBorder()
    {
        Array.ForEach(buffSkill.skillTreeArr, skillTree => skillTree.InitBorderUI());
        Array.ForEach(attackSkill.skillTreeArr, skillTree => skillTree.InitBorderUI());
        Array.ForEach(skipSkill.skillTreeArr, skillTree => skillTree.InitBorderUI());
    }

    /// <summary>
    /// 선택한 스킬 상세설명 표시
    /// </summary>
    public void DisplaySkillDetail(int idx, SkillCate skillCate)
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);

        // 현재 선택된 스킬로 카테고리 및 인덱스 데이터 최신화
        curSelectCate = skillCate;
        curSelectIdx = idx;

        // UI 최신화
        titleTxt.text = $"{skillNameArr[(int)skillCate]} Lv{idx + 1}";
        priceTxt.text = $"{skillPriceArr[idx]}";

        InitSkillIconBorder();

        switch(skillCate) {
            case SkillCate.Buff:
                buffSkill.skillTreeArr[idx].SetSelectedBorderUI();
                skillImg.sprite = buffSkill.skillTreeArr[idx].IconSpr;
                descriptionTxt.text = buffSkill.GetDescription(idx);
                break;
            case SkillCate.Attack:
                attackSkill.skillTreeArr[idx].SetSelectedBorderUI();
                skillImg.sprite = attackSkill.skillTreeArr[idx].IconSpr;
                descriptionTxt.text = attackSkill.GetDescription(idx);
                break;
            case SkillCate.Skip:
                skipSkill.skillTreeArr[idx].SetSelectedBorderUI();
                skillImg.sprite = skipSkill.skillTreeArr[idx].IconSpr;
                descriptionTxt.text = skipSkill.GetDescription(idx);
                break;
        }
    }

    /// <summary>
    /// 스킬구매 처리
    /// </summary>
    /// <param name="lv">구매하려는 스킬레벨</param>
    /// <returns>구매가 가능하면 +1을 반환하여 레벨상승, 아니면 0을 반환</returns>
    private int LearnSkill(int lv)
    {
        var sttDB = DM._.DB.statusDB;
        int skillPrice = skillPriceArr[curSelectIdx];

        if(lv > curSelectIdx)
        {
            GM._.ui.ShowWarningMsgPopUp("이미 습득한 스킬입니다.");
            return 0;
        }
        else if(lv < curSelectIdx)
        {   
            GM._.ui.ShowWarningMsgPopUp("이전 레벨스킬을 배워야됩니다.");
            return 0;
        }
        else {
            // 스킬포션 수량 체크
            if(sttDB.GetInventoryItemVal(Enum.INV.SKILLPOTION) < skillPrice )
            {
                GM._.ui.ShowWarningMsgPopUp("스킬포인트 물약이 부족합니다.");
                return 0;
            }

            learnBtnParticleEF.Play();
            SoundManager._.PlaySfx(SoundManager.SFX.TranscendUpgradeSFX);
            GM._.ui.ShowNoticeMsgPopUp("스킬 습득완료!");
            sttDB.SetInventoryItemVal(Enum.INV.SKILLPOTION, -skillPrice);
            return 1;
        }
    }
#endregion
}
