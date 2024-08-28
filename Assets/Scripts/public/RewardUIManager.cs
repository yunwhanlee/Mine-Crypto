using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static Enum;

/// <summary>
/// (PUBLIC) 보상 표시 팝업 (게임종료 및 보상수령에 사용)
/// </summary>
public class RewardUIManager : MonoBehaviour
{
    public GameObject rewardUIPopUp;            // 보상 팝업
    public RewardSlotUI[] rewardSlotUIArr;      // 보상 슬롯UI 객체배열

    [Header("모든 보상템을 ContentTf안에 미리 만들고 IDX로 처리")]
    public Transform contentTf;

    void Start()
    {
        InitDataAndUI();
    }

    void Update() {
        //! 보상 테스트
        if(Input.GetKeyDown(KeyCode.V)) {

            // 보상획득 (결과팝업 표시)
            ShowReward (
                new Dictionary<RWD, int>
                {
                    { RWD.CRISTAL, 1000 },
                    { RWD.ORE1, 100 },
                    { RWD.ORE_CHEST, 10 },
                }
            );
        }
    }

#region EVENT
    /// <summary>
    ///  보상 슬롯UI 팝업 닫기
    /// </summary>
    public void OnClickDimScreenBtn() {
        switch(GM._.gameState)
        {
            case GameState.GAMEOVER:
                GM._.gameState = GameState.HOME;
                rewardUIPopUp.SetActive(false);
                GM._.hm.HomeWindow.SetActive(true);
                GM._.pm.InitPlayData();
                break;
            default:
                //TODO
                rewardUIPopUp.SetActive(false);
                break;
        }
    }
#endregion

#region FUNC
    /// <summary>
    /// 보상 관련 데이터 및 객체 초기화 (1회)
    /// </summary>
    private void InitDataAndUI() {
        // 보상슬롯UI를 배열로 저장 (미리 ContentTf에 슬롯UI 추가하여 준비)
        rewardSlotUIArr = new RewardSlotUI[contentTf.childCount];
        InitElement();
        ResetAllSlotUI();
    }

    /// <summary>
    /// 모든 아이템슬롯UI 객체 초기화 (1회)
    /// </summary>
    private void InitElement() {
        for(int i = 0; i < contentTf.childCount; i++)
        {
            rewardSlotUIArr[i] = new RewardSlotUI
            {
                name = GetRewardItemName((RWD)i),
                rwdType = (RWD)i,
                obj = contentTf.GetChild(i).gameObject,
                cntTxt = contentTf.GetChild(i).GetComponentInChildren<TMP_Text>()
            };
        }
    }

    /// <summary>
    /// 보상슬롯UI 리셋 초기화
    /// </summary>
    private void ResetAllSlotUI() {
        for(int i = 0; i < contentTf.childCount; i++)
        {
            rewardSlotUIArr[i].cntTxt.text = "0";
            rewardSlotUIArr[i].obj.SetActive(false); // 비표시
        }
    }

    /// <summary>
    ///* (게임오버) 획득한 보상 팝업 표시 
    /// </summary>
    public void ShowGameoverReward(int[] gameoverRwdArr)
    {
        // 보상슬롯 팝업 표시
        GM._.rwm.rewardUIPopUp.SetActive(true);

        // 보상슬롯UI 리셋 초기화
        ResetAllSlotUI();

        // 획득한 보상아이템 표시
        int i = 0;
        foreach(int rwd in gameoverRwdArr)
        {
            // 보상중에 획득한게 있는 경우
            if(rwd > 0)
            {
                // 해당 보상슬롯 표시
                GM._.rwm.rewardSlotUIArr[i].obj.SetActive(true);
                GM._.rwm.rewardSlotUIArr[i].cntTxt.text = rwd.ToString();
            }
            i++;
        }
    }

    /// <summary>
    /// 수령할 보상 팝업 표시 및 수치 증가
    /// </summary>
    /// <param name="rwdDic">수령할 보상 Dic리스트</param>
    public void ShowReward(Dictionary<RWD, int> rwdDic) {
        // 보상슬롯 팝업 표시
        GM._.rwm.rewardUIPopUp.SetActive(true);

        // 보상슬롯UI 리셋 초기화
        GM._.rwm.ResetAllSlotUI();

        // 획득할 보상아이템 처리
        foreach(var rwd in rwdDic)
        {
            if(rwd.Value > 0)
            {
                StatusDB sttDB = DM._.DB.statusDB;
                RWD rwdType = rwd.Key;     // 타입
                int val = rwd.Value;            // 획득량

                switch(rwdType)
                {
                    case RWD.ORE1:
                    case RWD.ORE2:
                    case RWD.ORE3:
                    case RWD.ORE4:
                    case RWD.ORE5:
                    case RWD.ORE6:
                    case RWD.ORE7:
                    case RWD.ORE8:
                    case RWD.CRISTAL: // 타겟재화 추가
                        val = sttDB.SetRscArr((int)rwdType, val);
                        break;
                    case RWD.ORE_TICKET: // 광석 입장티켓
                        sttDB.OreTicket += val;
                        break;
                    case RWD.RED_TICKET: // 시련의광산 입장티켓
                        sttDB.RedTicket += val;
                        break;
                    case RWD.TREASURE_CHEST: // 보물상자
                        sttDB.TreasureChest += val;
                        break;
                    case RWD.ORE_CHEST: // 광석상자
                        sttDB.OreChest += val;
                        break;
                    case RWD.FAME: // 명성포인트
                        sttDB.Fame += val;
                        break;
                    // 여기에 추가
                }

                // 해당 보상슬롯UI 표시
                GM._.rwm.rewardSlotUIArr[(int)rwdType].obj.SetActive(true);
                GM._.rwm.rewardSlotUIArr[(int)rwdType].cntTxt.text = val.ToString();
            }
        }
    }
#endregion
}
