using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private DialogManager dialog;
    [SerializeField] private bool cutScene;
    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            if(!cutScene){
                button.SetActive(true);
            }
            else{
                dialog.OpenDialog();
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            if(!cutScene){
                button.SetActive(false);
            }
            else{
                gameObject.SetActive(false);
            }
        }
    }
    private void FixedUpdate() {
        if(dialog.gameObject.activeSelf == false){
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
