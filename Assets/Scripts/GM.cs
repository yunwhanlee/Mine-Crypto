using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM _;

    //* Outside
    public MineManager mm;
    public UIManager ui;

    void Awake()
    {
        _ = this;

        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        mm = GameObject.Find("MineManager").GetComponent<MineManager>();
    }
}
