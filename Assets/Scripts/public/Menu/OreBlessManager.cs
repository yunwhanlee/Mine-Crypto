using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enum;
using Random = UnityEngine.Random;

public class OreBlessManager : MonoBehaviour
{
    StatusDB sttDB;
    OreBlessDB oreDB;

    const int RESET_CRISTAL_PRICE = 10;         // 능력치 재설정위한 크리스탈 가격
    const int RESET_ORE_PRICE = 10000;          // 능력치 재설정위한 광석조각 가격
    const int INT_TYPE = 0, FLOAT_TYPE = 1;     // 능력치 데이터 타입

    [field:Header("광산의 축복 능력치 데이터 (INT)")]
    [field:SerializeField] public OreBlessAbilityDB_Int[] Int_Abilities {get; private set;}
    [field:Header("광산의 축복 능력치 데이터 (FLOAT)")]
    [field:SerializeField] public OreBlessAbilityDB_Float[] Float_Abilities {get; private set;}

    public GameObject windowObj;
    public OreBlessFormat[] oreBlessFormatArr;

    IEnumerator Start() {
        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);

        sttDB = DM._.DB.statusDB;
        oreDB = DM._.DB.oreBlessDB;

        UpdateUI();
    }

#region EVENT
    /// <summary>
    /// 축복 능력지 재설정
    /// </summary>
    /// <param name="oreBlessIdx">광석축복 슬롯 IDX</param>
    public void OnClickAbilityResetBtn(int oreBlessIdx) {

        // 필요재화 수량 체크
        if(sttDB.RscArr[(int)RSC.CRISTAL] >= RESET_CRISTAL_PRICE
        && sttDB.RscArr[oreBlessIdx] >= RESET_ORE_PRICE)
        {
            GM._.ui.ShowNoticeMsgPopUp($"제{oreBlessIdx + 1}광산 축복 능력 재설정 성공!");

            // 재화 데이터 수치 감소
            sttDB.SetRscArr((int)RSC.CRISTAL, -RESET_CRISTAL_PRICE);
            sttDB.SetRscArr(oreBlessIdx, -RESET_ORE_PRICE);
        }
        else
        {
            GM._.ui.ShowWarningMsgPopUp($"재화가 부족합니다!");
            return;
        }

        ResetAbilities(oreBlessIdx);
    }
#endregion

#region FUNC
    /// <summary>
    /// 로드한 데이터로 잠금해제 및 능력치UI 표시
    /// </summary>
    private void UpdateUI() {
        // 로드데이터 UI 확인 및 표시
        for(int i = 0; i < oreDB.saveDts.Length; i++)
        {
            OreBlessSaveData saveDt = oreDB.saveDts[i];

            // 잠금해제 화면
            oreBlessFormatArr[i].IsUnlock = saveDt.IsUnlock;

            // 능력치 텍스트 초기화
            oreBlessFormatArr[i].AbilityTxt.text = "";

            // 능력치
            if(saveDt.AbilityList != null)
            {
                saveDt.AbilityList.ForEach( ability =>
                {
                    Debug.Log($"ability:: grade= {ability.grade}, type= {ability.type}, val= {ability.val}");

                    string unit = "";
                    OreBlessAbilityDB_Int intAbility = null;
                    OreBlessAbilityDB_Float floatAbility = null;

                    // 능력치 자료형
                    bool isInt = (
                        ability.type == OREBLESS_ABT.INC_TIMER
                        || ability.type == OREBLESS_ABT.INC_CRISTAL
                        || ability.type == OREBLESS_ABT.INC_POPULATION
                    );

                    // 자료형에 따른 처리
                    if(isInt)
                    {
                        // 정수형 능력 인덱스 정규화
                        int idx = (int)ability.type;

                        intAbility = Int_Abilities[idx];
                        unit = (intAbility.Type == OREBLESS_ABT.INC_TIMER)? "초" : ""; // 단위표시
                    }
                    else 
                    {
                        // 소수형 능력 인덱스 정규화
                        int idx = (int)ability.type - (int)OREBLESS_ABT.ATK_PER;

                        floatAbility = Float_Abilities[idx];
                        unit = "%";
                    }

                    // 등급 텍스트 색상
                    string colorTag = GetGradeTagColor(ability.grade);

                    // 수치 표시 (Float의 경우, 소수점 3자리까지만)
                    string displayValue = isInt? ability.val.ToString() : (ability.val * 100).ToString("0.###");

                    //* 능력치 UI텍스트 업데이트
                    oreBlessFormatArr[i].AbilityTxt.text += 
                        $"<color={colorTag}>{(isInt? intAbility.AbtName : floatAbility.AbtName)} +{displayValue}{unit}</color>\n";
                });
            }
        }
    }

    /// <summary>
    /// 능력치 리셋
    /// </summary>
    /// <param name="oreBlessIdx">축복의 광산 IDX</param>
    public void ResetAbilities(int oreBlessIdx) {
        OreBlessSaveData saveDt = oreDB.saveDts[oreBlessIdx];

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

            // 랜덤 등급
            int randPer = Random.Range(0, 100);
            GRADE grade = randPer < 40? GRADE.COMMON
                : randPer < 70? GRADE.UNCOMMON
                : randPer < 85? GRADE.RARE
                : randPer < 95? GRADE.UNIQUE
                : randPer < 99? GRADE.LEGEND
                : GRADE.MYTH;

            // 랜덤 능력치 설정
            SetRandomAbilities(randDataType, (int)grade, oreBlessIdx);
        }
    }

    /// <summary>
    /// 랜덤 능력치 설정
    /// </summary>
    /// <param name="dataType">자료형 타입</param>
    /// <param name="gradeIdx">등급 IDX</param>
    /// <param name="oreBlessIdx">광석축복 슬롯 IDX</param>
    private void SetRandomAbilities(int dataType, int gradeIdx, int oreBlessIdx) {
        float value = 0;
        string unit = "";
        OreBlessAbilityDB_Int intAbility = null;
        OreBlessAbilityDB_Float floatAbility = null;

        // 랜덤 능력치 처리
        switch(dataType)
        {
            case INT_TYPE: {
                int rdTypeIdx = Random.Range(0, Int_Abilities.Length);
                intAbility = Int_Abilities[rdTypeIdx];
                // Debug.Log($"Int_Abilities.Length= {Int_Abilities.Length}, rdTypeIdx= {rdTypeIdx}, intAbility.Type=" + intAbility.Type);
                value = Random.Range(intAbility.MinArr[gradeIdx], intAbility.MaxArr[gradeIdx]);
                unit = (intAbility.Type == OREBLESS_ABT.INC_TIMER)? "초" : ""; // 단위표시
                break;
            }
            case FLOAT_TYPE: {
                int rdTypeIdx = Random.Range(0, Float_Abilities.Length);
                floatAbility = Float_Abilities[rdTypeIdx];
                // Debug.Log($"Float_Abilities.Length= {Float_Abilities.Length}, rdTypeIdx= {rdTypeIdx}, floatAbility.Type=" + floatAbility.Type);
                value = Random.Range(floatAbility.MinArr[gradeIdx], floatAbility.MaxArr[gradeIdx]);
                value = (float)Math.Round(value, 3); // 최대 소수점 3자리수
                unit = "%";
                break;
            }
        }

        bool isInt = dataType == INT_TYPE;

        //* 능력치 DB데이터 추가
        oreBlessFormatArr[oreBlessIdx].AbilityList.Add (
            new OreBlessAbilityData {
                grade = (GRADE)gradeIdx,
                type = isInt? intAbility.Type : floatAbility.Type,
                val = value
            }
        );

        // 등급 텍스트 색상
        string colorTag = GetGradeTagColor((GRADE)gradeIdx);

        // 수치 표시 (Float의 경우, 소수점 3자리까지만)
        string displayValue = isInt? value.ToString() : (value * 100).ToString("0.###");

        //* 능력치 UI텍스트 업데이트
        oreBlessFormatArr[oreBlessIdx].AbilityTxt.text += 
            $"<color={colorTag}>{(isInt? intAbility.AbtName : floatAbility.AbtName)} +{displayValue}{unit}</color>\n";
    }

    /// <summary>
    /// 광산의축복 능력치 값 반환
    /// </summary>
    /// <param name="type">능력치 타입</param>
    /// <returns>INC_TIMER, INC_CRISTAL, INC_POPULATION의 경우에 (int)형변환하기!</returns>
    public float GetAbilityValue(OREBLESS_ABT type)
    {
        float val = 0;

        // 능력치가 활성화되있는 축복만 검출
        var activeOreBlessArr = Array.FindAll(oreBlessFormatArr, oreBless => oreBless.AbilityList != null);

        // 활성화 축복 리스트
        Array.ForEach(activeOreBlessArr, oreBless => {
            // 능력치 리스트
            oreBless.AbilityList.ForEach(ability => {
                // 검출할 타입과 같은 경우
                if(ability.type == type)
                    val += ability.val; // 능력치 값 더하기
            });
        });
        return val; // 다 더해진 능력치 반환
    }
#endregion
}
