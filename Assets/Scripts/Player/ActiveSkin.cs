using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkin : MonoBehaviour
{
    [SerializeField] GameObject[] skins;
    private void Awake() {
        skins[PlayerPrefs.GetInt("Player")].SetActive(true);
    }
    void Start()
    {
        PlayerPrefs.SetInt("LevelEnded" + PlayerPrefs.GetInt("Level"), 1);
    }
}
