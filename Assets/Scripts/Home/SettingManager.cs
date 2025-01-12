using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;
using static Enum;

public class SettingManager : MonoBehaviour
{
    public DOTweenAnimation DOTAnim;

    // Element
    public GameObject windowObj;
    public TMP_Text rebornCntTxt;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public GameObject languagePanel;
    public Sprite[] languageIconSprs;
    public Image languageSelectBtnImg;

    public Sprite[] languageTitleSprs; // 홈 타이틀 국가별 이미지
    public Image TitleImg;

    //! TEST MODE
    // public GameObject testMode;
    int testModeCnt = 0;

    IEnumerator Start()
    {
        // testMode.SetActive(false);

        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        rebornCntTxt.text = $": {DM._.DB.rebornCnt}";
        bgmSlider.value = DM._.DB.bgmVolume;
        sfxSlider.value = DM._.DB.sfxVolume;
    }

#region EVENT
    /// <summary>
    /// TEST 모드 : 설정창 아이콘 10번클릭
    /// </summary>
    public void OnClickTestModeBtn()
    {
        testModeCnt++;

        // 데이터 리셋
        if(testModeCnt % 10 == 0) {
            OnClickResetBtn();
        }

        // // TEST 모드
        // if(testModeCnt % 10 == 0) {
        //     // 보상획득
        //     GM._.rwm.ShowReward (
        //         new Dictionary<RWD, int>
        //         {
        //             { RWD.ORE_CHEST, 100 },
        //             { RWD.TREASURE_CHEST, 100 },
        //             { RWD.LIGHTSTONE, 5000 },
        //         }
        //     );
        //     // 층수 증가
        //     DM._.DB.stageDB.BestFloorArr[0] += 12;
        //     DM._.DB.stageDB.BestFloorArr[1] += 12;
        //     DM._.DB.stageDB.BestFloorArr[2] += 12;
        //     DM._.DB.stageDB.BestFloorArr[3] += 12;
        //     DM._.DB.stageDB.BestFloorArr[4] += 12;
        //     DM._.DB.stageDB.BestFloorArr[5] += 12;
        //     DM._.DB.stageDB.BestFloorArr[6] += 12;
        //     DM._.DB.stageDB.BestFloorArr[7] += 12;

        //     DM._.DB.bestTotalFloor = DM._.DB.stageDB.GetTotalBestFloor();
        //     DM._.DB.statusDB.FameLv++;
        //     GM._.ui.ShowNoticeMsgPopUp("모든광산 12층씩 추가 및 +명예레벨1");
        // }
    }

    public void OnClickSelectLanguage()
    {
        if(GM._.gameState != GameState.HOME)
        {
            // 플레이중에는 불가능합니다 메세지 표시
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotAvailableInPlayMsg));
            return;
        }

        languagePanel.SetActive(true);
    }

    /// <summary>
    /// 언어변경 버튼 클릭
    /// </summary>
    /// <param name="lauguageIdx">언어종류 인덱스(EN:0, JP:1, KR:2)</param>
    public void OnClickChangeLanguage(int lauguageIdx)
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap2SFX);
        languagePanel.SetActive(false);
        languageSelectBtnImg.sprite = languageIconSprs[lauguageIdx];
        LM._.ChangeLocale(lauguageIdx);
    }

    public void OnClickResetBtn()
    {
        if(GM._.gameState != GameState.HOME)
        {
            // 플레이중에는 불가능합니다 메세지 표시
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotAvailableInPlayMsg));
            return;
        }

        GM._.ui.ShowConfirmPopUp(LM._.Localize(LM.Caution), LM._.Localize(LM.AskResetDataMsg));
        GM._.ui.OnClickConfirmBtnAction = () => {
            DM._.Reset();
            SceneManager.LoadScene("Game");
        };
    }

    public void OnClickAppQuitBtn()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void OnSliderBgmVolumeChanged()
    {
        // 슬라이더의 값을 0.1 단위로 반올림
        float roundedValue = Mathf.Round(bgmSlider.value * 10f) / 10f;
        bgmSlider.value = roundedValue;
        SoundManager._.SetBgmVolume(bgmSlider.value);
    }

    public void OnSliderSfxVolumeChanged()
    {
        // 슬라이더의 값을 0.1 단위로 반올림
        float roundedValue = Mathf.Round(sfxSlider.value * 10f) / 10f;
        sfxSlider.value = roundedValue;
        SoundManager._.SetSfxVolume(sfxSlider.value);
    }

    public void OnClickReviewBtn()
    {
        if(DM._.isPC)
            Application.OpenURL(STEAM_URL);
        else
            Application.OpenURL(GOOGLE_URL);
    }
#endregion

#region FUNC
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
    }

    /// <summary>
    /// 국가아이콘 최신화
    /// </summary>
    public void SetConturyIcon(int curLanguageIdx)
    {
        languageSelectBtnImg.sprite = languageIconSprs[curLanguageIdx];
    }

    /// <summary>
    /// 타이블 언어 최신화
    /// </summary>
    public void SetTitleLogo(int curLanguageIdx)
    {
        TitleImg.sprite = languageTitleSprs[curLanguageIdx];
    }
#endregion
}
