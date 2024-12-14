using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Version : MonoBehaviour
{
    public static Version _;

    public int MAJOR;
    public int MINOR;
    public int REVISION;

    public TMP_Text versionTxt;
    
    void Start()
    {
        _ = this;
        SetVersion();
    }

    public void SetVersion()
    {
        versionTxt.text = $"VER. {MAJOR}.{MINOR}.{REVISION}({(GM._.spm.isPC? "PC" : "MB")})";
    }
}