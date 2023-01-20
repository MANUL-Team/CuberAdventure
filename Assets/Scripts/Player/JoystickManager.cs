using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickManager : MonoBehaviour
{
    [SerializeField] private GameObject forGround, forWater;

    public void JoystickSwitch(bool uw){
        if(uw){
            forWater.SetActive(true);
            forGround.SetActive(false);
        }
        else{
            forWater.SetActive(false);
            forGround.SetActive(true);
        }
    }

}
