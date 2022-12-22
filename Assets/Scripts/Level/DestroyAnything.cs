using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnything : MonoBehaviour
{
    public GameObject anything;
    public void Destroy(){
        anything.SetActive(false);
    }
}
