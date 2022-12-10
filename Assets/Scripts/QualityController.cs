using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityController : MonoBehaviour
{
    private void FixedUpdate() {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphics"), true);
    }
}
