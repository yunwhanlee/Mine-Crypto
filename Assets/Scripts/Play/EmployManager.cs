using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmployManager : MonoBehaviour
{
    //* Component
    public DOTweenAnimation DOTAnim;

    //* Elements
    public GameObject employPopUp;
    public GameObject fameInfoPanel;

    //* Value
    int workerCnt;

    void Start()
    {
        workerCnt = 0;
        DOTAnim.DORestart();
        ShowCharaInfoUI(gradeIdx: 0);
    }

#region EVENT
    /// <summary>
    /// 캐릭터 생성(고용)
    /// </summary>
    public void OnClickPlayBtn()
    {
        if(workerCnt < GM._.ugm.upgIncPopulation.Val)
        {
            //TODO Grade Random
            int randomIdx = Random.Range(0, 1); 

            var ins = Instantiate(GM._.mnm.goblinPrefs[randomIdx], GM._.mnm.workerGroupTf);
            ins.transform.position = GM._.mnm.homeTf.position;
            workerCnt++;
        }
    }

    /// <summary>
    /// 명예에 따른 캐릭터 소환 등급 확률표 표시
    /// </summary>
    public void OnClickFameInfoIconBtn() {
        fameInfoPanel.SetActive(true);
    }

    
#endregion

#region FUNC
    public void ShowPopUp() {
        employPopUp.SetActive(true);
        DOTAnim.DORestart();
    }

    public void ShowCharaInfoUI(int gradeIdx) {
        
    }
#endregion
}
