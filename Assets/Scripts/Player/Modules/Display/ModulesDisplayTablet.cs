using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModulesDisplayTablet : MonoBehaviour
{
    [SerializeField] private Image tracks, laser1, laser2, turbine1, turbine2, armor, core;
    [SerializeField] private ModulesController controller;
    private void FixedUpdate() {
        tracks.sprite = controller._tracks.sprite;
        laser1.sprite = controller._laser.sprite;
        laser2.sprite = controller._laser.sprite;
        turbine1.sprite = controller._turbine.sprite;
        turbine2.sprite = controller._turbine.sprite;
        armor.sprite = controller._armor.sprite;
        core.sprite = controller._core.sprite;
    }
}
