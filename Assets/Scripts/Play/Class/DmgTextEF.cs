using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DmgTextEF : MonoBehaviour
{
    [field:SerializeField] public GameObject obj {get; set;}
    [field:SerializeField] public TMP_Text txt {get; set;}
    [field:SerializeField] public DOTweenAnimation DOTAnim {get; set;}
}
