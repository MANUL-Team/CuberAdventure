using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTheMission : MonoBehaviour
{
    [SerializeField] private GameObject check;
    [SerializeField] private CloseAnything close;
    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("CheckTheMission") == 1){
            check.SetActive(true);
        }
        else{
            check.SetActive(false);
        }
    }
    public void Checked(){
        PlayerPrefs.SetInt("CheckTheMission", 0);
        close.Close();
    }
}
