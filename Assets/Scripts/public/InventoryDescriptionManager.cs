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

    public InvItem_Info[] INV_ITEM_INFO;

    void Start() {
        sttDB = DM._.DB.statusDB;
    }

#region EVENT
    /// <summary>
    /// 인벤토리 아이템 상세정보
    /// </summary>
    /// <param name="invIdx">인벤토리 아이템 인덱스</param>
    public void OnClickInvItemSlot(INV invIdx) {
        SoundManager._.PlaySfx(SoundManager.SFX.InvSlotClickSFX);
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
                confirmBtnTxt.text = LM._.Localize(LM.OpenAll);

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("보물상자 오픈!");
                    SoundManager._.PlaySfx(SoundManager.SFX.OpenTreasureChestSFX);

                    // 보상으로 나오는 모든 아이템 Dic
                    Dictionary<RWD, int> rwdDic = new Dictionary<RWD, int>() {
                        {RWD.ORE_TICKET, 0},
                        {RWD.RED_TICKET, 0},
                        {RWD.CRISTAL, 0},
                        {RWD.SKILLPOTION, 0},
                        {RWD.TIMEPOTION, 0},
                    };

                    // 모든 보물상자 열기
                    for(int i = 0; i < sttDB.TreasureChest; i++)
                    {
                        // 랜덤티켓
                        int random = Random.Range(0, 100);
                        RWD reward = (random < 50)? RWD.ORE_TICKET : RWD.RED_TICKET;

                        // 보상 랜덤티켓 추가
                        switch(reward)
                        {
                            case RWD.ORE_TICKET:
                                rwdDic[RWD.ORE_TICKET]++;
                                break;
                            case RWD.RED_TICKET:
                                rwdDic[RWD.RED_TICKET]++;
                                break;
                        }

                        // 보상 크리스탈 추가
                        rwdDic[RWD.CRISTAL] += Random.Range(1, 6);
                        // 5%확률 스킬포인트물약 획득
                        rwdDic[RWD.SKILLPOTION] += Random.Range(0, 100) < 5? 1: 0;
                        // 5%확률 시간의물약 획득
                        rwdDic[RWD.TIMEPOTION] += Random.Range(0, 100) < 5? 1: 0;
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
                            {RWD.CRISTAL, rwdDic[RWD.CRISTAL]},
                            {RWD.SKILLPOTION, rwdDic[RWD.SKILLPOTION]},
                            {RWD.TIMEPOTION, rwdDic[RWD.TIMEPOTION]}
                        }
                    );

                    // 인벤토리 슬롯UI 최신화
                    GM._.ivm.UpdateSlotUI();

                    // 상세정보 수량UI 최신화
                    UpdateConsumedItemCntUI(sttDB.TreasureChest);
                };
                break;
            case INV.ORE_CHEST:
                confirmBtnTxt.text = LM._.Localize(LM.OpenAll);

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("광석상자 오픈!");
                    SoundManager._.PlaySfx(SoundManager.SFX.OpenOreChestSFX);
                    
                    // 보상으로 나오는 모든 아이템 Dic
                    Dictionary<RWD, int> rwdDic = new Dictionary<RWD, int>() {
                        {RWD.ORE1, 0}, {RWD.ORE2, 0}, {RWD.ORE3, 0}, {RWD.ORE4, 0},
                        {RWD.ORE5, 0}, {RWD.ORE6, 0}, {RWD.ORE7, 0}, {RWD.ORE8, 0}
                    };

                    // 광석 3종류 랜덤
                    RWD[] rwdArr = new RWD[3];
                    int[] cntArr = new int[3];

                    // 대량일 경우 루프 경량화를 위한 나누기 변수
                    int divide = (sttDB.OreChest > 100)? 10 // 100개 이상일 경우, 나누기 10
                        : (sttDB.OreChest > 1000)? 100      // 1000개 이상일 경우, 나누기 100
                        : 1;                                // 그 이외는 1개씩 처리
                    
                    // 나머지 값
                    int remainCnt = sttDB.OreChest % divide;

                    Debug.Log($"remainCnt= {remainCnt}");

                    // 모든 보물상자 열기
                    for(int i = 0; i < sttDB.OreChest / divide; i++)
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
                            int val = 100 + (bestFloor * 100 * divide); // 계산식

                            rwdDic[(RWD)randIdx] += val;

                            // 리스트 제거 (중복방지)
                            rwdList.RemoveAt(randIdx);
                        }
                    }

                    // 나눈나머지 광석획득
                    for(int i = 0; i < remainCnt; i++)
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
            case INV.MUSH_BOX1:
                confirmBtnTxt.text = LM._.Localize(LM.OpenAll);

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("버섯상자1 오픈!");
                    SoundManager._.PlaySfx(SoundManager.SFX.OpenMushBoxSFX);

                    // 보상으로 나오는 모든 아이템 Dic
                    Dictionary<RWD, int> rwdDic = new Dictionary<RWD, int>() {
                        {RWD.MUSH1, 0}, {RWD.MUSH2, 0}, {RWD.MUSH4, 0}, {RWD.MUSH7, 0}, {RWD.MUSH8, 0}
                    };

                    for(int i = 0; i < sttDB.MushBox1; i++)
                    {
                        // 랜덤선택
                        int randPer = Random.Range(0, 1000);
                        RWD rewardIdx = (randPer < 320)? RWD.MUSH1
                            : (randPer < 640)? RWD.MUSH2
                            : (randPer < 960)? RWD.MUSH4
                            : (randPer < 995)? RWD.MUSH7
                            : RWD.MUSH8;

                        // 수량 추가
                        rwdDic[rewardIdx]++;
                    }

                    // 인벤토리 소비아이템 수량 감소
                    sttDB.MushBox1 = 0;

                    //* 보상 획득
                    GM._.rwm.ShowReward (
                        new Dictionary<RWD, int> {
                            {RWD.MUSH1, rwdDic[RWD.MUSH1]},
                            {RWD.MUSH2, rwdDic[RWD.MUSH2]},
                            {RWD.MUSH4, rwdDic[RWD.MUSH4]},
                            {RWD.MUSH7, rwdDic[RWD.MUSH7]},
                            {RWD.MUSH8, rwdDic[RWD.MUSH8]},
                        }
                    );

                    // 인벤토리 슬롯UI 최신화
                    GM._.ivm.UpdateSlotUI();

                    // 상세정보 수량UI 최신화
                    UpdateConsumedItemCntUI(sttDB.MushBox1);
                };
                break;
            case INV.MUSH_BOX2:
                confirmBtnTxt.text = LM._.Localize(LM.OpenAll);

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("버섯상자2 오픈!");
                    SoundManager._.PlaySfx(SoundManager.SFX.OpenMushBoxSFX);

                    // 보상으로 나오는 모든 아이템 Dic
                    Dictionary<RWD, int> rwdDic = new Dictionary<RWD, int>() {
                        {RWD.MUSH3, 0}, {RWD.MUSH5, 0}, {RWD.MUSH6, 0}, {RWD.MUSH7, 0}, {RWD.MUSH8, 0}
                    };

                    for(int i = 0; i < sttDB.MushBox2; i++)
                    {
                        // 랜덤선택
                        int randPer = Random.Range(0, 1000);
                        RWD rewardIdx = (randPer < 320)? RWD.MUSH3
                        : (randPer < 640)? RWD.MUSH5
                        : (randPer < 960)? RWD.MUSH6
                        : (randPer < 995)? RWD.MUSH7
                        : RWD.MUSH8;

                        // 수량 추가
                        rwdDic[rewardIdx]++;
                    }

                    // 인벤토리 소비아이템 수량 감소
                    sttDB.MushBox2 = 0;

                    //* 보상 획득
                    GM._.rwm.ShowReward (
                        new Dictionary<RWD, int> {
                            {RWD.MUSH3, rwdDic[RWD.MUSH3]},
                            {RWD.MUSH5, rwdDic[RWD.MUSH5]},
                            {RWD.MUSH6, rwdDic[RWD.MUSH6]},
                            {RWD.MUSH7, rwdDic[RWD.MUSH7]},
                            {RWD.MUSH8, rwdDic[RWD.MUSH8]},
                        }
                    );

                    // 인벤토리 슬롯UI 최신화
                    GM._.ivm.UpdateSlotUI();

                    // 상세정보 수량UI 최신화
                    UpdateConsumedItemCntUI(sttDB.MushBox2);
                };
                break;
            case INV.MUSH_BOX3:
                confirmBtnTxt.text = LM._.Localize(LM.OpenAll);

                //* 이벤트 구독
                OnConfirmBtnClicked = () => {
                    Debug.Log("버섯상자3 오픈!");
                    SoundManager._.PlaySfx(SoundManager.SFX.OpenMushBoxSFX);

                    // 보상으로 나오는 모든 아이템 Dic
                    Dictionary<RWD, int> rwdDic = new Dictionary<RWD, int>() {{RWD.MUSH8, 0}};

                    // 수량 추가
                    rwdDic[RWD.MUSH8] = sttDB.MushBox3;

                    // 인벤토리 소비아이템 수량 감소
                    sttDB.MushBox3 = 0;

                    //* 보상 획득
                    GM._.rwm.ShowReward (
                        new Dictionary<RWD, int> {{RWD.MUSH8, rwdDic[RWD.MUSH8]},}
                    );

                    // 인벤토리 슬롯UI 최신화
                    GM._.ivm.UpdateSlotUI();

                    // 상세정보 수량UI 최신화
                    UpdateConsumedItemCntUI(sttDB.MushBox3);
                };
                break;
            // 확인용 타입
            default:
                confirmBtnTxt.text = LM._.Localize(LM.Confirm);

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
    //* 위의 인벤토리 아이템 이름 및 정보
    /// <summary>
    //! (에디터) 인벤토리팝업 아이템 순서와 서로같게 하기
    public InvItem_Info[] SetInvItemDescriptionInfo()
    {
        //! 배열 숫자 증가시키기!
        return new InvItem_Info[9 + 8 + 8 + 11] {
        // (광석)재화
        new InvItem_Info(LM._.Localize(LM.Ore1), LM._.Localize(LM.Detail_Ore)),
        new InvItem_Info(LM._.Localize(LM.Ore2), LM._.Localize(LM.Detail_Ore)),
        new InvItem_Info(LM._.Localize(LM.Ore3), LM._.Localize(LM.Detail_Ore)),
        new InvItem_Info(LM._.Localize(LM.Ore4), LM._.Localize(LM.Detail_Ore)),
        new InvItem_Info(LM._.Localize(LM.Ore5), LM._.Localize(LM.Detail_Ore)),
        new InvItem_Info(LM._.Localize(LM.Ore6), LM._.Localize(LM.Detail_Ore)),
        new InvItem_Info(LM._.Localize(LM.Ore7), LM._.Localize(LM.Detail_Ore)),
        new InvItem_Info(LM._.Localize(LM.Ore8), LM._.Localize(LM.Detail_Ore)),
        new InvItem_Info(LM._.Localize(LM.Cristal), LM._.Localize(LM.Detail_Cristal)),
        // (연금술) 재료
        new InvItem_Info(LM._.Localize(LM.Mat1), LM._.Localize(LM.Detail_Mat)),
        new InvItem_Info(LM._.Localize(LM.Mat2), LM._.Localize(LM.Detail_Mat)),
        new InvItem_Info(LM._.Localize(LM.Mat3), LM._.Localize(LM.Detail_Mat)),
        new InvItem_Info(LM._.Localize(LM.Mat4), LM._.Localize(LM.Detail_Mat)),
        new InvItem_Info(LM._.Localize(LM.Mat5), LM._.Localize(LM.Detail_Mat)),
        new InvItem_Info(LM._.Localize(LM.Mat6), LM._.Localize(LM.Detail_Mat)),
        new InvItem_Info(LM._.Localize(LM.Mat7), LM._.Localize(LM.Detail_Mat)),
        new InvItem_Info(LM._.Localize(LM.Mat8), LM._.Localize(LM.Detail_Mat)),
        // (버섯도감) 버섯
        new InvItem_Info(LM._.Localize(LM.Mush1), LM._.Localize(LM.Detail_Mush)),
        new InvItem_Info(LM._.Localize(LM.Mush2), LM._.Localize(LM.Detail_Mush)),
        new InvItem_Info(LM._.Localize(LM.Mush3), LM._.Localize(LM.Detail_Mush)),
        new InvItem_Info(LM._.Localize(LM.Mush4), LM._.Localize(LM.Detail_Mush)),
        new InvItem_Info(LM._.Localize(LM.Mush5), LM._.Localize(LM.Detail_Mush)),
        new InvItem_Info(LM._.Localize(LM.Mush6), LM._.Localize(LM.Detail_Mush)),
        new InvItem_Info(LM._.Localize(LM.Mush7), LM._.Localize(LM.Detail_Mush)),
        new InvItem_Info(LM._.Localize(LM.Mush8), LM._.Localize(LM.Detail_Mush)),
        // (소비) 아이템
        new InvItem_Info(LM._.Localize(LM.OreTicket), LM._.Localize(LM.Detail_OreTicket)),
        new InvItem_Info(LM._.Localize(LM.RedTicket), LM._.Localize(LM.Detail_RedTicket)),
        new InvItem_Info(LM._.Localize(LM.MushBox1), LM._.Localize(LM.Detail_MushBox1)),
        new InvItem_Info(LM._.Localize(LM.MushBox2), LM._.Localize(LM.Detail_MushBox2)),
        new InvItem_Info(LM._.Localize(LM.MushBox3), LM._.Localize(LM.Detail_MushBox3)),
        new InvItem_Info(LM._.Localize(LM.OreChest), LM._.Localize(LM.Detail_OreChest)),
        new InvItem_Info(LM._.Localize(LM.TreasureChest), LM._.Localize(LM.Detail_TreasureChest)),
        new InvItem_Info(LM._.Localize(LM.SkillPotion), LM._.Localize(LM.Detail_SkillPotion)),
        new InvItem_Info(LM._.Localize(LM.LightStone), LM._.Localize(LM.Detail_LightStone)),
        new InvItem_Info(LM._.Localize(LM.TimePotion), LM._.Localize(LM.Detail_TimePotion)),
        new InvItem_Info(LM._.Localize(LM.GoldCoin), LM._.Localize(LM.Detail_GoldCoin)),
        //※ 여기 위에 추가
        };
    }

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
