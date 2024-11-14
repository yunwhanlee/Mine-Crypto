using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using AssetKits.ParticleImage;
using static Enum;

/// <summary>
/// 스킬 카테고리
/// </summary>
public enum SkillCate {Attack, Buff, Skip}

[System.Serializable]
public class SkillGradeIntroAnim
{
    public GameObject windowObj;
    public TMP_Text TitleTxt;
    public DOTweenAnimation msgBarAnim;
    public DOTweenAnimation charaAnim;

    /// <summary>
    /// 버프스킬 인트로 애니메이션 실행
    /// </summary>
    public IEnumerator CoPlay(SkillCate cate)
    {
        TitleTxt.text = $"{GM._.skm.skillNameArr[(int)cate]}";

        windowObj.SetActive(true);
        msgBarAnim.DORestart();
        charaAnim.DORestart();
        yield return Util.TIME2_5;
        windowObj.SetActive(false);
    }
}


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
    public SkillGradeIntroAnim[] introGradeAnimArr; // 스킬 인트로 등급 애니메이션

    public string[] skillNameArr = { "우주 대폭발", "힘내라 친구여", "시공을 거슬러" };
    private int[] skillPriceArr = { 0, 10, 20, 30 ,40 }; // 스킬가격 LV1은 기본적용 되어있음

    // Skill UI & Data
    public AttackSkillTree attackSkill;
    public BuffSkillTree buffSkill;
    public SkipSkillTree skipSkill;

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // skillTreeDB가 null일 경우 새로 초기화
        if (DM._.DB.skillTreeDB == null)
        {
            Debug.Log($"<color=red>{this.name}DB데이터가 없음으로 자체 초기화</color>");
            DM._.DB.skillTreeDB = new SkillTreeDB();
            DM._.DB.skillTreeDB.Init();
        }

        OnClickAttackSkillIconBtn(0);
        UpdateUI();
    }

#region EVENT
    /// <summary>
    /// 스킬아이콘 클릭 공격형, 버프형, 스킵형 3가지 함수
    /// </summary>
    public void OnClickAttackSkillIconBtn(int idx)
    {
        DisplaySkillDetail(idx, SkillCate.Attack);
    }
    public void OnClickBuffSkillIconBtn(int idx)
    {
        DisplaySkillDetail(idx, SkillCate.Buff);
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
            case SkillCate.Attack:
                attackSkill.Lv += LearnSkill(attackSkill.Lv);
                break;
            case SkillCate.Buff:
                buffSkill.Lv += LearnSkill(buffSkill.Lv);
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
        // 구매한 스킬이 없다면 실행 안 함
        if(attackSkill.Lv == 1 && buffSkill.Lv == 1 && skipSkill.Lv == 1)
            return;

        GM._.ui.ShowConfirmPopUp("스킬을 전부 리셋하시겠습니까?\n스킬포인트 물약은 전부 반환됩니다.");
        GM._.ui.OnClickConfirmBtnAction = () => {

            if(DM._.DB.statusDB.GetInventoryItemVal(INV.CRISTAL) < 1000)
            {
                // 구매를위한 아이템이 부족합니다.
                GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
                return;
            }


            SoundManager._.PlaySfx(SoundManager.SFX.BlessResetSFX);
            GM._.ui.ShowNoticeMsgPopUp("스킬 초기화 완료!");

            // 스킬포인트 물약 반환
            int total = 0;
            for(int i = 0; i < skillPriceArr.Length; i++)
            {
                int val = skillPriceArr[i];
                if(i <= attackSkill.Lv - 1) total += val;
                if(i <= buffSkill.Lv - 1) total += val;
                if(i <= skipSkill.Lv - 1) total += val;
            }

            Debug.Log($"SkillPoint Reset total= {total}");

            // 포션 반환
            GM._.rwm.ShowReward ( new Dictionary<RWD, int> {
                { RWD.SKILLPOTION, total }
            });

            // 스킬레벨 데이터 초기화
            DM._.DB.skillTreeDB.Init();

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
            case SkillCate.Attack:
                attackSkill.skillTreeArr[idx].SetSelectedBorderUI();
                skillImg.sprite = attackSkill.skillTreeArr[idx].IconSpr;
                descriptionTxt.text = attackSkill.GetDescription(idx);
                break;
            case SkillCate.Buff:
                buffSkill.skillTreeArr[idx].SetSelectedBorderUI();
                skillImg.sprite = buffSkill.skillTreeArr[idx].IconSpr;
                descriptionTxt.text = buffSkill.GetDescription(idx);
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
