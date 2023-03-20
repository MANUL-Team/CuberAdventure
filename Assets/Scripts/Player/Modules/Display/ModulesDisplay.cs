using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulesDisplay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tracks, laser1, laser2, turbine1, turbine2;
    [SerializeField] private ModulesController controller;
    private void FixedUpdate() {
        tracks.sprite = controller._tracks.sprite;
        laser1.sprite = controller._laser.sprite;
        laser2.sprite = controller._laser.sprite;
        turbine1.sprite = controller._turbine.sprite;
        turbine2.sprite = controller._turbine.sprite;
    }
}
