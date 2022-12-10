using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Close : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    public void CloseVoid(){
        gameObject.SetActive(false);
    }
}
