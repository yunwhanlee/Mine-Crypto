using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// 연금술 제작필요 아이템UI (오른쪽 제작영역)
/// </summary>
[Serializable]
public struct NeedItemUIFormat
{
    public GameObject obj;                    // 오브젝트
    public TMP_Text needCntTxt;               // <이미지태그> {필요한 아이템 수량 / 전체 수량}
}

/// <summary>
///  연금술 매니져
/// </summary>
public class AlchemyManager : MonoBehaviour
{
    public DOTweenAnimation DOTAnim;
    [Header("카테고리 아이콘 색깔")]
    public Color categorySelectedClr;           // 카테고리 선택 색깔
    public Color categoryUnSelectedClr;         // 카테고리 미선택 색깔

    [Header("아이템 버튼 이미지")]
    public Sprite itemBtnSelectedSpr;           // 아이템 버튼 선택 색깔
    public Sprite itemBtnUnSelectedSpr;         // 아이템 버튼 미선택 색깔

    // Element
    public GameObject windowObj;                // 팝업
    public Image[] categoryImgArr;              // 카테고리 아이콘 이미지배열
    public GameObject[] cateScrollRectArr;      // 카테고리별 스크롤 종류배열
    [Header("카테고리별 버튼 이미지배열")]
    public Image[] materialItemBtnImgArr;       // 재료아이템 버튼 이미지배열

    [Header("선택한 아이템 정보UI (오른쪽 제작영역)")]
    public Image targetItemImg;
    public TMP_Text targetitemInfoTxt;
    public NeedItemUIFormat[] needItemUIArr;        // 필요한 아이템UI
    public Slider createAmountControlSlider;        // 생산량 컨트롤 슬라이더UI
    public TMP_Text createAmountTxt;                // 생산량 표시

    // Value
    public ALCHEMY_CATE cateIdx;                    // 현재 선택한 카테고리 인덱스
    public int itemBtnIdx;                          // 현재 선택한 아이템버튼 인덱스
    public int creatableMax;
    public int createCnt;                           // 생산 수량
    public AlchemyDataSO_Material[] materialData;   // 재료 카테고리 목록 정보(SO)

    void Start() {
        // 초기화
        createCnt = 1;
        cateIdx = ALCHEMY_CATE.MATERIAL;
        SetCatetory();
        UpdateUI(0);
    }

#region EVENT
    /// <summary>
    /// 카테고리 탭 선택
    /// </summary>
    public void OnClickCategoryBtn(int cateIdx)
    {
        this.cateIdx = (ALCHEMY_CATE)cateIdx;
        SetCatetory();
    }

    /// <summary>
    /// 재료아이템 스크롤 버튼
    /// </summary>
    /// <param name="itemBtnIdx">재료아이템 버튼인덱스</param>
    public void OnClickMaterialItemBtn(int itemBtnIdx)
    {
        this.itemBtnIdx = itemBtnIdx;
        UpdateUI(itemBtnIdx);
    }

    //TODO OnClickConsumeItemBtn
    //TODO OnClickExchangeItemBtn
    //TODO OnClickDecorationItemBtn

    /// <summary>
    /// 생산수량 조절 슬라이더 핸들 변경시 업데이트
    /// </summary>
    public void OnSliderValueChanged()
    {
        // 최대 제작수량
        int creatableMax = (int)createAmountControlSlider.maxValue;
        // 제작수량 (기준 정수단위)
        createCnt = Mathf.RoundToInt(createAmountControlSlider.value * creatableMax) / creatableMax;
        // 슬라이더 이동
        createAmountControlSlider.value = createCnt;
        // 텍스트 수량 표시
        createAmountTxt.text = $"{createCnt}개";
    }

    public void OnClickCreateBtn()
    {
        var sttDB = DM._.DB.statusDB;

        switch (cateIdx)
        {
            //* 카테고리1: 재료
            case (int)ALCHEMY_CATE.MATERIAL:
                AlchemyDataSO_Material mtDt = materialData[itemBtnIdx];

                if(creatableMax > 0)
                {
                    // 제작에 필요한 아이템 감소
                    for(int i = 0; i < mtDt.needItemDataArr.Length; i++)
                    {
                        // 제작필요 아이템 데이터
                        NeedItemData itemNeedDt = mtDt.needItemDataArr[i];
                        int totalNeedVal = itemNeedDt.Val * createCnt;

                        switch(itemNeedDt.Type)
                        {
                            case INV.ORE1: case INV.ORE2: case INV.ORE3: case INV.ORE4:
                            case INV.ORE5: case INV.ORE6: case INV.ORE7: case INV.ORE8:
                                sttDB.SetRscArr((int)itemNeedDt.Type, -totalNeedVal);
                                break;
                            case INV.MAT1: case INV.MAT2: case INV.MAT3: case INV.MAT4:
                            case INV.MAT5: case INV.MAT6: case INV.MAT7: case INV.MAT8: 
                                sttDB.SetMatArr((int)itemNeedDt.Type - (int)INV.MAT1, -totalNeedVal);
                                break;
                        }
                    }

                    // 제작아이템 추가
                    sttDB.SetMatArr((int)mtDt.type, createCnt);

                    GM._.ui.ShowNoticeMsgPopUp($"{createCnt}개 제작 완료!");
                }
                else
                {
                    GM._.ui.ShowWarningMsgPopUp("제작에 필요한 재료가 부족합니다!");
                }

                break;
        }
        UpdateUI(itemBtnIdx);
    }
#endregion

#region FUNC
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        SetCatetory();
    }

    private void SetCatetory()
    {
        for(int i = 0; i < categoryImgArr.Length; i++)
        {
            bool isSameCate = (int)cateIdx == i;

            // 카테고리 색깔
            categoryImgArr[i].color = isSameCate? categorySelectedClr : categoryUnSelectedClr;

            // 카테고리별 스크롤 콘텐츠
            cateScrollRectArr[i].gameObject.SetActive(isSameCate);
        }

        // UpdateUI(idx);
    }

    private void UpdateUI(int itemBtnIdx)
    {
        var sttDB = DM._.DB.statusDB;

        // 제작필요 아이템UI 리스트 비표시
        Array.ForEach(needItemUIArr, needItemUI => needItemUI.obj.SetActive(false));

        switch (cateIdx)
        {
            //* 카테고리1: 재료
            case (int)ALCHEMY_CATE.MATERIAL:
                AlchemyDataSO_Material mtDt = materialData[itemBtnIdx];

                // 아이템버튼 미선택 이미지 초기화
                Array.ForEach(materialItemBtnImgArr, btnImg => btnImg.sprite = itemBtnUnSelectedSpr);

                // 선택한 아이템버튼 노란색 이미지로 변경
                materialItemBtnImgArr[itemBtnIdx].sprite = itemBtnSelectedSpr;

                // 선택한 아이템 이미지
                targetItemImg.sprite = mtDt.itemSpr;

                int[] maxCreateMatArr = new int[2];

                for(int i = 0; i < mtDt.needItemDataArr.Length; i++)
                {
                    // 제작필요 아이템 데이터
                    NeedItemData itemNeedDt = mtDt.needItemDataArr[i];
                    NeedItemUIFormat itemUI = needItemUIArr[i];

                    // 목록UI 표시
                    itemUI.obj.SetActive(true);

                    // 현재 아이템 수량
                    int curItemVal = sttDB.GetInvItemVal(itemNeedDt.Type);

                    // 필요 아이템을 나눈 값
                    maxCreateMatArr[i] = curItemVal / itemNeedDt.Val;

                    // 아이템제작 텍스트 표시
                    string colorTag = curItemVal >= itemNeedDt.Val? "green" : "red";
                    itemUI.needCntTxt.text = $"<sprite name={itemNeedDt.Type}>  <color={colorTag}>{itemNeedDt.Val}</color> / {curItemVal}";
                }

                // 제작가능한 최대수량
                creatableMax = Mathf.Min(maxCreateMatArr[0], maxCreateMatArr[1]);

                // 슬라이더 설정
                if(creatableMax > 0) {
                    createAmountControlSlider.maxValue = creatableMax; // 최대값 설정
                    createAmountControlSlider.interactable = true; // 슬라이더 활성화
                }
                else {
                    createAmountTxt.text = "1개";
                    createAmountControlSlider.value = 1; // 하나도 못 만들면 0으로 고정
                    createAmountControlSlider.interactable = false; // 슬라이더 비활성화
                }

                break;
        }
    }


#endregion
}
