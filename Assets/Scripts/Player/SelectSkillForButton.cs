using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSkillForButton : MonoBehaviour
{
    [SerializeField] private int skillId;
    [SerializeField] private SelectSkill skill;
    [SerializeField] private GameObject skillGO;

    public void NewSkill(){
        skillGO.SetActive(true);
        skill.SelectNewSkill(skillId);
    }
}
