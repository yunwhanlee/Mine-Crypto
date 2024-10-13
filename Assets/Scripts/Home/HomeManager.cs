using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public GameObject HomeWindow;

    #region EVENT
        public void OnClickGameStartBtn() {
            SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
            GM._.ssm.ShowPopUp();
        }
    #endregion

    #region FUNC
        /// <summary>
        /// 홈화면 표시 (각종 잠금해제 확인)
        /// </summary>
        public void Active()
        {
            HomeWindow.SetActive(true);

            // 축복 개방
            GM._.obm.CheckUnlock();

            // (초월)시스템 개방
            if(GM._.acm.decoObjArr[2].activeSelf)
            {
                GM._.tsm.Unlock();
            }
        }
    #endregion
}
