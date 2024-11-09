using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillTreeDB
{
    public int buffSkillTreeLv;
    public int attackSkillTreeLv;
    public int skipSkillTreeLv;

    public void Init()
    {
        buffSkillTreeLv = 1;
        attackSkillTreeLv = 1;
        skipSkillTreeLv = 1;
    }
}
