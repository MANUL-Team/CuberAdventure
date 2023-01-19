using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyList : MonoBehaviour
{
    private List<TakeMoney> coins = new List<TakeMoney>();

    void Awake()
    {
        for(int i = 0; i < transform.childCount; i++){
            coins.Add(transform.GetChild(i).GetComponent<TakeMoney>());
        }
        for(int i = 0; i < coins.Count; i++){
            coins[i].id = i;
        }
    }
}
