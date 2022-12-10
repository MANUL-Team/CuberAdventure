using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpScale : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private float exp, needExp;
    private float fill;
    private void Update() {
        exp = PlayerPrefs.GetInt("Exp");
        needExp = PlayerPrefs.GetInt("NeedExp");
        fill = exp/needExp;
        img.fillAmount = fill;
    }
}
