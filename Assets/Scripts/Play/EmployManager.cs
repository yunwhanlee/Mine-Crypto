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

    void Start()
    {
        workerCnt = 0;
        DOTAnim.DORestart();
        SetRandomGradeTableUI();
        CreateCharaCardUIContent();
        ShowSelectCharaInfo(charaIdx: 0);
    }

#region EVENT
    /// <summary>
    /// 캐릭터 생성(고용)
    /// </summary>
    public void OnClickPlayBtn()
    {
        if(workerCnt < GM._.ugm.upgIncPopulation.Val)
        {
            //TODO Grade Random
            int randomIdx = Random.Range(0, 1); 

            var ins = Instantiate(GM._.mnm.goblinPrefs[randomIdx], GM._.mnm.workerGroupTf);
            ins.transform.position = GM._.mnm.homeTf.position;
            workerCnt++;
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
    public void ShowPopUp() {
        employPopUp.SetActive(true);
        DOTAnim.DORestart();
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
