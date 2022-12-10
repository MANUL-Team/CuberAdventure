using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDisplay : MonoBehaviour
{
    [SerializeField] private GameObject[] laser, turbine, core, gusenici, armor;

    private void Update() {
        for(int i = 0; i < laser.Length; i++){
            if(i == PlayerPrefs.GetInt("ChangedLaser")){
                laser[i].SetActive(true);
            } else{laser[i].SetActive(false);}
        }
        for(int i = 0; i < turbine.Length; i++){
            if(i == PlayerPrefs.GetInt("ChangedTurbine")){
                turbine[i].SetActive(true);
            } else{turbine[i].SetActive(false);}
        }
        for(int i = 0; i < core.Length; i++){
            if(i == PlayerPrefs.GetInt("ChangedWeapon")){
                core[i].SetActive(true);
            } else{core[i].SetActive(false);}
        }
        for(int i = 0; i < gusenici.Length; i++){
            if(i == PlayerPrefs.GetInt("ChangedGusenici")){
                gusenici[i].SetActive(true);
            } else{gusenici[i].SetActive(false);}
        }
        for(int i = 0; i < armor.Length; i++){
            if(i == PlayerPrefs.GetInt("ChangedArmor")){
                armor[i].SetActive(true);
            } else{armor[i].SetActive(false);}
        }
    }
}
