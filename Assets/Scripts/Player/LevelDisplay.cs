using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    private Text text;
    private int lang;

    private void Start() {
        text = GetComponent<Text>();
        lang = PlayerPrefs.GetInt("Language");
        StartCoroutine(CheckLevel());
    }
    IEnumerator CheckLevel()
    {
        while (true)
        {
            switch (lang)
            {
                case 0:
                    text.text = "Уровень: " + PlayerPrefs.GetInt("PlayerLevel");
                    break;
                default:
                    text.text = "Level: " + PlayerPrefs.GetInt("PlayerLevel");
                    break;
            }
            yield return new WaitForSeconds(1);
        }
    }
}
