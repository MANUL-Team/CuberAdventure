using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlContentSize : MonoBehaviour
{
    private RectTransform rt;
    private void Start() {
        rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, -transform.GetChild(transform.childCount-1).position.y);
    }
}
