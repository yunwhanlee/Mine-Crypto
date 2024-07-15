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

    public IEnumerator CoPlayCoinAttractionPtcUIEF(int playCnt = 1) {
        int time = 0;
        while(time < playCnt) {
            time++;
            coinAttractionPtcImg.Play();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
