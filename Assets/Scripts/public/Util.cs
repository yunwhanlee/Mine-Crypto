using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;

public class Util : MonoBehaviour
{
    public static WaitForSeconds TIME0_05 = new WaitForSeconds(0.05f);
    public static WaitForSeconds TIME0_1 = new WaitForSeconds(0.1f);
    public static WaitForSeconds TIME0_2 = new WaitForSeconds(0.2f);
    public static WaitForSeconds TIME0_3 = new WaitForSeconds(0.3f);
    public static WaitForSeconds TIME0_5 = new WaitForSeconds(0.5f);
    public static WaitForSeconds TIME1 = new WaitForSeconds(1);

    public static string ConvertTimeFormat(int timeSec) {
        int sec = timeSec % 60;
        int min = timeSec / 60;
        int hour = min / 60;
        string hourStr = (hour == 0)? "" : $"{hour:00} : ";

        return $"{hourStr} {min:00} : {sec:00}";
    }
}
