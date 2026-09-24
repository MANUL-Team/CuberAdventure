using UnityEngine;

public static class EntitySorting
{
    public const string NpcLayer = "NPC";
    public const string BossLayer = "Boss";

    public static void Apply(GameObject root, string sortingLayerName)
    {
        if (root == null)
            return;

        // Keep the sprite's own lit material so 2D lights still shade it.
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
                renderer.sortingOrder += 50;
            renderer.enabled = true;
        }
    }
}
