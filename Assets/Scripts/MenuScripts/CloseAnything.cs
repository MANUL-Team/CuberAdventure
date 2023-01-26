using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAnything : MonoBehaviour
{
    [SerializeField] private GameObject anything;

    public void Close(){
        anything.SetActive(!anything.activeSelf);
    }
}
