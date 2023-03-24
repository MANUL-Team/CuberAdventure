using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveSkin : MonoBehaviour
{
    [SerializeField] Sprite[] skins;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Image spriteI;
    private void Awake() {
        if(sprite != null){
            sprite.sprite = skins[PlayerPrefs.GetInt("Player")];
        }
        else if(spriteI != null){
            spriteI.sprite = skins[PlayerPrefs.GetInt("Player")];
        }
    }
    void Start()
    {
        PlayerPrefs.SetInt("LevelEnded" + PlayerPrefs.GetInt("Level"), 1);
    }
}
