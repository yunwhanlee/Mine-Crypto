using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// 연금술 제작필요 아이템UI
/// </summary>
[Serializable]
public struct NeedItemUIFormat
{
    public GameObject obj;                    // 오브젝트
    public Image itemImg;                     // 아이템 이미지
    public TMP_Text needCntTxt;               // (필요한 아이템 수량 / 전체 수량)
}

/// <summary>
///  연금술 매니져
/// </summary>
public class AlchemyManager : MonoBehaviour
{
    public DOTweenAnimation DOTAnim;
    public Color categorySelectedClr;           // 카테고리 선택 색깔
    public Color categoryUnSelectedClr;         // 카테고리 미선택 색깔

    // Element
    public GameObject windowObj;                // 팝업
    public Image[] categoryImgArr;              // 카테고리 아이콘 이미지배열
    public GameObject[] cateScrollRectArr;      // 카테고리별 스크롤 종류배열
    public NeedItemUIFormat[] needItemUIArr;    // 필요한 아이템UI

    [Header("선택한 아이템 정보UI")]
    public Image targetItemImg;
    public TMP_Text targetitemInfoTxt;

    // Value
    public ALCHEMY_CATE cateIdx;                    // 현재 선택한 카테고리 인덱스
    public AlchemyDataSO_Material[] materialData;   // 재료 카테고리 목록 정보(SO)

    void Start() {
        cateIdx = 0;
    }

#region EVENT
    public void OnClickCategoryBtn(int cateIdx)
    {
        this.cateIdx = (ALCHEMY_CATE)cateIdx;
        SetCatetory();
    }

    public void OnClickMaterialItemBtn(int itemIdx)
    {
        UpdateUI(itemIdx);
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

    private void UpdateUI(int itemIdx)
    {
        // 제작필요 아이템UI 리스트 비표시
        Array.ForEach(needItemUIArr, needItemUI => needItemUI.obj.SetActive(false));

        switch (cateIdx)
        {
            case (int)ALCHEMY_CATE.MATERIAL:
                var mtDt = materialData[itemIdx];

                // 선택한 아이템 이미지
                targetItemImg.sprite = mtDt.itemSpr;

                for(int i = 0; i < mtDt.needItemDataArr.Length; i++)
                {
                    // 제작필요 아이템 데이터
                    var itemDt = mtDt.needItemDataArr[i];
                    var itemUI = needItemUIArr[i];

                    // 목록UI 표시
                    itemUI.obj.SetActive(true);

                    // 필요 아이템 수량
                    itemUI.needCntTxt.text = $"{itemDt.Val}";
                }

                targetitemInfoTxt.text = $"haha";
                break;
            
        }
    }

    private void UpdateUI()
    {

    }
#endregion
}
