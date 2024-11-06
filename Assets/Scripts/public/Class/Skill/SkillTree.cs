using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enum;



/// <summary>
/// 스킬트리 데이터
/// </summary>
[System.Serializable]
public class SkillTree
{
    [field:SerializeField] public int Id {get; private set;}
    [field:SerializeField] public SkillCate Cate {get; private set;}
    [field:SerializeField] public Sprite IconSpr {get; private set;}
    [field:SerializeField] public Image Border {get; private set;}
    [field:SerializeField] public GameObject Dim {get; private set;}
    [field:SerializeField] public bool IsLock {
        get {
            // 레벨로 현재 잠김을 파악
            switch(Cate) {
                case SkillCate.Buff:
                    return DM._.DB.skillTreeDB.buffSkillTreeLv < Id;
                case SkillCate.Attack:
                    return DM._.DB.skillTreeDB.attackSkillTreeLv < Id;
                case SkillCate.Skip:
                    return DM._.DB.skillTreeDB.skipSkillTreeLv < Id;
            }
            return false;
        } 
    }

    /// <summary> 선택테두리 초기화 </summary>
    public void InitBorderUI() => Border.color = Color.white;
    /// <summary> 잠김상태 최신화 </summary>
    public void UpdateDimUI() => Dim.SetActive(IsLock);
}