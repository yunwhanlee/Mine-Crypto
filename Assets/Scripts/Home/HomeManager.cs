using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public GameObject HomeWindow;
    public TMP_Text[] topRscTxtArr;

    void Start() {
        for(int i = 0; i < topRscTxtArr.Length; i++)
        {
            topRscTxtArr[i].text = $"{DM._.DB.statusDB.RscArr[i]}";
        }
    }

    #region EVENT
        public void OnClickGameStartBtn() {
            GM._.ssm.ShowPopUp();
        }

        public void OnClickUpgradeBtn() {
            GM._.ugm.ShowPopUp();
        }

        public void OnClickOreBlessBtn() {
            //TODO
        }

        public void OnClickAutoMiningBtn() {
            //TODO
        }

        public void OnClickChallengeBtn() {
            //TODO
        }
    #endregion
}
