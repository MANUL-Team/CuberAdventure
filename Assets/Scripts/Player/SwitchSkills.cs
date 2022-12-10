using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSkills : MonoBehaviour
{
    [SerializeField] private GameObject[] skillsButtons;

    public void SkillSwitch() {
        for(int i = 0; i<skillsButtons.Length; i++){
            if(i == PlayerPrefs.GetInt("Skill")){
                skillsButtons[i].SetActive(true);
            }
            else{
                skillsButtons[i].SetActive(false);
            }
        }
    }
    private void Start() {
        SkillSwitch();
    }
}
