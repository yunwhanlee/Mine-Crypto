using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleManager : MonoBehaviour
{
    public static LocaleManager _;

    bool isChaning;

    void Awake() {
        _ = this;
    }

#region FUNC
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

        isChaning = false;
    }
#endregion
}
