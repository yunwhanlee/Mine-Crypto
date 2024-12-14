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
    public const int FAME_SUPPLY_RESET_TIME_SEC = 20;  // 명예보급 리셋 대기시간(초)

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

    private List<Button> normalBtnList;                 // 일반상점 리스트
    private List<FameSupplyBtn> fameSupplyBtnList;      // 명예보급 리스트
    private List<RebornSupplyBtn> rebornSupplyBtnList;  // 환생보급 리스트

    public TMP_Text[] cateTxtArr;                // 카테고리 텍스트
    public SHOP_CATE cateIdx;                    // 현재 선택한 카테고리 인덱스

    public TMP_Text myGoldCntTxt;                // 현재 황금코인 수량 텍스트
    public TMP_Text fameSupplyResetTimerTxt;     // 명예보급 리셋타이머 텍스트
    public TMP_Text GoldPointTxt;                // 사용한 황금 포인트 텍스트

    // 명예보급 리셋타이머 현재 남은시간
    [field:SerializeField] private int fameSupplyTime; public int FameSupplyTime {
        get => DM._.DB.shopDB.fameSupplyTime;
        set => DM._.DB.shopDB.fameSupplyTime = value;
    }

    // 환생보급 황금포인트
    [field:SerializeField] private int goldPoint;   public int GoldPoint {
        get => DM._.DB.statusDB.GoldPoint;
        set => DM._.DB.statusDB.GoldPoint = value;
    }


    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // 처음시작 및 배열수량 수정시 Out of Index 에러 방지처리
        DM._.DB.shopDB.CheckNewDataErr();

        // 카테고리 초기화
        cateIdx = SHOP_CATE.NORMAL;

        normalBtnList = new List<Button>();
        fameSupplyBtnList = new List<FameSupplyBtn>();
        rebornSupplyBtnList = new List<RebornSupplyBtn>();

        // 일반상점목록 리스트 추가
        for(int i = 0; i < normalContentTf.childCount; i++)
        {
            normalBtnList.Add(normalContentTf.GetChild(i).GetComponentInChildren<Button>());
        }

        // 명예보급 오브젝트 생성 후 리스트 추가
        for(int i = 0; i < ShopDB.FAME_SUPPLY_ARRCNT; i++)
        {
            fameSupplyBtnList.Add(Instantiate(fameSupplyBtnPref, fameSupplyContentTf));
        }

        // 환생보급 오브젝트 생성 후 리스트 추가
        for(int i = 0; i < ShopDB.REBORN_SUPPLY_ARRCNT; i++)
        {
            rebornSupplyBtnList.Add(Instantiate(rebornSupplyBtnPref, rebornSupplyContentTf));
        }

        // 일반상점 아이템버튼 초기화
        // normalBtnList[0].onClick.AddListener(() => OnClickNormalItemBtn()); // 광고제거 : 황금덩어리 1000개 (1회구매가능) [pc버전에는 이부분 비활성]
        normalBtnList[1].onClick.AddListener(() => OnClickNormalItemBtn(RWD.ORE_CHEST, 10, goldPrice: 2)); // 광석상자 10개 : 황금덩어리 2개
        normalBtnList[2].onClick.AddListener(() => OnClickNormalItemBtn(RWD.ORE_CHEST, 100, goldPrice: 20)); // 광석상자 100개 : 황금덩어리 20개
        normalBtnList[3].onClick.AddListener(() => OnClickNormalItemBtn(RWD.TREASURE_CHEST, 3, goldPrice: 2)); // 보물상자 3개 : 황금덩어리 2개
        normalBtnList[4].onClick.AddListener(() => OnClickNormalItemBtn(RWD.TREASURE_CHEST, 30, goldPrice: 20)); // 보물상자 30개 : 황금덩어리 20개
        normalBtnList[5].onClick.AddListener(() => OnClickNormalItemBtn(RWD.TIMEPOTION, 5, goldPrice: 5)); // 시간의포션 5개 : 황금덩어리 5개
        normalBtnList[6].onClick.AddListener(() => OnClickNormalItemBtn(RWD.TIMEPOTION, 50, goldPrice: 50)); // 시간의포션 50개 : 황금덩어리 50개
        normalBtnList[7].onClick.AddListener(() => OnClickNormalItemBtn(RWD.SKILLPOTION, 1, goldPrice: 3)); // 스킬포션 1개 : 황금덩어리 3개
        normalBtnList[8].onClick.AddListener(() => OnClickNormalItemBtn(RWD.SKILLPOTION, 10, goldPrice: 30)); // 스킬포션 10개 : 황금덩어리 30개
        normalBtnList[9].onClick.AddListener(() => OnClickNormalItemBtn(RWD.FAME, 1, goldPrice: 5)); // 명예포인트+1(+추가명예) : 황금덩어리 5개 (명예레벨과 경험치표시)

        // 명예보급 데이터 및 UI 초기화
        fameSupplyBtnList[0].Init(id: 0, unlockedLv: 2, RWD.GOLDCOIN, 2);               // 명예레벨 2 : 황금덩어리 2개
        fameSupplyBtnList[1].Init(id: 1, unlockedLv: 4, RWD.TIMEPOTION, 5);             // 명예레벨 4 : 시간의포션 5개
        fameSupplyBtnList[2].Init(id: 2, unlockedLv: 6, RWD.TREASURE_CHEST, 10);        // 명예레벨 6 : 보물상자 10개
        fameSupplyBtnList[3].Init(id: 3, unlockedLv: 8, RWD.SKILLPOTION, 1);            // 명예레벨 8 : 스킬포인트물약 1개
        fameSupplyBtnList[4].Init(id: 4, unlockedLv: 10, RWD.CRISTAL, 500);             // 명예레벨 10 : 크리스탈 500개
        fameSupplyBtnList[5].Init(id: 5, unlockedLv: 12, RWD.GOLDCOIN, 5);              // 명예레벨 12 : 황금덩어리 5개
        fameSupplyBtnList[6].Init(id: 6, unlockedLv: 14, RWD.TIMEPOTION, 10);           // 명예레벨 12 : 황금덩어리 5개
        fameSupplyBtnList[7].Init(id: 7, unlockedLv: 16, RWD.TREASURE_CHEST, 20);       // 명예레벨 16 : 보물상자 20개
        fameSupplyBtnList[8].Init(id: 8, unlockedLv: 18, RWD.SKILLPOTION, 2);           // 명예레벨 18 : 스킬포인트물약 2개
        fameSupplyBtnList[9].Init(id: 9, unlockedLv: 20, RWD.CRISTAL, 1000);            // 명예레벨 20 : 크리스탈 1000개
        fameSupplyBtnList[10].Init(id: 0, unlockedLv: 22, RWD.GOLDCOIN, 10);            // 명예레벨 22 : 황금덩어리 10개
        fameSupplyBtnList[11].Init(id: 1, unlockedLv: 24, RWD.TIMEPOTION, 20);          // 명예레벨 24 : 시간의포션 20개
        fameSupplyBtnList[12].Init(id: 2, unlockedLv: 26, RWD.TREASURE_CHEST, 30);      // 명예레벨 26 : 보물상자 30개
        fameSupplyBtnList[13].Init(id: 3, unlockedLv: 28, RWD.SKILLPOTION, 3);          // 명예레벨 28 : 스킬포인트물약 3개
        fameSupplyBtnList[14].Init(id: 4, unlockedLv: 30, RWD.CRISTAL, 3000);           // 명예레벨 30 : 크리스탈 3000개

        // 환생보급 데이터 및 UI 초기화
        rebornSupplyBtnList[0].Init(id: 0, unlockedGoldPoint: 0,       RWD.ORE_CHEST, 10);       // 황금점수 0 : 광석상자 10개
        rebornSupplyBtnList[1].Init(id: 1, unlockedGoldPoint: 10,      RWD.TREASURE_CHEST, 3);   // 황금점수 10 : 보물상자 3개
        rebornSupplyBtnList[2].Init(id: 2, unlockedGoldPoint: 30,      RWD.TIMEPOTION, 3);       // 황금점수 30 : 시간의포션 3개
        rebornSupplyBtnList[3].Init(id: 3, unlockedGoldPoint: 50,      RWD.CRISTAL, 200);        // 황금점수 50 : 크리스탈 200개
        rebornSupplyBtnList[4].Init(id: 4, unlockedGoldPoint: 100,     RWD.ORE_CHEST, 10);       // 황금점수 100 : 광석상자 10개
        rebornSupplyBtnList[5].Init(id: 5, unlockedGoldPoint: 200,     RWD.TREASURE_CHEST, 3);   // 황금점수 200 : 보물상자 3개
        rebornSupplyBtnList[6].Init(id: 6, unlockedGoldPoint: 300,     RWD.TIMEPOTION, 3);       // 황금점수 300 : 시간의포션 3개
        rebornSupplyBtnList[7].Init(id: 7, unlockedGoldPoint: 400,     RWD.CRISTAL, 300);        // 황금점수 400 : 크리스탈 300개
        rebornSupplyBtnList[8].Init(id: 8, unlockedGoldPoint: 500,     RWD.SKILLPOTION, 2);      // 황금점수 500 : 스킬포인트물약 2개
        rebornSupplyBtnList[9].Init(id: 9, unlockedGoldPoint: 650,     RWD.ORE_CHEST, 20);       // 황금점수 650 : 광석상자 20개
        rebornSupplyBtnList[10].Init(id: 10, unlockedGoldPoint: 800,     RWD.MUSH_BOX1, 5);      // 황금점수 800 : 의문의버섯상자 5개
        rebornSupplyBtnList[11].Init(id: 11, unlockedGoldPoint: 1000,    RWD.MUSH_BOX2, 5);      // 황금점수 1000 : 신비한버섯상자 5개
        rebornSupplyBtnList[12].Init(id: 12, unlockedGoldPoint: 1500,    RWD.TIMEPOTION, 5);     // 황금점수 1500 : 시간의포션 5개
        rebornSupplyBtnList[13].Init(id: 13, unlockedGoldPoint: 2000,    RWD.SKILLPOTION, 3);    // 황금점수 2000 : 스킬포인트물약 3개
        rebornSupplyBtnList[14].Init(id: 14, unlockedGoldPoint: 3000,    RWD.TREASURE_CHEST, 10);// 황금점수 3000 : 보물상자 10개
        rebornSupplyBtnList[15].Init(id: 15, unlockedGoldPoint: 4000,    RWD.ORE_CHEST, 50);     // 황금점수 4000 : 광석상자 50개
        rebornSupplyBtnList[16].Init(id: 16, unlockedGoldPoint: 5000,    RWD.TIMEPOTION, 10);    // 황금점수 5000 : 시간의포션 10개
        rebornSupplyBtnList[17].Init(id: 17, unlockedGoldPoint: 7500,    RWD.MUSH_BOX3, 1);      // 황금점수 7500 : 전설의버섯상자 1개
        rebornSupplyBtnList[18].Init(id: 18, unlockedGoldPoint: 10000,   RWD.TREASURE_CHEST, 20);// 황금점수 10000 : 보물상자 20개
        rebornSupplyBtnList[19].Init(id: 19, unlockedGoldPoint: 15000,   RWD.ORE_CHEST, 100);    // 황금점수 15000 : 광석상자 100개
        rebornSupplyBtnList[20].Init(id: 20, unlockedGoldPoint: 20000,   RWD.TIMEPOTION, 20);    // 황금점수 20000 : 시간의포션 20개
        rebornSupplyBtnList[21].Init(id: 21, unlockedGoldPoint: 30000,   RWD.MUSH_BOX1, 20);     // 황금점수 30000 : 의문의버섯상자 20개
        rebornSupplyBtnList[22].Init(id: 22, unlockedGoldPoint: 40000,   RWD.MUSH_BOX2, 20);     // 황금점수 40000 : 신비한버섯상자 20개
        rebornSupplyBtnList[23].Init(id: 23, unlockedGoldPoint: 50000,   RWD.MUSH_BOX3, 3);      // 황금점수 50000 : 전설의버섯상자 3개
        rebornSupplyBtnList[24].Init(id: 24, unlockedGoldPoint: 75000,   RWD.SKILLPOTION, 5);    // 황금점수 75000 : 스킬포인트물약 5개
        rebornSupplyBtnList[25].Init(id: 25, unlockedGoldPoint: 100000,  RWD.ORE_CHEST, 200);    // 황금점수 100000 : 광석상자 200개
        rebornSupplyBtnList[26].Init(id: 26, unlockedGoldPoint: 150000,  RWD.TREASURE_CHEST, 50);// 황금점수 150000 : 보물상자 50개
        rebornSupplyBtnList[27].Init(id: 27, unlockedGoldPoint: 200000,  RWD.TIMEPOTION, 50);    // 황금점수 200000 : 시간의포션 50개
        rebornSupplyBtnList[28].Init(id: 28, unlockedGoldPoint: 250000,  RWD.MUSH_BOX3, 10);     // 황금점수 250000 : 전설의버섯상자 10개
        rebornSupplyBtnList[29].Init(id: 29, unlockedGoldPoint: 300000,  RWD.SKILLPOTION, 10);   // 황금점수 300000 : 스킬포인트물약 10개

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

    /// <summary>
    /// 일반상점 아이템구매 버튼
    /// </summary>
    /// <param name="rwdType">보상 종류</param>
    /// <param name="rwdCnt">보상 수량</param>
    /// <param name="goldPrice">구매가격 황금코인 (황금코인타입 고정)</param>
    private void OnClickNormalItemBtn(RWD rwdType, int rwdCnt, int goldPrice)
    {
        // 구매가능한지 확인
        if(DM._.DB.statusDB.GoldCoin < goldPrice)
        {
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
            return;
        }

        // 황금코인 감소
        DM._.DB.statusDB.GoldCoin -= goldPrice;

        // 구매 완료
        SoundManager._.PlaySfx(SoundManager.SFX.FameCompleteSFX);
        GM._.rwm.ShowReward(
            new Dictionary<RWD, int> {
                {rwdType, rwdCnt},
            }
        );
        
        // UI 업데이트
        UpdateUI();
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

        // 현재 황금코인 수량 표시(일반상점)
        myGoldCntTxt.text = $"{DM._.DB.statusDB.GoldCoin}";
        // 사용한 골드포인트 표시(환생상점)
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
