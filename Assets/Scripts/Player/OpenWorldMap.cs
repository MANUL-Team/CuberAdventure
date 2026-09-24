using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWorldMap : MonoBehaviour
{
    [SerializeField] private GameObject worldMap;
    private Animator anim;

    public void OpenMap(){
        worldMap.SetActive(true);
        if (anim == null)
            anim = worldMap.GetComponent<Animator>();
        if (anim == null)
            return;
        anim.enabled = true;
        anim.SetBool("Close", false);
    }
    public void CloseMap(){
        if (anim == null && worldMap != null)
            anim = worldMap.GetComponent<Animator>();
        if (anim == null)
            return;
        anim.enabled = true;
        anim.SetBool("Close", true);
    }
    private void Start() {
        anim = worldMap.GetComponent<Animator>();
    }
}
