using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Enum;

/// <summary>
/// (PUBLIC) 보상 표시 팝업 (게임종료 및 보상수령에 사용)
/// </summary>
public class RewardUIManager : MonoBehaviour
{
    public GameObject rewardUIPopUp;            // 보상 팝업
    public TMP_Text newBestFloorMsgTxt;         // NEW 최고층 달성 메세지 텍스트
    public RewardSlotUI[] rewardSlotUIArr;      // 보상 슬롯UI 객체배열

    [Header("모든 보상템을 ContentTf안에 미리 만들고 IDX로 처리")]
    public Transform contentTf;

    int rwdCnt = 0;

    void Start()
    {
        InitDataAndUI();
    }

    void Update() {
        //! 보상 테스트
        if(Input.GetKeyDown(KeyCode.V)) {
            // 보상획득 (결과팝업 표시)
            if(rwdCnt % 2 == 0) // 짝수
            {
                ShowReward (
                    new Dictionary<RWD, int>
                    {
                        { RWD.ORE1, 50000 },
                        { RWD.ORE2, 50000 },
                        { RWD.ORE3, 50000 },
                        { RWD.ORE4, 50000 },
                        { RWD.CRISTAL, 10000 },
                        { RWD.ORE_CHEST, 10 },
                        { RWD.MAT1, 100 },
                        { RWD.MAT2, 100 },
                        { RWD.MAT3, 100 },
                        { RWD.MAT4, 100 },
                        { RWD.MAT5, 100 },
                        { RWD.MAT6, 100 },
                        { RWD.MAT7, 100 },
                        { RWD.MAT8, 100 },
                        { RWD.SKILLPOTION, 50 },
                        { RWD.TIMEPOTION, 50 },
                    }
                );
                rwdCnt++;
            }
            else // 홀수
            {
                ShowReward (
                    new Dictionary<RWD, int>
                    {
                        { RWD.ORE5, 50000 },
                        { RWD.ORE6, 50000 },
                        { RWD.ORE7, 50000 },
                        { RWD.ORE8, 50000 },
                        { RWD.CRISTAL, 50000 },
                        { RWD.TREASURE_CHEST, 100 },
                        { RWD.MUSH1, 200 },
                        { RWD.MUSH2, 200 },
                        { RWD.MUSH3, 200 },
                        { RWD.MUSH4, 200 },
                        { RWD.MUSH5, 200 },
                        { RWD.MUSH6, 200 },
                        { RWD.MUSH7, 200 },
                        { RWD.MUSH8, 200 },
                        { RWD.LIGHTSTONE, 10000 },
                    }
                );
                rwdCnt++;
            }
        }
    }

#region EVENT
    /// <summary>
    ///  보상 슬롯UI 팝업 닫기
    /// </summary>
    public void OnClickDimScreenBtn() {
        // 환생하고난뒤 보상닫기시 게임 재로드
        if(GM._.rbm.isRebornTrigger)
        {
            SceneManager.LoadScene("Game");
            return;
        }

        switch(GM._.gameState)
        {
            //* 인게임 종료의 경우
            case GameState.TIMEOVER:
                GM._.gameState = GameState.HOME;
                rewardUIPopUp.SetActive(false);
                newBestFloorMsgTxt.gameObject.SetActive(false);

                GM._.pm.InitPlayData();

                // 시련의광산 경우 종료시 
                if(GM._.stgm.IsChallengeMode)
                {
                    SoundManager._.PlayBgm(SoundManager.BGM.Home);
                    GM._.clm.ShowPopUp(); // 시련의광산 팝업 표시
                    GM._.hm.Active();
                }
                // 일반광산의 경우 종료시
                else
                {
                    SoundManager._.PlayBgm(SoundManager.BGM.Home);
                    GM._.hm.Active();
                }
                break;
            //* 그 이외
            default:
                rewardUIPopUp.SetActive(false);
                newBestFloorMsgTxt.gameObject.SetActive(false);
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
                DOTAnim = contentTf.GetChild(i).GetComponent<DOTweenAnimation>(),
                obj = contentTf.GetChild(i).gameObject,
                cntTxt = contentTf.GetChild(i).GetComponentInChildren<TMP_Text>(),
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
    /// 보상 아이템 이미지 획득
    /// </summary>
    /// <param name="rwdType">보상타입</param>
    public Sprite GetRewardItemSprite(RWD rwdType)
    {
        const int ICON_ITEM = 1;

        Transform iconItemTf = rewardSlotUIArr[(int)rwdType].obj.transform.GetChild(ICON_ITEM);
        Image itemImg = iconItemTf.GetComponent<Image>();

        return itemImg.sprite;
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
                GM._.rwm.rewardSlotUIArr[i].DOTAnim.DORestart();
                GM._.rwm.rewardSlotUIArr[i].cntTxt.text = rwd.ToString();
            }
            i++;
        }
    }

    /// <summary>
    /// 수령할 보상 팝업 표시 및 수치 증가
    /// </summary>
    /// <param name="rwdDic">수령할 보상 Dic리스트</param>
    public void ShowReward(Dictionary<RWD, int> rwdDic, bool isAutoMine = false) {
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
                {   // (광석) 재화
                    case RWD.ORE1:
                    case RWD.ORE2:
                    case RWD.ORE3:
                    case RWD.ORE4:
                    case RWD.ORE5:
                    case RWD.ORE6:
                    case RWD.ORE7:
                    case RWD.ORE8:
                    case RWD.CRISTAL:
                        val = sttDB.SetRscArr((int)rwdType, val, isAutoMine);
                        break;
                    // (연금술) 재료
                    case RWD.MAT1:
                    case RWD.MAT2:
                    case RWD.MAT3:
                    case RWD.MAT4:
                    case RWD.MAT5:
                    case RWD.MAT6:
                    case RWD.MAT7:
                    case RWD.MAT8:
                        val = sttDB.SetMatArr((int)rwdType - (int)RWD.MAT1, val);
                        break;
                    // (버섯도감) 버섯
                    case RWD.MUSH1:
                    case RWD.MUSH2:
                    case RWD.MUSH3:
                    case RWD.MUSH4:
                    case RWD.MUSH5:
                    case RWD.MUSH6:
                    case RWD.MUSH7:
                    case RWD.MUSH8:
                        val = sttDB.SetMsrArr((int)rwdType - (int)RWD.MUSH1, val);
                        break;

                    // (소비) 아이템
                    case RWD.ORE_TICKET: // 광석 입장티켓
                        sttDB.OreTicket += val;
                        break;
                    case RWD.RED_TICKET: // 시련의광산 입장티켓
                        sttDB.RedTicket += val;
                        break;
                    case RWD.ORE_CHEST: // 광석상자
                        sttDB.OreChest += val;
                        break;
                    case RWD.TREASURE_CHEST: // 보물상자
                        sttDB.TreasureChest += val;
                        break;
                    case RWD.MUSH_BOX1: // 버섯상자1
                        sttDB.MushBox1 += val;
                        break;
                    case RWD.MUSH_BOX2: // 버섯상자2
                        sttDB.MushBox2 += val;
                        break;
                    case RWD.MUSH_BOX3: // 버섯상자3
                        sttDB.MushBox3 += val;
                        break;
                    case RWD.SKILLPOTION: // 스킬포션
                        sttDB.SkillPotion += val;
                        break;
                    case RWD.LIGHTSTONE: // 빛나는돌
                        sttDB.LightStone += val;
                        break;
                    case RWD.TIMEPOTION: // 시간의포션
                        sttDB.TimePotion += val;
                        break;

                    // ※여기 위에 추가
                    case RWD.FAME: // 명예포인트 + (초월) 명예 획득량
                        val += GM._.sttm.IncFame;
                        sttDB.Fame += val;
                        break;
                    
                }

                // 해당 보상슬롯UI 표시
                GM._.rwm.rewardSlotUIArr[(int)rwdType].obj.SetActive(true);
                GM._.rwm.rewardSlotUIArr[(int)rwdType].DOTAnim.DORestart();

                GM._.rwm.rewardSlotUIArr[(int)rwdType].cntTxt.text = val.ToString();
            }
        }

        // 업그레이드 가능알림🔴 최신화
        GM._.ugm.UpdateAlertRedDotUI();
        GM._.mrm.UpdateAlertRedDotUI();
    }
#endregion
}
