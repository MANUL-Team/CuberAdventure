using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWorldMap : MonoBehaviour
{
    [SerializeField] private GameObject worldMap;
    private Animator anim;

    public void OpenMap(){
        worldMap.SetActive(true);
        anim.SetBool("Close", false);
    }
    public void CloseMap(){
        anim.SetBool("Close", true);
    }
    private void Start() {
        anim = worldMap.GetComponent<Animator>();
    }
}
