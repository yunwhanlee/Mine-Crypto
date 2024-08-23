using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステータス
/// </summary>
[Serializable]
public class StatusDB
{
    [Header("일반재화 8종 및 특별재화(별사탕)")]
    [field:SerializeField] int[] rscArr; public int[] RscArr {
        get => rscArr;
    }

    [Header("광산 입장티켓")]
    [field:SerializeField] int oreTicket; public int OreTicket {
        get => oreTicket;
        set {
            oreTicket = value;
            if(oreTicket < 0) oreTicket = 0;

            // (홈) 광산선택 팝업 텍스트UI 최신화
            GM._.ssm.stageTicketCntTxt.text = $"{oreTicket} / 10";
        }
    }

    [Header("시련의광산 입장티켓")]
    [field:SerializeField] int redTicket; public int RedTicket {
        get {return redTicket;}
        set {
            redTicket = value;
            if(redTicket < 0) redTicket = 0;
            //TODO 텍스트UI 업데이트
        }
    }

    [Header("보물상자")]
    [field:SerializeField] int treasureChest; public int TreasureChest {
        get {return treasureChest;}
        set {
            treasureChest = value;
            if(treasureChest < 0) treasureChest = 0;
            //TODO 텍스트UI 업데이트
        }
    }

    [Header("광석상자")]
    [field:SerializeField] int oreChest; public int OreChest {
        get {return oreChest;}
        set {
            oreChest = value;
            if(oreChest < 0) oreChest = 0;
            //TODO 텍스트UI 업데이트
        }
    }

    [Header("명예 포인트")]
    [field:SerializeField] int fame; public int Fame {
        get {return fame;}
        set {
            fame = value;
            if(fame < 0) fame = 0;
            //TODO 텍스트UI 업데이트
        }
    }

#region FUNC
    /// <summary>
    /// 재화량 추가 및 UI업데이트
    /// </summary>
    /// <param name="idx">재화 타입(인덱스)</param>
    /// <param name="val">증가량</param>
    public void SetRscArr(int idx, int val) { //? 배열 setter는 요소가 바뀌어도 호출이 안되므로, 메서드 자체 제작
        rscArr[idx] += val;
        if(rscArr[idx] < 0)
                rscArr[idx] = 0;
        if(GM._.gameState == GameState.HOME)
            GM._.hm.topRscTxtArr[idx].text = $"{rscArr[idx]}";
        else
            GM._.stm.curRscCntTxt.text = $"{rscArr[idx]}";
    }

    /// <summary>
    /// (홈) 모든재화 텍스트UI 업데이트
    /// </summary>
    public void UpdateAllRscUIAtHome() {
        for(int i = 0; i < rscArr.Length; i++){
            GM._.hm.topRscTxtArr[i].text = $"{rscArr[i]}";
        }
    }
#endregion
}
