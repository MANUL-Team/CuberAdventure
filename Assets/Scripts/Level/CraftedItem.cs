using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftedItem : MonoBehaviour
{
    public string module;
    public int id;
    public string nameRu;
    [TextArea]
    public string descriptionRu, statsRu, commentRu;
    public string nameEng;
    [TextArea]
    public string descriptionEng, statsEng, commentEng;
    public Sprite icon;
}
