using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;

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

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        rebornCntTxt.text = $": {DM._.DB.rebornCnt}";
        bgmSlider.value = DM._.DB.bgmVolume;
        sfxSlider.value = DM._.DB.sfxVolume;
    }

#region EVENT
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

        GM._.ui.ShowConfirmPopUp(LM._.Localize(LM.AskResetDataMsg));
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
