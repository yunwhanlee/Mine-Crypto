using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreBlessManager : MonoBehaviour
{
    const int INT_TYPE = 0, FLOAT_TYPE = 1;

    [field:Header("광산의 축복 능력치 데이터 (INT)")]
    [field:SerializeField] public OreBlessAbilityDB_Int[] Int_Abilities {get; private set;}
    [field:Header("광산의 축복 능력치 데이터 (FLOAT)")]
    [field:SerializeField] public OreBlessAbilityDB_Float[] Float_Abilities {get; private set;}

    public GameObject windowObj;
    public OreBlessFormat[] oreBlessFormatArr;

#region EVENT
    /// <summary>
    /// 축복 능력지 재설정
    /// </summary>
    /// <param name="oreBlessIdx">광석축복 슬롯 IDX</param>
    public void OnClickAbilityResetBtn(int oreBlessIdx) {
        GM._.ui.ShowNoticeMsgPopUp($"제{oreBlessIdx + 1}광산 축복 능력 재설정 성공!");

        // 능력리스트 리셋
        oreBlessFormatArr[oreBlessIdx].AbilityList = new List<OreBlessAbilityData>();
        oreBlessFormatArr[oreBlessIdx].AbilityTxt.text = "";

        // 랜덤 능력치 개수 설정 (1개 ~ 3개)
        int abilityLen = Random.Range(1, 3 + 1);
        oreBlessFormatArr[oreBlessIdx].AbilityCnt = abilityLen;

        for(int i = 0; i < abilityLen; i++)
        {
            // 랜덤 타입
            int randDataType = Random.Range(INT_TYPE, FLOAT_TYPE + 1);
            // 랜덤 능력치 설정
            SetRandomAbilities(randDataType, oreBlessIdx);
        }
    }
#endregion

#region FUNC
    /// <summary>
    /// 랜덤 능력치 설정
    /// </summary>
    /// <param name="dataType">자료형 타입</param>
    /// <param name="oreBlessIdx">광석축복 슬롯 IDX</param>
    private void SetRandomAbilities(int dataType, int oreBlessIdx) {
        float value = 0;
        string unit = "";
        OreBlessAbilityDB_Int intAbility = null;
        OreBlessAbilityDB_Float floatAbility = null;

        // 랜덤 능력치 처리
        switch(dataType)
        {
            case INT_TYPE: {
                int rdType = Random.Range(0, Int_Abilities.Length);
                intAbility = Int_Abilities[rdType];
                value = Random.Range(intAbility.MinArr[0], intAbility.MaxArr[0]);
                unit = (intAbility.Type == Enum.OREBLESS_ABT.INC_TIMER)? "초" : ""; // 단위표시
                break;
            }
            case FLOAT_TYPE: {
                int rdType = Random.Range(0, Float_Abilities.Length);
                floatAbility = Float_Abilities[rdType];
                value = Random.Range(floatAbility.MinArr[0], floatAbility.MaxArr[0]);
                value = (float)Math.Round(value, 3);
                unit = "%";
                break;
            }
        }

        bool isInt = dataType == INT_TYPE;

        //* 능력치 데이터 추가
        oreBlessFormatArr[oreBlessIdx].AbilityList.Add (
            new OreBlessAbilityData { 
                type = isInt? intAbility.Type : floatAbility.Type,
                val = value
            }
        );

        //* 능력치 UI텍스트 업데이트
        oreBlessFormatArr[oreBlessIdx].AbilityTxt.text += 
            $"{(isInt? intAbility.AbtName : floatAbility.AbtName)} +{value * (isInt? 1 : 100)}{unit}\n";
    }
#endregion
}
