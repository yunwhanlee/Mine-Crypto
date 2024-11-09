using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillTreeDB
{
    public int attackSkillTreeLv;
    public int buffSkillTreeLv;
    public int skipSkillTreeLv;

    public void Init()
    {
        attackSkillTreeLv = 1;
        buffSkillTreeLv = 1;
        skipSkillTreeLv = 1;
    }
}
