using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TradeLook : MonoBehaviour
{
    static readonly Color Ink = new Color(0.78f, 0.62f, 0.42f, 0.96f);
    static readonly Color Sheet = new Color(0.90f, 0.78f, 0.58f, 1f);
    static readonly Color Card = new Color(0.98f, 0.94f, 0.86f, 1f);
    static readonly Color CardInner = new Color(0.93f, 0.84f, 0.66f, 1f);
    static readonly Color Slot = new Color(1f, 0.97f, 0.91f, 1f);
    static readonly Color Selected = new Color(0.98f, 0.78f, 0.46f, 1f);
    static readonly Color Orange = new Color(0.92f, 0.55f, 0.28f, 1f);
    static readonly Color Cyan = new Color(0.36f, 0.74f, 0.70f, 1f);
    static readonly Color Muted = new Color(0.76f, 0.62f, 0.46f, 1f);
    static readonly Color Gold = new Color(0.62f, 0.36f, 0.08f, 1f);
    static readonly Color InkText = new Color(0.34f, 0.20f, 0.10f, 1f);
    const float Edge = 20f;
    const float Frame = 8f;
    const float Header = 80f;
    static bool hooked;
    static Sprite savedCoin;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Hook()
    {
        if (hooked)
            return;
        hooked = true;
        SceneManager.sceneLoaded += (_, __) => StylePrompts();
        StylePrompts();
    }

    public static void ApplyShop(ShopMenuController shop)
    {
        if (shop == null || shop.transform.parent == null)
            return;
        Transform menu = shop.transform.parent;
        EnsureChrome(menu);
        DressBackdrop(menu);
        RectTransform list = Find(menu, "Scroll View") as RectTransform;
        PlaceList(list);
        if (list != null)
            list.offsetMin = new Vector2(list.offsetMin.x, 72f);
        PlaceHeader(menu, Ru() ? "Лавка" : "Shop", null);
        SetIcon(menu.Find("TradeTitle"), MoneyBag() ?? HudCoin());
        LayoutShopColumn(menu, shop);
        LayoutPopup(Find(menu, "ScaleItems"), shop);
        LayoutNotice(Find(menu, "Successful"));
        LayoutNotice(Find(menu, "NotEnough"));
        StyleSlots(shop);
        PaintShopMode(shop);
        if (list != null)
            list.SetAsLastSibling();
        Raise(menu.Find("Buy"));
        Raise(menu.Find("Sell"));
        Raise(menu.Find("Close"));
        KeepClick(menu.Find("Buy"), shop.ChangeCount);
        KeepClick(menu.Find("Sell"), shop.ChangeCountForSell);
        KeepClick(menu.Find("Close"), null);
        PlaceMoney(menu);
    }

    static void PlaceMoney(Transform menu)
    {
        Transform money = menu.Find("Money");
        if (money == null)
            return;
        money.SetAsLastSibling();
    }

    static void ClearDecor(Transform target)
    {
        if (target == null)
            return;
        Transform icon = target.Find("TradeIcon");
        if (icon != null)
            icon.gameObject.SetActive(false);
        Text text = target.GetComponentInChildren<Text>(true);
        if (text == null)
            return;
        text.enabled = true;
        text.color = InkText;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        if (text.transform != target)
        {
            text.rectTransform.localScale = Vector3.one;
            Stretch(text.rectTransform, 8f, 4f, 8f, 4f);
        }
    }

    static void Raise(Transform widget)
    {
        if (widget != null)
            widget.SetAsLastSibling();
    }

    public static void ApplyCraft(CraftngMenuController craft)
    {
        if (craft == null || craft.transform.parent == null)
            return;
        Transform menu = craft.transform.parent;
        EnsureChrome(menu);
        DressBackdrop(menu);
        PlaceList(Find(menu, "Scroll View") as RectTransform);
        PlaceHeader(menu, Ru() ? "Крафт" : "Craft", "Menu/TradeCraft");
        LayoutCraftColumn(Find(menu, "Description"));
        LayoutNotice(Find(menu, "Successful"));
        LayoutNotice(Find(menu, "NotEnough"));
        StyleRecipes(menu);
        PlaceMenuClose(Find(menu, "Close") as RectTransform);
    }

    public static void PaintShopMode(ShopMenuController shop)
    {
        if (shop == null || shop.transform.parent == null)
            return;
        Transform menu = shop.transform.parent;
        Label(menu.Find("Buy"), Ru() ? "Купить" : "Buy");
        Label(menu.Find("Sell"), Ru() ? "Продать" : "Sell");
        ClearDecor(menu.Find("Buy"));
        ClearDecor(menu.Find("Sell"));
        string word = shop.sell ? (Ru() ? "Продать" : "Sell") : (Ru() ? "Купить" : "Buy");
        Transform confirm = Find(menu, "BuyButton");
        if (confirm == null)
            confirm = Find(menu, "SellButton");
        Label(confirm, word);
        StyleAction(confirm, shop.sell ? Cyan : Orange, InkText);
        ClearDecor(confirm);
        Transform tabs = menu.Find("TradeTabs");
        if (tabs != null)
            tabs.gameObject.SetActive(false);
    }

    public static void MarkSlot(BuyItem item)
    {
        if (item == null || item.transform.parent == null)
            return;
        Transform folder = item.transform.parent;
        for (int i = 0; i < folder.childCount; i++)
        {
            Transform child = folder.GetChild(i);
            if (child.GetComponent<BuyItem>() == null)
                continue;
            Image image = child.GetComponent<Image>();
            if (image == null)
                continue;
            image.color = child == item.transform ? Selected : Color.white;
            image.fillCenter = false;
            image.raycastTarget = true;
        }
    }

    public static void MarkCraft(Transform selected)
    {
        if (selected == null || selected.parent == null)
            return;
        Transform folder = selected.parent;
        for (int i = 0; i < folder.childCount; i++)
        {
            Transform child = folder.GetChild(i);
            if (child.GetComponent<NeedToCraft>() == null)
                continue;
            Image image = child.GetComponent<Image>();
            if (image == null)
                continue;
            image.color = child == selected ? Selected : Color.white;
        }
    }

    public static void Fade(Transform menu)
    {
        if (menu == null)
            return;
        CanvasGroup group = menu.GetComponent<CanvasGroup>();
        if (group == null)
            group = menu.gameObject.AddComponent<CanvasGroup>();
        TradeFade fade = menu.GetComponent<TradeFade>();
        if (fade == null)
            fade = menu.gameObject.AddComponent<TradeFade>();
        fade.Play(group);
    }

    static void EnsureChrome(Transform menu)
    {
        if (menu.Find("TradeChrome") == null)
            new GameObject("TradeChrome").transform.SetParent(menu, false);
    }

    static void DressBackdrop(Transform menu)
    {
        Transform background = Find(menu, "Background");
        if (background is RectTransform rect)
            Stretch(rect, 0f, 0f, 0f, 0f);
        Dress(background, Ink);
        Image image = background != null ? background.GetComponent<Image>() : null;
        if (image != null && image.GetComponent<Button>() == null)
            image.raycastTarget = false;
    }

    static void PlaceList(RectTransform scroll)
    {
        if (scroll == null)
            return;
        scroll.localScale = Vector3.one;
        scroll.anchorMin = new Vector2(0f, 0f);
        scroll.anchorMax = new Vector2(0.46f, 1f);
        scroll.pivot = new Vector2(0.5f, 0.5f);
        scroll.offsetMin = new Vector2(Edge, Edge);
        scroll.offsetMax = new Vector2(-12f, -Header);
        Dress(scroll, Sheet);
        Image plate = scroll.GetComponent<Image>();
        if (plate != null)
            plate.raycastTarget = false;
        ScrollRect scroller = scroll.GetComponent<ScrollRect>();
        if (scroller != null)
            scroller.horizontal = false;
        InsetViewport(scroll);
        TradeGridFit fit = scroll.GetComponent<TradeGridFit>();
        if (fit == null)
            fit = scroll.gameObject.AddComponent<TradeGridFit>();
        fit.Arm();
    }

    static void InsetViewport(RectTransform scroll)
    {
        Transform viewport = scroll.Find("Viewport");
        if (viewport is not RectTransform rect)
            return;
        Stretch(rect, Frame, Frame, Frame, Frame);
        if (rect.GetComponent<Mask>() == null && rect.GetComponent<RectMask2D>() == null)
            rect.gameObject.AddComponent<RectMask2D>();
        FitGrid(scroll, rect);
    }

    public static void RefitScroll(RectTransform scroll)
    {
        if (scroll == null)
            return;
        Transform viewport = scroll.Find("Viewport");
        if (viewport is RectTransform rect)
            FitGrid(scroll, rect);
    }

    static void FitGrid(RectTransform scroll, RectTransform viewport)
    {
        Transform content = viewport.Find("Content");
        if (content == null)
            return;
        GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
        if (grid == null)
            return;
        Canvas.ForceUpdateCanvases();
        float inner = viewport.rect.width;
        if (inner < 80f)
            inner = Mathf.Max(80f, scroll.rect.width - Frame * 2f);
        const int pad = 8;
        const float gap = 8f;
        const float cell = 64f;
        int columns = Mathf.FloorToInt((inner - pad * 2f + gap) / (cell + gap));
        if (columns < 1)
            columns = 1;
        while (columns > 1 && pad * 2f + columns * cell + (columns - 1) * gap > inner + 0.5f)
            columns--;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.padding = new RectOffset(pad, pad, pad, pad);
        grid.spacing = new Vector2(gap, gap);
        grid.cellSize = new Vector2(cell, cell);
        ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        Canvas.ForceUpdateCanvases();
    }

    static void PlaceHeader(Transform menu, string caption, string icon)
    {
        RectTransform title = menu.Find("TradeTitle") as RectTransform;
        if (title == null)
        {
            title = new GameObject("TradeTitle", typeof(RectTransform)).GetComponent<RectTransform>();
            title.SetParent(menu, false);
            Text text = NewText(title, caption, 28, InkText);
            text.alignment = TextAnchor.MiddleLeft;
            Stretch(text.rectTransform, 42f, 0f, 8f, 0f);
        }
        title.localScale = Vector3.one;
        title.anchorMin = title.anchorMax = new Vector2(0f, 1f);
        title.pivot = new Vector2(0f, 1f);
        title.sizeDelta = new Vector2(260f, 52f);
        title.anchoredPosition = new Vector2(Edge, -18f);
        title.SetAsLastSibling();
        if (!string.IsNullOrEmpty(icon))
            AddIcon(title, icon);

        Transform money = Find(menu, "Money");
        if (money is RectTransform moneyRect)
        {
            moneyRect.localScale = Vector3.one;
            moneyRect.anchorMin = moneyRect.anchorMax = new Vector2(0f, 0f);
            moneyRect.pivot = new Vector2(0f, 0f);
            moneyRect.sizeDelta = new Vector2(120f, 40f);
            moneyRect.anchoredPosition = new Vector2(Edge + 36f, 16f);
            Text label = money.GetComponent<Text>();
            if (label != null)
            {
                label.color = InkText;
                label.alignment = TextAnchor.MiddleLeft;
                label.fontSize = 24;
                label.horizontalOverflow = HorizontalWrapMode.Overflow;
                label.verticalOverflow = VerticalWrapMode.Overflow;
                label.raycastTarget = false;
                label.enabled = true;
            }
            Transform picture = Find(money, "MoneyImage");
            if (picture is RectTransform iconRect)
            {
                iconRect.anchorMin = iconRect.anchorMax = new Vector2(0f, 0.5f);
                iconRect.pivot = new Vector2(1f, 0.5f);
                iconRect.sizeDelta = new Vector2(28f, 28f);
                iconRect.anchoredPosition = new Vector2(-6f, 0f);
                Image image = picture.GetComponent<Image>();
                if (image != null)
                {
                    if (image.sprite != null && image.sprite.name != "TradeCoin")
                        savedCoin = image.sprite;
                    else if (savedCoin != null)
                        image.sprite = savedCoin;
                    image.color = Color.white;
                    image.preserveAspect = true;
                    image.type = Image.Type.Simple;
                }
            }
        }

        PlaceMenuClose(Find(menu, "Close") as RectTransform);
    }

    static void PlaceMenuClose(RectTransform close)
    {
        if (close == null || close.parent == null || close.parent.name == "Description")
            return;
        if (close.sizeDelta.y > 100f)
            return;
        close.localScale = Vector3.one;
        close.anchorMin = close.anchorMax = new Vector2(1f, 1f);
        close.pivot = new Vector2(1f, 1f);
            close.sizeDelta = new Vector2(50f, 50f);
        close.anchoredPosition = new Vector2(-Edge, -18f);
        StyleAction(close, Muted, InkText);
        UseCross(close);
    }

    static void UseCross(RectTransform close)
    {
        Text[] labels = close.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].text = "";
            labels[i].enabled = false;
        }
        Transform existing = close.Find("TradeIcon");
        RectTransform iconRect;
        Image image;
        if (existing == null)
        {
            iconRect = new GameObject("TradeIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
            iconRect.SetParent(close, false);
            image = iconRect.GetComponent<Image>();
            image.raycastTarget = false;
        }
        else
        {
            iconRect = existing as RectTransform;
            image = existing.GetComponent<Image>();
        }
        Sprite sprite = Resources.Load<Sprite>("Menu/TradeClose");
        if (sprite != null && image != null)
            image.sprite = sprite;
        if (image != null)
        {
            image.color = InkText;
            image.preserveAspect = true;
            image.type = Image.Type.Simple;
        }
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(22f, 22f);
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.localScale = Vector3.one;
    }

    static void LayoutShopColumn(Transform menu, ShopMenuController shop)
    {
        Transform card = Find(menu, "Description");
        if (card is RectTransform cardRect)
        {
            cardRect.localScale = Vector3.one;
            cardRect.anchorMin = new Vector2(0.48f, 0f);
            cardRect.anchorMax = new Vector2(1f, 1f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.offsetMin = new Vector2(10f, 84f);
            cardRect.offsetMax = new Vector2(-Edge, -Header);
            EnsureImage(card, Card);
            Image cardImage = card.GetComponent<Image>();
            if (cardImage != null && card.GetComponent<Button>() == null)
                cardImage.raycastTarget = false;
            Transform background = card.Find("Background");
            if (background != null)
            {
                Image backImage = background.GetComponent<Image>();
                if (backImage != null)
                    backImage.raycastTarget = false;
            }
            if (background is RectTransform backRect)
                Stretch(backRect, Frame, Frame, Frame, Frame);
            Dress(background, Card);
            Transform name = card.Find("Name");
            if (name is RectTransform nameRect)
            {
                nameRect.localScale = Vector3.one;
                nameRect.anchorMin = new Vector2(0f, 1f);
                nameRect.anchorMax = new Vector2(1f, 1f);
                nameRect.pivot = new Vector2(0.5f, 1f);
                nameRect.offsetMin = new Vector2(10f, -62f);
                nameRect.offsetMax = new Vector2(-10f, -10f);
                Dress(name, CardInner);
                if (name.childCount > 0 && name.GetChild(0) is RectTransform nameText)
                    Stretch(nameText, 10f, 6f, 10f, 6f);
                FitText(name.GetComponentInChildren<Text>(true), 24, InkText, TextAnchor.MiddleCenter);
            }
            Transform body = background != null ? background.Find("Text") : null;
            if (body is RectTransform bodyRect)
            {
                Stretch(bodyRect, 12f, 12f, 12f, 64f);
                FitText(body.GetComponent<Text>(), 18, InkText, TextAnchor.UpperLeft);
            }
            Text[] cardTexts = card.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < cardTexts.Length; i++)
            {
                if (cardTexts[i].GetComponentInParent<Button>() == null)
                    cardTexts[i].raycastTarget = false;
            }
            PaintOwnedImages(card);
        }

        PlaceShopButtons(menu, shop);
    }

    static void PlaceShopButtons(Transform menu, ShopMenuController shop)
    {
        Transform tabs = menu.Find("TradeTabs");
        if (tabs != null)
            tabs.gameObject.SetActive(false);
        RectTransform buy = EnsureDealButton(menu, "Buy", "Sell", false, shop);
        RectTransform sell = EnsureDealButton(menu, "Sell", "Buy", true, shop);
        PlaceHalfButton(buy, true, Orange);
        PlaceHalfButton(sell, false, Cyan);
        Label(buy, Ru() ? "Купить" : "Buy");
        Label(sell, Ru() ? "Продать" : "Sell");
        ClearDecor(buy);
        ClearDecor(sell);
    }

    static RectTransform EnsureDealButton(Transform menu, string name, string donor, bool selling, ShopMenuController shop)
    {
        Transform existing = menu.Find(name);
        if (existing is RectTransform ready)
            return ready;
        Transform source = menu.Find(donor);
        if (source == null)
            return null;
        GameObject clone = Object.Instantiate(source.gameObject, source.parent);
        clone.name = name;
        Button button = clone.GetComponent<Button>();
        if (button != null)
        {
            button.onClick = new Button.ButtonClickedEvent();
            if (selling)
                button.onClick.AddListener(shop.ChangeCountForSell);
            else
                button.onClick.AddListener(shop.ChangeCount);
        }
        return clone.transform as RectTransform;
    }

    static void PlaceHalfButton(RectTransform button, bool left, Color color)
    {
        if (button == null)
            return;
        button.gameObject.SetActive(true);
        button.localScale = Vector3.one;
        button.anchorMin = new Vector2(left ? 0.48f : 0.74f, 0f);
        button.anchorMax = new Vector2(left ? 0.73f : 1f, 0f);
        button.pivot = new Vector2(0.5f, 0f);
        button.offsetMin = new Vector2(left ? 10f : 4f, 16f);
        button.offsetMax = new Vector2(left ? -4f : -Edge, 72f);
        StyleAction(button, color, InkText);
    }

    static void PlaceDealButton(RectTransform button)
    {
        if (button == null)
            return;
        button.localScale = Vector3.one;
        button.anchorMin = new Vector2(0.48f, 0f);
        button.anchorMax = new Vector2(1f, 0f);
        button.pivot = new Vector2(0.5f, 0f);
        button.offsetMin = new Vector2(10f, 16f);
        button.offsetMax = new Vector2(-Edge, 68f);
        StyleAction(button, Orange, InkText);
    }

    static void PlaceTabs(Transform menu, ShopMenuController shop)
    {
        RectTransform row = menu.Find("TradeTabs") as RectTransform;
        if (row == null)
        {
            row = new GameObject("TradeTabs", typeof(RectTransform)).GetComponent<RectTransform>();
            row.SetParent(menu, false);
            MakeTab(row, "TradeBuyTab", Ru() ? "Купить" : "Buy", 0f, () => shop.SetSelling(false));
            MakeTab(row, "TradeSellTab", Ru() ? "Продать" : "Sell", 1f, () => shop.SetSelling(true));
        }
        row.localScale = Vector3.one;
        row.anchorMin = new Vector2(0.48f, 0f);
        row.anchorMax = new Vector2(1f, 0f);
        row.pivot = new Vector2(0.5f, 0f);
        row.offsetMin = new Vector2(10f, 76f);
        row.offsetMax = new Vector2(-Edge, 120f);
        row.SetAsLastSibling();
    }

    static void LayoutCraftColumn(Transform card)
    {
        if (card is not RectTransform cardRect)
            return;
        cardRect.localScale = Vector3.one;
        cardRect.anchorMin = new Vector2(0.48f, 0f);
        cardRect.anchorMax = new Vector2(1f, 1f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.offsetMin = new Vector2(10f, Edge);
        cardRect.offsetMax = new Vector2(-Edge, -Header);
        EnsureImage(card, Card);

        if (card.Find("Close") is RectTransform close)
        {
            close.localScale = Vector3.one;
            close.anchorMin = close.anchorMax = new Vector2(1f, 1f);
            close.pivot = new Vector2(1f, 1f);
            close.sizeDelta = new Vector2(44f, 44f);
            close.anchoredPosition = new Vector2(-10f, -10f);
            StyleAction(close, Muted, InkText);
            UseCross(close);
        }

        if (card.Find("Name") is RectTransform name)
        {
            name.localScale = Vector3.one;
            name.anchorMin = new Vector2(0f, 1f);
            name.anchorMax = new Vector2(1f, 1f);
            name.pivot = new Vector2(0.5f, 1f);
            name.offsetMin = new Vector2(10f, -58f);
            name.offsetMax = new Vector2(-62f, -10f);
            Dress(name, CardInner);
            Transform nameText = name.childCount > 0 ? name.GetChild(0) : name;
            if (nameText is RectTransform nameTextRect)
                Stretch(nameTextRect, 10f, 6f, 10f, 6f);
            FitText(name.GetComponentInChildren<Text>(true), 22, InkText, TextAnchor.MiddleLeft);
        }

        Transform body = card.Find("Text");
        if (body is RectTransform bodyRect)
        {
            bodyRect.localScale = Vector3.one;
            bodyRect.anchorMin = new Vector2(0f, 0f);
            bodyRect.anchorMax = new Vector2(1f, 1f);
            bodyRect.offsetMin = new Vector2(Frame, 156f);
            bodyRect.offsetMax = new Vector2(-Frame, -68f);
            LayoutCraftTexts(body);
        }

        if (card.Find("Items") is RectTransform items)
        {
            items.localScale = Vector3.one;
            items.anchorMin = new Vector2(0f, 0f);
            items.anchorMax = new Vector2(1f, 0f);
            items.pivot = new Vector2(0.5f, 0f);
            items.offsetMin = new Vector2(Frame, 64f);
            items.offsetMax = new Vector2(-Frame, 148f);
            Dress(items, Sheet);
            if (items.GetComponent<TradePack>() == null)
                items.gameObject.AddComponent<TradePack>();
            for (int i = 0; i < items.childCount; i++)
                FitCount(items.GetChild(i));
        }

        if (card.Find("Craft") is RectTransform craft)
        {
            craft.localScale = Vector3.one;
            craft.anchorMin = new Vector2(0f, 0f);
            craft.anchorMax = new Vector2(1f, 0f);
            craft.pivot = new Vector2(0.5f, 0f);
            craft.offsetMin = new Vector2(Frame, 8f);
            craft.offsetMax = new Vector2(-Frame, 56f);
            StyleAction(craft, Orange, InkText);
            AddIcon(craft, "Menu/TradeCraft");
        }

        PaintOwnedImages(card);
    }

    static void LayoutCraftTexts(Transform body)
    {
        PlaceInner(body.Find("Description") as RectTransform, 0f, 0.44f, 1f, 1f, 0f, 8f, 0f, 0f);
        PlaceInner(body.Find("Stats") as RectTransform, 0f, 0f, 0.5f, 0.42f, 0f, 0f, 6f, 0f);
        PlaceInner(body.Find("Comment") as RectTransform, 0.5f, 0f, 1f, 0.42f, 6f, 0f, 0f, 0f);
        DressNamed(body, "Description", CardInner);
        DressNamed(body, "Stats", CardInner);
        DressNamed(body, "Comment", CardInner);
        StretchChildText(body.Find("Description"), 20, TextAnchor.UpperLeft);
        StretchChildText(body.Find("Stats"), 16, TextAnchor.UpperLeft);
        StretchChildText(body.Find("Comment"), 16, TextAnchor.UpperLeft);
    }

    static void LayoutPopup(Transform panel, ShopMenuController shop)
    {
        if (panel is not RectTransform rect)
            return;
        rect.localScale = Vector3.one;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(420f, 280f);
        rect.anchoredPosition = Vector2.zero;
        Dress(panel, Card);
        HideBigDismiss(panel);

        Text counter = shop != null ? shop.Counter : null;
        Text[] texts = panel.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null || texts[i] == counter)
                continue;
            if (texts[i].GetComponentInParent<Button>() != null)
                continue;
            string label = texts[i].gameObject.name;
            if (label == "TradePayout" || label == "TradeDealTitle")
                continue;
            texts[i].gameObject.SetActive(false);
        }
        Text title = EnsureLine(panel, "TradeDealTitle", Ru() ? "Количество" : "Amount", 22, InkText);
        Pin(title.rectTransform, 0.5f, 1f, 340f, 36f, 0f, -16f);
        if (shop != null)
            shop.BindTitle(title);
        if (counter != null)
        {
            Pin(counter.rectTransform, 0.5f, 1f, 180f, 32f, 0f, -56f);
            FitText(counter, 22, InkText, TextAnchor.MiddleCenter);
        }
        Text payout = EnsureLine(panel, "TradePayout", "", 20, Gold);
        Pin(payout.rectTransform, 0.5f, 0f, 340f, 28f, 0f, 78f);
        if (shop != null)
            shop.BindPayout(payout);
        if (shop != null && shop.Amount != null && shop.Amount.transform is RectTransform slider)
        {
            slider.localScale = Vector3.one;
            Pin(slider, 0.5f, 0.5f, 200f, 22f, 0f, 6f);
            StyleSlider(slider);
        }
        if (shop != null)
        {
            EnsureStep(panel, "TradeMinus", "−", -132f, shop.NudgeDown);
            EnsureStep(panel, "TradePlus", "+", 132f, shop.NudgeUp);
        }
        Transform confirm = panel.Find("BuyButton");
        if (confirm == null)
            confirm = panel.Find("SellButton");
        if (confirm is RectTransform confirmRect)
        {
            confirmRect.localScale = Vector3.one;
            Pin(confirmRect, 0.5f, 0f, 220f, 48f, 0f, 18f);
            StyleAction(confirm, shop != null && shop.sell ? Cyan : Orange, InkText);
            ClearDecor(confirm);
        }
    }

    static Text EnsureLine(Transform panel, string name, string caption, int size, Color color)
    {
        Transform existing = panel.Find(name);
        Text text;
        if (existing == null)
        {
            text = NewText(panel, caption, size, color);
            text.gameObject.name = name;
        }
        else
            text = existing.GetComponent<Text>();
        if (text == null)
            return null;
        text.text = caption;
        FitText(text, size, color, TextAnchor.MiddleCenter);
        text.raycastTarget = false;
        return text;
    }

    static void EnsureStep(Transform panel, string name, string caption, float x, UnityEngine.Events.UnityAction action)
    {
        Transform existing = panel.Find(name);
        RectTransform rect;
        if (existing == null)
        {
            rect = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button)).GetComponent<RectTransform>();
            rect.SetParent(panel, false);
            Button button = rect.GetComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();
            button.onClick.AddListener(action);
            Text label = NewText(rect, caption, 28, InkText);
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;
            Stretch(label.rectTransform, 0f, 0f, 0f, 0f);
        }
        else
            rect = existing as RectTransform;
        rect.localScale = Vector3.one;
        Pin(rect, 0.5f, 0.5f, 44f, 44f, x, 6f);
        StyleAction(rect, Muted, InkText);
        Text text = rect.GetComponentInChildren<Text>(true);
        if (text != null)
        {
            text.text = caption;
            text.enabled = true;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = InkText;
        }
    }

    static void StyleSlider(Transform slider)
    {
        RectTransform track = slider.Find("Background") as RectTransform;
        if (track != null)
        {
            track.anchorMin = new Vector2(0f, 0.5f);
            track.anchorMax = new Vector2(1f, 0.5f);
            track.pivot = new Vector2(0.5f, 0.5f);
            track.sizeDelta = new Vector2(-8f, 8f);
            track.anchoredPosition = Vector2.zero;
        }
        RectTransform fillArea = slider.Find("Fill Area") as RectTransform;
        if (fillArea != null)
        {
            fillArea.anchorMin = new Vector2(0f, 0.5f);
            fillArea.anchorMax = new Vector2(1f, 0.5f);
            fillArea.pivot = new Vector2(0.5f, 0.5f);
            fillArea.sizeDelta = new Vector2(-16f, 8f);
            fillArea.anchoredPosition = Vector2.zero;
        }
        RectTransform handleArea = slider.Find("Handle Slide Area") as RectTransform;
        if (handleArea != null)
        {
            handleArea.anchorMin = Vector2.zero;
            handleArea.anchorMax = Vector2.one;
            handleArea.offsetMin = new Vector2(8f, 0f);
            handleArea.offsetMax = new Vector2(-8f, 0f);
        }
        Transform handle = handleArea != null ? handleArea.Find("Handle") : slider.Find("Handle");
        if (handle is RectTransform knob)
        {
            knob.anchorMin = new Vector2(knob.anchorMin.x, 0.5f);
            knob.anchorMax = new Vector2(knob.anchorMax.x, 0.5f);
            knob.pivot = new Vector2(0.5f, 0.5f);
            knob.sizeDelta = new Vector2(18f, 18f);
            Image image = knob.GetComponent<Image>();
            if (image != null)
            {
                image.type = Image.Type.Simple;
                image.preserveAspect = true;
            }
        }
        Image[] images = slider.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < images.Length; i++)
        {
            string name = images[i].gameObject.name;
            if (name == "Background")
                images[i].color = CardInner;
            else if (name == "Fill")
                images[i].color = Orange;
            else if (name == "Handle")
                images[i].color = InkText;
        }
    }

    static void LayoutNotice(Transform panel)
    {
        if (panel is not RectTransform rect)
            return;
        rect.localScale = Vector3.one;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(440f, 220f);
        rect.anchoredPosition = Vector2.zero;
        Dress(panel, Card);
        HideBigDismiss(panel);
        Text text = null;
        Text[] texts = panel.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].GetComponentInParent<Button>() != null)
                continue;
            text = texts[i];
            break;
        }
        if (text != null)
        {
            Stretch(text.rectTransform, 36f, 32f, 36f, 32f);
            FitText(text, 22, InkText, TextAnchor.MiddleCenter);
        }
    }

    static void HideBigDismiss(Transform panel)
    {
        if (panel == null)
            return;
        Transform close = panel.Find("Close");
        if (close != null)
        {
            Graphic[] graphics = close.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
                graphics[i].raycastTarget = false;
            Button buried = close.GetComponent<Button>();
            if (buried != null)
                buried.interactable = false;
        }
        Transform dismiss = panel.Find("TradeDismiss");
        RectTransform rect;
        if (dismiss == null)
        {
            rect = new GameObject("TradeDismiss", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button)).GetComponent<RectTransform>();
            rect.SetParent(panel, false);
            Image image = rect.GetComponent<Image>();
            Button button = rect.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => panel.gameObject.SetActive(false));
        }
        else
            rect = dismiss as RectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.sizeDelta = new Vector2(36f, 36f);
        rect.anchoredPosition = new Vector2(-8f, -8f);
        rect.localScale = Vector3.one;
        Dress(rect, Muted);
        Image hit = rect.GetComponent<Image>();
        if (hit != null)
        {
            hit.raycastTarget = true;
            hit.fillCenter = true;
        }
        Button dismissButton = rect.GetComponent<Button>();
        if (dismissButton != null)
            dismissButton.interactable = true;
        UseCross(rect);
        rect.SetAsLastSibling();
    }

    static void KeepClick(Transform widget, UnityEngine.Events.UnityAction action)
    {
        if (widget == null)
            return;
        Button button = widget.GetComponent<Button>();
        if (button == null)
            return;
        button.interactable = true;
        Graphic graphic = button.targetGraphic != null ? button.targetGraphic : button.GetComponent<Graphic>();
        if (graphic != null)
            graphic.raycastTarget = true;
        if (action == null)
            return;
        bool armed = false;
        int count = button.onClick.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            if (button.onClick.GetPersistentListenerState(i) != UnityEngine.Events.UnityEventCallState.Off)
                armed = true;
        }
        button.onClick.RemoveListener(action);
        if (!armed)
            button.onClick.AddListener(action);
    }

    static void PaintOwnedImages(Transform root)
    {
        Image[] images = root.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].GetComponent<ForCraftItem>() != null)
                continue;
            if (images[i].transform.parent != null && images[i].transform.parent.GetComponent<NeedToCraft>() != null)
                continue;
            string name = images[i].gameObject.name;
            if (name == "Close" || name == "Craft" || name == "Buy" || name == "Sell")
                continue;
            if (name == "Name" || name == "Stats" || name == "Comment" || name == "Description" || name == "Items")
                Dress(images[i].transform, CardInner);
            else if (name == "Background" || images[i].transform == root)
                Dress(images[i].transform, Card);
        }
    }

    static void StylePrompts()
    {
        LangText[] labels = Object.FindObjectsByType<LangText>(FindObjectsInactive.Include);
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i] == null)
                continue;
            Button button = labels[i].GetComponentInParent<Button>();
            if (button == null)
                continue;
            RectTransform rect = button.transform as RectTransform;
            if (rect == null || rect.sizeDelta.y <= 0f || rect.sizeDelta.y > 48f || rect.sizeDelta.x > 200f)
                continue;
            if (button.transform.parent != null && button.transform.parent.name == "Description")
                continue;
            StylePrompt(button);
        }
    }

    static void StylePrompt(Button button)
    {
        string name = button.name;
        Color plate = Orange;
        string icon = "Menu/TradeBuy";
        if (name.Contains("Dialog"))
        {
            plate = Cyan;
            icon = "Menu/TradeTalk";
        }
        else if (name.Contains("Craft"))
        {
            plate = Gold;
            icon = "Menu/TradeCraft";
        }
        else if (name.Contains("Shop"))
        {
            plate = Orange;
            icon = null;
        }
        RectTransform rect = button.transform as RectTransform;
        rect.localScale = Vector3.one;
        float rightEdge = rect.anchoredPosition.x + (1f - rect.pivot.x) * rect.sizeDelta.x;
        if (rect.anchorMin.x >= 0.5f)
        {
            rect.pivot = new Vector2(1f, 0.5f);
            rect.sizeDelta = new Vector2(148f, 46f);
            rect.anchoredPosition = new Vector2(Mathf.Min(-Edge, rightEdge), rect.anchoredPosition.y);
        }
        else if (rect.anchorMax.x <= 0.5f)
        {
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(148f, 46f);
            rect.anchoredPosition = new Vector2(Mathf.Max(Edge, rect.anchoredPosition.x), rect.anchoredPosition.y);
        }
        else
        {
            rect.sizeDelta = new Vector2(Mathf.Min(rect.sizeDelta.x, 160f), 46f);
        }
        Dress(button.transform, plate);
        Text text = button.GetComponentInChildren<Text>(true);
        if (text != null)
        {
            text.color = InkText;
            text.alignment = TextAnchor.MiddleCenter;
            Stretch(text.rectTransform, 44f, 14f, 16f, 14f);
        }
        if (string.IsNullOrEmpty(icon))
            SetIcon(button.transform, MoneyBag() ?? HudCoin());
        else
            AddIcon(button.transform, icon);
        if (button.GetComponent<PromptChip>() == null)
            button.gameObject.AddComponent<PromptChip>();
    }

    static void MakeTab(RectTransform row, string name, string caption, float side, UnityEngine.Events.UnityAction click)
    {
        RectTransform rect = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button)).GetComponent<RectTransform>();
        rect.SetParent(row, false);
        rect.anchorMin = new Vector2(side * 0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f + side * 0.5f, 1f);
        rect.offsetMin = new Vector2(side == 0f ? 0f : 4f, 0f);
        rect.offsetMax = new Vector2(side == 0f ? -4f : 0f, 0f);
        Image image = rect.GetComponent<Image>();
        Sprite plate = Plate();
        if (plate != null)
        {
            image.sprite = plate;
            image.type = Image.Type.Sliced;
        }
        image.color = Muted;
        rect.GetComponent<Button>().onClick.AddListener(click);
        Text text = NewText(rect, caption, 20, InkText);
        Stretch(text.rectTransform, 10f, 6f, 10f, 6f);
    }

    static void StyleSlots(ShopMenuController shop)
    {
        Transform root = shop.transform.parent != null ? shop.transform.parent : shop.transform;
        BuyItem[] slots = root.GetComponentsInChildren<BuyItem>(true);
        for (int i = 0; i < slots.Length; i++)
        {
            Image frame = slots[i].GetComponent<Image>();
            FitFrame(frame);
            if (frame != null)
            {
                frame.raycastTarget = true;
                frame.fillCenter = false;
            }
            if (slots[i].transform.childCount > 0)
            {
                Image icon = slots[i].transform.GetChild(0).GetComponent<Image>();
                if (icon != null)
                    icon.raycastTarget = true;
                FitIcon(icon);
                PlaceSlotIcon(slots[i].transform as RectTransform, icon, true);
            }
            if (slots[i].transform.childCount > 1 && slots[i].transform.GetChild(1) is RectTransform priceRect)
            {
                priceRect.anchorMin = new Vector2(0f, 0f);
                priceRect.anchorMax = new Vector2(1f, 0f);
                priceRect.pivot = new Vector2(0.5f, 0f);
                priceRect.offsetMin = new Vector2(8f, 6f);
                priceRect.offsetMax = new Vector2(-8f, 24f);
                Text price = priceRect.GetComponent<Text>();
                if (price != null)
                {
                    price.color = Gold;
                    price.alignment = TextAnchor.MiddleCenter;
                    price.fontSize = 15;
                    price.horizontalOverflow = HorizontalWrapMode.Overflow;
                    price.verticalOverflow = VerticalWrapMode.Overflow;
                    price.resizeTextForBestFit = false;
                }
            }
        }
    }

    static void StyleRecipes(Transform menu)
    {
        NeedToCraft[] recipes = menu.GetComponentsInChildren<NeedToCraft>(true);
        for (int i = 0; i < recipes.Length; i++)
        {
            FitFrame(recipes[i].GetComponent<Image>());
            if (recipes[i].transform.childCount > 0)
            {
                Image icon = recipes[i].transform.GetChild(0).GetComponent<Image>();
                FitIcon(icon);
                PlaceSlotIcon(recipes[i].transform as RectTransform, icon, false);
            }
        }
    }

    static void PlaceSlotIcon(RectTransform slot, Image icon, bool price)
    {
        if (slot == null || icon == null)
            return;
        float side = Mathf.Min(slot.rect.width, slot.rect.height);
        if (side < 8f)
            side = 64f;
        float iconSide = side * 0.68f;
        RectTransform rect = icon.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(iconSide, iconSide);
        rect.anchoredPosition = new Vector2(0f, price ? 6f : 0f);
        icon.raycastTarget = true;
    }

    public static void FitFrame(Image frame)
    {
        if (frame == null)
            return;
        Sprite plate = Plate();
        if (plate != null)
        {
            frame.sprite = plate;
            frame.type = Image.Type.Sliced;
        }
        frame.color = Color.white;
        frame.fillCenter = false;
        Button button = frame.GetComponent<Button>();
        if (button == null)
            return;
        ColorBlock colors = button.colors;
        colors.fadeDuration = 0f;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.96f, 0.88f, 1f);
        colors.pressedColor = new Color(0.96f, 0.86f, 0.7f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.55f);
        button.colors = colors;
    }

    public static void FitIcon(Image icon)
    {
        if (icon == null)
            return;
        icon.type = Image.Type.Simple;
        icon.preserveAspect = true;
        icon.color = Color.white;
        RectTransform rect = icon.rectTransform;
        rect.localScale = Vector3.one;
        float width = Mathf.Abs(rect.sizeDelta.x);
        float height = Mathf.Abs(rect.sizeDelta.y);
        if (width < 8f)
            width = 52f;
        if (height < 8f)
            height = 52f;
        float side = Mathf.Min(width, height);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(side, side);
    }

    static void FitCount(Transform icon)
    {
        Transform count = icon.Find("Count");
        if (count is not RectTransform rect)
            return;
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.sizeDelta = new Vector2(0f, 16f);
        rect.anchoredPosition = new Vector2(0f, 1f);
        Text text = rect.GetComponent<Text>();
        if (text == null)
            return;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 14;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
    }

    static void StyleAction(Transform target, Color plate, Color textColor)
    {
        if (target == null)
            return;
        Dress(target, plate);
        Text text = target.GetComponentInChildren<Text>(true);
        if (text == null)
            return;
        text.color = textColor;
        text.alignment = TextAnchor.MiddleCenter;
        if (text.rectTransform.anchorMax.x > 0.9f && text.rectTransform.anchorMin.x < 0.1f)
            Stretch(text.rectTransform, target.Find("TradeIcon") != null ? 40f : 10f, 6f, 10f, 6f);
        text.fontSize = 22;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 16;
        text.resizeTextMaxSize = 22;
    }

    static void Dress(Transform target, Color color)
    {
        if (target == null)
            return;
        Image image = target.GetComponent<Image>();
        if (image == null)
            return;
        Sprite plate = Plate();
        if (plate != null)
        {
            image.sprite = plate;
            image.type = Image.Type.Sliced;
        }
        image.color = color;
    }

    static void DressNamed(Transform root, string name, Color color)
    {
        Transform target = root.Find(name);
        if (target == null)
            return;
        Dress(target, color);
    }

    static void EnsureImage(Transform target, Color color)
    {
        if (target.GetComponent<Image>() == null)
            target.gameObject.AddComponent<Image>();
        Dress(target, color);
    }

    static void Tint(Transform target, Color plate, Color textColor)
    {
        if (target == null)
            return;
        Image image = target.GetComponent<Image>();
        if (image != null)
            image.color = plate;
        Text text = target.GetComponentInChildren<Text>(true);
        if (text != null)
            text.color = textColor;
    }

    static void Label(Transform target, string caption)
    {
        if (target == null)
            return;
        Text text = target.GetComponentInChildren<Text>(true);
        if (text != null)
            text.text = caption;
    }

    static void SetIcon(Transform target, Sprite sprite)
    {
        if (target == null || sprite == null)
            return;
        Transform existing = target.Find("TradeIcon");
        RectTransform rect;
        Image image;
        if (existing == null)
        {
            rect = new GameObject("TradeIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
            rect.SetParent(target, false);
            image = rect.GetComponent<Image>();
            image.raycastTarget = false;
        }
        else
        {
            rect = existing as RectTransform;
            image = existing.GetComponent<Image>();
        }
        if (image != null)
        {
            image.sprite = sprite;
            image.color = Color.white;
            image.preserveAspect = true;
            image.type = Image.Type.Simple;
        }
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.sizeDelta = new Vector2(26f, 26f);
        rect.anchoredPosition = new Vector2(12f, 0f);
        Text text = target.GetComponentInChildren<Text>(true);
        if (text != null && text.transform != target && text.rectTransform.anchorMax.x > 0.9f)
            text.rectTransform.offsetMin = new Vector2(42f, text.rectTransform.offsetMin.y);
    }

    static Sprite HudCoin()
    {
        if (savedCoin != null && savedCoin.name != "TradeCoin")
            return savedCoin;
        Image[] images = Object.FindObjectsByType<Image>(FindObjectsInactive.Include);
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] == null || images[i].name != "MoneyImage" || images[i].sprite == null)
                continue;
            if (images[i].sprite.name == "TradeCoin")
                continue;
            savedCoin = images[i].sprite;
            return savedCoin;
        }
        return savedCoin;
    }

    static Sprite MoneyBag()
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Мешочек с деньгами.png");
#else
        return null;
#endif
    }

    static void AddIcon(Transform target, string resource)
    {
        if (target == null)
            return;
        Transform existing = target.Find("TradeIcon");
        RectTransform rect;
        Image image;
        if (existing == null)
        {
            rect = new GameObject("TradeIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
            rect.SetParent(target, false);
            image = rect.GetComponent<Image>();
            image.raycastTarget = false;
        }
        else
        {
            rect = existing as RectTransform;
            image = existing.GetComponent<Image>();
        }
        Sprite sprite = Resources.Load<Sprite>(resource);
        if (sprite != null && image != null)
            image.sprite = sprite;
        if (image != null)
        {
            image.color = Color.white;
            image.preserveAspect = true;
        }
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.sizeDelta = new Vector2(26f, 26f);
        rect.anchoredPosition = new Vector2(12f, 0f);
        Text text = target.GetComponentInChildren<Text>(true);
        if (text != null && text.transform != target && text.rectTransform.anchorMax.x > 0.9f)
            text.rectTransform.offsetMin = new Vector2(42f, text.rectTransform.offsetMin.y);
    }

    static void FitText(Text text, int size, Color color, TextAnchor anchor)
    {
        if (text == null)
            return;
        text.color = color;
        text.alignment = anchor;
        text.fontSize = size;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = Mathf.Max(14, size - 4);
        text.resizeTextMaxSize = size;
    }

    static void StretchChildText(Transform box, int size, TextAnchor anchor)
    {
        if (box == null)
            return;
        Text text = box.GetComponentInChildren<Text>(true);
        if (text == null)
            return;
        if (text.transform is RectTransform rect && text.transform != box)
            Stretch(rect, 10f, 8f, 10f, 8f);
        FitText(text, size, InkText, anchor);
    }

    static void PlaceInner(RectTransform rect, float x0, float y0, float x1, float y1, float left, float bottom, float right, float top)
    {
        if (rect == null)
            return;
        rect.localScale = Vector3.one;
        rect.anchorMin = new Vector2(x0, y0);
        rect.anchorMax = new Vector2(x1, y1);
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    static void Pin(RectTransform rect, float ax, float ay, float width, float height, float x, float y)
    {
        if (rect == null)
            return;
        rect.anchorMin = rect.anchorMax = new Vector2(ax, ay);
        rect.pivot = new Vector2(0.5f, ay > 0.75f ? 1f : ay < 0.25f ? 0f : 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);
    }

    static Text NewText(Transform parent, string caption, int size, Color color)
    {
        RectTransform rect = new GameObject("Text", typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        Text text = rect.gameObject.AddComponent<Text>();
        Canvas canvas = parent.GetComponentInParent<Canvas>();
        Text sample = canvas != null ? canvas.GetComponentInChildren<Text>(true) : null;
        text.font = sample != null && sample.font != null ? sample.font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = caption;
        text.fontSize = size;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        return text;
    }

    static void Stretch(RectTransform rect, float left, float bottom, float right, float top)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
        rect.localScale = Vector3.one;
    }

    static Transform Find(Transform root, string name)
    {
        if (root == null)
            return null;
        Transform direct = root.Find(name);
        if (direct != null)
            return direct;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform nested = Find(root.GetChild(i), name);
            if (nested != null)
                return nested;
        }
        return null;
    }

    static Sprite Plate()
    {
        return Resources.Load<Sprite>("Menu/TradePlate");
    }

    static bool Ru()
    {
        return PlayerPrefs.GetInt("Language") == 0;
    }
}

public class TradeGridFit : MonoBehaviour
{
    bool again;

    public void Arm()
    {
        again = true;
        enabled = true;
        TradeLook.RefitScroll(transform as RectTransform);
    }

    void LateUpdate()
    {
        if (!again)
            return;
        again = false;
        TradeLook.RefitScroll(transform as RectTransform);
        enabled = false;
    }
}

public class TradePack : MonoBehaviour
{
    void LateUpdate()
    {
        RectTransform rect = transform as RectTransform;
        if (rect == null)
            return;
        float width = rect.rect.width;
        if (width < 40f)
            width = 280f;
        float x = 10f;
        float y = -10f;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!child.gameObject.activeSelf)
                continue;
            if (child is not RectTransform item)
                continue;
            item.anchorMin = item.anchorMax = new Vector2(0f, 1f);
            item.pivot = new Vector2(0f, 1f);
            item.sizeDelta = new Vector2(68f, 68f);
            item.anchoredPosition = new Vector2(x, y);
            item.localScale = Vector3.one;
            Image icon = child.GetComponent<Image>();
            if (icon != null)
            {
                icon.type = Image.Type.Simple;
                icon.preserveAspect = true;
                icon.color = Color.white;
            }
            x += 76f;
            if (x + 68f > width - 8f)
            {
                x = 10f;
                y -= 76f;
            }
        }
    }
}

public class TradeFade : MonoBehaviour
{
    CanvasGroup group;
    float time;
    bool playing;

    public void Play(CanvasGroup target)
    {
        group = target;
        time = 0f;
        playing = true;
        if (group != null)
            group.alpha = 0f;
        transform.localScale = Vector3.one * 0.96f;
    }

    void Update()
    {
        if (!playing)
            return;
        time += Time.unscaledDeltaTime * 4.5f;
        float along = Mathf.Clamp01(time);
        float eased = 1f - Mathf.Pow(1f - along, 3f);
        if (group != null)
            group.alpha = eased;
        transform.localScale = Vector3.one * Mathf.Lerp(0.96f, 1f, eased);
        if (along >= 1f)
            playing = false;
    }
}

public class PromptChip : MonoBehaviour
{
    Vector3 home = Vector3.one;
    float time;
    bool playing;

    void Awake()
    {
        home = transform.localScale;
        if (home.sqrMagnitude < 0.01f)
            home = Vector3.one;
    }

    void OnEnable()
    {
        time = 0f;
        playing = true;
        transform.localScale = home * 0.82f;
    }

    void Update()
    {
        if (!playing)
            return;
        time += Time.unscaledDeltaTime * 8f;
        float along = Mathf.Clamp01(time);
        transform.localScale = home * Mathf.SmoothStep(0.82f, 1f, along);
        if (along >= 1f)
        {
            transform.localScale = home;
            playing = false;
        }
    }
}
