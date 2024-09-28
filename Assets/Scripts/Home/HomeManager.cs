using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public GameObject HomeWindow;

    #region EVENT
        public void OnClickGameStartBtn() {
            GM._.ssm.ShowPopUp();
        }
    #endregion
}
