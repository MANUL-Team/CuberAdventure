using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCraftingItems : MonoBehaviour
{
    [SerializeField] private GameObject craftingMenu, button;

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
    public void CraftingMenu(){
        craftingMenu.SetActive(!craftingMenu.activeSelf);
    }
}
