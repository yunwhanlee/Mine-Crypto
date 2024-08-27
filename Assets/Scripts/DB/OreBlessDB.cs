using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class OreBlessDB
{
    [field:SerializeField] bool[] isUnlockArr;
    public bool[] IsUnlockArr { get => isUnlockArr; set => isUnlockArr = value;}


}
