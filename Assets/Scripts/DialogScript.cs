using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogScript : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject[] dialogPages;
    [SerializeField] private int dialogPage;
    [SerializeField] private bool pageNotZero;
    private Animator cam;

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            button.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            button.SetActive(false);
        }
    }
    public void OpenDialog(){
        dialogPage = 1;
        button.SetActive(false);
    }
    public void SwitchPage(){
        dialogPage++;
    }
    private void Start() {
        if(pageNotZero == false){
            dialogPage = 0;
        }
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
    }
    private void Update() {
        if(dialogPage > dialogPages.Length){
            for(int i = 0; i < dialogPages.Length; i++){
                dialogPages[i].SetActive(false);
            }
            cam.SetBool("Dialog", false);
            PlayerPrefs.SetInt("MaskD", 0);
            dialogPage = 0;
        }
        if(dialogPage != 0){
            dialogPages[dialogPage - 1].SetActive(true);
            cam.SetBool("Dialog", true);
            PlayerPrefs.SetInt("MaskD", 1);
        }
        if(dialogPage > 1){
            dialogPages[dialogPage - 2].SetActive(false);
        }
    }
}
