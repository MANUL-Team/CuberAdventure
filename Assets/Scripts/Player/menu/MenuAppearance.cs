using UnityEngine;
using UnityEngine.UI;

public static class MenuAppearance
{
    static readonly Color Ink = new Color(0.06f, 0.045f, 0.035f, 0.96f);
    static readonly Color Sheet = new Color(0.14f, 0.10f, 0.08f, 1f);
    static readonly Color Panel = new Color(0.20f, 0.15f, 0.11f, 1f);
    static readonly Color Card = new Color(0.28f, 0.21f, 0.15f, 1f);
    static readonly Color Slot = new Color(0.36f, 0.26f, 0.16f, 1f);
    static readonly Color Cream = new Color(0.96f, 0.90f, 0.78f, 1f);
    static readonly Color Orange = new Color(0.89f, 0.48f, 0.22f, 1f);
    static readonly Color Cyan = new Color(0.22f, 0.72f, 0.70f, 1f);
    static readonly Color Muted = new Color(0.33f, 0.26f, 0.20f, 1f);
    static readonly Color Road = new Color(0.93f, 0.62f, 0.28f, 1f);
    static readonly Color Plate = new Color(0.42f, 0.26f, 0.14f, 1f);
    static readonly Color PlateLocked = new Color(0.16f, 0.12f, 0.10f, 1f);
    static readonly Color PlateOn = new Color(0.16f, 0.55f, 0.52f, 1f);

    const float SideWidth = 292f;
    const float Margin = 18f;
    const float Gap = 14f;
    const float NodeSize = 86f;
    const float NodeReach = 124f;
    static Sprite panelSprite;
    static Sprite glowSprite;
    static Sprite ringSprite;
    static Sprite sparkSprite;
    static Sprite vignetteSprite;

    public static void Apply(GameObject inventory, GameObject skillsMenu)
    {
        if (inventory != null)
        {
            BuildInventory(inventory.transform);
            Attach(inventory, false);
        }
        if (skillsMenu != null)
        {
            BuildSkills(skillsMenu.transform);
            Attach(skillsMenu, true);
        }
    }

    public static void AlignInventory(Transform root)
    {
        if (root == null)
            return;
        PlaceColumns(root);
        LayoutItemBrowser(Find(root, "AllScroll"));
        LayoutSideColumn(Find(root, "Inv") as RectTransform);
        StyleFilters(root);
    }

    public static void AlignSkills(Transform root)
    {
        if (root == null)
            return;
        LayoutSkillColumn(Find(root, "DarkBG") as RectTransform);
        DressSkillNodes(root);
        PlaceSkillTree(root);
        StyleRoads(Find(root, "Roads"));
        ClearSkillPlates(Find(root, "Skill0"));
        EnsureStage(root);
        PlaceSkillStage(root);
    }

    public static RectTransform FindRect(Transform root, string name)
    {
        return Find(root, name) as RectTransform;
    }

    static void Attach(GameObject menu, bool skillsMenu)
    {
        MenuMotion motion = menu.GetComponent<MenuMotion>();
        bool created = motion == null;
        if (created)
            motion = menu.AddComponent<MenuMotion>();
        motion.skills = skillsMenu;
        if (!created && menu.activeInHierarchy)
            motion.Begin();
    }

    static void BuildInventory(Transform root)
    {
        SetImage(Find(root, "BGInv"), Ink);
        PlaceColumns(root);

        RectTransform inv = Find(root, "Inv") as RectTransform;
        RectTransform all = Find(root, "All") as RectTransform;
        SetImage(all, Sheet);
        SetImage(inv, Panel);
        AddShadow(inv, new Color(0f, 0f, 0f, 0.45f), new Vector2(-6f, 0f));

        LayoutItemBrowser(Find(root, "AllScroll"));
        LayoutSideColumn(inv);
        StyleFilters(root);
        StyleItemSlots(root);
        StyleButton(Find(root, "Back"), Muted, Cream);
    }

    static void LayoutItemBrowser(Transform allScroll)
    {
        if (allScroll == null)
            return;
        SetImage(allScroll, Sheet);

        RectTransform filters = null;
        RectTransform items = null;
        for (int i = 0; i < allScroll.childCount; i++)
        {
            Transform child = allScroll.GetChild(i);
            if (child.name == "Scroll View")
                filters = child as RectTransform;
            else if (child.name == "Viewport")
                items = child as RectTransform;
        }

        if (filters != null)
        {
            filters.anchorMin = new Vector2(0f, 1f);
            filters.anchorMax = new Vector2(1f, 1f);
            filters.pivot = new Vector2(0.5f, 1f);
            filters.sizeDelta = new Vector2(-24f, 52f);
            filters.anchoredPosition = new Vector2(0f, -12f);

            ScrollRect scroll = filters.GetComponent<ScrollRect>();
            if (scroll != null)
            {
                scroll.horizontal = false;
                scroll.vertical = false;
                scroll.movementType = ScrollRect.MovementType.Clamped;
            }

            Transform content = filters.Find("Viewport/Content");
            if (content is RectTransform tabBar)
            {
                tabBar.anchorMin = Vector2.zero;
                tabBar.anchorMax = Vector2.one;
                tabBar.offsetMin = Vector2.zero;
                tabBar.offsetMax = Vector2.zero;
                tabBar.pivot = new Vector2(0.5f, 0.5f);

                ContentSizeFitter fitter = tabBar.GetComponent<ContentSizeFitter>();
                if (fitter != null)
                    fitter.enabled = false;

                HorizontalLayoutGroup row = tabBar.GetComponent<HorizontalLayoutGroup>();
                if (row == null)
                    row = tabBar.gameObject.AddComponent<HorizontalLayoutGroup>();
                row.padding = new RectOffset(4, 4, 4, 4);
                row.spacing = 8f;
                row.childAlignment = TextAnchor.MiddleCenter;
                row.childControlWidth = true;
                row.childControlHeight = true;
                row.childForceExpandWidth = true;
                row.childForceExpandHeight = true;
            }

            Transform wash = filters.Find("ScrollBG");
            if (wash != null)
            {
                Image washImage = wash.GetComponent<Image>();
                if (washImage != null)
                    washImage.color = new Color(0f, 0f, 0f, 0f);
            }
        }

        if (items != null)
        {
            items.anchorMin = Vector2.zero;
            items.anchorMax = Vector2.one;
            items.offsetMin = new Vector2(12f, 12f);
            items.offsetMax = new Vector2(-12f, -72f);
            FitItemGrid(items);
        }
    }

    static void PlaceColumns(Transform root)
    {
        RectTransform inv = Find(root, "Inv") as RectTransform;
        RectTransform all = Find(root, "All") as RectTransform;
        if (inv != null)
        {
            inv.anchorMin = new Vector2(1f, 0f);
            inv.anchorMax = new Vector2(1f, 1f);
            inv.pivot = new Vector2(1f, 0.5f);
            inv.anchoredPosition = new Vector2(-Margin, 0f);
            inv.sizeDelta = new Vector2(SideWidth, -(Margin * 2f));
        }
        if (all != null)
        {
            all.anchorMin = Vector2.zero;
            all.anchorMax = Vector2.one;
            all.pivot = new Vector2(0.5f, 0.5f);
            all.offsetMin = new Vector2(Margin, Margin);
            all.offsetMax = new Vector2(-(Margin + Gap + SideWidth), -Margin);
        }
    }

    static void FitItemGrid(RectTransform viewport)
    {
        Transform content = viewport.Find("Content");
        GridLayoutGroup grid = content != null ? content.GetComponent<GridLayoutGroup>() : null;
        if (grid == null)
            return;

        Canvas.ForceUpdateCanvases();
        float width = viewport.rect.width;
        if (width < 80f)
            return;

        float spacing = 10f;
        float inner = width - 8f;
        int columns = Mathf.Clamp(Mathf.FloorToInt((inner + spacing) / 86f), 2, 6);
        float cell = Mathf.Floor((inner - spacing * (columns - 1)) / columns);
        float row = columns * cell + spacing * (columns - 1);
        int side = Mathf.Max(0, Mathf.RoundToInt((width - row) * 0.5f));

        grid.cellSize = new Vector2(cell, cell);
        grid.spacing = new Vector2(spacing, spacing);
        grid.padding = new RectOffset(side, side, 12, 16);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
    }

    static void LayoutSideColumn(RectTransform inv)
    {
        if (inv == null)
            return;

        Font font = null;
        Text sample = inv.GetComponentInChildren<Text>(true);
        if (sample != null)
            font = sample.font;

        Transform existing = inv.Find("InvTitle");
        Text title = existing != null ? existing.GetComponent<Text>() : null;
        if (title == null && font != null)
            title = CreateLabel(inv, "InvTitle", font, 22, Cream);
        float height = inv.rect.height;
        if (height < 120f)
            height = 640f;
        const float pad = 16f;
        const float space = 10f;
        const float titleH = 36f;
        const float backH = 44f;
        float descH = Mathf.Clamp(height * 0.26f, 120f, 188f);
        float cursor = pad;

        if (title != null)
        {
            bool russian = PlayerPrefs.GetInt("Language") == 1;
            title.text = russian ? "Инвентарь" : "Inventory";
            title.alignment = TextAnchor.MiddleLeft;
            title.fontSize = 22;
            title.color = Cream;
            DockTop(title.rectTransform, cursor, pad, titleH);
            cursor += titleH + space;
        }

        RectTransform card = Find(inv, "MainDescription") as RectTransform;
        if (card != null)
        {
            DockTop(card, cursor, pad, descH);
            cursor += descH + space;
            SetImage(card, Card);
            Text heading = card.GetComponent<Text>();
            float headingBand = 8f;
            if (heading != null)
            {
                heading.color = Cream;
                heading.alignment = TextAnchor.UpperLeft;
                heading.horizontalOverflow = HorizontalWrapMode.Wrap;
                heading.verticalOverflow = VerticalWrapMode.Overflow;
                heading.fontSize = 18;
                headingBand = 34f;
            }
            Transform body = card.childCount > 0 ? card.GetChild(0) : null;
            if (body is RectTransform bodyRect)
            {
                bodyRect.anchorMin = Vector2.zero;
                bodyRect.anchorMax = Vector2.one;
                bodyRect.offsetMin = new Vector2(12f, 10f);
                bodyRect.offsetMax = new Vector2(-12f, -headingBand);
            }
            if (body != null)
            {
                Text[] lines = body.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i].color = Cream;
                    lines[i].alignment = TextAnchor.UpperLeft;
                    lines[i].horizontalOverflow = HorizontalWrapMode.Wrap;
                    lines[i].verticalOverflow = VerticalWrapMode.Overflow;
                    lines[i].fontSize = 15;
                }
            }
        }

        RectTransform potions = Find(inv, "PotionDescriptions") as RectTransform;
        if (potions != null)
        {
            potions.anchorMin = Vector2.zero;
            potions.anchorMax = Vector2.one;
            potions.pivot = new Vector2(0.5f, 0.5f);
            potions.offsetMin = new Vector2(pad, pad + backH + space);
            potions.offsetMax = new Vector2(-pad, -cursor);

            for (int i = 0; i < potions.childCount; i++)
            {
                if (potions.GetChild(i) is RectTransform potion)
                    Fill(potion);
                LayoutPotionCard(potions.GetChild(i));
            }
        }

        RectTransform back = Find(inv, "Back") as RectTransform;
        if (back != null)
            DockBottom(back, pad, pad, backH);
    }

    static void LayoutPotionCard(Transform potion)
    {
        if (potion == null)
            return;
        TintTexts(potion, Cream);
        for (int i = 0; i < potion.childCount; i++)
        {
            if (potion.GetChild(i) is RectTransform block)
            {
                block.anchorMin = Vector2.zero;
                block.anchorMax = Vector2.one;
                block.offsetMin = new Vector2(0f, 58f);
                block.offsetMax = new Vector2(0f, -4f);
            }
        }
        DockBottom(Find(potion, "Use") as RectTransform, 8f, 0f, 44f);
        DockBottom(Find(potion, "Used") as RectTransform, 8f, 0f, 44f);
        StyleButton(Find(potion, "Use"), Cyan, Ink);
        StyleButton(Find(potion, "Used"), Muted, Cream);
        Text[] lines = potion.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].transform.parent == potion)
            {
                lines[i].alignment = TextAnchor.UpperCenter;
                lines[i].horizontalOverflow = HorizontalWrapMode.Wrap;
                lines[i].verticalOverflow = VerticalWrapMode.Overflow;
                lines[i].fontSize = 16;
            }
        }
    }

    static void StyleFilters(Transform root)
    {
        string[] tabs = { "AllButton", "MaterealsButton", "CraftedButton", "PosionsButton" };
        Sprite buttonSprite = null;
        Transform back = Find(root, "Back");
        if (back != null)
        {
            Image backImage = back.GetComponent<Image>();
            if (backImage != null)
                buttonSprite = backImage.sprite;
        }

        for (int i = 0; i < tabs.Length; i++)
        {
            Transform tab = Find(root, tabs[i]);
            if (tab == null)
                continue;
            Image image = tab.GetComponent<Image>();
            if (image != null)
            {
                if (buttonSprite != null)
                    image.sprite = buttonSprite;
                image.type = Image.Type.Simple;
                image.color = Orange;
            }
            Transform overlay = Find(tab, tabs[i] + "Clickable");
            if (overlay != null)
            {
                if (overlay is RectTransform overlayRect)
                    Fill(overlayRect);
                Image overlayImage = overlay.GetComponent<Image>();
                if (overlayImage != null)
                {
                    if (buttonSprite != null)
                        overlayImage.sprite = buttonSprite;
                    overlayImage.color = new Color(0.18f, 0.13f, 0.10f, 0.94f);
                }
            }
            Text[] labels = tab.GetComponentsInChildren<Text>(true);
            for (int n = 0; n < labels.Length; n++)
            {
                RectTransform labelRect = labels[n].rectTransform;
                labelRect.localScale = Vector3.one;
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.pivot = new Vector2(0.5f, 0.5f);
                labelRect.offsetMin = new Vector2(8f, 0f);
                labelRect.offsetMax = new Vector2(-8f, 0f);
                labels[n].color = Cream;
                labels[n].alignment = TextAnchor.MiddleCenter;
                labels[n].alignByGeometry = true;
                labels[n].horizontalOverflow = HorizontalWrapMode.Overflow;
                labels[n].verticalOverflow = VerticalWrapMode.Overflow;
                labels[n].resizeTextForBestFit = true;
                labels[n].resizeTextMinSize = 10;
                labels[n].resizeTextMaxSize = 15;
                labels[n].raycastTarget = false;
                labels[n].transform.SetAsLastSibling();
            }
        }
    }

    static void StyleItemSlots(Transform root)
    {
        Item[] items = root.GetComponentsInChildren<Item>(true);
        for (int i = 0; i < items.Length; i++)
        {
            Image frame = items[i].GetComponent<Image>();
            if (frame != null)
                frame.color = Slot;

            if (items[i].transform.childCount > 0)
            {
                Transform icon = items[i].transform.GetChild(0);
                if (icon is RectTransform iconRect)
                {
                    iconRect.anchorMin = Vector2.zero;
                    iconRect.anchorMax = Vector2.one;
                    iconRect.offsetMin = new Vector2(10f, 14f);
                    iconRect.offsetMax = new Vector2(-10f, -8f);
                }
                Image iconImage = icon.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.color = Color.white;
                    iconImage.preserveAspect = true;
                    iconImage.raycastTarget = false;
                }
            }

            Text count = items[i].GetComponentInChildren<Text>(true);
            if (count != null)
            {
                RectTransform countRect = count.rectTransform;
                countRect.localScale = Vector3.one;
                countRect.anchorMin = new Vector2(1f, 0f);
                countRect.anchorMax = new Vector2(1f, 0f);
                countRect.pivot = new Vector2(1f, 0f);
                countRect.sizeDelta = new Vector2(40f, 18f);
                countRect.anchoredPosition = new Vector2(-4f, 3f);
                count.fontSize = 14;
                count.alignment = TextAnchor.LowerRight;
                count.horizontalOverflow = HorizontalWrapMode.Overflow;
                count.verticalOverflow = VerticalWrapMode.Overflow;
                count.color = Cream;
                count.raycastTarget = false;
                Outline outline = count.GetComponent<Outline>();
                if (outline != null)
                    outline.effectDistance = new Vector2(1f, -1f);
            }
        }
    }

    static void BuildSkills(Transform root)
    {
        CoverSkillScreen(root);
        AddShadow(Find(root, "DarkBG"), new Color(0f, 0f, 0f, 0.45f), new Vector2(-8f, 0f));

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == "Scroll View")
                SetImage(child, new Color(0.05f, 0.04f, 0.03f, 0.18f));
        }

        LayoutSkillColumn(Find(root, "DarkBG") as RectTransform);
        DressSkillNodes(root);
        PlaceSkillTree(root);
        StyleRoads(Find(root, "Roads"));
        ClearSkillPlates(Find(root, "Skill0"));
        BuildSkillStage(root);
    }

    static void LayoutSkillColumn(RectTransform column)
    {
        if (column == null)
            return;

        ShapeSkillColumn(column);
        Canvas.ForceUpdateCanvases();
        float height = column.rect.height;
        if (height < 160f)
            height = 640f;
        const float pad = 20f;
        const float space = 12f;
        const float headerH = 46f;
        const float needH = 44f;
        const float buttonH = 46f;
        float used = pad * 2f + headerH + needH + buttonH * 2f + space * 4f;
        float descH = Mathf.Max(110f, height - used);

        RectTransform points = Find(column, "SkillPoints") as RectTransform;
        DockTop(points, pad, pad, headerH);
        TintTexts(points, Cream);
        Text pointsText = points != null ? points.GetComponent<Text>() : null;
        if (pointsText == null && points != null)
            pointsText = points.GetComponentInChildren<Text>(true);
        if (pointsText != null)
        {
            pointsText.alignment = TextAnchor.MiddleCenter;
            pointsText.fontSize = 18;
            pointsText.color = Cream;
        }

        RectTransform description = Find(column, "Description") as RectTransform;
        DockTop(description, pad + headerH + space, pad, descH);
        Round(description, new Color(0.14f, 0.125f, 0.15f, 1f));
        TintTexts(description, Cream);
        Text[] body = description != null ? description.GetComponentsInChildren<Text>(true) : new Text[0];
        for (int i = 0; i < body.Length; i++)
        {
            body[i].alignment = TextAnchor.UpperLeft;
            body[i].horizontalOverflow = HorizontalWrapMode.Wrap;
            body[i].verticalOverflow = VerticalWrapMode.Overflow;
            body[i].fontSize = 16;
            body[i].color = Cream;
        }

        RectTransform need = Find(column, "YouNeed") as RectTransform;
        DockTop(need, pad + headerH + space + descH + space, pad, needH);
        TintTexts(need, new Color(0.95f, 0.78f, 0.42f, 1f));
        Text needText = need != null ? need.GetComponent<Text>() : null;
        if (needText == null && need != null)
            needText = need.GetComponentInChildren<Text>(true);
        if (needText != null)
        {
            needText.alignment = TextAnchor.MiddleCenter;
            needText.fontSize = 14;
            needText.horizontalOverflow = HorizontalWrapMode.Wrap;
        }

        DockBottom(Find(column, "Back") as RectTransform, pad, pad, buttonH);
        DockBottom(Find(column, "Use") as RectTransform, pad + buttonH + space, pad, buttonH);
        DockBottom(Find(column, "Buy") as RectTransform, pad + buttonH + space, pad, buttonH);
        StyleButton(Find(column, "Buy"), Orange, Ink);
        StyleButton(Find(column, "Use"), Cyan, Ink);
        StyleButton(Find(column, "Back"), Muted, Cream);
    }

    static void ShapeSkillColumn(RectTransform column)
    {
        column.anchorMin = new Vector2(1f, 0f);
        column.anchorMax = new Vector2(1f, 1f);
        column.pivot = new Vector2(0.5f, 0.5f);
        column.sizeDelta = new Vector2(272f, -64f);
        column.anchoredPosition = new Vector2(column.anchoredPosition.x, 0f);

        Image image = column.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = PanelSprite();
            image.type = Image.Type.Sliced;
            image.color = new Color(0.07f, 0.065f, 0.08f, 0.98f);
        }

        Transform accent = column.Find("SkillAccent");
        RectTransform accentRect;
        Image accentImage;
        if (accent == null)
        {
            GameObject go = new GameObject("SkillAccent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(column, false);
            go.transform.SetAsFirstSibling();
            accentRect = go.GetComponent<RectTransform>();
            accentImage = go.GetComponent<Image>();
        }
        else
        {
            accentRect = accent as RectTransform;
            accentImage = accent.GetComponent<Image>();
        }
        accentRect.anchorMin = new Vector2(0f, 0f);
        accentRect.anchorMax = new Vector2(0f, 1f);
        accentRect.pivot = new Vector2(0f, 0.5f);
        accentRect.sizeDelta = new Vector2(5f, -40f);
        accentRect.anchoredPosition = new Vector2(12f, 0f);
        if (accentImage != null)
        {
            accentImage.sprite = PanelSprite();
            accentImage.type = Image.Type.Sliced;
            accentImage.color = Orange;
            accentImage.raycastTarget = false;
        }
    }

    static void DressSkillNodes(Transform root)
    {
        string[] names = { "Skill-1", "Skill0", "Skill1", "Skill2", "Skill3" };
        Vector2[] spots =
        {
            Vector2.zero,
            new Vector2(-NodeReach, -NodeReach),
            new Vector2(NodeReach, -NodeReach),
            new Vector2(NodeReach, NodeReach),
            new Vector2(-NodeReach, NodeReach)
        };

        for (int i = 0; i < names.Length; i++)
        {
            Transform node = Find(root, names[i]);
            if (node == null)
                continue;
            RectTransform rect = node as RectTransform;
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(NodeSize, NodeSize);
                rect.anchoredPosition = spots[i];
            }

            Image plate = node.GetComponent<Image>();
            if (plate == null)
                continue;

            Transform icon = node.Find("SkillIcon");
            if (icon == null && plate.sprite != null)
            {
                GameObject iconObject = new GameObject("SkillIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                iconObject.transform.SetParent(node, false);
                icon = iconObject.transform;
                Image iconImage = iconObject.GetComponent<Image>();
                iconImage.sprite = plate.sprite;
                iconImage.preserveAspect = true;
                iconImage.raycastTarget = false;
                iconImage.color = Color.white;
                plate.sprite = PanelSprite();
                plate.type = Image.Type.Sliced;
            }
            else if (icon != null)
            {
                plate.sprite = PanelSprite();
                plate.type = Image.Type.Sliced;
            }

            if (icon is RectTransform iconRect)
            {
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.offsetMin = new Vector2(8f, 8f);
                iconRect.offsetMax = new Vector2(-8f, -8f);
                Image iconImage = icon.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.color = Color.white;
                    iconImage.preserveAspect = true;
                    iconImage.raycastTarget = false;
                }
            }

            SelectSkillForButton skill = node.GetComponent<SelectSkillForButton>();
            int id = skill != null ? skill.id : 0;
            bool locked = PlayerPrefs.GetInt("OpenSkill" + id) == 0 && id != -1;
            bool selected = PlayerPrefs.GetInt("Skill") == id;
            plate.color = selected ? PlateOn : locked ? PlateLocked : Plate;

            Outline outline = node.GetComponent<Outline>();
            if (outline == null)
                outline = node.gameObject.AddComponent<Outline>();
            outline.effectDistance = new Vector2(3f, -3f);
            outline.effectColor = selected
                ? new Color(0.55f, 1f, 0.95f, 1f)
                : new Color(0.05f, 0.03f, 0.02f, 0.9f);
        }
    }

    static void ClearSkillPlates(Transform node)
    {
        if (node == null)
            return;
        Transform content = node.parent;
        if (content == null)
            return;

        Hide(content.Find("SkillHalo"));
        Hide(content.Find("SkillBoard"));
        Transform roads = content.Find("Roads");
        if (roads != null)
            roads.SetAsFirstSibling();
    }

    static void Hide(Transform target)
    {
        if (target != null)
            target.gameObject.SetActive(false);
    }

    static void BuildSkillStage(Transform root)
    {
        EnsureVignette(root);
        EnsureStage(root);
        Transform backdrop = root.Find("SkillBackdrop");
        Transform vignette = root.Find("SkillVignette");
        Transform stage = root.Find("SkillStage");
        if (backdrop != null)
            backdrop.SetAsFirstSibling();
        if (vignette != null)
            vignette.SetSiblingIndex(1);
        if (stage != null)
            stage.SetSiblingIndex(2);
        PlaceSkillStage(root);
    }

    static void EnsureVignette(Transform root)
    {
        RectTransform rect = Centered(root, "SkillVignette", Vector2.zero, Vector2.zero);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image image = rect.GetComponent<Image>();
        if (image == null)
            return;
        image.sprite = VignetteSprite();
        image.color = Color.white;
        image.raycastTarget = false;
    }

    static void EnsureStage(Transform root)
    {
        RectTransform stage = Centered(root, "SkillStage", new Vector2(640f, 640f), Vector2.zero);
        Image stageImage = stage.GetComponent<Image>();
        if (stageImage != null)
        {
            stageImage.sprite = null;
            stageImage.color = new Color(1f, 1f, 1f, 0f);
            stageImage.raycastTarget = false;
        }

        RectTransform glow = Centered(stage, "SkillGlow", new Vector2(470f, 470f), Vector2.zero);
        glow.SetAsFirstSibling();
        Image glowImage = glow.GetComponent<Image>();
        if (glowImage != null)
        {
            glowImage.sprite = GlowSprite();
            glowImage.color = new Color(1f, 0.78f, 0.46f, 0.13f);
            glowImage.raycastTarget = false;
        }

        RectTransform ring = Centered(stage, "SkillRing", new Vector2(540f, 540f), Vector2.zero);
        Image ringImage = ring.GetComponent<Image>();
        if (ringImage != null)
        {
            ringImage.sprite = RingSprite();
            ringImage.color = ringImage.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
            ringImage.preserveAspect = true;
            ringImage.raycastTarget = false;
        }

        Vector2[] spots =
        {
            new Vector2(0f, 292f),
            new Vector2(292f, 0f),
            new Vector2(0f, -292f),
            new Vector2(-292f, 0f)
        };
        for (int i = 0; i < spots.Length; i++)
        {
            RectTransform spark = Centered(stage, "SkillSpark" + i, new Vector2(36f, 36f), spots[i]);
            Image sparkImage = spark.GetComponent<Image>();
            if (sparkImage == null)
                continue;
            sparkImage.sprite = SparkSprite();
            sparkImage.color = sparkImage.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
            sparkImage.preserveAspect = true;
            sparkImage.raycastTarget = false;
        }
    }

    static void PlaceSkillStage(Transform root)
    {
        RectTransform stage = Find(root, "SkillStage") as RectTransform;
        if (stage == null)
            return;
        RectTransform column = Find(root, "DarkBG") as RectTransform;
        float panel = column != null && column.rect.width > 40f ? column.rect.width : 231f;
        stage.anchorMin = new Vector2(0.5f, 0.5f);
        stage.anchorMax = new Vector2(0.5f, 0.5f);
        stage.pivot = new Vector2(0.5f, 0.5f);
        stage.sizeDelta = new Vector2(640f, 640f);
        stage.anchoredPosition = new Vector2(-panel * 0.5f, 0f);
    }

    static RectTransform Centered(Transform parent, string name, Vector2 size, Vector2 position)
    {
        Transform found = parent.Find(name);
        RectTransform rect;
        if (found == null)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            rect = go.GetComponent<RectTransform>();
        }
        else
            rect = found as RectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return rect;
    }

    static Sprite RingSprite()
    {
        if (ringSprite == null)
            ringSprite = Resources.Load<Sprite>("Menu/SkillRing");
        return ringSprite;
    }

    static Sprite SparkSprite()
    {
        if (sparkSprite == null)
            sparkSprite = Resources.Load<Sprite>("Menu/SkillSpark");
        return sparkSprite;
    }

    static Sprite VignetteSprite()
    {
        if (vignetteSprite != null)
            return vignetteSprite;
        const int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        Color32[] pixels = new Color32[size * size];
        float half = (size - 1) * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float nx = (x - half) / half;
                float ny = (y - half) / half;
                float radius = Mathf.Sqrt(nx * nx + ny * ny);
                float alpha = Mathf.SmoothStep(0.42f, 1.05f, radius);
                pixels[y * size + x] = new Color32(0, 0, 0, (byte)(Mathf.Clamp01(alpha) * 150f));
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        vignetteSprite = Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        return vignetteSprite;
    }

    static void CoverSkillScreen(Transform root)
    {
        Transform found = root.Find("SkillBackdrop");
        RectTransform rect;
        Image image;
        if (found == null)
        {
            GameObject go = new GameObject("SkillBackdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(root, false);
            rect = go.GetComponent<RectTransform>();
            image = go.GetComponent<Image>();
        }
        else
        {
            rect = found as RectTransform;
            image = found.GetComponent<Image>();
        }
        found = rect.transform;
        found.SetAsFirstSibling();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        if (image != null)
        {
            image.sprite = null;
            image.color = new Color(0.045f, 0.048f, 0.055f, 1f);
            image.raycastTarget = true;
        }
    }

    static RectTransform Disc(Transform parent, string name, float size, Sprite sprite, Color color)
    {
        Transform found = parent.Find(name);
        RectTransform rect;
        Image image;
        if (found == null)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            rect = go.GetComponent<RectTransform>();
            image = go.GetComponent<Image>();
        }
        else
        {
            rect = found as RectTransform;
            image = found.GetComponent<Image>();
        }
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(size, size);
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
        if (image != null)
        {
            image.sprite = sprite;
            image.type = sprite == PanelSprite() ? Image.Type.Sliced : Image.Type.Simple;
            image.color = color;
            image.raycastTarget = false;
        }
        return rect;
    }

    static void Round(Transform target, Color color)
    {
        if (target == null)
            return;
        Image image = target.GetComponent<Image>();
        if (image == null)
            return;
        image.sprite = PanelSprite();
        image.type = Image.Type.Sliced;
        image.color = color;
    }

    static Sprite PanelSprite()
    {
        if (panelSprite == null)
            panelSprite = SoftDisc(96, 28, 2.2f, true);
        return panelSprite;
    }

    static Sprite GlowSprite()
    {
        if (glowSprite == null)
            glowSprite = SoftDisc(160, 80, 22f, false);
        return glowSprite;
    }

    static Sprite SoftDisc(int size, int radius, float softness, bool sliced)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[size * size];
        float half = size * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float px = x + 0.5f - half;
                float py = y + 0.5f - half;
                float dx = Mathf.Abs(px) - (half - radius);
                float dy = Mathf.Abs(py) - (half - radius);
                float outside = Mathf.Sqrt(Mathf.Max(dx, 0f) * Mathf.Max(dx, 0f) + Mathf.Max(dy, 0f) * Mathf.Max(dy, 0f))
                    + Mathf.Min(Mathf.Max(dx, dy), 0f);
                byte alpha = (byte)(Mathf.Clamp01((softness - outside) / Mathf.Max(0.01f, softness)) * 255f);
                pixels[y * size + x] = new Color32(255, 255, 255, alpha);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        Vector4 border = sliced ? new Vector4(radius, radius, radius, radius) : Vector4.zero;
        return Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
    }

    static void PlaceSkillTree(Transform root)
    {
        Transform node = Find(root, "Skill0");
        if (node == null)
            return;
        RectTransform content = node.parent as RectTransform;
        RectTransform menu = root as RectTransform;
        if (content == null || menu == null)
            return;

        Canvas.ForceUpdateCanvases();
        float height = menu.rect.height;
        if (height < 200f)
            height = 720f;
        RectTransform column = Find(root, "DarkBG") as RectTransform;
        float panel = column != null && column.rect.width > 40f ? column.rect.width : 231f;
        content.sizeDelta = new Vector2(0f, height);
        content.anchoredPosition = new Vector2(-panel * 0.5f, -height * 0.5f);

        ScrollRect scroll = content.GetComponentInParent<ScrollRect>();
        if (scroll != null)
        {
            scroll.horizontal = false;
            scroll.vertical = false;
            scroll.inertia = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.StopMovement();
        }
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
            {
                float span = (NodeReach - NodeSize) * 1.41421356f + 20f;
                float mid = NodeReach * 0.5f;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(12f, span);
                if (road.name == "Road1")
                    rect.anchoredPosition = new Vector2(-mid, -mid);
                else if (road.name == "Road2")
                    rect.anchoredPosition = new Vector2(mid, -mid);
                else if (road.name == "Road3")
                    rect.anchoredPosition = new Vector2(-mid, mid);
                else if (road.name == "Road4")
                    rect.anchoredPosition = new Vector2(mid, mid);
            }
            Image image = road.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = PanelSprite();
                image.type = Image.Type.Sliced;
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
        Text text = button != null ? button.GetComponentInChildren<Text>(true) : null;
        if (text != null)
        {
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = Mathf.Max(text.fontSize, 16);
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = 18;
        }
    }

    static void DockTop(RectTransform rect, float top, float inset, float height)
    {
        if (rect == null)
            return;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(-inset * 2f, height);
        rect.anchoredPosition = new Vector2(0f, -top);
    }

    static void DockBottom(RectTransform rect, float bottom, float inset, float height)
    {
        if (rect == null)
            return;
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.sizeDelta = new Vector2(-inset * 2f, height);
        rect.anchoredPosition = new Vector2(0f, bottom);
    }

    static void Fill(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
    }

    static Text CreateLabel(Transform parent, string name, Font font, int size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        Text text = go.GetComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.color = color;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
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
