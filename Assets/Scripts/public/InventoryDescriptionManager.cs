using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public void OnClickInvItemSlot(Enum.INV invIdx) {
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
            case Enum.INV.TREASURE_CHEST:
                confirmBtnTxt.text = "열기";

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("보물상자 오픈!");

                    // 인벤토리 소비아이템 수량 감소
                    sttDB.TreasureChest--;

                    // 1.보상 아이템
                    int random = Random.Range(0, 100);
                    Enum.RWD reward = (random < 50)? Enum.RWD.ORE_TICKET
                        : (random < 80)? Enum.RWD.RED_TICKET
                        : Enum.RWD.CRISTAL;
                    
                    // 2.보상 수량
                    int cnt = (reward == Enum.RWD.CRISTAL)? Random.Range(1, 6)
                        : 1;

                    //* 보상 획득
                    GM._.rwm.ShowReward (
                        new Dictionary<Enum.RWD, int>
                        {
                            { reward, cnt }
                        }
                    );

                    // 인벤토리 슬롯UI 최신화
                    GM._.ivm.UpdateSlotUI();

                    // 상세정보 수량UI 최신화
                    UpdateConsumedItemCntUI(sttDB.TreasureChest);
                };
                break;
            case Enum.INV.ORE_CHEST:
                confirmBtnTxt.text = "열기";

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("광석상자 오픈!");
                    
                    // 인벤토리 소비아이템 수량 감소
                    sttDB.OreChest--;

                    // 광석 3종류 랜덤
                    List<Enum.RWD> rwdList = new() {
                        Enum.RWD.ORE1, Enum.RWD.ORE2, Enum.RWD.ORE3,
                        Enum.RWD.ORE4, Enum.RWD.ORE5, Enum.RWD.ORE6,
                        Enum.RWD.ORE7, Enum.RWD.ORE8,
                    };

                    Enum.RWD[] RwdArr = new Enum.RWD[3];
                    int[] cntArr = new int[3];

                    // 중복X 랜덤 3회
                    for(int i = 0; i < RwdArr.Length; i++)
                    {
                        int randIdx = Random.Range(0, rwdList.Count);

                        // 1.보상 아이템
                        RwdArr[i] = rwdList[randIdx];                    

                        // 2.보상 수량
                        int bestFloor = DM._.DB.bestFloorDB.OreArr[randIdx];
                        cntArr[i] = 100 + (bestFloor * 100); // 계산식

                        rwdList.RemoveAt(randIdx);
                    }

                    //* 보상 획득
                    GM._.rwm.ShowReward (
                        new Dictionary<Enum.RWD, int>
                        {
                            { RwdArr[0], cntArr[0] },
                            { RwdArr[1], cntArr[1] },
                            { RwdArr[2], cntArr[2] },
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
