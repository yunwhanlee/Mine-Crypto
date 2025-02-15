using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Enum;

public class EmployManager : MonoBehaviour
{
    //* Component
    public DOTweenAnimation DOTAnim;

    //* Elements
    public GameObject employPopUp;
    public GameObject charaGachaPopUp;

    [Header("캐릭 랜덤뽑기 팝업")]
    private Coroutine corCreateRandomCharaID;
    private Coroutine corCreateGachaResultID;
    public Transform charaGachaContentTf;
    public TMP_Text needTicketCntTxt;
    public TMP_Text CurNeedTicketCntTxt;

    [Header("SECTION1: 캐릭터 랜덤 확률표")]
    public TMP_Text randomGradeTableValTxt;
    public TMP_Text curFameLvTxt;
    public TMP_Text PlayBtnNeedTicketCntTxt;
    public TMP_Text PlayBtnEmployCntTxt;
    public Button PlayBtn;

    [Header("SECTION2: 캐릭터 카드 리스트")]
    public MiningController[] charaPrefArr;
    public Sprite[] cardGradeBgSprs;
    public GameObject charaCardUIPref;
    public Transform charaCardContentTf;

    [Header("SECTION3: 선택 캐릭터 정보")]
    public Image charaImg;
    public Image charaBg;
    public TMP_Text charaNameTxt;
    public TMP_Text charaAbilityValTxt;

    //* Value
    const int NEED_TICKET_CNT = 1;
    int workerMax;
    int workerCnt;
    public List<(GRADE, int)> gachaResultList = new List<(GRADE, int)>();
    public WaitForSeconds waitCreateCharaSec;   // 캐릭터 오브젝트 생성시 대기간격


    void Start() {
        waitCreateCharaSec = Util.TIME0_1;
    }

#region EVENT
    public void OnClickCloseBtn()
    {
        GM._.epm.employPopUp.SetActive(false);

        if(PlayBtn.interactable == false)
            return;

        GM._.hm.Active();

        // 시련의광산의 경우 시련의광산 팝업 표시
        if(GM._.stgm.OreType == RSC.CRISTAL)
        {
            GM._.clm.windowObj.SetActive(true);
            GM._.ui.topRscGroup.SetActive(true);
        }
    }

    /// <summary>
    /// 캐릭터 랜덤뽑기
    /// </summary>
    public void OnClickCharaGachaBtn()
    {
        if(GM._.gameState == GameState.PLAY)
        {
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.AlreadyPlayGame)); // 이미 게임이 진행중입니다.
            return;
        }

        // 티켓 수량 확인 및 텍스트 업데이트 처리
        if(CheckAndUpdateTicketByMode() == false)
            return;

        CurNeedTicketCntTxt.gameObject.SetActive(true); // 현재 소지중인 티켓 텍스트 표시

        SoundManager._.PlayBgm((SoundManager.BGM)GM._.stgm.OreType + 1); //stageType != RSC.CRISTAL);

        // 팝업 표시
        employPopUp.SetActive(false);
        charaGachaPopUp.SetActive(true);

        // 랜덤뽑기 리스트 데이터 작성
        SetRandomGachaGradeList();

        // 랜덤뽑기 결과UI 표시
        corCreateGachaResultID = StartCoroutine(CoCreateGachaResultCardUIContent());
    }

    /// <summary>
    /// 다음레벨 명예팝업 표시용 (i)정보버튼
    /// </summary>
    public void OnClickFameInfoIconBtn()
    {
        SoundManager._.PlaySfx(SoundManager.SFX.ItemDrop1SFX);
        GM._.fm.SetFameLevelToogleUI(isOn: true);
        GM._.fm.ShowFameNextLevelUpGradeTable();
    }

    /// <summary>
    /// 게임시작
    /// </summary>
    public void OnClickPlayBtn()
    {
        charaGachaPopUp.SetActive(false);
        GM._.spm.shopMenuIconBtnObj.SetActive(true);

        // 스테이지 시작
        GM._.stgm.StartStage();

        // 캐릭터 생성
        corCreateRandomCharaID = StartCoroutine(CoCreateRandomCharaIns());
    }

    /// <summary>
    /// 랜덤뽑기 재시도
    /// </summary>
    public void OnClickRetryBtn()
    {
        // 티켓 수량 확인 및 텍스트 업데이트 처리
        if(CheckAndUpdateTicketByMode() == false)
            return;

        // 데이터 및 오브젝트 초기화
        gachaResultList = new List<(GRADE, int)>();
        for(int i = 0; i < charaGachaContentTf.childCount; i++)
            Destroy(charaGachaContentTf.GetChild(i).gameObject);

        // 랜덤뽑기 데이터 작성
        SetRandomGachaGradeList();

        // 랜덤뽑기 결과UI 표시
        if(corCreateGachaResultID != null) StopCoroutine(corCreateGachaResultID);
        corCreateGachaResultID = StartCoroutine(CoCreateGachaResultCardUIContent());
    }

    /// <summary>
    ///* 랜덤뽑기 데이터 작성
    /// </summary>
    private void SetRandomGachaGradeList()
    {
        // 고용횟수 만큼 반복
        for(workerCnt = 0; workerCnt < workerMax; workerCnt++)
        {
            const int A = 0, B = 1, C = 2, D = 3, E = 4, F = 5;

            // 랜덤 등급 설정
            int[] gTb = GM._.fm.GetRandomGradeArrByFame();
            int max = gTb[A] + gTb[B] + gTb[C] + gTb[D] + gTb[E] + gTb[F];
            int rdPer = Random.Range(0, max); 

            GRADE grade = rdPer < gTb[A]? GRADE.COMMON
                : rdPer < gTb[A] + gTb[B]?GRADE.UNCOMMON
                : rdPer < gTb[A] + gTb[B] + gTb[C]? GRADE.RARE
                : rdPer < gTb[A] + gTb[B] + gTb[C] + gTb[D]? GRADE.UNIQUE
                : rdPer < gTb[A] + gTb[B] + gTb[C] + gTb[D] + gTb[E]? GRADE.LEGEND
                : GRADE.MYTH;

            // 같은 등급안에서 랜덤 선택
            int randIdx = -1;
            switch(grade)
            {
                case GRADE.COMMON: randIdx = Random.Range(0, 4); break;
                case GRADE.UNCOMMON: randIdx = Random.Range(4, 8); break;
                case GRADE.RARE: randIdx = Random.Range(8, 11); break;
                case GRADE.UNIQUE: randIdx = Random.Range(11, 14); break;
                case GRADE.LEGEND: randIdx = Random.Range(14, 16); break;
                case GRADE.MYTH: randIdx = 16; break;
            }

            Debug.Log($"Create Chara: workerCnt({workerCnt}) < workerMax({workerMax}), Random Grade Per= {rdPer} => grade={grade}, randIdx={randIdx}");

            gachaResultList.Add((grade, randIdx));
        }
    }

    /// <summary>
    ///* 랜덤뽑기 캐릭터 오브젝트 생성
    /// </summary>
    /// <returns></returns>
    IEnumerator CoCreateRandomCharaIns()
    {
        // 고용횟수 만큼 반복
        for(int i = 0; i < gachaResultList.Count; i++)
        {
            var tupleGachaRes = gachaResultList[i];
            int charaIdx = tupleGachaRes.Item2;

            yield return Util.TIME0_1; // 약간 대기하여 캐릭터가 겹치지 않도록

            // 캐릭터 생성
            var ins = Instantiate(GM._.mnm.goblinPrefs[charaIdx], GM._.mnm.workerGroupTf);
            ins.transform.position = GM._.mnm.homeTf.position;
        }

        // 생성된 캐릭터 등급표 배열 작성
        GM._.mnm.CalcWorkerGradeTableArr();
    }
#endregion

#region FUNC
    /// <summary>
    /// 티켓 수량 확인 및 텍스트 업데이트 처리
    /// </summary>
    private bool CheckAndUpdateTicketByMode()
    {
        Debug.Log($"CheckAndUpdateTicketByMode():: sttDB.OreTicket= {DM._.DB.statusDB.OreTicket}, sttDB.RedTicket= {DM._.DB.statusDB.RedTicket}");
        var sttDB = DM._.DB.statusDB;
        var stgm = GM._.stgm;

        // 티켓 수량 확인처리
        if(stgm.IsChallengeMode)
        {
            if(sttDB.RedTicket < NEED_TICKET_CNT)
            {
                GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
                return false;
            }
            else
                sttDB.RedTicket--; // 붉은티겟 수량 감소
        }
        else
        {
            if(sttDB.OreTicket < NEED_TICKET_CNT)
            {
                GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
                return false;
            }
            else
                sttDB.OreTicket--; // 광석티겟 수량 감소
        }

        // 모드에 따른 티켓타입
        string ticketTag = stgm.IsChallengeMode? "RED_TICKET" : "ORE_TICKET";
        int ticketVal = stgm.IsChallengeMode? sttDB.RedTicket : sttDB.OreTicket;

        // 필요티켓 표시 텍스트
        needTicketCntTxt.text = $"<size=70%><sprite name={ticketTag}></size> 1";

        // 현재 소지중인 티켓 수량 표시 텍스트
        CurNeedTicketCntTxt.text = $"<sprite name={ticketTag}> {ticketVal}";

        return true;
    }

    /// <summary>
    /// 캐릭터 생성시 등급표시 메세지 코루틴 정지
    /// </summary>
    public void StopCorCreateRandomCharaID()
    {
        if(corCreateRandomCharaID != null)
            StopCoroutine(corCreateRandomCharaID);
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    public void ShowPopUp(bool isPlayBtnActive = true)
    {
        PlayBtn.interactable = isPlayBtnActive;
        CurNeedTicketCntTxt.gameObject.SetActive(false);
        employPopUp.SetActive(true);
        DOTAnim.DORestart();
        UpdateUIAndData(); //* 리스트초기화 및 UI업데이트
    }

    /// <summary>
    /// 데이터 및 UI 최신화
    /// </summary>
    private void UpdateUIAndData() {
        //* 고용 팝업
        workerCnt = 0;

        // 입장소비 티켓 표시
        var ticketTag = GM._.stgm.IsChallengeMode? INV.RED_TICKET : INV.ORE_TICKET;
        PlayBtnNeedTicketCntTxt.text = $"<size=70%><sprite name={ticketTag}></size> {NEED_TICKET_CNT}";

        // 소환캐릭 수
        workerMax = GM._.sttm.TotalPopulation;
        PlayBtnEmployCntTxt.text = $"{workerMax} {LM._.Localize(LM.Cnt)} {LM._.Localize(LM.Summon)}";

        //* 랜덤뽑기결과 팝업
        gachaResultList = new List<(GRADE, int)>();

        DOTAnim.DORestart();

        SetRandomGradeTableUI();
        CreateCharaCardUIContent();
        ShowSelectCharaInfo(charaIdx: 0);
    }

    /// <summary>
    /// 캐릭터 소환등급 확률표 표시 (섹션1)
    /// </summary>
    public void SetRandomGradeTableUI() {
        curFameLvTxt.text = $"{LM._.Localize(LM.Fame)} LV.{GM._.fm.FameLv}";

        int[] gTb = GM._.fm.GetRandomGradeArrByFame();
        // 등급표 작성
        randomGradeTableValTxt.text = $"{gTb[0]}%" + $"\n<color=green>{gTb[1]}%</color>" + $"\n<color=blue>{gTb[2]}%</color>" + $"\n<color=purple>{gTb[3]}%</color>" + $"\n<color=yellow>{gTb[4]}%</color>" + $"\n<color=red>{gTb[5]}%</color>";
    }

    /// <summary>
    /// 사용가능한 캐릭터 카드리스트UI 생성 및 표시
    /// </summary>
    private void CreateCharaCardUIContent() {
        const int BG = 0, CHARA_IMG = 1;

        // 선택카드UI 초기화
        for(int i = 0; i < charaCardContentTf.childCount; i++)
            Destroy(charaCardContentTf.GetChild(i).gameObject);

        // 선택카드UI 생성
        for(int i = 0; i < charaPrefArr.Length; i++)
        {
            var card = Instantiate(charaCardUIPref, charaCardContentTf);

            // 배경 이미지 (등급)
            Sprite bgSpr = cardGradeBgSprs[(int)charaPrefArr[i].Grade];
            card.transform.GetChild(BG).GetComponent<Image>().sprite = bgSpr;

            //* 카드 클릭시 이벤트 등록
            int copyIdx = i;
            card.transform.GetChild(BG).GetComponent<Button>().onClick.AddListener(() => {
                SoundManager._.PlaySfx(SoundManager.SFX.Tap2SFX);
                ShowSelectCharaInfo(copyIdx);
            });

            // 캐릭터 이미지
            card.transform.GetChild(CHARA_IMG).GetComponent<Image>().sprite = charaPrefArr[i].iconCharaImg;
        }
    }

    /// <summary>
    /// 랜덤뽑기 결과 카드UI 표시
    /// </summary>
    IEnumerator  CoCreateGachaResultCardUIContent() {
        const int BG = 0, CHARA_IMG = 1;
        const int RARE_EF = 2, UNIQUE_EF = 3, LEGEND_EF = 4, MYTH_EF = 5;

        List<GameObject> cardList = new List<GameObject>();

        // 결과카드UI 초기화
        for(int i = 0; i < charaGachaContentTf.childCount; i++)
            Destroy(charaGachaContentTf.GetChild(i).gameObject);

        // 결과카드UI 생성
        for(int i = 0; i < gachaResultList.Count; i++)
        {
            yield return Util.TIME0_05;

            SoundManager._.PlaySfx(SoundManager.SFX.SummonNormalSFX);

            // 등급
            (GRADE, int) tupleGachaRes = gachaResultList[i];

            var grade = tupleGachaRes.Item1;
            var charaIdx = tupleGachaRes.Item2;

            // 카드 생성
            cardList.Add(Instantiate(charaCardUIPref, charaGachaContentTf));
            // 배경 이미지 (등급)
            Sprite bgSpr = cardGradeBgSprs[(int)grade];
            cardList[i].transform.GetChild(BG).GetComponent<Image>().sprite = bgSpr;
            // 캐릭터 이미지
            cardList[i].transform.GetChild(CHARA_IMG).GetComponent<Image>().sprite = charaPrefArr[charaIdx].iconCharaImg;
        }

        // 결과카드가 다 배치된 후에 등급이펙트 표시
        for(int i = 0; i < gachaResultList.Count; i++)
        {
            // 등급
            (GRADE, int) tupleGachaRes = gachaResultList[i];
            var grade = tupleGachaRes.Item1;

            // 등급 이펙트
            switch(grade)
            {
                case GRADE.RARE:
                cardList[i].transform.GetChild(RARE_EF).gameObject.SetActive(true);
                    SoundManager._.PlaySfx(SoundManager.SFX.SummonRareUniqueSFX);
                    break;
                case GRADE.UNIQUE:
                    SoundManager._.PlaySfx(SoundManager.SFX.SummonRareUniqueSFX);
                    cardList[i].transform.GetChild(UNIQUE_EF).gameObject.SetActive(true);
                    break;
                case GRADE.LEGEND:
                    SoundManager._.PlaySfx(SoundManager.SFX.SummonLegendSFX);
                    cardList[i].transform.GetChild(LEGEND_EF).gameObject.SetActive(true);
                    break;
                case GRADE.MYTH:
                    SoundManager._.PlaySfx(SoundManager.SFX.SummonMythSFX);
                    cardList[i].transform.GetChild(MYTH_EF).gameObject.SetActive(true);
                    break;
            }
        }
        corCreateGachaResultID = null;
    }

    /// <summary>
    /// 선택한 카드의 캐릭터 정보 표시 (섹션3)
    /// </summary>
    private void ShowSelectCharaInfo(int charaIdx) {
        Debug.Log($"ShowSelectCharaInfo(charaIdx= {charaIdx})::");
        var charaDt = charaPrefArr[charaIdx];

        charaImg.sprite = charaDt.iconCharaImg;
        charaBg.sprite = cardGradeBgSprs[(int)charaDt.Grade];
        charaNameTxt.text = LM._.Localize(charaDt.Name);
        charaAbilityValTxt.text = $"{charaDt.AttackVal}"
            + $"\n{Util.FloatToStr(charaDt.AttackSpeed)}"
            + $"\n{Util.FloatToStr(charaDt.MoveSpeed)}"
            + $"\n{charaDt.BagStorageSize}";
    }
#endregion
}
