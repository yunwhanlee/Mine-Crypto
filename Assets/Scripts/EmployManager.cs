using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployManager : MonoBehaviour
{
    //* Component
    public DOTweenAnimation DOTAnim;

    //* Elements
    public GameObject windowObj;
    public TMP_Text populationIncInfoTxt;
    public TMP_Text workerPriceTxt;
    public TMP_Text populationPriceTxt;

    const int WORKER_PRICE_DEF = 1000;
    const int POPULATION_PRICE_DEF = 5000;

    //* Value
    [SerializeField] int workerCnt;
    public int WorkerCnt {
        get {
            workerCnt = GM._.mnm.workerGroupTf.childCount; // 값 최신화
            return workerCnt;
        } 
        set {
            workerCnt = GM._.mnm.workerGroupTf.childCount; 
            GM._.ui.SetTopWorkerInfoTxt(workerCnt, population);
        }
    }
    public int populationMax;
    [SerializeField] int population;
    public int Population {
        get => population;
        set {
            population = value; // 값 최신화
            GM._.ui.SetTopWorkerInfoTxt(workerCnt, population);
        }
    }

    private int workerPrice;
    private int populationPrice;

    void Start()
    {
        WorkerCnt = GM._.mnm.workerGroupTf.childCount;
        population = 3;
        GM._.ui.SetTopWorkerInfoTxt(workerCnt, population);
    }

#region FUNC
    private void UpdateUIAndData()
    {
        populationIncInfoTxt.text = $"{population} => {population + 1}";

        workerPrice = WORKER_PRICE_DEF + workerCnt * (workerCnt - 1) * 4000 / 2;
        workerPriceTxt.text = workerPrice.ToString();

        populationPrice = POPULATION_PRICE_DEF + population * (population - 1) * 5000 / 2;
        populationPriceTxt.text = populationPrice.ToString();
    }
#endregion

#region EVENT FUNC
    /// <summary>
    /// 팝업 열기
    /// </summary>
    public void OnClickPlusBtn()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        UpdateUIAndData();
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    public void OnClickDimScreen()
    {
        windowObj.SetActive(false);
    }

    /// <summary>
    /// 고블린 소환
    /// </summary>
    public void OnClickAddWorkerBtn() 
    {
        if(workerCnt < population)
        {
            if(DM._.DB.statusDB.Coin >= workerPrice)
            {
                GM._.ui.ShowNoticeMsgPopUp("고블린 소환 완료!");
                DM._.DB.statusDB.Coin -= workerPrice;
                var ins = Instantiate(GM._.mnm.goblinPrefs[0], GM._.mnm.workerGroupTf);
                ins.transform.position = GM._.mnm.homeTf.position;
                WorkerCnt++;

                UpdateUIAndData();
            }
            else
            {
                GM._.ui.ShowWarningMsgPopUp("돈이 부족합니다.");
            }
        }
        else
        {
            GM._.ui.ShowWarningMsgPopUp("인구수가 꽉찼습니다. 최대인구를 늘리세요.");
        }
    }

    /// <summary>
    /// 최대인구 증가
    /// </summary>
    public void OnClickAddPopulationBtn() {
        if(population < populationMax)
        {
            if(DM._.DB.statusDB.Coin >= populationPrice)
            {
                GM._.ui.ShowNoticeMsgPopUp("인구수 증가 완료!");
                DM._.DB.statusDB.Coin -= populationPrice;
                Population++;

                UpdateUIAndData();
            }
            else
            {
                GM._.ui.ShowWarningMsgPopUp("돈이 부족합니다.");
            }
        }
        else
        {
            GM._.ui.ShowWarningMsgPopUp("인구수가 MAX입니다.");
        }


    }
#endregion
}
