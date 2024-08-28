using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OreBless Ability Data", menuName = "Scriptable Object/OreBlessAbilityData_Int")]
public class OreBlessAbilityDB_Int : ScriptableObject
{
    [SerializeField] private Enum.OREBLESS_ABT type;
    public Enum.OREBLESS_ABT Type {
        get => type;
        set => type = value;
    }

    [SerializeField] private string abtName;
    public string AbtName {get => abtName;}

    [Header("축복능력치 랜덤최소값 [일반, 고급, 레어, 유니크, 전설, 신화]")]
    [SerializeField] private int[] minArr = new int[6]; 
    public int[] MinArr {get => minArr;}

    [Header("축복능력치 랜덤최대값 [일반, 고급, 레어, 유니크, 전설, 신화]")]
    [SerializeField] private int[] maxArr = new int[6]; 
    public int[] MaxArr {get => maxArr;}
}