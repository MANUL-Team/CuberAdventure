#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.Animation;

public static class FixSpiderDisplay
{
    const string PrefKey = "CuberAdventure.SpidersFixed.v7";

    [InitializeOnLoadMethod]
    static void AutoFix()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            if (EditorPrefs.GetBool(PrefKey, false))
                return;
            try
            {
                FixAll();
                EditorPrefs.SetBool(PrefKey, true);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        };
    }

    [MenuItem("Tools/CuberAdventure/Fix Spider Display")]
    public static void FixAll()
    {
        FixExpanded(
            "Assets/Prefabs/Mobs/BlackSpider.prefab",
            "Assets/Prefabs/Mobs/Черный паук.psb",
            "Черный паук",
            "Assets/Animations/Mobs/Черный паук.controller");
        FixExpanded(
            "Assets/Prefabs/Mobs/RedSpider.prefab",
            "Assets/Prefabs/Mobs/Красный паук.psb",
            "Красный паук",
            "Assets/Animations/Mobs/Красный паук.controller");
        FixMaterialsOnly("Assets/Prefabs/Mobs/MiniBossSpider.prefab");
        FixMaterialsOnly("Assets/Prefabs/Mobs/Паук-матка.prefab");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("CuberAdventure: spider display fix applied.");
    }

    [MenuItem("Tools/CuberAdventure/Reset Spider Fix Flag")]
    public static void ResetFlag()
    {
        EditorPrefs.DeleteKey(PrefKey);
        Debug.Log("Spider fix flag cleared. Reimport/recompile to run again, or use Fix Spider Display.");
    }

    static void FixExpanded(string prefabPath, string psbPath, string visualName, string controllerPath)
    {
        AssetDatabase.ImportAsset(psbPath, ImportAssetOptions.ForceUpdate);
        GameObject psbRoot = FindPsbPrefabRoot(psbPath);
        if (psbRoot == null)
        {
            Debug.LogError("No generated prefab inside " + psbPath);
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            ApplyUnlitKeepSkins(root);

            Transform oldVisual = FindVisual(root, visualName);
            Transform parent;
            Vector3 localPos;
            Vector3 localScale;
            if (oldVisual != null)
            {
                parent = oldVisual.parent;
                localPos = oldVisual.localPosition;
                localScale = oldVisual.localScale;
                Object.DestroyImmediate(oldVisual.gameObject);
            }
            else
            {
                MobController mob = root.GetComponentInChildren<MobController>(true);
                parent = mob != null ? mob.transform : root.transform;
                localPos = new Vector3(0f, -0.5f, 0f);
                localScale = new Vector3(0.3f, 0.3f, 1f);
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(psbRoot, parent);
            instance.name = visualName;
            instance.transform.localPosition = localPos;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = localScale;
            ApplyUnlitKeepSkins(instance);

            // Pathfinding spiders should not tilt out of the XY plane.
            foreach (Pathfinding.AIPath path in root.GetComponentsInChildren<Pathfinding.AIPath>(true))
                path.enableRotation = false;

            SerializedObject mobSo = null;
            MobController mobCtrl = root.GetComponentInChildren<MobController>(true);
            if (mobCtrl != null)
            {
                mobSo = new SerializedObject(mobCtrl);
                SerializedProperty face = mobSo.FindProperty("faceAngleOffset");
                if (face != null)
                    face.floatValue = 90f;
                mobSo.ApplyModifiedPropertiesWithoutUndo();
            }

            WireMobRefs(root, instance, controllerPath);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Debug.Log("Rebuilt visual from PSB for " + prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void WireMobRefs(GameObject root, GameObject visual, string controllerPath)
    {
        MobController controller = root.GetComponentInChildren<MobController>(true);
        if (controller == null)
            return;

        // Animator must live on the bone root so clip paths (Нога1Н, Голова, ...) resolve.
        Animator onMob = controller.GetComponent<Animator>();
        if (onMob != null)
            Object.DestroyImmediate(onMob);

        Animator animator = visual.GetComponent<Animator>();
        if (animator == null)
            animator = visual.AddComponent<Animator>();
        RuntimeAnimatorController rac = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
        if (rac != null)
            animator.runtimeAnimatorController = rac;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.updateMode = AnimatorUpdateMode.Normal;

        SerializedObject so = new SerializedObject(controller);
        SerializedProperty mobObj = so.FindProperty("mobObj");
        if (mobObj != null)
            mobObj.objectReferenceValue = visual;
        SerializedProperty anim = so.FindProperty("anim");
        if (anim != null)
            anim.objectReferenceValue = animator;
        so.ApplyModifiedPropertiesWithoutUndo();

        foreach (MobCanvasPos canvasPos in root.GetComponentsInChildren<MobCanvasPos>(true))
        {
            SerializedObject canvasSo = new SerializedObject(canvasPos);
            SerializedProperty target = canvasSo.FindProperty("target");
            if (target != null)
                target.objectReferenceValue = controller.transform;
            canvasSo.ApplyModifiedPropertiesWithoutUndo();
        }

        // Health bar fill should be red (white tint makes the bar sprite look yellow).
        SerializedProperty healthBar = so.FindProperty("healthBar");
        if (healthBar != null && healthBar.objectReferenceValue is UnityEngine.UI.Image img)
        {
            img.color = Color.red;
            EditorUtility.SetDirty(img);
        }

        // Also tint any Image named Bar under bars.
        if (controller.bars != null)
        {
            foreach (UnityEngine.UI.Image image in controller.bars.GetComponentsInChildren<UnityEngine.UI.Image>(true))
            {
                if (image.gameObject.name == "Bar")
                {
                    image.color = Color.red;
                    EditorUtility.SetDirty(image);
                }
            }
        }
    }

    static void FixMaterialsOnly(string prefabPath)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            ApplyUnlitKeepSkins(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static GameObject FindPsbPrefabRoot(string psbPath)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(psbPath);
        GameObject best = null;
        int bestCount = -1;
        foreach (Object asset in assets)
        {
            GameObject go = asset as GameObject;
            if (go == null)
                continue;
            int count = go.GetComponentsInChildren<SpriteRenderer>(true).Length;
            if (count > bestCount)
            {
                bestCount = count;
                best = go;
            }
        }
        return best;
    }

    static Transform FindVisual(GameObject root, string visualName)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == visualName)
                return t;
        }
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.GetComponent<MobController>() != null)
                continue;
            if (t.GetComponentsInChildren<SpriteRenderer>(true).Length >= 8)
                return t;
        }
        return null;
    }

    static void ApplyUnlitKeepSkins(GameObject root)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/MobUnlit.mat");
        foreach (SpriteRenderer renderer in root.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (mat != null)
                renderer.sharedMaterial = mat;
            renderer.color = Color.white;
            renderer.sortingLayerName = "Default";
            if (renderer.sortingOrder < 50)
                renderer.sortingOrder = 50;
            renderer.enabled = true;
        }
        foreach (SpriteSkin skin in root.GetComponentsInChildren<SpriteSkin>(true))
        {
            if (skin != null)
                skin.enabled = true;
        }
    }
}
#endif
