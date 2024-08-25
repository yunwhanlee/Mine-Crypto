using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreBlessManager : MonoBehaviour
{
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
    /// <param name="idx">광석축복 슬롯 IDX</param>
    public void OnClickAbilityResetBtn(int idx) {
        GM._.ui.ShowNoticeMsgPopUp($"제{idx + 1}광산 축복 능력 재설정 성공!");

        var type = Enum.OREBLESS_ABT.ATK_PER;
        float val = 0;
        string name = "";

        // 능력리스트 리셋
        oreBlessFormatArr[idx].AbilityList = new List<OreBlessAbilityData>();
        oreBlessFormatArr[idx].AbilityTxt.text = "";

        // 랜덤 능력치 개수 (1개 ~ 3개)
        int abilityLen = Random.Range(1, 3 + 1);

        oreBlessFormatArr[idx].AbilityCnt = abilityLen;

        for(int i = 0; i < abilityLen; i++)
        {
            // 타입 랜덤
            const int INT_TYPE = 0, FLOAT_TYPE = 1;
            int randType = Random.Range(0, 2);

            // 랜덤 능력
            switch(randType)
            {
                case INT_TYPE:
                {
                    int rdType = Random.Range(0, Int_Abilities.Length);
                    var rdAbility = Int_Abilities[rdType];
                    int randVal = Random.Range(rdAbility.MinArr[0], rdAbility.MaxArr[0]);

                    // 능력치 데이터 추가
                    oreBlessFormatArr[idx].AbilityList.Add (
                        new OreBlessAbilityData { type = rdAbility.Type, val = randVal}
                    );

                    // 능력치 UI텍스트
                    oreBlessFormatArr[idx].AbilityTxt.text += $"{rdAbility.AbtName} {randVal}\n";
                    break;
                }
                case FLOAT_TYPE:
                {
                    int rdType = Random.Range(0, Float_Abilities.Length);
                    var rdAbility = Float_Abilities[rdType];
                    float randVal = Random.Range(rdAbility.MinArr[0], rdAbility.MaxArr[0]);
                    randVal = (float)Math.Round(randVal, 3);

                    // 능력치 데이터 추가
                    
                    oreBlessFormatArr[idx].AbilityList.Add (
                        new OreBlessAbilityData { type = rdAbility.Type, val = randVal}
                    );

                    // 능력치 UI텍스트
                    oreBlessFormatArr[idx].AbilityTxt.text += $"{rdAbility.AbtName} {randVal * 100}\n";
                    break;
                }
            }


        }
    }
#endregion

#region FUNC
    
#endregion
}
