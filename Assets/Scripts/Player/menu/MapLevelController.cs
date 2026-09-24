using System.Collections.Generic;
using UnityEngine;

public class MapLevelController : MonoBehaviour
{
    readonly List<MapLevelChange> descriptions = new List<MapLevelChange>();
    [SerializeField] TeleportScript tp;
    bool collected;

    public void OpenDescription(int id)
    {
        Ensure();
        for (int i = 0; i < descriptions.Count; i++)
        {
            MapLevelChange card = descriptions[i];
            if (card == null)
                continue;
            bool show = card.id == id;
            card.gameObject.SetActive(show);
            if (show && tp != null)
                tp.currentId = card.id;
        }
        if (tp != null)
            tp.gameObject.SetActive(true);
    }

    void Awake()
    {
        Ensure();
    }

    void Ensure()
    {
        if (collected)
            return;
        collected = true;
        descriptions.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            MapLevelChange card = transform.GetChild(i).GetComponent<MapLevelChange>();
            if (card != null)
                descriptions.Add(card);
        }
    }
}
