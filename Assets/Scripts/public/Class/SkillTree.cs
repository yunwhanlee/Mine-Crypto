using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillTreeCate { Buff, Attack, Skip }

[System.Serializable]
public class SkillTree
{
    [field:SerializeField] public int Id {get; private set;}
    [field:SerializeField] public SkillTreeCate Cate {get; private set;}
    [field:SerializeField] public Image IconImg {get; private set;}
    [field:SerializeField] public Image Border {get; private set;}
    [field:SerializeField] public GameObject Dim {get; private set;}
    [field:SerializeField] public bool IsLock {
        get {
            // 레벨로 현재 잠김을 파악
            switch(Cate) {
                case SkillTreeCate.Buff:
                    return DM._.DB.skillTreeDB.buffSkillTreeLv < Id;
                case SkillTreeCate.Attack:
                    return DM._.DB.skillTreeDB.attackSkillTreeLv < Id;
                case SkillTreeCate.Skip:
                    return DM._.DB.skillTreeDB.skipSkillTreeLv < Id;
            }
            return false;
        } 
    }

    /// <summary>
    /// 선택테두리 초기화
    /// </summary>
    public void InitBorderUI() => Border.color = Color.white;
    /// <summary>
    /// 잠김상태 최신화
    /// </summary>
    public void UpdateDimUI() => Dim.SetActive(IsLock);
}
