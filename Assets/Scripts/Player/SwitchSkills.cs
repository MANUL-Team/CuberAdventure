using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSkills : MonoBehaviour
{
    [SerializeField] private GameObject[] skillsButtons;

    public void SkillSwitch() {
        for(int i = -1; i<skillsButtons.Length; i++){
            if(i == PlayerPrefs.GetInt("Skill")){
                skillsButtons[i+1].SetActive(true);
            }
            else{
                skillsButtons[i+1].SetActive(false);
            }
        }
    }
    private void FixedUpdate() {
        SkillSwitch();
    }
}
