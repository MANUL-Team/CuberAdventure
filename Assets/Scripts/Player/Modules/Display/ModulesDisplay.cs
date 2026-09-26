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
        // Parent Skins uses Y180 when facing left (mirrors positions).
        // Counter with local Y180 so sprites face the camera — but also swap the
        // laser aim (Z) so barrels still point OUTWARD, not into the hull.
        float yaw = mirrored ? 180f : 0f;
        float laser1Z = mirrored ? 180f : 0f;
        float laser2Z = mirrored ? 0f : 180f;
        SetLocalRotation(laser1, Quaternion.Euler(0f, yaw, laser1Z));
        SetLocalRotation(laser2, Quaternion.Euler(0f, yaw, laser2Z));
        SetLocalRotation(turbine1, Quaternion.Euler(0f, yaw, -90f));
        SetLocalRotation(turbine2, Quaternion.Euler(0f, yaw, -90f));
        KeepLasersBehindSkin(laser1);
        KeepLasersBehindSkin(laser2);
    }

    static void KeepLasersBehindSkin(SpriteRenderer renderer)
    {
        if (renderer == null)
            return;
        renderer.sortingOrder = 15;
        Vector3 position = renderer.transform.localPosition;
        position.z = 0f;
        renderer.transform.localPosition = position;
    }

    static void SetLocalRotation(SpriteRenderer renderer, Quaternion rotation)
    {
        if (renderer == null)
            return;
        renderer.transform.localRotation = rotation;
        renderer.flipX = false;
    }
}
