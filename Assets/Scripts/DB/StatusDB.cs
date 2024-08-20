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

    // 배열 setter는 요소가 바뀌어도 호출이 안되므로, 메서드 자체 제작
    public void SetRscArr(int idx, int val) {
        rscArr[idx] += val;
        if(rscArr[idx] < 0)
                rscArr[idx] = 0;
        if(GM._.gameState == GameState.HOME)
            GM._.hm.topRscTxtArr[idx].text = $"{rscArr[idx]}";
        else
            GM._.stm.curRscCntTxt.text = $"{rscArr[idx]}";
    }

    //TODO 광석티켓 -> 일반광산
    [field:SerializeField] int stageTicket; public int StageTicket {
        get => stageTicket;
        set {
            stageTicket = value;
            if(stageTicket < 0)
                stageTicket = 0;
            GM._.ssm.stageTicketCntTxt.text = $"{stageTicket} / 10";
        }
    }

    //TODO 붉은티켓 -> 시련의광산
    // [field:SerializeField] int redTicket; public int RedTicket {
    //     get {return redTicket;}
    //     set {
    //         redTicket = value;
    //         if(redTicket < 0) redTicket = 0;
    //     }
    // }

    [field:SerializeField] int coin; public int Coin {
        get {return coin;}
        set {
            coin = value;
            if(coin < 0) coin = 0;
            GM._.ui.coinTxt.text = coin.ToString();
        }
    }

    public StatusDB() {
        coin = 0;
    }

}
