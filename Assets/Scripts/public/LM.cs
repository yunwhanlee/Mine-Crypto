using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LM : MonoBehaviour
{
    public static LM _;

    //* TABLE KEY
    public const string
        BestRecord = "UI_BestRecord",
        Cnt = "UI_Cnt",
        All = "UI_All",
        Summon = "UI_Summon",
        Fame = "UI_Fame",
        NextFameLvGradeTable = "UI_NextFameLvGradeTable",
        Lv = "UI_Lv",
        Floor = "UI_Floor",
        ChallengeMine = "UI_ChallengeMine",
        MiningPerMin = "UI_MiningPerMin",
        Complete = "UI_Complete",
        InProgress = "UI_InProgress",
        Ammount = "UI_Ammount",
        Owned = "UI_Owned",
        Upgrade = "UI_Upgrade",
        OpenAll = "UI_OpenAll",
        Confirm = "UI_Confirm",

        Ore1 = "UI_Ore1",
        Ore2 = "UI_Ore2",
        Ore3 = "UI_Ore3",
        Ore4 = "UI_Ore4",
        Ore5 = "UI_Ore5",
        Ore6 = "UI_Ore6",
        Ore7 = "UI_Ore7",
        Ore8 = "UI_Ore8",
        Cristal = "UI_Cristal",

        Detail_Ore = "Detail_Ore",
        Detail_Cristal = "Detail_Cristal",
        Detail_Mat = "Detail_Mat",
        Detail_Mush = "Detail_Mush",
        Detail_OreTicket = "Detail_OreTicket",
        Detail_RedTicket = "Detail_RedTicket",
        Detail_OreChest = "Detail_OreChest",
        Detail_TreasureChest = "Detail_TreasureChest",
        Detail_MushBox1 = "Detail_MushBox1",
        Detail_MushBox2 = "Detail_MushBox2",
        Detail_MushBox3 = "Detail_MushBox3",

        Mat1 = "UI_Mat1",
        Mat2 = "UI_Mat2",
        Mat3 = "UI_Mat3",
        Mat4 = "UI_Mat4",
        Mat5 = "UI_Mat5",
        Mat6 = "UI_Mat6",
        Mat7 = "UI_Mat7",
        Mat8 = "UI_Mat8",

        Mush1 = "UI_Mush1",
        Mush2 = "UI_Mush2",
        Mush3 = "UI_Mush3",
        Mush4 = "UI_Mush4",
        Mush5 = "UI_Mush5",
        Mush6 = "UI_Mush6",
        Mush7 = "UI_Mush7",
        Mush8 = "UI_Mush8",

        OreTicket = "UI_OreTicket",
        RedTicket = "UI_RedTicket",
        OreChest = "UI_OreChest",
        TreasureChest = "UI_TreasureChest",
        MushBox1 = "UI_MushBox1",
        MushBox2 = "UI_MushBox2",
        MushBox3 = "UI_MushBox3",

        Attack = "UI_Attack",
        AttackSpeed = "UI_AttackSpeed",
        MoveSpeed = "UI_MoveSpeed",
        BagStorage = "UI_BagStorage",
        MiningTime = "UI_MiningTime",
        NextStageSkip = "UI_NextStageSkip",
        ExtraCristal = "UI_ExtraCristal",
        IncPopulation = "UI_IncPopulation",
        StartingFloor = "UI_StartingFloor",
        IncChestSpawnPer = "UI_IncChestSpawnPer",
        ExtraOre1 = "UI_ExtraOre1",
        ExtraOre2 = "UI_ExtraOre2",
        ExtraOre3 = "UI_ExtraOre3",
        ExtraOre4 = "UI_ExtraOre4",
        ExtraOre5 = "UI_ExtraOre5",
        ExtraOre6 = "UI_ExtraOre6",
        ExtraOre7 = "UI_ExtraOre7",
        ExtraOre8 = "UI_ExtraOre8",
        AutoOrePer = "UI_AutoOrePer",
        AutoCristalPer = "UI_AutoCristalPer",
        DecAlchemyMaterialPer = "UI_DecAlchemyMaterialPer",
        IncTreasureChest = "UI_IncTreasureChest",
        AutoOreBagStorage = "UI_AutoOreBagStorage",
        AutoCristalBagStorage = "UI_AutoCristalBagStorage",
        IncFame = "UI_IncFame",

        PickCharaMsg = "UI_PickCharaMsg",

        NONE = "";

    bool isChaning;

    void Awake() {
        _ = this;
    }

#region FUNC
    /// <summary>
    /// 현재선택한 언어에 맞게 로컬라이징
    /// </summary>
    /// <param name="tbKey">로컬라이징 테이블 키</param>
    /// <returns></returns>
    public string Localize(string tbKey)
    {
        const string TABLE_NAME = "MyTable";
        Locale curLocale = LocalizationSettings.SelectedLocale;
        return LocalizationSettings.StringDatabase.GetLocalizedString(TABLE_NAME, tbKey, curLocale);
    }

    /// <summary>
    /// 언어 변경
    /// </summary>
    /// <param name="languageIdx">언어종류 인덱스</param>
    public void ChangeLocale(int languageIdx)
    {
        if(isChaning)
            return;

        StartCoroutine(CoChange(languageIdx));
    }

    IEnumerator CoChange(int languageIdx)
    {
        isChaning = true;

        // 초기화가 완료되기까지 대기
        yield return LocalizationSettings.InitializationOperation;

        // 언어 변경
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIdx];

        // 인벤토리 상세페이지 아이템 정보 언어변경
        GM._.idm.INV_ITEM_INFO = GM._.idm.SetInvItemDescriptionInfo();
        GM._.ivm.InitDataAndUI();

        isChaning = false;
    }
#endregion
}
