using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using static SoundManager;
using static Enum;

/// <summary>
/// 실제 인게임에서의 스킬 발동 컨트롤
/// </summary>
public class SkillController : MonoBehaviour
{
    const int WAIT_COOLTIME = 10;
    public int coolTime;

    //* VALUE
    SkillManager skm;
    MineManager mnm;
    Coroutine corActiveSkillID;                     // 1분마다 스킬발동 ID
    Coroutine corSkillIntroGradeAnimID;             // 스킬 인트로 등급 애니메이션 ID
    Coroutine corAttackSkillID;                     // 공격용 지진스킬 ID
    Coroutine corActiveMeteoSkillID;                // 공격용 메테오스킬 ID
    Coroutine corMeteoLoopID;                       // 공격용 메테오루프 ID

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

        if(corActiveSkillID != null) {
            StopCoroutine(corActiveSkillID);
            corActiveSkillID = null;
        }
        if(corSkillIntroGradeAnimID != null) {
            StopCoroutine(corSkillIntroGradeAnimID);
            corSkillIntroGradeAnimID = null;
        }
        if(corActiveMeteoSkillID != null) {
            StopCoroutine(corActiveMeteoSkillID);
            corActiveMeteoSkillID = null;
        }
        if(corMeteoLoopID != null) {
            StopCoroutine(corMeteoLoopID);
            corMeteoLoopID = null;
        }
    }

    /// <summary>
    /// 1분마다 스킬발동 처리
    /// </summary>
    IEnumerator CoActiveSkill()
    {
        coolTime = WAIT_COOLTIME;

        while(true)
        {
            if(coolTime <= 0)
            {
                // 스킬쿨타임 초기화
                coolTime = WAIT_COOLTIME;
                // 만약에 공격용스킬 : 올 클리어 보너스가 발동중이라면, 끝날때까지 추가대기
                yield return new WaitUntil(() => GM._.skm.attackSkill.allClearBonusCnt <= 0);
                yield return Util.TIME1;
                // 랜덤스킬 발동
                RandomSkill();
            }
            
            else
            {
                --coolTime;
                yield return Util.TIME1;
            }
        }
    }

    /// <summary>
    /// 랜덤스킬 설정
    /// </summary>
    private void RandomSkill()
    {
        // 타이머가 8초 이하로 남았다면, 버그방지로 실행하지 않음.
        if(GM._.pm.TimerVal <= 8)
            return;

        SkillCate randSkillCate;

        // 공격용스킬 올클리어가 남아있다면, 랜덤스킬에서 제외
        int start = skm.attackSkill.allClearBonusCnt > 0? (int)SkillCate.Buff : (int)SkillCate.Attack;
        int end = (int)SkillCate.Skip + 1;

        randSkillCate = (SkillCate)Random.Range(start, end);

        randSkillCate = SkillCate.Skip; //! TEST

        switch(randSkillCate)
        {
            case SkillCate.Attack:
                /*
                if(skm.attackSkill.allClearBonusCnt > 0)
                {
                    if(corAttackSkillID != null)
                    {
                        // (중복지진 방지) 아직 올클리어카운트가 남았는데 실행될 경우, 이전 코루틴ID를 종료 후 재시작
                        StopCoroutine(corAttackSkillID);
                        corAttackSkillID = null;
                        StopCoroutine(corActiveMeteoSkillID);
                        corActiveMeteoSkillID = null;
                        StopCoroutine(corMeteoLoopID);
                        corMeteoLoopID = null;
                    }
                }
                */
                corAttackSkillID = StartCoroutine(CoAttackSkill());
                break;
            case SkillCate.Buff:
                StartCoroutine(CoBuffSkill());
                break;
            case SkillCate.Skip:
                StartCoroutine(CoSkipSkill());
                break;
        }
    }

    #region BUFF
    /// <summary>
    /// 버프스킬
    /// </summary>
    IEnumerator CoBuffSkill()
    {
        _.PlaySfx(SFX.OpenMushBoxSFX);

        // 스킬레벨
        BuffSkillTree skill = skm.buffSkill;
        int skLv = skill.Lv;

        int buffColorIdx = (skLv <= 1)? 0 : (skLv <= 3)? 1 : (skLv <= 4)? 2 : 3;

        //! 캐릭터 등급랜덤 설정해야됨
        int gradeIdx = skLv - 1;

        corSkillIntroGradeAnimID = StartCoroutine(skm.introGradeAnimArr[gradeIdx].CoPlay(SkillCate.Buff));
        isActiveBuff = true;
        SetAllWorkerBuffFireEF(true, buffColorIdx);

        yield return skill.Time;
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

    #region ATTACK
    public IEnumerator CoAttackSkill()
    {
        SoundManager._.PlaySfx(SFX.OpenMushBoxSFX);

        // 스킬레벨
        AttackSkillTree skill = skm.attackSkill;

        skill.SetAllClearBonusCntMax();

        corActiveMeteoSkillID = StartCoroutine(skill.CoActiveMeteoSkill());
        corMeteoLoopID = StartCoroutine(skill.CoMeteoLoop());

        yield return CoAttackSkill_EarthQauke(skill);
    }

    /// <summary>
    /// 공격용버프 지진
    /// </summary>
    public IEnumerator CoAttackSkill_EarthQauke(AttackSkillTree atkSkill)
    {
        //! 캐릭터 수량에 따른 랜덤등급 설정해야됨!
        atkSkill.skillGrade = Random.Range(0, (int)Enum.GRADE.CNT);

        // 캐릭터 등급 인트로 애니메이션
        corSkillIntroGradeAnimID = StartCoroutine(skm.introGradeAnimArr[atkSkill.skillGrade].CoPlay(SkillCate.Attack));

        // 카메라 흔들림 애니메이션
        Camera.main.GetComponent<DOTweenAnimation>().DORestart();

        yield return Util.TIME1;
        SoundManager._.PlayRandomSfxs(SFX.EarthQuakeA_SFX, SFX.EarthQuakeA_SFX);

        // 지진오브젝트 활성화
        atkSkill.ActiveEarthQuakeObj(atkSkill.skillGrade);

        SoundManager._.PlayRandomSfxs(SFX.ItemDrop1SFX, SFX.ItemDrop2SFX);

        for(int i = 0; i < GM._.mnm.oreGroupTf.childCount; i++)
        {
            // 타겟 광석
            Ore targetOre = GM._.mnm.oreGroupTf.GetChild(i).GetComponent<Ore>();

            if(targetOre == null)
                continue;

            // 인게임 채굴재화 수령
            MiningController.AcceptRsc(targetOre, atkSkill.EarthQuakeDmg);

            // 광석 체력감소
            MiningController.DecreaseOreHpBar(targetOre, atkSkill.EarthQuakeDmg);
        }

        yield return null;

        // 광석을 전부 제거했을시, 올클리어카운트 살리고, -1 카운팅
        if(GM._.mnm.oreGroupTf.childCount <= 0 && atkSkill.allClearBonusCnt > 0)
        {
            atkSkill.allClearBonusCnt--;
            atkSkill.SetAllClearBonusEF(true);
        }
        // 광석을 전부 제거못했을경우, 카운트를 0으로 없애고 종료
        else
        {
            atkSkill.allClearBonusCnt = 0;
        }

        Debug.Log($"지진후 남아있는 광석수: {GM._.mnm.oreGroupTf.childCount}, 올클리어카운트: {atkSkill.allClearBonusCnt}");

        yield return Util.TIME2;
        atkSkill.InitEarthQuakeObj();
        atkSkill.SetAllClearBonusEF(false);
    }
    #endregion

    #region SKIP
    public IEnumerator CoSkipSkill()
    {
        var skill = skm.skipSkill;

        _.PlaySfx(SFX.Portal_SFX);

        //! 캐릭터 수량에 따른 랜덤등급 설정해야됨!
        skill.grade = Random.Range(0, (int)Enum.GRADE.CNT);

        // GM._.gameState = GameState.TIMEOVER;

        // 캐릭터 등급 인트로 애니메이션
        corSkillIntroGradeAnimID = StartCoroutine(skm.introGradeAnimArr[skill.grade].CoPlay(SkillCate.Skip));

        yield return Util.TIME0_5;

        // 광석 전부제거
        for(int j = 0; j < GM._.mnm.oreGroupTf.childCount; j++)
        {
            // 타겟 광석
            Ore targetOre = GM._.mnm.oreGroupTf.GetChild(j).GetComponent<Ore>();

            if(targetOre == null)
                continue;

            MiningController.DecreaseOreHpBar(targetOre, targetOre.MaxHp);
            MiningController.AcceptRsc(targetOre, targetOre.MaxHp);
        }

        // 캐릭터 가운데로 강제이동
        for(int i = 0; i < mnm.workerGroupTf.childCount; i++)
        {
            MiningController worker = mnm.workerGroupTf.GetChild(i).GetComponent<MiningController>();
            worker.transform.position = GM._.mnm.homeTf.position;
        }

        skill.PlaySkipAnim(skill.grade);

        // 보너스상자 획득 (스킬레벨 LV4이상)
        skill.CheckBonusChestLv4();

        int targetFloor = GM._.stgm.Floor + skill.MoveNextFloor;
        StartCoroutine(GM._.stgm.ShowStageUpAnim(targetFloor));

        // 스테이지 층 상승
        GM._.stgm.Floor = targetFloor;
        // 타이머 % 감소
        GM._.pm.TimerVal = (int)(GM._.pm.TimerVal * skill.DecTimerPer);
        // 스킬쿨타임 % 감소
        coolTime = (int)(coolTime * skill.DecSkillCoolTimePer);

        yield return Util.TIME3;

        yield return Util.TIME1;
        // GM._.gameState = GameState.PLAY;
        skill.EndSkipAnim();

        yield return Util.TIME1;
        // 타이틀표시 및 이펙트
        GM._.stgm.ShowStageTitleUIAnim($"{GM._.stgm.Floor}{LM._.Localize(LM.Floor)}");
    }
    #endregion
#endregion
}
