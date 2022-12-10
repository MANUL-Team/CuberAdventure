using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnim : MonoBehaviour
{
    private Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }
    public void End(){
        anim.SetBool("Right", false);
        anim.SetBool("Left", false);
    }
}
