using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// (PUBLIC) 보상 표시 팝업 (게임종료 및 보상수령에 사용)
/// </summary>
public class RewardUIManager : MonoBehaviour
{
    public RewardResultData rewardResultData;   // 보상 결과 데이터관리
    public RewardSlotUI[] rewardSlotUIArr;      // 보상 슬롯UI 객체배열

    public GameObject rewardUIPopUp;            // 보상 슬롯UI 팝업

    [Header("모든 보상템을 ContentTf안에 미리 만들고 IDX로 처리")]
    public Transform contentTf;

    void Start()
    {
        InitDataAndUI();
    }

    void Update() {
        //! TEST
        if(Input.GetKeyDown(KeyCode.V)) {
            // 수령할 보상 데이터 (결과팝업 표시)
            rewardResultData.ShowResultReward (
                new Dictionary<Enum.RWD, int>
                {
                    { Enum.RWD.CRISTAL, 1000 },
                    { Enum.RWD.ORE1, 100 }
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
    /// 보상 관련 데이터 및 오브젝트 초기화 (1회)
    /// </summary>
    private void InitDataAndUI() {
        // 보상 결과 Dictionary객체 생성
        rewardResultData = new RewardResultData();

        // 보상슬롯UI를 배열로 저장 (미리 ContentTf에 슬롯UI 추가하여 준비)
        rewardSlotUIArr = new RewardSlotUI[contentTf.childCount];

        InitRwdSlotUIArr();
        ResetAllSlotUI();
    }

    /// <summary>
    /// 보상슬롯UI 객체 생성 (1회)
    /// </summary>
    private void InitRwdSlotUIArr() {
        for(int i = 0; i < contentTf.childCount; i++)
        {
            rewardSlotUIArr[i] = new RewardSlotUI
            {
                name = Enum.GetRewardItemName((Enum.RWD)i),
                rwdType = (Enum.RWD)i,
                obj = contentTf.GetChild(i).gameObject,
                cntTxt = contentTf.GetChild(i).GetComponentInChildren<TMP_Text>()
            };
        }
    }

    /// <summary>
    /// 보상슬롯UI 리셋 초기화
    /// </summary>
    public void ResetAllSlotUI() {
        for(int i = 0; i < contentTf.childCount; i++)
        {
            rewardSlotUIArr[i].cntTxt.text = "0";
            rewardSlotUIArr[i].obj.SetActive(false); // 비표시
        }
    }

    /// <summary>
    /// (게임오버) 획득한 재화수량 팝업 표시 
    /// </summary>
    public void ShowResultRscUI(int[] resultRscArr)
    {
        // 보상슬롯 팝업 표시
        GM._.rwm.rewardUIPopUp.SetActive(true);

        // 보상슬롯UI 리셋 초기화
        ResetAllSlotUI();

        // 재화 표시
        int i = 0;
        foreach(int rsc in resultRscArr)
        {
            // 획득한 재화인 경우
            if(rsc > 0)
            {
                // 해당 보상슬롯 표시
                GM._.rwm.rewardSlotUIArr[i].obj.SetActive(true);
                GM._.rwm.rewardSlotUIArr[i].cntTxt.text = rsc.ToString();
            }

            i++;
        }
    }
#endregion
}
