using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public DOTweenAnimation DOTAnim;

    // Element
    public GameObject windowObj;


    // Value

#region EVENT
    public void OnClickResetBtn()
    {
        GM._.ui.ShowConfirmPopUp("정말로 데이터를 리셋하겠습니까?");
        GM._.ui.OnClickConfirmBtnAction = () => {
            DM._.Reset();
            SceneManager.LoadScene("Game");
        };
    }
#endregion

#region FUNC
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
    }
#endregion
}
