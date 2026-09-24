#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class PixelWaterSetup
{
    const string MatPath = "Assets/Matereals/Water/PixelWater.mat";
    const string PrefabPath = "Assets/Prefabs/PixelWater.prefab";

    [MenuItem("Tools/CuberAdventure/Build Pixel Water Prefab")]
    public static void EnsurePrefab()
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
        if (mat == null)
        {
            Debug.LogError("PixelWater material missing at " + MatPath);
            return;
        }

        GameObject root = new GameObject("PixelWater");
        root.layer = 4;
        root.tag = "Water";

        SpriteRenderer sr = root.AddComponent<SpriteRenderer>();
        // Same white square used by existing water / walls
        Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        Sprite square = null;
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null && sprites[i].name == "Square")
            {
                square = sprites[i];
                break;
            }
        }
        sr.sprite = square;
        sr.color = Color.white;
        sr.sortingOrder = 17;
        sr.sharedMaterial = mat;

        BoxCollider2D trigger = root.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = Vector2.one;
        trigger.usedByEffector = true;

        BuoyancyEffector2D buoyancy = root.AddComponent<BuoyancyEffector2D>();
        buoyancy.density = 40f;
        buoyancy.linearDamping = 5f;
        buoyancy.angularDamping = 5f;
        buoyancy.surfaceLevel = 0.5f;

        PixelWaterBody body = root.AddComponent<PixelWaterBody>();
        SerializedObject so = new SerializedObject(body);
        SerializedProperty matProp = so.FindProperty("waterMaterial");
        if (matProp != null)
            matProp.objectReferenceValue = mat;
        so.ApplyModifiedPropertiesWithoutUndo();

        GameObject physics = new GameObject("WaterCollider");
        physics.layer = 4;
        physics.tag = "Water";
        physics.transform.SetParent(root.transform, false);
        BoxCollider2D solid = physics.AddComponent<BoxCollider2D>();
        solid.isTrigger = false;
        solid.size = Vector2.one;
        solid.usedByEffector = true;
        physics.AddComponent<WaterCollision>();

        root.transform.localScale = new Vector3(10f, 4f, 1f);

        System.IO.Directory.CreateDirectory("Assets/Prefabs");
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("PixelWater prefab saved to " + PrefabPath);
    }

    [MenuItem("Tools/CuberAdventure/Upgrade Scene Water To PixelWater")]
    public static void UpgradeOpenScenes()
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
        if (mat == null)
        {
            Debug.LogError("PixelWater material missing.");
            return;
        }

        SpriteRenderer[] renderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int count = 0;
        for (int i = 0; i < renderers.Length; i++)
        {
            SpriteRenderer sr = renderers[i];
            if (sr == null)
                continue;

            bool isWaterRoot = sr.gameObject.name == "Water" && (sr.CompareTag("Water") || sr.gameObject.layer == 4);
            if (!isWaterRoot)
            {
                // Hide duplicate visuals on WaterCollider children
                if (sr.gameObject.name == "WaterCollider" && sr.transform.parent != null && sr.transform.parent.name == "Water")
                {
                    sr.enabled = false;
                    EditorUtility.SetDirty(sr);
                }
                continue;
            }

            sr.sharedMaterial = mat;
            sr.color = Color.white;

            PixelWaterBody body = sr.GetComponent<PixelWaterBody>();
            if (body == null)
                body = sr.gameObject.AddComponent<PixelWaterBody>();

            SerializedObject so = new SerializedObject(body);
            SerializedProperty matProp = so.FindProperty("waterMaterial");
            if (matProp != null)
                matProp.objectReferenceValue = mat;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(sr.gameObject);
            count++;
        }

        Debug.Log("Upgraded " + count + " Water object(s) to PixelWater.");
    }

    [MenuItem("Tools/CuberAdventure/Build Pixel Waterfall Prefab")]
    public static void EnsureWaterfallPrefab()
    {
        const string FallMatPath = "Assets/Matereals/Water/PixelWaterfall.mat";
        const string FallPrefabPath = "Assets/Prefabs/PixelWaterfall.prefab";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(FallMatPath);
        if (mat == null)
        {
            Debug.LogError("PixelWaterfall material missing at " + FallMatPath);
            return;
        }

        GameObject root = new GameObject("PixelWaterfall");
        SpriteRenderer sr = root.AddComponent<SpriteRenderer>();
        Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        Sprite square = null;
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null && sprites[i].name == "Square")
            {
                square = sprites[i];
                break;
            }
        }
        sr.sprite = square;
        sr.color = Color.white;
        sr.sortingOrder = 19;
        sr.sharedMaterial = mat;

        PixelWaterfall fall = root.AddComponent<PixelWaterfall>();
        SerializedObject so = new SerializedObject(fall);
        SerializedProperty matProp = so.FindProperty("waterfallMaterial");
        if (matProp != null)
            matProp.objectReferenceValue = mat;
        so.ApplyModifiedPropertiesWithoutUndo();

        root.transform.localScale = new Vector3(8f, 20f, 1f);
        PrefabUtility.SaveAsPrefabAsset(root, FallPrefabPath);
        Object.DestroyImmediate(root);
        AssetDatabase.SaveAssets();
        Debug.Log("PixelWaterfall prefab saved to " + FallPrefabPath);
    }
}
#endif
