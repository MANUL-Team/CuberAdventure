using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintableText : MonoBehaviour
{
    [SerializeField] private GameObject button, skip;
    [SerializeField] private Text text;
    public float speedOfPrint = 0.02f;
    private string words;
    public void PrintText(string strText){
        button.SetActive(false);
        skip.SetActive(true);
        words = strText;
        speedOfPrint = 0.02f;
        text.text = "";
        StartCoroutine("Print");
    }
    private IEnumerator Print(){
        foreach(char abc in words){
            text.text += abc;
            if(text.text == words && button != null){
                button.SetActive(true);
                skip.SetActive(false);
                StopCoroutine(Print());
            }
            yield return new WaitForSeconds(speedOfPrint);
        }
    }
}
