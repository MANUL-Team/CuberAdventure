using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBossfightRoom : MonoBehaviour
{
    [SerializeField] private int id, step;

    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("Mission " + id + " Step " + step) == 1){
            GetComponent<AnotherLevel>().enabled = true;
        }
        else{
            GetComponent<AnotherLevel>().enabled = false;
        }
    }
}
