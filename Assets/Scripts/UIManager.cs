using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public ParticleImage coinAttractionPtcImg;

    public TMP_Text coinTxt;
    public TMP_Text workerCntTxt;

    void Start() {
        coinTxt.text = DM._.DB.statusDB.Coin.ToString();
        workerCntTxt.text = GM._.mm.workerGroupTf.childCount.ToString();
    }
}
