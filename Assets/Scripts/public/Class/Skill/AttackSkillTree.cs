using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;
using static SoundManager;

/// <summary>
/// 두번째 스킬(공격형)
/// </summary>
[System.Serializable]
public class AttackSkillTree
{
    public SkillTree[] skillTreeArr;
    public GameObject[] earthQuakeEFArr;    // 지진공격이펙트 배열
    public GameObject meteoParticleEF;

    public GameObject allClearBonusEFObj; // 올클리어 보너스 부모 오브젝트
    public DOTweenAnimation allClearBonusTxtAnim; // 올클리어 보너스 텍스트 애니메이션
    public TMP_Text allClearBonusTxt; // 올클리어 보너스 텍스트

    public int skillGrade;

    // 스킬레벨
    public int Lv {
        get => DM._.DB.skillTreeDB.attackSkillTreeLv;
        set => DM._.DB.skillTreeDB.attackSkillTreeLv = value;
    }
    // 올클리어 지진 발동횟수 (다음층마다 1번씩 발동)
    public int allClearBonusCnt = 0;
    // 올클리어 지진 최대횟수
    public int AllClearBonusMax {
        get {
            if(Lv == 5) return 10;
            else if(Lv >= 3) return 3;
            else return 1;
        }
    }
    // 메테오 지속시간
    public WaitForSeconds MeteoTime {
        get {
            if(Lv >= 4) return Util.TIME10;
            else if(Lv >= 2) return Util.TIME5;
            else return null;
        }
    }
    // 지진 공격력
    public int EarthQuakeDmg {
        get {
            int[] dmgGradeArr = { 200, 400, 800, 1500, 3000, 6000 };
            int dmg = dmgGradeArr[skillGrade];
            int extraVal = GM._.sttm.ExtraAtk;
            float extraPer = 1 + GM._.sttm.ExtraAtkPer;

            int result = Mathf.RoundToInt((dmg + extraVal) * extraPer);
            return result;
        }
    }
    // 메테오 공격력
    public int MeteoDmg {
        get {
            int[] dmgGradeArr = { 50, 100, 200, 400, 1000, 2000 };
            int dmg = dmgGradeArr[skillGrade];
            int extraVal = GM._.sttm.ExtraAtk;
            float extraPer = 1 + GM._.sttm.ExtraAtkPer;

            int result = Mathf.RoundToInt((dmg + extraVal) * extraPer);
            return result;
        }
    }

#region METEO
    /// <summary>
    /// 메테오 스킬 발동
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoActiveMeteoSkill()
    {
        meteoParticleEF.SetActive(true);
        yield return MeteoTime;
        meteoParticleEF.SetActive(false);
    }

    public IEnumerator CoMeteoLoop()
    {
        int cnt = (MeteoTime == Util.TIME10)? 10 : (MeteoTime == Util.TIME5)? 5 : 0;

        for(int i = 0; i < cnt; i++)
        {
            yield return Util.TIME1;
            _.PlayRandomSfxs(SFX.FireballA_SFX, SFX.FireballB_SFX);
            MeteoAttack();
        }
    }

    /// <summary>
    /// 메테오 공격처리
    /// </summary>
    private void MeteoAttack()
    {
        for(int j = 0; j < GM._.mnm.oreGroupTf.childCount; j++)
        {
            // 타겟 광석
            Ore targetOre = GM._.mnm.oreGroupTf.GetChild(j).GetComponent<Ore>();

            if(targetOre == null)
                continue;

            MiningController.DecreaseOreHpBar(targetOre, MeteoDmg);
            MiningController.AcceptRsc(targetOre, MeteoDmg);
        }
    }
#endregion

#region EARTHQUAKE
    /// <summary>
    /// 올클리어 카운트 설정
    /// </summary>
    public void SetAllClearBonusCntMax()
    {
        if(allClearBonusCnt <= 0)
            allClearBonusCnt = AllClearBonusMax;
    }

    /// <summary>
    /// 올클리어 보너스 남은카운트 체크(남은 횟수만큼 다음층마다 발동)
    /// </summary>
    public bool CheckAllClearBonusCnt()
    {
        int cnt = allClearBonusCnt;
        int max = AllClearBonusMax;

        Debug.Log($"CheckAllClearBonusCnt():: {cnt > 0 && cnt < max} = AllClearBonusCnt= {cnt}, allClearBonusMax= {max}");
        return 0 < cnt && cnt < max;
    }

    /// <summary>
    /// 올 클리어 이펙트 표시
    /// </summary>
    public void SetAllClearBonusEF(bool isActive)
    {
        // 올클리어 이펙트 레벨3 이하라면 안함
        if(Lv < 3)
            return;

        if(isActive)
        {
            allClearBonusEFObj.SetActive(true);
            allClearBonusTxtAnim.DORestart();
            string cntTxt = allClearBonusCnt > 0? $"( {allClearBonusCnt} / {AllClearBonusMax} ) " : "";
            allClearBonusTxt.text = $"ALL CLEAR BONUS !\n{cntTxt}";
        }
        else
            allClearBonusEFObj.SetActive(false);
    }

    public void InitEarthQuakeObj()
    {
        for(int i = 0; i < earthQuakeEFArr.Length; i++)
            earthQuakeEFArr[i].SetActive(false);
    }

    /// <summary>
    /// 등급만큼 지진오브젝트 숫자 활성화
    /// </summary>
    /// <param name="maxIdx">활성화할 지진오브젝트 인덱스</param>
    public void ActiveEarthQuakeObj(int maxIdx)
    {
        InitEarthQuakeObj();

        for(int i = 0; i < earthQuakeEFArr.Length; i++)
        {
            earthQuakeEFArr[i].SetActive(i <= maxIdx);
        }
    }
#endregion

    /// <summary>
    /// 스킬 상세설명
    /// </summary>
    public string GetDescription(int idx)
    {
        switch(idx)
        {
            default: return "전범위 지진을일으켜 (200/400/800/1500/3000) + 공격력증가 x 공격력증가%의 피해를 준다.";
            case 1: return "5초간 메테오를 내려 초당 (50/100/200/400/1000) + 공격력증가 x 공격력증가%의 피해를 준다.";
            case 2: return "지진으로 모든광석을 파괴하면 다음층 이동 후 1회 더 사용. (최대3회)";
            case 3: return "메테오 지속시간이 10초로 증가.";
            case 4: return "지진이 최대 10회까지 연속 발동.";
        }
    }
}
