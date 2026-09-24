#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class FixSpiderDisplay
{
    // The earlier auto-fix swapped the rigged spider for a flat PSB instance.
    // Walk and attack clips bind to Нога* / bone_* under the old visual, so that
    // replacement left the spiders visible but frozen. Do not rebuild the visual.

    [MenuItem("Tools/CuberAdventure/Fix Spider Display")]
    public static void FixAll()
    {
        Debug.Log("CuberAdventure: spider rig is left intact. Visibility is applied at runtime by EntitySorting.");
    }
}
#endif
