using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWorldMap : MonoBehaviour
{
    [SerializeField] private GameObject worldMap;

    public void OpenMap(){
        worldMap.SetActive(!worldMap.activeSelf);
    }
}
