using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using static Enum;

/// <summary>
/// 세번째 스킬(스킵형)
/// </summary>
[System.Serializable]
public class SkipSkillTree
{
    public SkillTree[] skillTreeArr;

    public DOTweenAnimation spaceDarkFadeEFAnim;
    public ParticleImage skipEmitterParticleEF;
    public GameObject[] portalGradeParticleEFArr;
    public DOTweenAnimation[] charaFallInAnimArr;
    public ParticleSystem chestShowerParticleEF;

    public int grade;

    // 스킬레벨
    public int Lv {
        get => DM._.DB.skillTreeDB.skipSkillTreeLv;
        set => DM._.DB.skillTreeDB.skipSkillTreeLv = value;
    }
    // 다음층 이동
    public int MoveNextFloor {
        get {
            int[] skipGradeFloorArr = {3, 4, 5, 6, 7, 8};
            return skipGradeFloorArr[Random.Range(0, skipGradeFloorArr.Length)];
        }
    }
    // 광산 남은시간 감소%
    public float DecTimerPer {
        get {
            if(Lv == 5) return 1 - 0.2f;        // -20% 감소
            else if(Lv >= 3) return 1 - 0.35f;  // -35% 감소
            else return 0.5f;                   // -50% 감소 (기본)
        }
    }
    // 스킬쿨타임 감소%
    public float DecSkillCoolTimePer {
        get {
            if(Lv == 5) return 1 - 0.2f; // -20% 감소
            else if(Lv >= 2) return 1 - 0.1f; // -10% 감소
            else return 1.0f; // 감소 없음
        }
    }
    // 보너스상자 (광석상자 및 보물상자 획득)
    public bool IsBonusChest { get => Lv >= 4;}

    /// <summary>
    /// 포탈 애니메이션 비표시 초기화
    /// </summary> <summary>
    /// 
    /// </summary>
    public void InitPortalEF()
    {
        for(int i = 0; i < portalGradeParticleEFArr.Length; i++)
            portalGradeParticleEFArr[i].SetActive(false);
    }

    /// <summary>
    /// 포탈 애니메이션 재생
    /// </summary>
    public void PlayPortalEF(int gradeIdx)
    {
        InitPortalEF();
        portalGradeParticleEFArr[gradeIdx].SetActive(true);
    }

    public void PlaySkipAnim(int gradeIdx)
    {
        skipEmitterParticleEF.Play();
        spaceDarkFadeEFAnim.DORestart();
        PlayPortalEF(gradeIdx);
        Array.ForEach(charaFallInAnimArr, anim => anim.DORestart());
    }

    public void EndSkipAnim()
    {
        InitPortalEF();
    }

    /// <summary>
    /// 보너스상자 획득 (스킬레벨 LV4이상)
    /// </summary>
    public void CheckBonusChestLv4()
    {
        Debug.Log($"CheckBonusChestLv4():: IsBonusChest= {IsBonusChest}");
        // 스킬레벨 4이상인지 확인
        if(!IsBonusChest) return;

        chestShowerParticleEF.Play();

        const int ORECHEST_CNT = 30;
        const int TREASURECHEST_CNT = 10;

        // 광석상자 획득
        GM._.pm.playResRwdArr[(int)RWD.ORE_CHEST] += ORECHEST_CNT; // 결과수치 UI
        DM._.DB.statusDB.OreChest += ORECHEST_CNT; // 데이터
        // 보물상자 획득
        GM._.pm.playResRwdArr[(int)RWD.TREASURE_CHEST] += TREASURECHEST_CNT; // 결과수치 UI
        DM._.DB.statusDB.TreasureChest += TREASURECHEST_CNT; // 데이터
    }

    /// <summary>
    /// 스킬 상세설명
    /// </summary>
    public string GetDescription(int idx)
    {
        switch(idx)
        {
            default: return "광산의 남은시간이 50% 감소하고 (3/<color=green>4</color>/<color=blue>5</color>/<color=purple>6</color>/<color=orange>7</color>/<color=red>8</color>)층 이동.";
            case 1: return "남은 스킬쿨타임이 10% 감소.";
            case 2: return "광산의 남은시간 감소수치가 35%로 줄어듦.";
            case 3: return "발동시 광석상자 30개와 보물상자 10개를 획득.";
            case 4: return "남은 스킬쿨타임이 20%로 감소, 광산의 남은시간 감소수치가 20%로 줄어듦.";
        }
    }
}
