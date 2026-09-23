using UnityEngine;

public static class EntitySorting
{
    public const string NpcLayer = "NPC";
    public const string BossLayer = "Boss";
    const string UnlitResource = "MobUnlit";
    static Material unlitMaterial;

    public static void Apply(GameObject root, string sortingLayerName)
    {
        if (root == null)
            return;
        EnsureUnlit();

        // Keep SpriteSkin for bone animations. Visibility comes from Unlit + flat Z.
        int layerId = SortingLayer.NameToID("Default");
        SpriteRenderer[] renderers = root.GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            SpriteRenderer renderer = renderers[i];
            if (renderer == null)
                continue;
            renderer.sortingLayerID = layerId;
            renderer.sortingLayerName = "Default";
            if (renderer.sortingOrder < 50)
                renderer.sortingOrder = 50;
            if (unlitMaterial != null)
                renderer.sharedMaterial = unlitMaterial;
            renderer.color = Color.white;
            renderer.enabled = true;
        }
    }

    static void EnsureUnlit()
    {
        if (unlitMaterial != null)
            return;
        unlitMaterial = Resources.Load<Material>(UnlitResource);
        if (unlitMaterial == null)
            unlitMaterial = Resources.Load<Material>("AttackUnlit");
    }
}
