using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Enum;

/// <summary>
/// 인벤토리 아이템 상세정보 팝업
/// </summary>
public class InventoryDescriptionManager : MonoBehaviour
{
    Action OnConfirmBtnClicked;     // 확인버튼 이벤트: 아이템 소비인지 확인타입인지 구분

    public GameObject windowObj;    // 상세정보 팝업

    public TMP_Text titleTxt;
    public Image itemImg;
    public TMP_Text contentTxt;
    public TMP_Text cntTxt;
    public TMP_Text confirmBtnTxt;

    private StatusDB sttDB;
void Start() {
    sttDB = DM._.DB.statusDB;
}

#region EVENT
    /// <summary>
    /// 인벤토리 아이템 상세정보
    /// </summary>
    /// <param name="invIdx">인벤토리 아이템 인덱스</param>
    public void OnClickInvItemSlot(INV invIdx) {
        windowObj.SetActive(true);

        var invSlotUI = GM._.ivm.invSlotUIArr[(int)invIdx];

        titleTxt.text = invSlotUI.name;
        itemImg.sprite = invSlotUI.itemSpr;
        contentTxt.text = invSlotUI.contentMsg;
        cntTxt.text = invSlotUI.cntTxt.text;

        // 확인버튼 이벤트
        switch(invIdx)
        {
            // 소비아이템 타입
            case INV.TREASURE_CHEST:
                confirmBtnTxt.text = "모두 열기";

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("보물상자 오픈!");

                    // 보상으로 나오는 모든 아이템 Dic
                    Dictionary<RWD, int> rwdDic = new Dictionary<RWD, int>() {
                        {RWD.ORE_TICKET, 0},
                        {RWD.RED_TICKET, 0},
                        {RWD.CRISTAL, 0}
                    };

                    // 모든 보물상자 열기
                    for(int i = 0; i < sttDB.TreasureChest; i++)
                    {
                        // 1.보상 아이템 랜덤선택
                        int random = Random.Range(0, 100);
                        RWD reward = (random < 50)? RWD.ORE_TICKET
                            : (random < 80)? RWD.RED_TICKET
                            : RWD.CRISTAL;
                        
                        // 2.보상 수량 설정
                        int cnt = (reward == RWD.CRISTAL)? Random.Range(1, 6) // 크리스탈이면 1 ~ 5개 랜덤
                            : 1; // 그 이외 1개

                        // 3.보상 Dic에 획득할 수량 추가
                        switch(reward)
                        {
                            case RWD.ORE_TICKET:
                                rwdDic[RWD.ORE_TICKET] += cnt;
                                break;
                            case RWD.RED_TICKET:
                                rwdDic[RWD.RED_TICKET] += cnt;
                                break;
                            case RWD.CRISTAL:
                                rwdDic[RWD.CRISTAL] += cnt;
                                break;
                        }
                    }

                    //* 숙련도 경험치 증가
                    GM._.pfm.proficiencyArr[(int)PROFICIENCY.TREASURE_CHEST].Exp += sttDB.TreasureChest;

                    // 인벤토리 소비아이템 수량 감소
                    sttDB.TreasureChest = 0;

                    //* 보상 획득
                    GM._.rwm.ShowReward (
                        new Dictionary<RWD, int>
                        {
                            {RWD.ORE_TICKET, rwdDic[RWD.ORE_TICKET]},
                            {RWD.RED_TICKET, rwdDic[RWD.RED_TICKET]},
                            {RWD.CRISTAL, rwdDic[RWD.CRISTAL]}
                        }
                    );

                    // 인벤토리 슬롯UI 최신화
                    GM._.ivm.UpdateSlotUI();

                    // 상세정보 수량UI 최신화
                    UpdateConsumedItemCntUI(sttDB.TreasureChest);
                };
                break;
            case INV.ORE_CHEST:
                confirmBtnTxt.text = "모두 열기";

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("광석상자 오픈!");
                    
                    // 보상으로 나오는 모든 아이템 Dic
                    Dictionary<RWD, int> rwdDic = new Dictionary<RWD, int>() {
                        {RWD.ORE1, 0}, {RWD.ORE2, 0}, {RWD.ORE3, 0}, {RWD.ORE4, 0},
                        {RWD.ORE5, 0}, {RWD.ORE6, 0}, {RWD.ORE7, 0}, {RWD.ORE8, 0}
                    };

                    // 광석 3종류 랜덤
                    RWD[] rwdArr = new RWD[3];
                    int[] cntArr = new int[3];

                    // 모든 보물상자 열기
                    for(int i = 0; i < sttDB.OreChest; i++)
                    {
                        // 중복없는 보상 종류선택을 위한 리스트
                        List<RWD> rwdList = new() {
                            RWD.ORE1, RWD.ORE2, RWD.ORE3, RWD.ORE4,
                            RWD.ORE5, RWD.ORE6, RWD.ORE7, RWD.ORE8,
                        };

                        // 중복없는 랜덤 재화 3개 선택
                        for(int j = 0; j < rwdArr.Length; j++)
                        {
                            // 1.보상 아이템 랜덤선택
                            int randIdx = Random.Range(0, rwdList.Count);

                            Debug.Log($"randIdx= {randIdx}");

                            // 2.보상 수량
                            int bestFloor = DM._.DB.stageDB.BestFloorArr[randIdx];
                            int val = 100 + (bestFloor * 100); // 계산식

                            rwdDic[(RWD)randIdx] += val;

                            // 리스트 제거 (중복방지)
                            rwdList.RemoveAt(randIdx);
                        }
                    }

                    //* 숙련도 경험치 증가
                    GM._.pfm.proficiencyArr[(int)PROFICIENCY.ORE_CHEST].Exp += sttDB.OreChest;

                    // 인벤토리 소비아이템 수량 감소
                    sttDB.OreChest = 0;

                    //* 보상 획득
                    GM._.rwm.ShowReward (
                        new Dictionary<RWD, int>
                        {
                            { RWD.ORE1, rwdDic[RWD.ORE1] },
                            { RWD.ORE2, rwdDic[RWD.ORE2] },
                            { RWD.ORE3, rwdDic[RWD.ORE3] },
                            { RWD.ORE4, rwdDic[RWD.ORE4] },
                            { RWD.ORE5, rwdDic[RWD.ORE5] },
                            { RWD.ORE6, rwdDic[RWD.ORE6] },
                            { RWD.ORE7, rwdDic[RWD.ORE7] },
                            { RWD.ORE8, rwdDic[RWD.ORE8] },
                        }
                    );

                    // 인벤토리 슬롯UI 최신화
                    GM._.ivm.UpdateSlotUI();

                    // 상세정보 수량UI 최신화
                    UpdateConsumedItemCntUI(sttDB.OreChest);

                };
                break;
            // 확인용 타입
            default:
                confirmBtnTxt.text = "확인";

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("닫기");
                    windowObj.SetActive(false);
                };
                break;
        }
    }

    /// <summary>
    /// 확인버튼 클릭
    /// </summary>
    public void OnClickConfirmBtn()
    {
        OnConfirmBtnClicked?.Invoke();
    }
#endregion

#region FUNC
    /// <summary>
    /// (소비아이템) 사용한 뒤에 수량UI 업데이트
    /// </summary>
    /// <param name="curCnt"></param>
    private void UpdateConsumedItemCntUI(int curCnt) {
        // 상세정보 팝업 현재 표시중 아이템 최신화
        if(curCnt > 0)
            cntTxt.text = curCnt.ToString(); // 수량
        else
            windowObj.SetActive(false); // 수량이 0이면, 팝업 닫기
    }
#endregion
}
