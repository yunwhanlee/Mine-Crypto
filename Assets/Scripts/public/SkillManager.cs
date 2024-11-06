using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

/// <summary>
/// 스킬 카테고리
/// </summary>
public enum SkillCate {Buff, Attack, Skip }

public class SkillManager : MonoBehaviour
{
    public DOTweenAnimation DOTAnim;

    //* ELEMENT
    public GameObject windowObj;
    public Image skillImg;
    public TMP_Text titleTxt;
    public TMP_Text descriptionTxt;

    //* VALUE
    string[] skillNameArr = { "힘내라 친구여", "우주 대폭발", "시공을 거슬러" };
    public SkillCate cate;
    
    public int curIdx;

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

        Array.ForEach(buffSkill.skillTreeArr, skillTree => skillTree.UpdateDimUI());
        Array.ForEach(attackSkill.skillTreeArr, skillTree => skillTree.UpdateDimUI());
        Array.ForEach(skipSkill.skillTreeArr, skillTree => skillTree.UpdateDimUI());
    }

#region EVENT
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
#endregion

#region FUNC
    /// <summary>
    /// 팝업표시
    /// </summary>
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        // UpdateDataAndUI();
    }

    /// <summary>
    /// 선택한 스킬 상세설명 표시
    /// </summary>
    public void DisplaySkillDetail(int idx, SkillCate skillCate)
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        titleTxt.text = $"{skillNameArr[(int)skillCate]} Lv{idx + 1}";

        switch(skillCate) {
            case SkillCate.Buff:
                skillImg.sprite = buffSkill.skillTreeArr[idx].IconSpr;
                descriptionTxt.text = buffSkill.GetDescription(idx);
                break;
            case SkillCate.Attack:
                skillImg.sprite = attackSkill.skillTreeArr[idx].IconSpr;
                descriptionTxt.text = attackSkill.GetDescription(idx);
                break;
            case SkillCate.Skip:
                skillImg.sprite = skipSkill.skillTreeArr[idx].IconSpr;
                descriptionTxt.text = skipSkill.GetDescription(idx);
                break;
        }
    }
#endregion
}
