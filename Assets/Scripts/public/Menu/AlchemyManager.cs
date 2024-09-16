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
    public Image[] consumeItemBtnImgArr;        // 소비아이템 버튼 이미지배열
    public Image[] exchangeItemBtnImgArr;       // 교환아이템 버튼 이미지배열
    public Image[] decoItemBtnImgArr;           // 장식아이템 버튼 이미지배열

    [Header("장식 오브젝트배열")]
    public GameObject[] decoObjArr;             // 장식 오브젝트배열

    [Header("선택한 아이템 정보UI (오른쪽 제작영역)")]
    public Image targetItemImg;
    public TMP_Text targetitemInfoTxt;
    public NeedItemUIFormat[] needItemUIArr;        // 필요한 아이템UI
    public Slider createAmountControlSlider;        // 생산량 컨트롤 슬라이더UI
    public TMP_Text createAmountTxt;                // 생산량 표시
    public Button createBtn;                        // 생산 버튼

    // Value
    public ALCHEMY_CATE cateIdx;                    // 현재 선택한 카테고리 인덱스
    public int itemBtnIdx;                          // 현재 선택한 아이템버튼 인덱스
    public int creatableMax;
    public int createCnt;                           // 생산 수량

    [Header("(연금술) 아이템 SO데이터 정보")]
    public AlchemyDataSO_Material[] materialData;       // 재료
    public AlchemyDataSO_Consume[]  consumeItemData;    // 소비 아이템
    public AlchemyDataSO_Exchange[] exchangeItemData;   // 교환 아이템
    public AlchemyDataSO_Deco[] decoItemData;           // 장식 아이템

    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

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
        UpdateUI(0);
    }

    /// <summary>
    /// 스크롤 아이템버튼
    /// </summary>
    /// <param name="itemBtnIdx">아이템버튼 인덱스</param>
    public void OnClickScrollItemBtn(int itemBtnIdx)
    {
        this.itemBtnIdx = itemBtnIdx;
        UpdateUI(itemBtnIdx);
    }

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
        createAmountTxt.text = $"{createCnt * (cateIdx == ALCHEMY_CATE.EXCHANGE? 100 : 1)}개";
    }

    /// <summary>
    /// 아이템 제작버튼
    /// </summary>
    public void OnClickCreateBtn()
    {
        var sttDB = DM._.DB.statusDB;
        AlchemyDataSO itemDt = null;

        // 카테고리에서 선택한 아이템 데이터 취득
        switch(cateIdx)
        {
            case ALCHEMY_CATE.MATERIAL: itemDt = materialData[itemBtnIdx]; break;
            case ALCHEMY_CATE.CONSUME_ITEM: itemDt = consumeItemData[itemBtnIdx]; break;
            case ALCHEMY_CATE.EXCHANGE: itemDt = exchangeItemData[itemBtnIdx]; break;
            case ALCHEMY_CATE.DECORATION: itemDt = decoItemData[itemBtnIdx]; break;
        }

        // 선택한 수량만큼 생성
        if(creatableMax > 0)
        {
            //* 제작에 필요한 아이템 감소
            for(int i = 0; i < itemDt.needItemDataArr.Length; i++)
            {
                // 제작필요 아이템 데이터
                NeedItemData needItemDt = itemDt.needItemDataArr[i];

                int needItemVal = cateIdx == ALCHEMY_CATE.MATERIAL? ApplyDecMatPer(needItemDt.Val) : needItemDt.Val;
                int totalNeedVal = needItemVal * createCnt;

                // 제작에 필요한 아이템 수량 감소
                switch(needItemDt.Type)
                {
                    case INV.ORE1: case INV.ORE2: case INV.ORE3: case INV.ORE4:
                    case INV.ORE5: case INV.ORE6: case INV.ORE7: case INV.ORE8:
                        sttDB.SetRscArr((int)needItemDt.Type, -totalNeedVal);
                        break;
                    case INV.MAT1: case INV.MAT2: case INV.MAT3: case INV.MAT4:
                    case INV.MAT5: case INV.MAT6: case INV.MAT7: case INV.MAT8: 
                        sttDB.SetMatArr((int)needItemDt.Type - (int)INV.MAT1, -totalNeedVal);
                        break;
                    case INV.MUSH1: case INV.MUSH2: case INV.MUSH3: case INV.MUSH4:
                    case INV.MUSH5: case INV.MUSH6: case INV.MUSH7: case INV.MUSH8:
                        sttDB.SetMsrArr((int)needItemDt.Type - (int)INV.MUSH1, -totalNeedVal);
                        break;
                    case INV.ORE_TICKET:
                        sttDB.OreTicket -= totalNeedVal;
                        break;
                    case INV.RED_TICKET:
                        sttDB.RedTicket -= totalNeedVal;
                        break;
                    case INV.ORE_CHEST: 
                        sttDB.OreChest -= totalNeedVal;
                        break;
                    case INV.TREASURE_CHEST:
                        sttDB.TreasureChest -= totalNeedVal;
                        break;
                }
            }

            //* 제작한 아이템 증가
            if(itemDt is AlchemyDataSO_Material) // 재료
            {
                var mtDt = itemDt as AlchemyDataSO_Material;

                // 수량 추가
                sttDB.SetMatArr((int)mtDt.type, createCnt); 
            }
            else if(itemDt is AlchemyDataSO_Consume) // 소비아이템
            {
                var csDt = itemDt as AlchemyDataSO_Consume;

                // 수량 추가
                switch(csDt.type)
                {
                    case CONSUME.ORE_TICKET: sttDB.OreTicket += createCnt; break;
                    case CONSUME.RED_TICKET: sttDB.RedTicket += createCnt; break;
                    case CONSUME.ORE_CHEST: sttDB.OreChest += createCnt; break;
                    case CONSUME.TREASURE_CHEST: sttDB.TreasureChest += createCnt; break;
                    case CONSUME.MUSH_BOX1: sttDB.MushBox1 += createCnt; break;
                    case CONSUME.MUSH_BOX2: sttDB.MushBox2 += createCnt; break;
                    case CONSUME.MUSH_BOX3: sttDB.MushBox3 += createCnt; break;
                }
            }
            else if(itemDt is AlchemyDataSO_Exchange) // 교환아이템
            {
                var excDt = itemDt as AlchemyDataSO_Exchange;
                Debug.Log($"Exchange:: type={(int)excDt.type}");
                // 수량 추가
                sttDB.SetRscArr((int)excDt.type, createCnt * 100);
            }
            else if(itemDt is AlchemyDataSO_Deco) // 장식아이템
            {
                var dcDt = itemDt as AlchemyDataSO_Deco;

                dcDt.IsBuyed = true;
                dcDt.Obj.SetActive(true);
            }

            // 업데이트 UI
            UpdateUI(itemBtnIdx);

            GM._.ui.ShowNoticeMsgPopUp($"{createCnt}개 제작 완료!");
        }
        else
        {
            GM._.ui.ShowWarningMsgPopUp("제작에 필요한 재료가 부족합니다!");
        }
    }
#endregion

#region FUNC
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        SetCatetory();
    }

    /// <summary>
    /// 강화에 필요한 재료 제작비용 감소 적용
    /// </summary>
    /// <returns></returns>
    private int ApplyDecMatPer(int needItemVal)
    {
        // 제작비용 감소 % 적용
        float decMatPer = 1 - GM._.sttm.DecAlchemyMaterialPer;
        return Mathf.RoundToInt(needItemVal * decMatPer);
    }

    /// <summary>
    /// 카테고리 설정
    /// </summary>
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

    /// <summary>
    /// 제작에 필요한 아이템 업데이트
    /// </summary>
    /// <param name="cateDt">현재 카테고리군 데이터</param>
    /// <param name="idx">아이템 인덱스</param>
    /// <returns></returns>
    private int UpdateNeedItem(AlchemyDataSO cateDt, int idx)
    {
        // 제작필요 아이템 데이터
        NeedItemData needItemDt = cateDt.needItemDataArr[idx];
        NeedItemUIFormat itemUI = needItemUIArr[idx];

        // 제작비용 감소 % 적용

        int needItemVal = cateIdx == ALCHEMY_CATE.MATERIAL? ApplyDecMatPer(needItemDt.Val) : needItemDt.Val;

        // 목록UI 표시
        itemUI.obj.SetActive(true);
        // 현재 아이템 수량
        int curItemVal = DM._.DB.statusDB.GetInventoryItemVal(needItemDt.Type);
        // 아이템제작 텍스트 표시
        string colorTag = curItemVal >= needItemVal? "green" : "red";
        itemUI.needCntTxt.text = $"<sprite name={needItemDt.Type}>  <color={colorTag}>{needItemVal}</color> / {curItemVal}";
        // 필요 아이템을 나눈 값
        return curItemVal / needItemVal;
    }

    /// <summary>
    /// 생산수량 슬라이더 설정
    /// </summary>
    private void SetSlider()
    {
        if(creatableMax > 0) {
            createAmountControlSlider.maxValue = creatableMax; // 최대값 설정
            createAmountControlSlider.interactable = true; // 슬라이더 활성화
        }
        else {
            // createAmountTxt.text = "1개";
            createAmountControlSlider.value = 1; // 하나도 못 만들면 0으로 고정
            createAmountControlSlider.interactable = false; // 슬라이더 비활성화
        }
        
        // 슬라이더UI 업데이트
        OnSliderValueChanged();
    }

    /// <summary>
    /// 업데이트 UI
    /// </summary>
    /// <param name="itemBtnIdx">아이템버튼 인덱스</param>
    private void UpdateUI(int itemBtnIdx)
    {
        var sttDB = DM._.DB.statusDB;

        //* 초기화
        // 제작필요 아이템UI 리스트 비표시
        Array.ForEach(needItemUIArr, needItemUI => needItemUI.obj.SetActive(false));
        // 슬라이더 표시
        createAmountControlSlider.gameObject.SetActive(true);
        // 생산버튼 활성화
        createBtn.interactable = true;

        switch (cateIdx)
        {
            //* 카테고리1: 재료
            case ALCHEMY_CATE.MATERIAL:
            {
                var mtDt = materialData[itemBtnIdx];
                int[] maxCreateMatArr = new int[mtDt.needItemDataArr.Length];

                // 아이템버튼 미선택 이미지 초기화
                Array.ForEach(materialItemBtnImgArr, btnImg => btnImg.sprite = itemBtnUnSelectedSpr);
                // 선택한 아이템버튼 노란색 이미지로 변경
                materialItemBtnImgArr[itemBtnIdx].sprite = itemBtnSelectedSpr;
                // 선택한 아이템 이미지
                targetItemImg.sprite = mtDt.itemSpr;

                // 제작필요 아이템 업데이트
                for(int i = 0; i < mtDt.needItemDataArr.Length; i++)
                    maxCreateMatArr[i] = UpdateNeedItem(mtDt, i);

                // 제작가능한 최대수량
                creatableMax = Mathf.Min(maxCreateMatArr);

                // 슬라이더 설정
                SetSlider();

                // 현재보유량 표시
                targetitemInfoTxt.text = $"보유량: {DM._.DB.statusDB.MatArr[itemBtnIdx]}";

                break;
            }
            //* 카테고리2: 소비아이템
            case ALCHEMY_CATE.CONSUME_ITEM:
            {
                var csDt = consumeItemData[itemBtnIdx];
                int[] maxCreateMatArr = new int[csDt.needItemDataArr.Length];

                // 아이템버튼 미선택 이미지 초기화
                Array.ForEach(consumeItemBtnImgArr, btnImg => btnImg.sprite = itemBtnUnSelectedSpr);
                // 선택한 아이템버튼 노란색 이미지로 변경
                consumeItemBtnImgArr[itemBtnIdx].sprite = itemBtnSelectedSpr;
                // 선택한 아이템 이미지
                targetItemImg.sprite = csDt.itemSpr;

                // 제작필요 아이템 업데이트
                for(int i = 0; i < csDt.needItemDataArr.Length; i++)
                    maxCreateMatArr[i] = UpdateNeedItem(csDt, i);

                // 제작가능한 최대수량
                creatableMax = Mathf.Min(maxCreateMatArr);

                // 슬라이더 설정
                SetSlider();

                // 현재보유량 표시
                int val = 0;
                switch(csDt.type)
                {
                    case CONSUME.ORE_TICKET: val = sttDB.OreTicket; break;
                    case CONSUME.RED_TICKET: val = sttDB.RedTicket; break;
                    case CONSUME.ORE_CHEST: val = sttDB.OreChest; break;
                    case CONSUME.TREASURE_CHEST: val = sttDB.TreasureChest; break;
                    case CONSUME.MUSH_BOX1: val = sttDB.MushBox1; break;
                    case CONSUME.MUSH_BOX2: val = sttDB.MushBox2; break;
                    case CONSUME.MUSH_BOX3: val = sttDB.MushBox3; break;
                }
                targetitemInfoTxt.text = $"보유량: {val}";

                break;
            }
            //* 카테고리3: 교환
            case ALCHEMY_CATE.EXCHANGE:
            {
                var excDt = exchangeItemData[itemBtnIdx];
                int[] maxCreateMatArr = new int[excDt.needItemDataArr.Length];

                // 아이템버튼 미선택 이미지 초기화
                Array.ForEach(exchangeItemBtnImgArr, btnImg => btnImg.sprite = itemBtnUnSelectedSpr);
                // 선택한 아이템버튼 노란색 이미지로 변경
                exchangeItemBtnImgArr[itemBtnIdx].sprite = itemBtnSelectedSpr;
                // 선택한 아이템 이미지
                targetItemImg.sprite = excDt.itemSpr;

                // 제작필요 아이템 업데이트
                for(int i = 0; i < excDt.needItemDataArr.Length; i++)
                    maxCreateMatArr[i] = UpdateNeedItem(excDt, i);

                // 제작가능한 최대수량
                creatableMax = Mathf.Min(maxCreateMatArr);

                // 슬라이더 설정
                SetSlider();

                // 현재보유량 표시
                targetitemInfoTxt.text = $"보유량: {DM._.DB.statusDB.RscArr[itemBtnIdx]}";

                break;
            }
            //* 카테고리4: 장식
            case ALCHEMY_CATE.DECORATION:
            {
                var dcDt = decoItemData[itemBtnIdx];
                int[] maxCreateMatArr = new int[dcDt.needItemDataArr.Length];

                // 아이템버튼 미선택 이미지 초기화
                Array.ForEach(decoItemBtnImgArr, btnImg => btnImg.sprite = itemBtnUnSelectedSpr);
                // 선택한 아이템버튼 노란색 이미지로 변경
                decoItemBtnImgArr[itemBtnIdx].sprite = itemBtnSelectedSpr;
                // 선택한 아이템 이미지
                targetItemImg.sprite = dcDt.itemSpr;

                // 제작필요 아이템 업데이트
                if(dcDt.IsBuyed)
                {
                    createAmountTxt.text = "보유 중";
                    createBtn.interactable = false; // 생산버튼 비활성화
                }
                else
                {
                    createAmountTxt.text = "";
                    for(int i = 0; i < dcDt.needItemDataArr.Length; i++)
                        maxCreateMatArr[i] = UpdateNeedItem(dcDt, i);
                }

                // 제작가능한 최대수량
                creatableMax = Mathf.Min(maxCreateMatArr);

                // 슬라이더 설정
                createAmountControlSlider.gameObject.SetActive(false);

                // 추가능력 표시
                switch (dcDt.abilityType)
                {
                    case DECO_ABT.ATK_PER:
                        targetitemInfoTxt.text = $"추가 공격력 +{dcDt.AbilityVal_ShowTxt * 100}%";
                        break;
                    case DECO_ABT.ATKSPD_PER:
                        targetitemInfoTxt.text = $"추가 이동속도 +{dcDt.AbilityVal_ShowTxt * 100}%";
                        break;
                    case DECO_ABT.MOVSPD_PER:
                        targetitemInfoTxt.text = $"공격속도 +{dcDt.AbilityVal_ShowTxt * 100}%";
                        break;
                    case DECO_ABT.INC_POPULATION:
                        targetitemInfoTxt.text = $"소환 캐릭터 +{dcDt.AbilityVal_ShowTxt}";
                        break;
                    case DECO_ABT.INC_FAME:
                        targetitemInfoTxt.text = $"추가 명예 +{dcDt.AbilityVal_ShowTxt}";
                        break;
                }
                break;
            }
        }
    }


#endregion
}
