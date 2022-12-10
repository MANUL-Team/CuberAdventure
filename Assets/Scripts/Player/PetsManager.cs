using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetsManager : MonoBehaviour
{

    [SerializeField] private GameObject[] petUseButtons;

    private void FixedUpdate(){
        for(int i = 0; i <= petUseButtons.Length; i++){
            if(PlayerPrefs.GetInt("Pet" + i.ToString()) == 1){
                petUseButtons[i].SetActive(true);
            }else{
                petUseButtons[i].SetActive(false);
            }
        }
    }

}
