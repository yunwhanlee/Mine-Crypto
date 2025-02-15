using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using static SoundManager;
using static Enum;
using TMPro;

/// <summary>
/// 실제 인게임에서의 스킬 발동 컨트롤
/// </summary>
public class SkillController : MonoBehaviour
{
    const int WAIT_COOLTIME = 60;
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
        GM._.skm.skillCooltimeObj.SetActive(false);

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
        // 스킬 쿨타임시간 
        int skipDefCoolTime = Mathf.RoundToInt(WAIT_COOLTIME * skm.skipSkill.DecSkillCoolTimePer);
        int waitTime = skipDefCoolTime - GM._.rbm.upgDecSkillTime.Val;
        // 스킬쿨타임 처음 초기화
        coolTime = waitTime;

        GM._.skm.skillCooltimeObj.SetActive(true);

        while(true)
        {
            GM._.skm.skillCooltimeTxt.text = $"{coolTime}";

            if(coolTime <= 0)
            {
                // 스킬쿨타임 초기화
                coolTime = waitTime;
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
        {
            GM._.ui.ShowNoticeMsgPopUp(LM._.Localize(LM.SkillUnActiveMsg)); // 종료시간이 임박해서 스킬발동이 사라집니다.
            return;
        }

        SkillCate randSkillCate;

        // 공격용스킬 올클리어가 남아있다면, 랜덤스킬에서 제외
        int start = skm.attackSkill.allClearBonusCnt > 0? (int)SkillCate.Buff : (int)SkillCate.Attack;
        int end = (int)SkillCate.Skip + 1;

        randSkillCate = (SkillCate)Random.Range(start, end);

        switch(randSkillCate)
        {
            case SkillCate.Attack:
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
        SoundManager._.PlaySfx(SFX.OpenMushBoxSFX);

        // 스킬레벨
        BuffSkillTree bufskill = skm.buffSkill;

        // 소환된 캐릭터 등급배열표에 따른 랜덤 스킬등급 선택
        int[] workerGradeTbArr = GM._.mnm.GetWorkerGradeTableArr();
        bufskill.skillGrade = workerGradeTbArr[Random.Range(0, workerGradeTbArr.Length)];

        // 함성사운드
        if(bufskill.skillGrade == (int)GRADE.UNIQUE
        || bufskill.skillGrade == (int)GRADE.LEGEND)
            SoundManager._.PlaySfx(SFX.SkillBuffWoman_SFX);
        else
            SoundManager._.PlaySfx(SFX.SkillBuffMan_SFX);

        corSkillIntroGradeAnimID = StartCoroutine(skm.introGradeAnimArr[bufskill.skillGrade].CoPlay(SkillCate.Buff, bufskill.Lv));
        isActiveBuff = true;
        SetAllWorkerBuffFireEF(true, bufskill.skillGrade);

        yield return bufskill.Time;
        SetAllWorkerBuffFireEF(false, 0);
        isActiveBuff = false;
    }

    /// <summary>
    /// 버프 파이어 이펙트 ON
    /// </summary>
    /// <param name="isActive"></param>
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
        // 소환된 캐릭터 등급배열표에 따른 랜덤 스킬등급 선택
        int[] workerGradeTbArr = GM._.mnm.GetWorkerGradeTableArr();
        atkSkill.skillGrade = workerGradeTbArr[Random.Range(0, workerGradeTbArr.Length)];

        // 캐릭터 등급 인트로 애니메이션
        corSkillIntroGradeAnimID = StartCoroutine(skm.introGradeAnimArr[atkSkill.skillGrade].CoPlay(SkillCate.Attack, atkSkill.Lv));

        // 카메라 흔들림 애니메이션
        Camera.main.GetComponent<DOTweenAnimation>().DORestart();

        yield return Util.TIME1;
        // 지진사운드
        _.PlayRandomSfxs(SFX.EarthQuakeA_SFX, SFX.EarthQuakeA_SFX);

        // 광석이 있을경우만, 광석 피격사운드 및 아이템획득 사운드 1회 실행
        if(GM._.mnm.oreGroupTf.childCount > 0) {
            _.PlayRandomSfxs(SFX.Metal1SFX, SFX.Metal2SFX);
            _.PlayRandomSfxs(SFX.ItemDrop1SFX, SFX.ItemDrop2SFX);
        }

        // 지진오브젝트 활성화
        atkSkill.ActiveEarthQuakeObj(atkSkill.skillGrade);

        for(int i = 0; i < GM._.mnm.oreGroupTf.childCount; i++)
        {
            // 타겟 광석
            Ore targetOre = GM._.mnm.oreGroupTf.GetChild(i).GetComponent<Ore>();

            if(targetOre == null)
                continue;

            // 크리스탈타입(보물상자)인 경우, 획득 제외
            if(targetOre.OreType != RSC.CRISTAL)
            {
                // 인게임 채굴재화 수령
                MiningController.AcceptRsc(targetOre.OreType, atkSkill.EarthQuakeDmg, isNoSFX: true);
            }

            // 광석 체력감소
            MiningController.DecreaseOreHpBar(targetOre, atkSkill.EarthQuakeDmg, isNoSFX: true);
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
        var skipSkill = skm.skipSkill;

        _.PlaySfx(SFX.Portal_SFX);

        // 소환된 캐릭터 등급배열표에 따른 랜덤 스킬등급 선택
        int[] workerGradeTbArr = GM._.mnm.GetWorkerGradeTableArr();
        skipSkill.grade = workerGradeTbArr[Random.Range(0, workerGradeTbArr.Length)];

        // 캐릭터 등급 인트로 애니메이션
        corSkillIntroGradeAnimID = StartCoroutine(skm.introGradeAnimArr[skipSkill.grade].CoPlay(SkillCate.Skip, skipSkill.Lv));

        yield return Util.TIME0_5;

        // 광석 전부제거
        GM._.mnm.CurTotalMiningCnt = 0;
        for(int j = 0; j < GM._.mnm.oreGroupTf.childCount; j++)
        {
            // 타겟 광석
            Ore targetOre = GM._.mnm.oreGroupTf.GetChild(j).GetComponent<Ore>();

            if(targetOre == null)
                continue;

            targetOre.IsDestroied = true;
            Destroy(targetOre.gameObject);
        }

        // 캐릭터 가운데로 강제이동
        for(int i = 0; i < mnm.workerGroupTf.childCount; i++)
        {
            MiningController worker = mnm.workerGroupTf.GetChild(i).GetComponent<MiningController>();
            worker.transform.position = GM._.mnm.homeTf.position;
        }

        skipSkill.PlaySkipAnim(skipSkill.grade);

        // 보너스상자 획득 (스킬레벨 LV4이상)
        skipSkill.CheckBonusChestLv4();

        int targetFloor = GM._.stgm.Floor + skipSkill.MoveNextFloor;
        StartCoroutine(GM._.stgm.ShowStageUpAnim(targetFloor));

        // 스테이지 층 상승
        GM._.stgm.Floor = targetFloor;

        // 타이머시간 감소 텍스트 애니메이션 표시
        float decTimeValPer = 1 - skipSkill.DecTimerPer;
        int decTimeVal = (int)(GM._.pm.TimerVal * decTimeValPer);
        int min = decTimeVal / 60;
        int sec = decTimeVal % 60;
        skipSkill.DecTimerTxtAnim.GetComponent<TMP_Text>().text = $"-{min:00} : {sec:00}";
        skipSkill.DecTimerTxtAnim.DORestart();
        skipSkill.DecTimerPtcEF.Play();

        // 타이머 % 감소
        GM._.pm.TimerVal = (int)(GM._.pm.TimerVal * skipSkill.DecTimerPer);

        // 스킬쿨타임 감소 텍스트 애니메이션 표시
        if(skipSkill.Lv >= 2) 
        {
            float decSkillCoolTimeValPer = 1 - skipSkill.DecSkillCoolTimePer;
            int decSkillCoolTime = Mathf.RoundToInt(GM._.skc.coolTime * decSkillCoolTimeValPer);
            skipSkill.DecSkillCoolTimeTxtAnim.GetComponent<TMP_Text>().text = $"-{decSkillCoolTime}";
            skipSkill.DecSkillCoolTimeTxtAnim.DORestart();
            skipSkill.DecSkillCoolTimePtcEF.Play();
        }

        // 스킬쿨타임 % 감소
        // RandomSkill()발동 쿨타임변수에 직접적용

        yield return Util.TIME3;
        yield return Util.TIME1;
        skipSkill.EndSkipAnim();

        yield return Util.TIME1;
        // 타이틀표시 및 이펙트
        GM._.stgm.ShowStageTitleUIAnim($"{GM._.stgm.Floor}{LM._.Localize(LM.Floor)}\n<color=yellow><size=30%>( +{skipSkill.MoveNextFloor}{LM._.Localize(LM.Floor)} )</size></color>");
    }
    #endregion
#endregion
}
