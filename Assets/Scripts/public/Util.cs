using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

public class Util : MonoBehaviour
{
    public static WaitForSeconds TIME0_05 = new WaitForSeconds(0.05f);
    public static WaitForSeconds TIME0_075 = new WaitForSeconds(0.075f);
    public static WaitForSeconds TIME0_1 = new WaitForSeconds(0.1f);
    public static WaitForSeconds TIME0_2 = new WaitForSeconds(0.2f);
    public static WaitForSeconds TIME0_3 = new WaitForSeconds(0.3f);
    public static WaitForSeconds TIME0_5 = new WaitForSeconds(0.5f);
    public static WaitForSeconds TIME1 = new WaitForSeconds(1);
    public static WaitForSeconds TIME2 = new WaitForSeconds(2);
    public static WaitForSeconds TIME2_5 = new WaitForSeconds(2.5f);
    public static WaitForSeconds TIME3 = new WaitForSeconds(3);
    public static WaitForSeconds TIME5 = new WaitForSeconds(5);
    public static WaitForSeconds TIME10 = new WaitForSeconds(10);
    public static WaitForSeconds TIME15 = new WaitForSeconds(15);
    public static WaitForSeconds TIME30 = new WaitForSeconds(30);
    public static WaitForSeconds TIME60 = new WaitForSeconds(60);

    public static WaitForSecondsRealtime RT_TIME1 = new WaitForSecondsRealtime(1f);

    public static string ConvertTimeFormat(int timeSec) {
        int sec = timeSec % 60;
        int min = timeSec / 60;
        int hour = min / 60;
        string hourStr = (hour == 0)? "" : $"{hour:00} : ";

        return $"{hourStr} {min:00} : {sec:00}";
    }

    /// <summary>
    /// 소수 문자열 자동변환 (79.999999같은 float 부동소수점 버그방지)
    /// </summary>
    public static string FloatToStr(float n)
    {
        // 소수점 첫째자리까지 반올림
        float roundVal = Mathf.Round(n * 10) / 10;

        // 소수점 첫째 자리가 0이라면
        if (roundVal * 10 % 10 == 0)
            return $"{(int)roundVal}"; // 정수로 반환 ToString("D")
        else
            return $"{roundVal:0.0}"; // 소수점 첫째 자리까지 반환 ToString("F1");
    }
}
