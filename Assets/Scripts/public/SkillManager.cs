using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class SkillManager : MonoBehaviour
{
    public DOTweenAnimation DOTAnim;

    //* ELEMENT
    public GameObject windowObj;

    //* VALUE
    public SkillTreeCate cate;
    public int curIdx;

    // Skill UI & Data
    public SkillTree[] buffSkillTreeArr;
    public SkillTree[] attackSkillTreeArr;
    public SkillTree[] skipSkillTreeArr;

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

        Array.ForEach(buffSkillTreeArr, skillTree => skillTree.UpdateDimUI());
        Array.ForEach(attackSkillTreeArr, skillTree => skillTree.UpdateDimUI());
        Array.ForEach(skipSkillTreeArr, skillTree => skillTree.UpdateDimUI());
    }

#region EVENT
    public void OnClickBuffSkillIconBtn(int idx)
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
    }
    public void OnClickAttackSkillIconBtn(int idx)
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
    }
    public void OnClickSkipSkillIconBtn(int idx)
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
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
#endregion
}
