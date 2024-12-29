using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

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
        ExtraFame = "UI_ExtraFame",
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
        Yes = "UI_Yes",
        No = "UI_No",
        UpgradeMaxStorage = "UI_UpgradeMaxStorage",
        Caution = "UI_Caution",

        Ore1 = "ORE1",
        Ore2 = "ORE2",
        Ore3 = "ORE3",
        Ore4 = "ORE4",
        Ore5 = "ORE5",
        Ore6 = "ORE6",
        Ore7 = "ORE7",
        Ore8 = "ORE8",
        Cristal = "CRISTAL",

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
        Detail_SkillPotion = "Detail_SkillPotion",
        Detail_LightStone = "Detail_LightStone",
        Detail_TimePotion = "Detail_TimePotion",
        Detail_GoldCoin = "Detail_GoldCoin",

        Mat1 = "MAT1",
        Mat2 = "MAT2",
        Mat3 = "MAT3",
        Mat4 = "MAT4",
        Mat5 = "MAT5",
        Mat6 = "MAT6",
        Mat7 = "MAT7",
        Mat8 = "MAT8",

        Mush1 = "MUSH1",
        Mush2 = "MUSH2",
        Mush3 = "MUSH3",
        Mush4 = "MUSH4",
        Mush5 = "MUSH5",
        Mush6 = "MUSH6",
        Mush7 = "MUSH7",
        Mush8 = "MUSH8",

        OreTicket = "UI_OreTicket",
        RedTicket = "UI_RedTicket",
        OreChest = "UI_OreChest",
        TreasureChest = "UI_TreasureChest",
        MushBox1 = "UI_MushBox1",
        MushBox2 = "UI_MushBox2",
        MushBox3 = "UI_MushBox3",
        SkillPotion = "UI_SkillPotion",
        LightStone = "UI_LightStone",
        TimePotion = "UI_TimePotion",
        GoldCoin = "GOLDCOIN",

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
        ConsumePerSec = "UI_ConsumePerSec",
        Reward = "UI_Reward",
        UsedGoldCoinPoint = "UI_UsedGoldCoinPoint",

        AttackSkillTitle = "UI_AttackSkillTitle",
        BuffSkillTitle = "UI_BuffSkillTitle",
        SkipSkillTitle = "UI_SkipSkillTitle",

        AttackSkillLv1Msg = "UI_AttackSkillLv1Msg",
        AttackSkillLv2Msg = "UI_AttackSkillLv2Msg",
        AttackSkillLv3Msg = "UI_AttackSkillLv3Msg",
        AttackSkillLv4Msg = "UI_AttackSkillLv4Msg",
        AttackSkillLv5Msg = "UI_AttackSkillLv5Msg",

        BuffSkillLv1Msg = "UI_BuffSkillLv1Msg",
        BuffSkillLv2Msg = "UI_BuffSkillLv2Msg",
        BuffSkillLv3Msg = "UI_BuffSkillLv3Msg",
        BuffSkillLv4Msg = "UI_BuffSkillLv4Msg",
        BuffSkillLv5Msg = "UI_BuffSkillLv5Msg",

        SkipSkillLv1Msg = "UI_SkipSkillLv1Msg",
        SkipSkillLv2Msg = "UI_SkipSkillLv2Msg",
        SkipSkillLv3Msg = "UI_SkipSkillLv3Msg",
        SkipSkillLv4Msg = "UI_SkipSkillLv4Msg",
        SkipSkillLv5Msg = "UI_SkipSkillLv5Msg",

        PickCharaMsg = "UI_PickCharaMsg",
        NotEnoughItemMsg = "UI_NotEnoughItemMsg",
        NoOreYetMsg = "UI_NoOreYetMsg",
        MissionStillMsg = "UI_MissionStillMsg",
        NotEnoughExpMsg = "UI_NotEnoughExpMsg",
        UnlockTranscendMsg = "UI_UnlockTranscendMsg",
        MaxLvMsg = "UI_MaxLvMsg",
        UnlockMushroomDicMsg = "UI_UnlockMushroomDicMsg",
        AskResetDataMsg = "UI_AskResetDataMsg",

        UnlockOreMineMsg = "UI_UnlockOreMineMsg",
        NextStageSkipMsg = "UI_NextStageSkipMsg",
        ReceiptCompletedMsg = "UI_ReceiptCompletedMsg",
        UpgradeCompleteMsg = "UI_UpgradeCompleteMsg",
        BlessResetCompleteMsg = "UI_BlessResetCompleteMsg",
        ProficiencyLvUpMsg = "UI_ProficiencyLvUpMsg",

        Congraturation = "UI_CongraturationMsg",
        UnlockBlessTitleMsg = "UI_UnlockBlessTitleMsg",
        UnlockBlessContentMsg = "UI_UnlockBlessContentMsg",

        UnlockTranscendTitleMsg = "UI_UnlockTranscendTitleMsg",
        UnlockTranscendContentMsg = "UI_UnlockTranscendContentMsg",

        UnlockMushroomDicTitleMsg = "UI_UnlockMushroomDicTitleMsg",
        UnlockMushroomDicContentMsg = "UI_UnlockMushroomDicContentMsg",

        DecoCompleteMsg = "UI_DecoCompleteMsg",
        NotAvailableInPlayMsg = "UI_NotAvailableInPlayMsg",
        DropItemMsg = "UI_DropItemMsg",

        ComingSoonMsg = "UI_ComingSoonMsg",
        MaxValMsg = "UI_MaxValMsg", // 현재 최대치입니다.

        PrevLvSkillLearnMsg = "UI_PrevLvSkillLearnMsg",
        SkillAcquiredMsg = "UI_SkillAcquiredMsg",
        SkillResetMsg = "UI_SkillResetMsg",
        SkillResetCompleteMsg = "UI_SkillResetCompleteMsg",
        SkillUnActiveMsg = "UI_SkillUnActiveMsg",

        HundradFloorPossibleMsg = "UI_HundradFloorPossibleMsg",
        RebornAskTitle = "UI_RebornAskTitle",
        RebornAskMsg = "UI_RebornAskMsg",
        AlreadyPlayGame = "UI_AlreadyPlayGame",

        MyTotleBestFloor = "UI_MyTotleBestFloor",

        LockShopWarningMsg = "UI_lockShopWarningMsg", // 환생 1회 달성시, 이용가능합니다.
        UnlockShopTitleMsg = "UI_UnlockShopTitleMsg", // 황금고블린의 상점 개방!
        UnlockShopContentMsg = "UI_UnlockShopContentMsg", // 황금코인으로 다양한 아이템 구매와 추가보상 획득이 가능합니다!
        AdsRebornBonusMsg = "UI_AdsRebornBonusMsg", // 광고시청시 30% 추가획득

        AdFailedToLoad = "UI_AdFailedToLoad", // 광고로드 실패
        LoadAdPlsRetryMsg = "UI_LoadAdPlsRetryMsg", // 광고를 로드중입니다. 잠시후 다시 실행해주세요.

        NONE = "";

    bool isChaning;

    void Awake() {
        _ = this;
    }

    void Start() {
        GM._.idm.INV_ITEM_INFO = GM._.idm.SetInvItemDescriptionInfo();

        // 게임 맨처음 실행 또는 리셋
        if(DM._.DB.languageIdx == -1)
        {
            // 최초 언어설정
            GM._.stm.languagePanel.SetActive(true);
        }
        else
        {
            // 언어 적용
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[DM._.DB.languageIdx];
            // 국가아이콘 최신화
            GM._.stm.SetConturyIcon(DM._.DB.languageIdx);
            // 타이블 언어 최신화
            GM._.stm.SetTitleLogo(DM._.DB.languageIdx);
        }
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
        DM._.DB.languageIdx = languageIdx; // 언어설정 데이터 저장

        // 인벤토리 상세페이지 아이템 정보 언어변경
        GM._.idm.INV_ITEM_INFO = GM._.idm.SetInvItemDescriptionInfo();
        GM._.ivm.InitDataAndUI();
        // 국가아이콘 최신화
        GM._.stm.SetConturyIcon(languageIdx);
        // 타이블 언어 최신화
        GM._.stm.SetTitleLogo(languageIdx);

        isChaning = false;

        // 설정 볼륨 및 배경음악 데이터 저장후
        DM._.DB.bgmVolume = GM._.stm.bgmSlider.value;
        DM._.DB.sfxVolume = GM._.stm.sfxSlider.value;

        // 씬 재로드
        SceneManager.LoadScene("Game");
    }
#endregion
}
