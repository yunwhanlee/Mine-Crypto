using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

public class ShopManager : MonoBehaviour
{
    public static int FAME_SUPPLY_RESET_TIME_SEC = 15;  // 명예보급 리셋 대기시간

    public GameObject windowObj;
    public DOTweenAnimation DOTAnim;

    public FameSupplyBtn fameSupplyBtnPref;     // 명예보급 오브젝트 프리팹
    public RebornSupplyBtn rebornSupplyBtnPref; // 환생보급 오브젝트 프리팹

    public ScrollRect normalScrollRect;       // 일반상점 스크롤
    public ScrollRect fameSupplyScrollRect;   // 명예보급 스크롤
    public ScrollRect rebornSupplyScrollRect; // 환생보급 스크롤
    public ScrollRect inAppScrollRect;        // (인앱샵) 황금상점 스크롤

    public Transform normalContentTf;       // 일반상점 오브젝트 부모 Transform
    public Transform fameSupplyContentTf;   // 명예보급 오브젝트 부모 Transform
    public Transform rebornSupplyContentTf; // 환생보급 오브젝트 부모 Transform
    public Transform inAppContentTf;        // (인앱샵) 황금상점 오브젝트 부모 Transform

    private List<FameSupplyBtn> fameSupplyBtnList;      // 명예보급 리스트
    private List<RebornSupplyBtn> rebornSupplyBtnList;  // 환생보급 리스트

    public TMP_Text[] cateTxtArr;                // 카테고리 텍스트
    public SHOP_CATE cateIdx;                    // 현재 선택한 카테고리 인덱스

    public TMP_Text fameSupplyResetTimerTxt;     // 명예보급 리셋타이머 텍스트
    public TMP_Text GoldPointTxt;                // 사용한 골드 포인트 텍스트

    // 명예보급 리셋타이머 현재 남은시간
    [field:SerializeField] private int fameSupplyTime; public int FameSupplyTime {
        get => DM._.DB.shopDB.fameSupplyTime;
        set => DM._.DB.shopDB.fameSupplyTime = value;
    }


    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // 카테고리 초기화
        cateIdx = SHOP_CATE.NORMAL;

        fameSupplyBtnList = new List<FameSupplyBtn>();
        rebornSupplyBtnList = new List<RebornSupplyBtn>();

        // 명예보급 오브젝트 생성 후 리스트 추가
        for(int i = 0; i < 15; i++)
        {
            fameSupplyBtnList.Add(Instantiate(fameSupplyBtnPref, fameSupplyContentTf));
        }

        // 환생보급 오브젝트 생성 후 리스트 추가
        for(int i = 0; i < 30; i++)
        {
            rebornSupplyBtnList.Add(Instantiate(rebornSupplyBtnPref, rebornSupplyContentTf));
        }

        // 명예보급 데이터 및 UI 초기화
        fameSupplyBtnList[0].Init(id: 0, unlockedLv: 2, RWD.GOLDCOIN, 2);
        fameSupplyBtnList[1].Init(id: 1, unlockedLv: 4, RWD.TIMEPOTION, 5);
        fameSupplyBtnList[2].Init(id: 2, unlockedLv: 6, RWD.TREASURE_CHEST, 10);
        fameSupplyBtnList[3].Init(id: 3, unlockedLv: 8, RWD.SKILLPOTION, 1);
        fameSupplyBtnList[4].Init(id: 4, unlockedLv: 10, RWD.CRISTAL, 500);
        fameSupplyBtnList[5].Init(id: 5, unlockedLv: 12, RWD.GOLDCOIN, 5);
        fameSupplyBtnList[6].Init(id: 6, unlockedLv: 14, RWD.TIMEPOTION, 10);
        fameSupplyBtnList[7].Init(id: 7, unlockedLv: 16, RWD.TREASURE_CHEST, 20);
        fameSupplyBtnList[8].Init(id: 8, unlockedLv: 18, RWD.SKILLPOTION, 2);
        fameSupplyBtnList[9].Init(id: 9, unlockedLv: 20, RWD.CRISTAL, 1000);
        fameSupplyBtnList[10].Init(id: 0, unlockedLv: 22, RWD.GOLDCOIN, 10);
        fameSupplyBtnList[11].Init(id: 1, unlockedLv: 24, RWD.TIMEPOTION, 20);
        fameSupplyBtnList[12].Init(id: 2, unlockedLv: 26, RWD.TREASURE_CHEST, 30);
        fameSupplyBtnList[13].Init(id: 3, unlockedLv: 28, RWD.SKILLPOTION, 3);
        fameSupplyBtnList[14].Init(id: 4, unlockedLv: 30, RWD.CRISTAL, 3000);

        // 환생보급 데이터 및 UI 초기화
        rebornSupplyBtnList[0].Init(unlockedGoldPoint: 0,       RWD.ORE_CHEST, 10);     // 황금점수 0 : 광석상자 10개
        rebornSupplyBtnList[1].Init(unlockedGoldPoint: 10,      RWD.TREASURE_CHEST, 3); // 황금점수 10 : 보물상자 3개
        rebornSupplyBtnList[2].Init(unlockedGoldPoint: 30,      RWD.TIMEPOTION, 3);     // 황금점수 30 : 시간의포션 3개
        rebornSupplyBtnList[3].Init(unlockedGoldPoint: 50,      RWD.CRISTAL, 200);      // 황금점수 50 : 크리스탈 200개
        rebornSupplyBtnList[4].Init(unlockedGoldPoint: 100,     RWD.ORE_CHEST, 10);     // 황금점수 100 : 광석상자 10개
        rebornSupplyBtnList[5].Init(unlockedGoldPoint: 200,     RWD.TREASURE_CHEST, 3); // 황금점수 200 : 보물상자 3개
        rebornSupplyBtnList[6].Init(unlockedGoldPoint: 300,     RWD.TIMEPOTION, 3);     // 황금점수 300 : 시간의포션 3개
        rebornSupplyBtnList[7].Init(unlockedGoldPoint: 400,     RWD.CRISTAL, 300);      // 황금점수 400 : 크리스탈 300개
        rebornSupplyBtnList[8].Init(unlockedGoldPoint: 500,     RWD.SKILLPOTION, 2);    // 황금점수 500 : 스킬포인트물약 2개
        rebornSupplyBtnList[9].Init(unlockedGoldPoint: 650,     RWD.ORE_CHEST, 20);     // 황금점수 650 : 광석상자 20개
        rebornSupplyBtnList[10].Init(unlockedGoldPoint: 800,     RWD.MUSH_BOX1, 5);      // 황금점수 800 : 의문의버섯상자 5개
        rebornSupplyBtnList[11].Init(unlockedGoldPoint: 1000,    RWD.MUSH_BOX2, 5);      // 황금점수 1000 : 신비한버섯상자 5개
        rebornSupplyBtnList[12].Init(unlockedGoldPoint: 1500,    RWD.TIMEPOTION, 5);     // 황금점수 1500 : 시간의포션 5개
        rebornSupplyBtnList[13].Init(unlockedGoldPoint: 2000,    RWD.SKILLPOTION, 3);    // 황금점수 2000 : 스킬포인트물약 3개
        rebornSupplyBtnList[14].Init(unlockedGoldPoint: 3000,    RWD.TREASURE_CHEST, 10);// 황금점수 3000 : 보물상자 10개
        rebornSupplyBtnList[15].Init(unlockedGoldPoint: 4000,    RWD.ORE_CHEST, 50);     // 황금점수 4000 : 광석상자 50개
        rebornSupplyBtnList[16].Init(unlockedGoldPoint: 5000,    RWD.TIMEPOTION, 10);    // 황금점수 5000 : 시간의포션 10개
        rebornSupplyBtnList[17].Init(unlockedGoldPoint: 7500,    RWD.MUSH_BOX3, 1);      // 황금점수 7500 : 전설의버섯상자 1개
        rebornSupplyBtnList[18].Init(unlockedGoldPoint: 10000,   RWD.TREASURE_CHEST, 20);// 황금점수 10000 : 보물상자 20개
        rebornSupplyBtnList[19].Init(unlockedGoldPoint: 15000,   RWD.ORE_CHEST, 100);    // 황금점수 15000 : 광석상자 100개
        rebornSupplyBtnList[20].Init(unlockedGoldPoint: 20000,   RWD.TIMEPOTION, 20);    // 황금점수 20000 : 시간의포션 20개
        rebornSupplyBtnList[21].Init(unlockedGoldPoint: 30000,   RWD.MUSH_BOX1, 20);     // 황금점수 30000 : 의문의버섯상자 20개
        rebornSupplyBtnList[22].Init(unlockedGoldPoint: 40000,   RWD.MUSH_BOX2, 20);     // 황금점수 40000 : 신비한버섯상자 20개
        rebornSupplyBtnList[23].Init(unlockedGoldPoint: 50000,   RWD.MUSH_BOX3, 3);      // 황금점수 50000 : 전설의버섯상자 3개
        rebornSupplyBtnList[24].Init(unlockedGoldPoint: 75000,   RWD.SKILLPOTION, 5);    // 황금점수 75000 : 스킬포인트물약 5개
        rebornSupplyBtnList[25].Init(unlockedGoldPoint: 100000,  RWD.ORE_CHEST, 200);    // 황금점수 100000 : 광석상자 200개
        rebornSupplyBtnList[26].Init(unlockedGoldPoint: 150000,  RWD.TREASURE_CHEST, 50);// 황금점수 150000 : 보물상자 50개
        rebornSupplyBtnList[27].Init(unlockedGoldPoint: 200000,  RWD.TIMEPOTION, 50);    // 황금점수 200000 : 시간의포션 50개
        rebornSupplyBtnList[28].Init(unlockedGoldPoint: 250000,  RWD.MUSH_BOX3, 10);     // 황금점수 250000 : 전설의버섯상자 10개
        rebornSupplyBtnList[29].Init(unlockedGoldPoint: 300000,  RWD.SKILLPOTION, 10);   // 황금점수 300000 : 스킬포인트물약 10개

        //* 명예보급 오프라인 타이머경과 처리
        // yield return new WaitForSeconds(1); // 저장된 추가획득량 데이터가 로드안되는 문제가 있어 1초 대기

        // // 어플시작시 이전까지 경과한시간
        // int passedTime = DM._.DB.autoMiningDB.GetPassedSecData();

        // // 경과시간 확인
        // if(passedTime > )

        // fameSupplyTime
    }

#region EVENT
    /// <summary>
    /// 카테고리 클릭
    /// </summary>
    /// <param name="cateIdx">카테고리 타입 인덱스</param>
    public void OnClickCategoryBtn(int cateIdx)
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        this.cateIdx = (SHOP_CATE)cateIdx;
        SetCatetory();
        // UpdateUI(0); // UI 초기화
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
        UpdateUI();
    }

    private void UpdateUI()
    {
        // 카테고리
        SetCatetory();

        GoldPointTxt.text = $"사용한 골드 포인트 : {DM._.DB.statusDB.GoldPoint}";

        fameSupplyBtnList.ForEach(list => list.UpdateUI());
    }

    /// <summary>
    /// 선택된 카테고리 및 컨텐츠 표시
    /// </summary>
    private void SetCatetory()
    {
        ScrollRect[] contentTfObjArr = new ScrollRect[] {
            normalScrollRect,
            fameSupplyScrollRect,
            rebornSupplyScrollRect,
            inAppScrollRect,
        };

        for(int i = 0; i < cateTxtArr.Length; i++)
        {
            bool isSameCate = (int)cateIdx == i;

            cateTxtArr[i].color = isSameCate? Color.yellow : Color.white;

            contentTfObjArr[i].gameObject.SetActive(isSameCate);
        }
    }

    /// <summary>
    /// 명예보급 타이머 감소
    /// </summary>
    public void SetFameSupplyTimer()
    {
        FameSupplyTime--;
        string timeFormat = Util.ConvertTimeFormat(FameSupplyTime);
        fameSupplyResetTimerTxt.text = timeFormat;

        // 리셋
        if(FameSupplyTime < 1)
        {
            // 초기화
            FameSupplyTime = FAME_SUPPLY_RESET_TIME_SEC;

            // 명예보급 획득트리거 초기화
            fameSupplyBtnList.ForEach(fameSupply => 
                fameSupply.IsAccept = false
            );

            // UI 업데이트
            UpdateUI();
        }
    }
#endregion
}
