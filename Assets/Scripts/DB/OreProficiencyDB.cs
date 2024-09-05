using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Enum;

[Serializable]
public struct OreProficiencySaveData
{
    [field:SerializeField] public PROFICIENCY Type {get; private set;}      // 타입
    [field:SerializeField] public int Lv {get; set;}                // 레벨
    [field:SerializeField] public int Exp {get; set;}               // 경험치

    public OreProficiencySaveData(PROFICIENCY Type, int Lv, int Exp)
    {
        this.Type = Type;
        this.Lv = Lv;
        this.Exp = Exp;
    }
}

[Serializable]
public class OreProficiencyDB
{
    public OreProficiencySaveData[] saveDts;

    public void Init()
    {
        saveDts = new OreProficiencySaveData[10] {
            new OreProficiencySaveData( PROFICIENCY.ORE1, Lv: 1, Exp: 0 ),    
            new OreProficiencySaveData( PROFICIENCY.ORE2, Lv: 1, Exp: 0 ),    
            new OreProficiencySaveData( PROFICIENCY.ORE3, Lv: 1, Exp: 0 ),    
            new OreProficiencySaveData( PROFICIENCY.ORE4, Lv: 1, Exp: 0 ),    
            new OreProficiencySaveData( PROFICIENCY.ORE5, Lv: 1, Exp: 0 ),    
            new OreProficiencySaveData( PROFICIENCY.ORE6, Lv: 1, Exp: 0 ),    
            new OreProficiencySaveData( PROFICIENCY.ORE7, Lv: 1, Exp: 0 ),    
            new OreProficiencySaveData( PROFICIENCY.ORE8, Lv: 1, Exp: 0 ),    
            new OreProficiencySaveData( PROFICIENCY.ORE_CHEST, Lv: 1, Exp: 0 ),    
            new OreProficiencySaveData( PROFICIENCY.TREASURE_CHEST, Lv: 1, Exp: 0 ),    
        };
    }
}
