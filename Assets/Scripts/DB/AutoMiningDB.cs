using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct AutoMiningSaveData {
    [field:SerializeField] public bool IsUnlock {get; set;}
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int Time {get; set;}
    [field:SerializeField] public int CurStorage {get; set;}
}

[Serializable]
public class AutoMiningDB
{
    public AutoMiningSaveData[] saveDts;

    public void Init()
    {
        saveDts = new AutoMiningSaveData[9] {
            // 광석
            new() {IsUnlock= true,  Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            new() {IsUnlock= false, Lv= 1, Time= 0, CurStorage = 0},
            // 크리스탈
            new() {IsUnlock= true, Lv= 1, Time= 0, CurStorage = 0},
        };
    }

    // 어플시작시 경과한시간(초) 가져오기
    public int GetPassedSecData()
    {
        // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
        TimeSpan curTimeStamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);

        // 이전에 저장된 시간 
        // defaultValue: 는 저장된 KEY값이 없으면 지정한 값으로 초기화 값을 지정한다는 의미.
        int savedTimeSec = PlayerPrefs.GetInt(DM.PASSEDTIME_KEY, defaultValue: (int)curTimeStamp.TotalSeconds);

        // 경과한 시간 (현재시간 - 이전에 저장된 시간)
        return (int)curTimeStamp.TotalSeconds - savedTimeSec;
    }
}