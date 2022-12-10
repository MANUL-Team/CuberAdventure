using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuberCustomizeOpen : MonoBehaviour
{
    [SerializeField] private GameObject energySys, armor, laser, turbine, gusenici;

    public void EnergyOpen(){
        energySys.SetActive(true);
        armor.SetActive(false);
        laser.SetActive(false);
        turbine.SetActive(false);
        gusenici.SetActive(false);
    }
    public void ArmorOpen(){
        energySys.SetActive(false);
        armor.SetActive(true);
        laser.SetActive(false);
        turbine.SetActive(false);
        gusenici.SetActive(false);
    }
    public void LaserOpen(){
        energySys.SetActive(false);
        armor.SetActive(false);
        laser.SetActive(true);
        turbine.SetActive(false);
        gusenici.SetActive(false);
    }
    public void TurbineOpen(){
        energySys.SetActive(false);
        armor.SetActive(false);
        laser.SetActive(false);
        turbine.SetActive(true);
        gusenici.SetActive(false);
    }
    public void GuseniciOpen(){
        energySys.SetActive(false);
        armor.SetActive(false);
        laser.SetActive(false);
        turbine.SetActive(false);
        gusenici.SetActive(true);
    }
}
