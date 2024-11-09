using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 실제 인게임에서의 스킬 발동 컨트롤
/// </summary>
public class SkillController : MonoBehaviour
{
    //* VALUE
    SkillManager skm;
    MineManager mnm;
    Coroutine corActiveSkillID;
    Coroutine corBuffSkillIntroID;

    public bool isActiveBuff;

    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);
        yield return new WaitUntil(() => GM._ != null);
        skm = GM._.skm;
        mnm = GM._.mnm;
    }

#region FUNC
    /// <summary>
    /// (인게임) 스킬발동 코루틴 실행
    /// </summary>
    public void ActiveSkill()
    {
        corActiveSkillID = StartCoroutine(CoActiveSkill());
    }

    /// <summary>
    /// 인게임 종료시 스킬발동 및 애니메이션 코루틴 종료
    /// </summary>
    public void StopActiveSkill()
    {
        isActiveBuff = false;

        // 버프인트로 애니메이션 팝업 비표시
        Array.ForEach(skm.introGradeAnimArr, introGradeAnim => introGradeAnim.windowObj.SetActive(false));

        if(corActiveSkillID != null)
        {
            StopCoroutine(corActiveSkillID);
            corActiveSkillID = null;
        }
        if(corBuffSkillIntroID != null)
        {
            StopCoroutine(corBuffSkillIntroID);
            corBuffSkillIntroID = null;
        }
    }

    /// <summary>
    /// 스킬발동 처리
    /// </summary>
    IEnumerator CoActiveSkill()
    {
        while(true)
        {
            yield return Util.TIME5;
            RandomSkill();
        }
    }

    /// <summary>
    /// 랜덤스킬 설정
    /// </summary>
    private void RandomSkill()
    {
        SkillCate randSkillCate = (SkillCate)Random.Range((int)SkillCate.Buff, (int)SkillCate.Skip + 1);

        //! TEST BUFF
        randSkillCate = SkillCate.Buff;

        switch(randSkillCate)
        {
            case SkillCate.Buff:
                StartCoroutine(CoBuffSkill());
                break;
            case SkillCate.Attack:
                break;
            case SkillCate.Skip:
                break;
        }
    }

    /// <summary>
    /// 버프스킬
    /// </summary>
    IEnumerator CoBuffSkill()
    {
        BuffSkillTree skill = skm.buffSkill;

        int lv = skill.Lv;
        int buffColorIdx = (lv <= 1)? 0 : (lv <= 3)? 1 : (lv <= 4)? 2 : 3;

        //! 캐릭터 등급랜덤 설정해야됨
        int grade = lv;

        SoundManager._.PlaySfx(SoundManager.SFX.OpenMushBoxSFX);

        GM._.ui.ShowNoticeMsgPopUp("버프스킬 발동");
        corBuffSkillIntroID = StartCoroutine(skm.introGradeAnimArr[grade].CoPlay());
        isActiveBuff = true;
        SetAllWorkerBuffFireEF(true, buffColorIdx);

        yield return skill.Time;
        GM._.ui.ShowNoticeMsgPopUp("버프스킬 종료");
        SetAllWorkerBuffFireEF(false, 0);
        isActiveBuff = false;
    }

    /// <summary>
    /// 버프 파이어 이펙트 ON
    /// </summary>
    private void SetAllWorkerBuffFireEF(bool isActive, int buffColorIdx = 0)
    {
        for(int i = 0; i < mnm.workerGroupTf.childCount; i++)
        {
            MiningController worker = mnm.workerGroupTf.GetChild(i).GetComponent<MiningController>();
            worker.SetBuffFireEF(isActive, buffColorIdx);
        }
    }
#endregion
}
