using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using static Enum;

/// <summary>
/// (인게임) 드랍아이템 메시지바 매니저
/// </summary>
public class DropItemManager : MonoBehaviour
{
    public Transform inActiveGroup;
    public Transform activeGroup;

    // public GameObject[] dropMsgBarArr; // 아이템드랍 메세지바 배열

    void Start() {
        //! TEST
        // InvokeRepeating("DropBonusItem", 1, 0.4f);
    }

#region FUNC
    /// <summary>
    /// 광석채굴시 보너스아이템 드랍
    /// </summary>
    public void DropBonusItem()
    {
        StatusDB sttDB = DM._.DB.statusDB;
        List<RWD> rwdList = new List<RWD>(); // 드랍보상 리스트

        // 드랍아이템 가챠
        ItemGacha(rwdList, RWD.CRISTAL, 1);
        ItemGacha(rwdList, RWD.MUSH1, 1);
        ItemGacha(rwdList, RWD.MUSH2, 10000);
        ItemGacha(rwdList, RWD.MUSH3, 1);
        ItemGacha(rwdList, RWD.MUSH4, 10000);
        ItemGacha(rwdList, RWD.MUSH5, 10000);
        ItemGacha(rwdList, RWD.MUSH6, 10000);
        ItemGacha(rwdList, RWD.MUSH7, 100000);
        ItemGacha(rwdList, RWD.MUSH8, 10000000);
        ItemGacha(rwdList, RWD.MAT1, 1000);
        ItemGacha(rwdList, RWD.MAT2, 1000);
        ItemGacha(rwdList, RWD.MAT3, 1);
        ItemGacha(rwdList, RWD.MAT4, 1000);
        ItemGacha(rwdList, RWD.MAT5, 10000);
        ItemGacha(rwdList, RWD.MAT6, 100000);
        ItemGacha(rwdList, RWD.MAT7, 1000000);
        ItemGacha(rwdList, RWD.MAT8, 10000000);

        if(rwdList.Count == 0)
            return;

        SoundManager._.PlaySfx(SoundManager.SFX.DropItemSFX);

        int randIdx = Random.Range(0, rwdList.Count);

        // 이미 메세지바가 다 켜져있다면
        if(inActiveGroup.childCount == 0)
        {
            // 맨위의 메세지바를 비표시그룹 영역으로 돌려놓고
            activeGroup.GetChild(0).gameObject.SetActive(false);
            activeGroup.GetChild(0).SetParent(inActiveGroup);

            // 밑에서부터 메시지바 표시 추가
            Transform msgBarTf = inActiveGroup.GetChild(0);
            StartCoroutine(CoShowDropMsgBar(rwdList[randIdx], msgBarTf));
        }
        else
        {
            // 밑에서부터 메시지바 표시 추가
            Transform msgBarTf = inActiveGroup.GetChild(0);
            StartCoroutine(CoShowDropMsgBar(rwdList[randIdx], msgBarTf));
        }

        //* 아이템 획득
        switch(rwdList[randIdx])
        {
            case RWD.CRISTAL:
                sttDB.SetRscArr((int)rwdList[randIdx], 1);
                break;
            case RWD.MAT1:
            case RWD.MAT2:
            case RWD.MAT3:
            case RWD.MAT4:
            case RWD.MAT5:
            case RWD.MAT6:
            case RWD.MAT7:
            case RWD.MAT8:
                sttDB.SetMatArr((int)rwdList[randIdx] - (int)RWD.MAT1, 1);
                break;
            case RWD.MUSH1:
            case RWD.MUSH2:
            case RWD.MUSH3:
            case RWD.MUSH4:
            case RWD.MUSH5:
            case RWD.MUSH6:
            case RWD.MUSH7:
            case RWD.MUSH8:
                sttDB.SetMsrArr((int)rwdList[randIdx] - (int)RWD.MUSH1, 1);
                break;
        }
    }

    /// <summary>
    /// 메세지바 표시
    /// </summary>
    IEnumerator CoShowDropMsgBar(RWD rwd, Transform msgBarTf)
    {
        msgBarTf.SetParent(activeGroup); // 표시영역으로 메세지바 이동
        msgBarTf.gameObject.SetActive(true);
        msgBarTf.GetComponentInChildren<TMP_Text>().text = $"<sprite name={rwd}> {LM._.Localize($"{rwd}")} {LM._.Localize(LM.DropItemMsg)}";
        yield return Util.TIME2;
        msgBarTf.gameObject.SetActive(false);
        msgBarTf.SetParent(inActiveGroup);
    }

    /// <summary>
    /// 랜덤아이템 드랍확률 결과
    /// </summary>
    /// <param name="rwd">드랍아이템 보상</param>
    /// <param name="maxRandom">최대확률 : 정수로 0.01% => 10000 </param>
    private void ItemGacha(List<RWD> rwdList, RWD rwd, int maxRandom)
    {
        int randPer = Random.Range(0, maxRandom); // 랜덤확률
        int winPer = 0 + GM._.stgm.Floor; // 당첨확률

        Debug.Log($"ItemGacha(rwd={rwd}, max={maxRandom}):: randPer= {randPer}, winPer= {winPer}, 결과 : {winPer >= randPer}");

        // 당첨이라면 리스트추가
        if(winPer >= randPer)
            rwdList.Add(rwd);
    }
#endregion
}
