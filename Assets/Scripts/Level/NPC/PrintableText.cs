using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintableText : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private Text text;
    private string words;
    public void PrintText(string strText){
        button.SetActive(false);
        words = strText;
        text.text = "";
        StartCoroutine("Print");
    }
    private IEnumerator Print(){
        foreach(char abc in words){
            text.text += abc;
            if(text.text == words && button != null){
                button.SetActive(true);
                StopCoroutine(Print());
            }
            yield return new WaitForSeconds(0.02f);
        }
    }
    

}
