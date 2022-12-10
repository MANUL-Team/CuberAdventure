using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyDestroy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player")){
            Money.Coin += 1;
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", Money.Coin) + 1);
            Destroy(gameObject);
        }
    }
}
