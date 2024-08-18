using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EmployManager : MonoBehaviour
{
    //* Component
    public DOTweenAnimation DOTAnim;

    //* Elements
    public GameObject employPopUp;

    [Header("SECTION1: 캐릭터 랜덤 확률표")]
    public TMP_Text randomGradeTableValTxt;

    public GameObject fameInfoPanel; // 다음 명성레벨에 따른 캐릭터 고용(소환) 등급표
    public TMP_Text nextLvRandomGradeTableValTxt;

    public TMP_Text PlayBtnEmployCntTxt;

    [Header("SECTION2: 캐릭터 카드 리스트")]
    public MiningController[] charaPrefArr;
    public GameObject charaCardUIPref;
    public Transform charaCardContentTf;

    public Sprite[] cardGradeBgSprs;

    [Header("SECTION3: 선택 캐릭터 정보")]
    public Image charaImg;
    public Image charaBg;
    public TMP_Text charaNameTxt;
    public TMP_Text charaAbilityValTxt;

    //* Value
    int workerCnt;
    int workerMax;

#region EVENT
    /// <summary>
    /// 캐릭터 생성(고용)
    /// </summary>
    public void OnClickPlayBtn()
    {
        employPopUp.SetActive(false);

        // 고블린 생성
        StartCoroutine(CoCreateRandomCharaIns());
    }

    IEnumerator CoCreateRandomCharaIns() {
        // 고용횟수 만큼 반복
        for(workerCnt = 0; workerCnt < workerMax; workerCnt++)
        {   
            const int A = 0, B = 1, C = 2, D = 3, E = 4, F = 5;

            // 랜덤 등급 설정
            int[] gTb = GM._.fm.GetRandomGradeArrByFame(isNextLv: true);
            int max = gTb[A] + gTb[B] + gTb[C] + gTb[D] + gTb[E] + gTb[F];
            int rdPer = Random.Range(0, max); 

            var grade = rdPer < gTb[A]? Enum.GRADE.COMMON
                : rdPer < gTb[A] + gTb[B]? Enum.GRADE.UNCOMMON
                : rdPer < gTb[A] + gTb[B] + gTb[C]? Enum.GRADE.RARE
                : rdPer < gTb[A] + gTb[B] + gTb[C] + gTb[D]? Enum.GRADE.UNIQUE
                : rdPer < gTb[A] + gTb[B] + gTb[C] + gTb[D] + gTb[E]? Enum.GRADE.LEGEND
                : Enum.GRADE.MYTH;

            Debug.Log($"Create Chara: workerCnt({workerCnt}) < workerMax({workerMax}), Random Grade Per= {rdPer} => {grade}");

            // 한국어로 등급이름 번역
            string gradeName = grade == Enum.GRADE.COMMON? "일반"
                : grade == Enum.GRADE.UNCOMMON? "<color=green>고급</color>"
                : grade == Enum.GRADE.RARE? "<color=blue>레어</color>"
                : grade == Enum.GRADE.UNIQUE? "<color=purple>유니크</color>"
                : grade == Enum.GRADE.LEGEND? "<color=yellow>전설</color>"
                : "<color=red>신화</color>";

            GM._.ui.ShowNoticeMsgPopUp($"{workerCnt}. {gradeName} 등급 소환!");
            yield return Util.TIME0_5; // 약간 대기하여 캐릭터가 겹치지 않도록

            // 고블린 생성
            var ins = Instantiate(GM._.mnm.goblinPrefs[(int)grade], GM._.mnm.workerGroupTf);
            ins.transform.position = GM._.mnm.homeTf.position;
        }
    }

    /// <summary>
    /// 다음 명예레벨에 따른 소환등급 확률표 표시 (섹션1)
    /// </summary>
    public void OnClickFameInfoIconBtn() {
        fameInfoPanel.SetActive(true);
        int[] gTb = GM._.fm.GetRandomGradeArrByFame(isNextLv: true);
        // 등급표 작성
        nextLvRandomGradeTableValTxt.text = $"{gTb[0]}%" + $"\n<color=green>{gTb[1]}%</color>" + $"\n<color=blue>{gTb[2]}%</color>" + $"\n<color=purple>{gTb[3]}%</color>" + $"\n<color=yellow>{gTb[4]}%</color>" + $"\n<color=red>{gTb[5]}%</color>";
    }
#endregion

#region FUNC
    /// <summary>
    /// 팝업 열기
    /// </summary>
    public void ShowPopUp() {
        employPopUp.SetActive(true);
        DOTAnim.DORestart();
        UpdateUIAndData();
    }

    /// <summary>
    /// 데이터 및 UI 최신화
    /// </summary>
    private void UpdateUIAndData() {
        workerCnt = 0;
        workerMax = GM._.ugm.upgIncPopulation.Val;
        PlayBtnEmployCntTxt.text = $"{workerMax}마리 고용";

        DOTAnim.DORestart();
        SetRandomGradeTableUI();
        CreateCharaCardUIContent();
        ShowSelectCharaInfo(charaIdx: 0);
    }

    /// <summary>
    /// 캐릭터 소환등급 확률표 표시 (섹션1)
    /// </summary>
    public void SetRandomGradeTableUI() {
        int[] gTb = GM._.fm.GetRandomGradeArrByFame();
        // 등급표 작성
        randomGradeTableValTxt.text = $"{gTb[0]}%" + $"\n<color=green>{gTb[1]}%</color>" + $"\n<color=blue>{gTb[2]}%</color>" + $"\n<color=purple>{gTb[3]}%</color>" + $"\n<color=yellow>{gTb[4]}%</color>" + $"\n<color=red>{gTb[5]}%</color>";
    }

    /// <summary>
    /// 사용가능한 캐릭터 카드리스트 생성 및 표시
    /// </summary>
    private void CreateCharaCardUIContent() {
        const int BG = 0, CHARA_IMG = 1;

        for(int i = 0; i < charaPrefArr.Length; i++)
        {
            var card = Instantiate(charaCardUIPref, charaCardContentTf);

            // 배경 이미지 (등급)
            Sprite bgSpr = cardGradeBgSprs[(int)charaPrefArr[i].Grade];
            card.transform.GetChild(BG).GetComponent<Image>().sprite = bgSpr;

            // 배경 버튼 (클릭 이벤트 등록)
            int copyIdx = i;
            card.transform.GetChild(BG).GetComponent<Button>().onClick.AddListener(() => {
                ShowSelectCharaInfo(copyIdx);
            });

            // 캐릭터 이미지
            card.transform.GetChild(CHARA_IMG).GetComponent<Image>().sprite = charaPrefArr[i].iconCharaImg;
        }
    }

    /// <summary>
    /// 선택한 카드의 캐릭터 정보 표시 (섹션3)
    /// </summary>
    private void ShowSelectCharaInfo(int charaIdx) {
        Debug.Log($"ShowSelectCharaInfo(charaIdx= {charaIdx})::");
        var charaDt = charaPrefArr[charaIdx];

        charaImg.sprite = charaDt.iconCharaImg;
        charaBg.sprite = cardGradeBgSprs[(int)charaDt.Grade];
        charaNameTxt.text = charaDt.Name;
        charaAbilityValTxt.text = $"{charaDt.AttackVal}"
            + $"\n{charaDt.AttackSpeed}"
            + $"\n{charaDt.MoveSpeed}"
            + $"\n{charaDt.BagStorageSize}";
    }
#endregion
}
