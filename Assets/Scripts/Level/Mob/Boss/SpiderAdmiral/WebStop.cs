using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebStop : MonoBehaviour
{
    [SerializeField] private GameObject web;
    public void Attack(){
        GameObject newWeb = Instantiate(web, transform.position, Quaternion.identity);
        Destroy(newWeb, 10f);
    }
}
