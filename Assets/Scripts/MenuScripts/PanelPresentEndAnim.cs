using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelPresentEndAnim : MonoBehaviour
{
    public void EndAnim(){
        gameObject.GetComponent<Animator>().SetBool("DropPresent", false);
    }
}
