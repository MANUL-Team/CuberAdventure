using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllKeeped : MonoBehaviour
{
    [SerializeField] private CraftedItem item;
    [SerializeField] private DialogChange main;
    [SerializeField] private bool last;
    [SerializeField] private int id, step;

    // private void FixedUpdate() {
    //     if(PlayerPrefs.GetInt(item.module + item.id.ToString()) == 1){
    //         main.StartNewMissionStep(id, step, last);
    //     }
    // }
}
