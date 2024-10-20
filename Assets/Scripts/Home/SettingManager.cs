using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class SettingManager : MonoBehaviour
{
    public DOTweenAnimation DOTAnim;

    // Element
    public GameObject windowObj;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public GameObject languagePanel;
    public Sprite[] languageIconSprs;
    public Image languageSelectBtnImg;

    IEnumerator Start()
    {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        bgmSlider.value = DM._.DB.bgmVolume;
        sfxSlider.value = DM._.DB.sfxVolume;

        // 게임 맨처음 또는 리셋이라면 최초 언어설정
        if(DM._.DB.languageIdx == -1)
        {
            languagePanel.SetActive(true);
        }
        // 저장된 언어 적용
        else
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[DM._.DB.languageIdx];
            languageSelectBtnImg.sprite = languageIconSprs[DM._.DB.languageIdx]; // 국가아이콘 최신화
        }
    }

#region EVENT
    public void OnClickSelectLanguage()
    {
        languagePanel.SetActive(true);
    }

    /// <summary>
    /// 언어변경 버튼 클릭
    /// </summary>
    /// <param name="lauguageIdx">언어종류 인덱스(EN:0, JP:1, KR:2)</param>
    public void OnClickChangeLanguage(int lauguageIdx)
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap2SFX);
        LM._.ChangeLocale(lauguageIdx);
        languagePanel.SetActive(false);
        languageSelectBtnImg.sprite = languageIconSprs[lauguageIdx];
    }

    public void OnClickResetBtn()
    {
        GM._.ui.ShowConfirmPopUp(LM._.Localize(LM.AskResetDataMsg));
        GM._.ui.OnClickConfirmBtnAction = () => {
            DM._.Reset();
            SceneManager.LoadScene("Game");
        };
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
#endregion
}
