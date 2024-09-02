using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static Enum;
using TMPro;

[Serializable]
public class AutoMiningFormat
{
    //* Elements
    public GameObject lockedPanel;
    public TMP_Text titleTxt;
    public TMP_Text curStorageTxt;
    public TMP_Text productionValTxt;
    public Button UpgradeBtn;
    public TMP_Text UpgradePriceBtnTxt;
    public TMP_Text UpgradeInfoTxt;

    //* Value
    // 타입
    [field:SerializeField] private RSC type; public RSC Type { 
        get => type; private set => type = value;
    }
    // 레벨
    public int Lv {
        get => DM._.DB.autoMiningDB.saveDts[(int)type].Lv;
        set => DM._.DB.autoMiningDB.saveDts[(int)type].Lv = value;
    }
    // 잠금상태
    public bool IsUnlock { 
        get => DM._.DB.autoMiningDB.saveDts[(int)type].IsUnlock;
        set {
            DM._.DB.autoMiningDB.saveDts[(int)type].IsUnlock = value;

            // UI 업데이트
            lockedPanel.SetActive(!value);
            UpgradeBtn.interactable = value;
        }
    }   
    // 경과시간
    public int time { 
        get => DM._.DB.autoMiningDB.saveDts[(int)type].Time;
        set => DM._.DB.autoMiningDB.saveDts[(int)type].Time = value;
    }
    // 현재수량
    public int curStorage { 
        get => DM._.DB.autoMiningDB.saveDts[(int)type].CurStorage;
        set => DM._.DB.autoMiningDB.saveDts[(int)type].CurStorage = value;
    }

    public int productionVal; // 생산량
    public int maxStorage;    // 최대보관량
    public int upgradePrice;  // 업그레이드 가격

#region FUNC

#endregion
}