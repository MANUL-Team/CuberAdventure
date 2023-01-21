using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickManager : MonoBehaviour
{
    [SerializeField] private Joystick forGround, forWater;
    [SerializeField] private GameObject groundHandle, waterHandle;

    public void JoystickSwitch(bool uw){
        if(uw){
            forWater.gameObject.SetActive(true);
            forGround.input = Vector2.zero;
            groundHandle.transform.localPosition = new Vector2(0, 0);
            forGround.gameObject.SetActive(false);
        }
        else{
            waterHandle.transform.localPosition = new Vector2(0, 0);
            forWater.gameObject.SetActive(false);
            forGround.gameObject.SetActive(true);
            forWater.input = Vector2.zero;
        }
    }

}
