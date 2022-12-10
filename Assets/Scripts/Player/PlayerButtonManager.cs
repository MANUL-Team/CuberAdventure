using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonManager : MonoBehaviour
{
    [SerializeField] GameObject PM;
    private Animator PManim;
    private void Start() {
        PManim = PM.GetComponent<Animator>();
    }
    public void OpenClosePM(){
        if(PM.activeSelf == false){
            PM.SetActive(true);
        }else{
            PManim.SetTrigger("Close");
        }
    }
}
