using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMissions : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject target;

    public void Close(){
        anim.SetBool("Close", true);
    }
    public void Open(){
        target.SetActive(true);
        anim.SetBool("Close", false);
    }
}
