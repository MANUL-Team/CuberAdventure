using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private GameObject mainTutorial;
    [SerializeField] private DialogManager dialog;
    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("Tutorial " + id) == 1){
            gameObject.SetActive(false);
        }
        if(dialog.dialogEnded){
            EndTutorial();
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            mainTutorial.SetActive(true);
            if(dialog != null){
                dialog.OpenDialog();
            }
        }
    }
    public void EndTutorial(){
        mainTutorial.SetActive(false);
        PlayerPrefs.SetInt("Tutorial " + id, 1);
    }
}
