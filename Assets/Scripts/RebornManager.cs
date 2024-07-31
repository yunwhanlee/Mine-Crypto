using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RebornManager : MonoBehaviour
{
    //* Component
    public DOTweenAnimation DOTAnim;

    //* Elements
    public GameObject windowObj;

    void Start()
    {
        
    }

#region EVENT FUNC
    /// <summary>
    /// 팝업 열기
    /// </summary>
    public void OnClickPlusBtn()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        //TODO UpdateUIAndData();
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    public void OnClickDimScreen()
    {
        windowObj.SetActive(false);
    }
#endregion
}
