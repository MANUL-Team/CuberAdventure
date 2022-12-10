using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{

    public Text scoreText;
    public float points;

    private void Update() {
        points++;

        scoreText.text = "Score: " + points.ToString();
    }

}
