using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesQuest : MonoBehaviour
{
    [SerializeField] private DialogChange main;
    [SerializeField] private int lvlArmor, lvlCore, lvlLaser, id, step;
    [SerializeField] private bool last;

    // private void FixedUpdate() {
    //     if(PlayerPrefs.GetInt("Armor") >= lvlArmor && PlayerPrefs.GetInt("Core") >= lvlCore && PlayerPrefs.GetInt("Laser") >= lvlLaser){
    //         main.StartNewMissionStep(id, step, last);
    //     }
    // }
}
