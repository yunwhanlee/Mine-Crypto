using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OreProficiencyManager : MonoBehaviour
{

    // Element
    public GameObject windowObj;
    public GameObject alertRedDotObj;
    public TMP_Text totalAttackPerTxt;
    public Sprite grayBtnSpr;
    public Sprite yellowBtnSpr;

    // Value
    [field:Header("단계별 필요 경험치")]
    [field:SerializeField] public int[] oreMaxExpArr {get; private set;} = new int[7];
    [field:SerializeField] public int[] chestMaxExpArr {get; private set;} = new int[7];

    public float totalAttackPer; // 총 채굴 공격력 %

    // Class
    [field:Header("숙련도 미션 객체배열")]
    public ProficiencyFormat[] proficiencyArr; // 숙련도 클래스 배열

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // 객체 초기화
        for(int i = 0; i < proficiencyArr.Length; i++)
            proficiencyArr[i].Init();

        UpdateTotalAtkPerDataAndUI();
    }

    void Update()
    {
        //! TEST EXP 증가
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("TEST 숙련도 및 미션 EXP 증가");
            for(int i = 0; i < proficiencyArr.Length; i++)
                proficiencyArr[i].Exp += 5000;

            GM._.ui.ShowNoticeMsgPopUp("(테스트모드) 모든 명예와 숙련도 <color=green>EXP</color> 증가");

            UpdateAll();
        }
    }

#region EVENT
    public void OnClickAcceptBtn(int idx)
    {
        var proficiency = proficiencyArr[idx];

        if(!proficiency.IsAccept)
        {   
            GM._.ui.ShowWarningMsgPopUp("아직 경험치가 부족합니다!");
            return;
        }

        SoundManager._.PlaySfx(SoundManager.SFX.ProficiencyFinishSFX);

        GM._.ui.ShowNoticeMsgPopUp("숙련도 레벨 업!");

        // 미션 레벨 업
        proficiency.Lv++;

        // 총 공격력% 증가

        UpdateAll();
    }
#endregion

#region FUNC
    public void UpdateAll()
    {
        // 알림 비표시 초기화
        alertRedDotObj.SetActive(false);

        // 숙련도 데이터 업데이트
        for(int i = 0; i < proficiencyArr.Length; i++)
        {
            proficiencyArr[i].UpdateData();
            proficiencyArr[i].UpdateUI();
        }

        // 총 채굴 공격력 데이터 및 UI 업데이트
        UpdateTotalAtkPerDataAndUI();
    }

    /// <summary>
    /// 총 채굴 공격력 데이터 및 UI 업데이트
    /// </summary>
    private void UpdateTotalAtkPerDataAndUI()
    {
        totalAttackPer = CalcTotalAttackPer();
        totalAttackPerTxt.text = $"{LM._.Localize(LM.All)} {LM._.Localize(LM.Attack)} +{Mathf.RoundToInt(totalAttackPer * 100)}%";
    }

    /// <summary>
    /// 전체 추가공격력% 계산
    /// </summary>
    private float CalcTotalAttackPer()
    {
        const float UNIT_PER = 0.01f;

        int lvUpCnt = 0;

        for(int i = 0; i < proficiencyArr.Length; i++)
        {
            lvUpCnt += proficiencyArr[i].Lv - 1;
        }

        return lvUpCnt * UNIT_PER;
    }

#endregion
}