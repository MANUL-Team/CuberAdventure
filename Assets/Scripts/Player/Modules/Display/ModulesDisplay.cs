using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulesDisplay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tracks, laser1, laser2, turbine1, turbine2;
    [SerializeField] private Transform skins;
    [SerializeField] private ModulesController controller;

    private void FixedUpdate()
    {
        tracks.sprite = controller._tracks.sprite;
        laser1.sprite = controller._laser.sprite;
        laser2.sprite = controller._laser.sprite;
        turbine1.sprite = controller._turbine.sprite;
        turbine2.sprite = controller._turbine.sprite;

        int rotation = PlayerPrefs.GetInt("PlayerRotation");
        bool faceRight = rotation == 1;
        bool faceLeft = rotation == -1;
        if (!faceRight && !faceLeft)
            return;

        tracks.flipX = faceLeft;

        // Y-rotation on Skins turns child SpriteRenderers away from the camera.
        // Keep skins mirrored with Y180 for the body, but counter-rotate module sprites.
        if (faceRight)
        {
            skins.localRotation = Quaternion.identity;
            BillboardModules(false);
        }
        else
        {
            skins.localRotation = Quaternion.Euler(0f, 180f, 0f);
            BillboardModules(true);
        }
    }

    void BillboardModules(bool mirrored)
    {
        Quaternion fix = mirrored ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity;
        SetLocalRotation(laser1, fix);
        SetLocalRotation(laser2, fix);
        SetLocalRotation(turbine1, fix);
        SetLocalRotation(turbine2, fix);
    }

    static void SetLocalRotation(SpriteRenderer renderer, Quaternion rotation)
    {
        if (renderer == null)
            return;
        renderer.transform.localRotation = rotation;
        renderer.flipX = false;
    }
}
