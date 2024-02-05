using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSkills : MonoBehaviour
{
    [SerializeField] private GameObject[] skillsButtons;

    private void Start()
    {
        StartCoroutine("SkillCheck");
    }

    public void SkillSwitch() {
        for (int i = 0; i < skillsButtons.Length; i++)
        {
            skillsButtons[i].SetActive(false);
        }
        skillsButtons[PlayerPrefs.GetInt("Skill")+1].SetActive(true);
    }

    private IEnumerator SkillCheck()
    {
        while (true)
        {
            SkillSwitch();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
