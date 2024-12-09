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
    public GameObject windowObj;
    public DOTweenAnimation DOTAnim;

    public FameSupplyBtn fameSupplyBtnPref; // 명예보급 오브젝트 프리팹
    public GameObject rebornSupplyBtnPref;  // 환생보급 오브젝트 프리팹

    public ScrollRect normalScrollRect;       // 일반상점 스크롤
    public ScrollRect fameSupplyScrollRect;   // 명예보급 스크롤
    public ScrollRect rebornSupplyScrollRect; // 환생보급 스크롤
    public ScrollRect inAppScrollRect;        // (인앱샵) 황금상점 스크롤

    public Transform normalContentTf;       // 일반상점 오브젝트 부모 Transform
    public Transform fameSupplyContentTf;   // 명예보급 오브젝트 부모 Transform
    public Transform rebornSupplyContentTf; // 환생보급 오브젝트 부모 Transform
    public Transform inAppContentTf;        // (인앱샵) 황금상점 오브젝트 부모 Transform

    private List<FameSupplyBtn> fameSupplyBtnList; // 명예보급 리스트
    //TODO private List<RebornSupplyBtn> rebornSupplyBtnList; // 환생보급 리스트

    public TMP_Text[] cateTxtArr;                // 카테고리 텍스트
    public SHOP_CATE cateIdx;                    // 현재 선택한 카테고리 인덱스

    public TMP_Text GoldPointTxt;                // 사용한 골드 포인트 텍스트


    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        // 카테고리 초기화
        cateIdx = SHOP_CATE.NORMAL;

        fameSupplyBtnList = new List<FameSupplyBtn>();

        // 명예보급 오브젝트 생성 후 리스트 추가
        for(int i = 0; i < 15; i++)
        {
            fameSupplyBtnList.Add(Instantiate(fameSupplyBtnPref, fameSupplyContentTf));
        }

        // 데이터 및 UI 초기화
        fameSupplyBtnList[0].Init(unlockedLv: 2, RWD.GOLDCOIN, 2);
        fameSupplyBtnList[1].Init(unlockedLv: 4, RWD.TIMEPOTION, 5);
        fameSupplyBtnList[2].Init(unlockedLv: 6, RWD.TREASURE_CHEST, 10);
        fameSupplyBtnList[3].Init(unlockedLv: 8, RWD.SKILLPOTION, 1);
        fameSupplyBtnList[4].Init(unlockedLv: 10, RWD.CRISTAL, 500);
        fameSupplyBtnList[5].Init(unlockedLv: 12, RWD.GOLDCOIN, 5);
        fameSupplyBtnList[6].Init(unlockedLv: 14, RWD.TIMEPOTION, 10);
        fameSupplyBtnList[7].Init(unlockedLv: 16, RWD.TREASURE_CHEST, 20);
        fameSupplyBtnList[8].Init(unlockedLv: 18, RWD.SKILLPOTION, 2);
        fameSupplyBtnList[9].Init(unlockedLv: 20, RWD.CRISTAL, 1000);
        fameSupplyBtnList[10].Init(unlockedLv: 22, RWD.GOLDCOIN, 10);
        fameSupplyBtnList[11].Init(unlockedLv: 24, RWD.TIMEPOTION, 20);
        fameSupplyBtnList[12].Init(unlockedLv: 26, RWD.TREASURE_CHEST, 30);
        fameSupplyBtnList[13].Init(unlockedLv: 28, RWD.SKILLPOTION, 3);
        fameSupplyBtnList[14].Init(unlockedLv: 30, RWD.CRISTAL, 3000);
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
#endregion
}
