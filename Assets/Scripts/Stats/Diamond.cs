using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diamond : MonoBehaviour
{
    public static int Diamonds;
    Text textDiamond;
    void Start()
    {
        textDiamond = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Diamonds = PlayerPrefs.GetInt("Diamonds", Diamonds);
        textDiamond.text = Diamonds.ToString();
    }
}
