using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillTreeDB : MonoBehaviour
{
    public int buffSkillTreeLv;
    public int attackSkillTreeLv;
    public int skipSkillTreeLv;

    public void Init()
    {
        buffSkillTreeLv = 0;
        attackSkillTreeLv = 0;
        skipSkillTreeLv = 0;
    }
}
