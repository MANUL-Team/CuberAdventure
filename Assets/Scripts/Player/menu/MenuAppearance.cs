using UnityEngine;
using UnityEngine.UI;

public static class MenuAppearance
{
    static readonly Color Ink = new Color(0.12f, 0.09f, 0.07f, 1f);
    static readonly Color Panel = new Color(0.20f, 0.16f, 0.12f, 1f);
    static readonly Color Card = new Color(0.29f, 0.23f, 0.17f, 1f);
    static readonly Color Cream = new Color(0.96f, 0.90f, 0.78f, 1f);
    static readonly Color Orange = new Color(0.89f, 0.48f, 0.22f, 1f);
    static readonly Color Cyan = new Color(0.22f, 0.72f, 0.70f, 1f);
    static readonly Color Muted = new Color(0.38f, 0.31f, 0.24f, 1f);
    static readonly Color Road = new Color(0.89f, 0.64f, 0.32f, 1f);

    public static void Apply(GameObject inventory, GameObject skillsMenu)
    {
        if (inventory != null)
            StyleInventory(inventory.transform);
        if (skillsMenu != null)
            StyleSkills(skillsMenu.transform);
    }

    static void StyleInventory(Transform root)
    {
        SetImage(Find(root, "BGInv"), Ink);
        SetImage(Find(root, "Inv"), Panel);
        AddShadow(Find(root, "Inv"), new Color(0.05f, 0.03f, 0.02f, 0.65f), new Vector2(4f, -4f));
        SetImage(Find(root, "AllScroll"), Ink);
        SetImage(Find(root, "ScrollBG"), new Color(0.16f, 0.12f, 0.09f, 0.55f));

        Transform scroll = Find(root, "Scroll View");
        if (scroll != null)
            SetImage(scroll, new Color(1f, 0.94f, 0.82f, 0.04f));

        StyleButton(Find(root, "Back"), Muted, Cream);
        StylePotionButtons(Find(root, "PotionDescriptions"));
        StyleFilters(root);
        StyleItemSlots(root);
        TintTexts(Find(root, "MainDescription"), Cream);
    }

    static void StyleFilters(Transform root)
    {
        string[] tabs = { "AllButton", "MaterealsButton", "CraftedButton", "PosionsButton" };
        for (int i = 0; i < tabs.Length; i++)
        {
            Transform tab = Find(root, tabs[i]);
            if (tab == null)
                continue;
            SetImage(tab, Orange);
            Transform overlay = Find(tab, tabs[i] + "Clickable");
            SetImage(overlay, new Color(0.22f, 0.17f, 0.13f, 0.94f));
            TintTexts(tab, Cream);
            Text label = tab.GetComponentInChildren<Text>(true);
            if (label != null)
                label.transform.SetAsLastSibling();
        }
    }

    static void StylePotionButtons(Transform potions)
    {
        if (potions == null)
            return;
        for (int i = 0; i < potions.childCount; i++)
        {
            Transform potion = potions.GetChild(i);
            TintTexts(potion, Cream);
            StyleButton(Find(potion, "Use"), Cyan, Ink);
            StyleButton(Find(potion, "Used"), Muted, Cream);
        }
    }

    static void StyleItemSlots(Transform root)
    {
        Item[] items = root.GetComponentsInChildren<Item>(true);
        for (int i = 0; i < items.Length; i++)
        {
            Image frame = items[i].GetComponent<Image>();
            if (frame != null)
                frame.color = new Color(1f, 0.96f, 0.88f, 1f);
            Outline outline = items[i].GetComponent<Outline>();
            if (outline == null)
                outline = items[i].gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.35f, 0.16f, 0.05f, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);
        }
    }

    static void StyleSkills(Transform root)
    {
        SetImage(Find(root, "DarkBG"), Panel);
        AddShadow(Find(root, "DarkBG"), new Color(0.05f, 0.03f, 0.02f, 0.7f), new Vector2(5f, -5f));
        SetImage(Find(root, "Description"), Card);
        StyleButton(Find(root, "Buy"), Orange, Ink);
        StyleButton(Find(root, "Use"), Cyan, Ink);
        StyleButton(Find(root, "Back"), Muted, Cream);
        TintTexts(Find(root, "SkillPoints"), Cream);
        TintTexts(Find(root, "YouNeed"), new Color(0.95f, 0.78f, 0.42f, 1f));
        TintTexts(Find(root, "Description"), Cream);

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name.IndexOf("скролл", System.StringComparison.OrdinalIgnoreCase) >= 0
                || child.name.IndexOf("Рамк", System.StringComparison.Ordinal) >= 0)
                SetImage(child, new Color(0.55f, 0.40f, 0.24f, 1f));
            if (child.name == "Scroll View")
                SetImage(child, new Color(0.08f, 0.06f, 0.05f, 0.28f));
        }

        FitSkillTree(root);
        StyleRoads(Find(root, "Roads"));
    }

    static void FitSkillTree(Transform root)
    {
        Transform node = Find(root, "Skill0");
        if (node == null)
            return;
        RectTransform content = node.parent as RectTransform;
        if (content == null)
            return;
        // Horizontal anchors are stretched, so sizeDelta.x is extra width, not the tree size.
        content.sizeDelta = new Vector2(content.sizeDelta.x, 360f);
        content.anchoredPosition = new Vector2(-96f, content.anchoredPosition.y);
    }

    static void StyleRoads(Transform roads)
    {
        if (roads == null)
            return;
        for (int i = 0; i < roads.childCount; i++)
        {
            Transform road = roads.GetChild(i);
            RectTransform rect = road as RectTransform;
            if (rect != null)
                rect.sizeDelta = new Vector2(12f, 72f);
            Image image = road.GetComponent<Image>();
            if (image != null)
            {
                image.color = Road;
                image.raycastTarget = false;
            }
            float angle = (road.name == "Road1" || road.name == "Road4") ? -45f : 45f;
            road.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    static void StyleButton(Transform button, Color fill, Color label)
    {
        SetImage(button, fill);
        TintTexts(button, label);
    }

    static void AddShadow(Transform target, Color color, Vector2 distance)
    {
        if (target == null)
            return;
        Shadow shadow = target.GetComponent<Shadow>();
        if (shadow == null)
            shadow = target.gameObject.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    static void SetImage(Transform target, Color color)
    {
        if (target == null)
            return;
        Image image = target.GetComponent<Image>();
        if (image != null)
            image.color = color;
    }

    static void TintTexts(Transform target, Color color)
    {
        if (target == null)
            return;
        Text[] texts = target.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
            texts[i].color = color;
    }

    static Transform Find(Transform root, string name)
    {
        if (root == null)
            return null;
        if (root.name == name)
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = Find(root.GetChild(i), name);
            if (found != null)
                return found;
        }
        return null;
    }
}
