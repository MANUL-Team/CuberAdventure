using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MiniMapBoot : MonoBehaviour
{
    float until;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Hook()
    {
        SceneManager.sceneLoaded += (_, __) => Begin();
        Begin();
    }

    static void Begin()
    {
        if (MiniMapHud.Live != null)
            return;
        var go = new GameObject("MiniMapBoot");
        go.AddComponent<MiniMapBoot>();
    }

    void OnEnable()
    {
        until = Time.unscaledTime + 4f;
    }

    void Update()
    {
        if (MiniMapHud.TrySpawn() || Time.unscaledTime > until)
            Destroy(gameObject);
    }
}

public class MiniMapHud : MonoBehaviour
{
    public static MiniMapHud Live { get; private set; }

    const float Width = 176f;
    const float Height = 70f;
    const float Border = 2f;
    const float MaxInnerW = 206f;
    const float MaxInnerH = 104f;

    static Sprite pixel;

    Transform player;
    Camera eye;
    RenderTexture rt;
    RawImage picture;
    int texW;
    int texH;
    RectTransform panel;
    RectTransform view;
    RectTransform dot;
    CanvasGroup group;
    readonly List<GameObject> covers = new List<GameObject>();
    bool built;
    bool framed;
    bool locked;
    int fits;
    float nextFit;

    public static bool TrySpawn()
    {
        if (Live != null)
            return true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || Camera.main == null)
            return false;
        Canvas hud = null;
        Canvas[] canvases = player.GetComponentsInChildren<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i].transform.Find("LeftUp") != null)
            {
                hud = canvases[i];
                break;
            }
        }
        if (hud == null || hud.transform.lossyScale.sqrMagnitude < 0.0001f)
            return false;

        var host = new GameObject("MiniMap");
        Live = host.AddComponent<MiniMapHud>();
        Live.Build(player.transform, hud);
        return true;
    }

    void OnDestroy()
    {
        if (Live == this)
            Live = null;
        if (eye != null)
        {
            eye.targetTexture = null;
            Destroy(eye.gameObject);
        }
        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
        }
    }

    void Build(Transform playerTransform, Canvas hud)
    {
        player = playerTransform;
        CacheCovers(playerTransform.gameObject);
        EnsurePixel();

        panel = NewRect("Panel", hud.transform);
        panel.gameObject.layer = hud.gameObject.layer;
        PlaceAtMapButton(hud.transform);
        panel.sizeDelta = new Vector2(Width, Height);

        Canvas layer = panel.gameObject.AddComponent<Canvas>();
        layer.overrideSorting = true;
        layer.sortingOrder = -1;
        panel.gameObject.AddComponent<GraphicRaycaster>();

        Image frame = panel.gameObject.AddComponent<Image>();
        frame.sprite = pixel;
        frame.type = Image.Type.Simple;
        frame.color = new Color(0.16f, 0.10f, 0.06f, 0.88f);
        frame.raycastTarget = true;
        MiniMapWindow window = panel.gameObject.AddComponent<MiniMapWindow>();
        window.hud = this;

        group = panel.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        view = NewRect("View", panel);
        Stretch(view, Border, Border, Border, Border);
        view.gameObject.AddComponent<RectMask2D>();

        picture = view.gameObject.AddComponent<RawImage>();
        picture.raycastTarget = false;
        picture.color = Color.white;
        EnsureRt(352, 136);

        dot = NewRect("You", view);
        dot.anchorMin = dot.anchorMax = new Vector2(0.5f, 0.5f);
        dot.pivot = new Vector2(0.5f, 0.5f);
        dot.sizeDelta = new Vector2(7f, 7f);
        Image mark = dot.gameObject.AddComponent<Image>();
        mark.sprite = CoreSprite();
        mark.color = Color.white;
        mark.raycastTarget = false;
        mark.preserveAspect = true;

        eye = new GameObject("MinimapCamera").AddComponent<Camera>();
        eye.enabled = false;
        eye.orthographic = true;
        eye.orthographicSize = 24f;
        eye.clearFlags = CameraClearFlags.SolidColor;
        Color sky = Camera.main.backgroundColor;
        sky.a = 1f;
        eye.backgroundColor = sky;
        eye.cullingMask = TerrainMask(Camera.main.cullingMask);
        eye.nearClipPlane = Camera.main.nearClipPlane;
        eye.farClipPlane = Camera.main.farClipPlane;
        eye.depth = -20f;
        eye.allowHDR = false;
        eye.allowMSAA = false;
        eye.useOcclusionCulling = false;
        UniversalAdditionalCameraData mainData = Camera.main.GetUniversalAdditionalCameraData();
        UniversalAdditionalCameraData data = eye.GetUniversalAdditionalCameraData();
        data.renderType = CameraRenderType.Base;
        data.renderPostProcessing = false;
        data.renderShadows = false;
        data.requiresDepthTexture = false;
        data.requiresColorTexture = false;
        data.antialiasing = AntialiasingMode.None;
        data.SetRenderer(RendererIndex(mainData));
        eye.targetTexture = rt;
        eye.aspect = (float)rt.width / rt.height;
        ReleaseTitleClicks();

        built = true;
        nextFit = 0f;
    }

    void Update()
    {
        if (!built || panel == null)
            return;
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
                player = found.transform;
        }

        bool menu = OtherMenuOpen();
        bool atlas = AtlasOpen();
        if (Input.GetKeyDown(KeyCode.M) && !Typing() && !menu)
            ToggleAtlas();

        bool show = player != null && !menu && !atlas;
        if (panel.gameObject.activeSelf != show)
            panel.gameObject.SetActive(show);
        if (!show && eye != null)
            eye.enabled = false;
        if (show && framed && group.alpha < 1f)
            group.alpha = Mathf.MoveTowards(group.alpha, 1f, Time.unscaledDeltaTime * 3.2f);
    }

    void LateUpdate()
    {
        if (!built || eye == null || player == null || panel == null || !panel.gameObject.activeSelf)
            return;

        if (!locked && Time.unscaledTime >= nextFit)
        {
            nextFit = Time.unscaledTime + 0.5f;
            Fit();
        }

        eye.enabled = framed;
        if (!framed)
            return;
        if (Camera.main != null)
        {
            Color sky = Camera.main.backgroundColor;
            sky.a = 1f;
            eye.backgroundColor = sky;
        }

        PlaceDot();
    }

    void Fit()
    {
        fits++;
        if (TryMeasure(out Bounds box))
        {
            ApplyFrame(box);
            framed = true;
        }
        if (framed && fits >= 3)
            locked = true;
    }

    void ApplyFrame(Bounds box)
    {
        float worldW = Mathf.Max(1f, box.size.x) + 0.5f;
        float worldH = Mathf.Max(1f, box.size.y) + 0.5f;
        float aspect = worldW / worldH;
        float viewW = MaxInnerW;
        float viewH = viewW / aspect;
        if (viewH > MaxInnerH)
        {
            viewH = MaxInnerH;
            viewW = viewH * aspect;
        }
        panel.sizeDelta = new Vector2(viewW + Border * 2f, viewH + Border * 2f);

        int pixelsW = 320;
        int pixelsH = Mathf.RoundToInt(pixelsW * (viewH / viewW));
        if (pixelsH > 240)
        {
            pixelsH = 240;
            pixelsW = Mathf.Max(8, Mathf.RoundToInt(pixelsH * (viewW / viewH)));
        }
        else if (pixelsH < 8)
        {
            pixelsH = 8;
            pixelsW = Mathf.Max(8, Mathf.RoundToInt(pixelsH * (viewW / viewH)));
        }
        EnsureRt(pixelsW, pixelsH);

        eye.aspect = viewW / viewH;
        eye.orthographicSize = worldH * 0.5f;
        float z = Camera.main != null ? Camera.main.transform.position.z : -10f;
        eye.transform.position = new Vector3(box.center.x, box.center.y, z);
    }

    void PlaceDot()
    {
        Vector3 vp = eye.WorldToViewportPoint(player.position);
        bool onMap = vp.z > 0f;
        if (dot.gameObject.activeSelf != onMap)
            dot.gameObject.SetActive(onMap);
        if (!onMap)
            return;
        float x = Mathf.Clamp(vp.x, 0.04f, 0.96f);
        float y = Mathf.Clamp(vp.y, 0.08f, 0.92f);
        dot.anchorMin = dot.anchorMax = new Vector2(x, y);
        dot.anchoredPosition = Vector2.zero;
    }

    bool TryMeasure(out Bounds box)
    {
        bool any = false;
        box = new Bounds(Vector3.zero, Vector3.zero);
        Tilemap[] maps = FindObjectsByType<Tilemap>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < maps.Length; i++)
        {
            Tilemap map = maps[i];
            if (map == null || UnderParallax(map.transform))
                continue;
            if (!TileWorld(map, out Bounds world))
                continue;
            if (!any)
                box = world;
            else
                box.Encapsulate(world);
            any = true;
        }
        if (any && box.size.x > 3f && box.size.y > 1f)
            return true;

        any = false;
        int ground = LayerMask.NameToLayer("Ground");
        int notGround = LayerMask.NameToLayer("NotGround");
        int walk = LayerMask.NameToLayer("ColliderForPlayer");
        Collider2D[] cols = FindObjectsByType<Collider2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < cols.Length; i++)
        {
            Collider2D col = cols[i];
            if (col == null || UnderParallax(col.transform))
                continue;
            int layer = col.gameObject.layer;
            bool terrain = col is TilemapCollider2D || col is CompositeCollider2D || layer == ground || layer == notGround || layer == walk;
            if (!terrain)
                continue;
            if (!Take(col.bounds, ref box, ref any))
                continue;
        }
        if (!any)
            any |= TakeScenery(ref box);
        return any && box.size.x > 3f && box.size.y > 1f;
    }

    static bool TileWorld(Tilemap map, out Bounds world)
    {
        world = default;
        map.CompressBounds();
        Bounds local = map.localBounds;
        if (local.size.x < 0.2f || local.size.y < 0.05f)
            return false;
        Vector3 scale = map.transform.lossyScale;
        Vector3 size = new Vector3(Mathf.Abs(local.size.x * scale.x), Mathf.Abs(local.size.y * scale.y), 0f);
        if (size.x < 2f || size.y < 0.4f || size.x > 4000f || size.y > 1500f)
            return false;
        world = new Bounds(map.transform.TransformPoint(local.center), size);
        return true;
    }

    static bool UnderParallax(Transform t)
    {
        while (t != null)
        {
            if (t.name == "Parallax")
                return true;
            t = t.parent;
        }
        return false;
    }

    static bool TakeScenery(ref Bounds box)
    {
        bool any = false;
        int ui = LayerMask.NameToLayer("UI");
        int playerLayer = LayerMask.NameToLayer("Player");
        int skin = LayerMask.NameToLayer("Skin");
        int mob = LayerMask.NameToLayer("Mob");
        int bmob = LayerMask.NameToLayer("BMob");
        SpriteRenderer[] sprites = FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < sprites.Length; i++)
        {
            SpriteRenderer sprite = sprites[i];
            int layer = sprite.gameObject.layer;
            if (layer == ui || layer == playerLayer || layer == skin || layer == mob || layer == bmob)
                continue;
            Bounds b = sprite.bounds;
            if (b.size.x < 0.4f || b.size.y < 0.4f || b.size.x > 48f || b.size.y > 36f)
                continue;
            if (!any && box.size.sqrMagnitude < 0.01f)
            {
                box = b;
                any = true;
                continue;
            }
            box.Encapsulate(b);
            any = true;
        }
        return any;
    }

    static bool Take(Bounds candidate, ref Bounds box, ref bool any)
    {
        if (float.IsNaN(candidate.center.x) || float.IsNaN(candidate.size.x))
            return false;
        if (candidate.size.x < 0.4f || candidate.size.y < 0.15f)
            return false;
        if (candidate.size.x > 4000f || candidate.size.y > 1500f)
            return false;
        if (!any || box.size.sqrMagnitude < 0.01f)
            box = candidate;
        else
            box.Encapsulate(candidate);
        any = true;
        return true;
    }

    void PlaceAtMapButton(Transform hud)
    {
        Transform left = hud.Find("LeftUp");
        RectTransform button = left != null ? left.Find("MapButton") as RectTransform : null;
        panel.anchorMin = panel.anchorMax = new Vector2(0f, 1f);
        panel.pivot = new Vector2(0f, 1f);
        panel.anchoredPosition = new Vector2(8f, -8f);
        if (button == null)
            return;
        panel.SetParent(button.parent, false);
        panel.anchorMin = button.anchorMin;
        panel.anchorMax = button.anchorMax;
        panel.pivot = new Vector2(0f, 1f);
        Vector2 topLeft = button.anchoredPosition;
        topLeft.x -= button.sizeDelta.x * button.pivot.x;
        topLeft.y += button.sizeDelta.y * (1f - button.pivot.y);
        panel.anchoredPosition = topLeft;
        button.gameObject.SetActive(false);
    }

    public void ToggleAtlas()
    {
        if (player == null || OtherMenuOpen())
            return;
        OpenWorldMap opener = player.GetComponentInChildren<OpenWorldMap>(true);
        if (opener == null)
            return;
        if (AtlasOpen())
            opener.CloseMap();
        else
            opener.OpenMap();
    }

    void CacheCovers(GameObject playerObject)
    {
        covers.Clear();
        Transform[] all = playerObject.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
        {
            string name = all[i].name;
            if (name == "Map" || name == "GameMenu" || name == "SkillsMenu" || name == "Inv" || name == "Inventory")
                covers.Add(all[i].gameObject);
        }
    }

    bool OtherMenuOpen()
    {
        for (int i = 0; i < covers.Count; i++)
        {
            GameObject go = covers[i];
            if (go == null || go.name == "Map")
                continue;
            if (go.activeInHierarchy)
                return true;
        }
        return false;
    }

    bool AtlasOpen()
    {
        for (int i = 0; i < covers.Count; i++)
        {
            GameObject go = covers[i];
            if (go != null && go.name == "Map" && go.activeInHierarchy)
                return true;
        }
        return false;
    }

    static bool Typing()
    {
        EventSystem events = EventSystem.current;
        if (events == null || events.currentSelectedGameObject == null)
            return false;
        return events.currentSelectedGameObject.GetComponent<InputField>() != null;
    }

    void EnsureRt(int w, int h)
    {
        if (rt != null && texW == w && texH == h)
            return;
        if (eye != null)
            eye.targetTexture = null;
        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
        }
        rt = new RenderTexture(w, h, 16, RenderTextureFormat.ARGB32)
        {
            name = "MiniMapRT",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            antiAliasing = 1
        };
        rt.Create();
        texW = w;
        texH = h;
        if (picture != null)
            picture.texture = rt;
        if (eye != null)
        {
            eye.targetTexture = rt;
            eye.aspect = (float)w / h;
        }
    }

    static void ReleaseTitleClicks()
    {
        LangText[] labels = FindObjectsByType<LangText>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i].gameObject.name != "Name" || labels[i].GetComponent<Animator>() == null)
                continue;
            Text text = labels[i].GetComponent<Text>();
            if (text != null)
                text.raycastTarget = false;
        }
    }

    static int TerrainMask(int mainMask)
    {
        int mask = mainMask;
        mask &= ~(1 << 5);
        int playerLayer = LayerMask.NameToLayer("Player");
        int skin = LayerMask.NameToLayer("Skin");
        int mob = LayerMask.NameToLayer("Mob");
        int bmob = LayerMask.NameToLayer("BMob");
        if (playerLayer >= 0)
            mask &= ~(1 << playerLayer);
        if (skin >= 0)
            mask &= ~(1 << skin);
        if (mob >= 0)
            mask &= ~(1 << mob);
        if (bmob >= 0)
            mask &= ~(1 << bmob);
        return mask;
    }

    static int RendererIndex(UniversalAdditionalCameraData data)
    {
        if (data == null)
            return 0;
        FieldInfo field = typeof(UniversalAdditionalCameraData).GetField("m_RendererIndex", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
            return 0;
        int index = (int)field.GetValue(data);
        return index < 0 ? 0 : index;
    }

    static Sprite coreSprite;

    static Sprite CoreSprite()
    {
        if (coreSprite != null)
            return coreSprite;
        const int n = 7;
        Texture2D tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color hot = new Color(1f, 0.86f, 0.76f, 1f);
        Color bright = new Color(1f, 0.40f, 0.28f, 1f);
        Color red = new Color(0.80f, 0.16f, 0.13f, 1f);
        Color deep = new Color(0.55f, 0.08f, 0.08f, 1f);
        Color rim = new Color(0.26f, 0.04f, 0.05f, 1f);
        string rows =
            "..rrr.." +
            ".rdhrd." +
            "rdhbhrd" +
            "rdb*brd" +
            "rddhddr" +
            ".rdddr." +
            "..rrr..";
        for (int row = 0; row < n; row++)
        {
            for (int col = 0; col < n; col++)
            {
                char ink = rows[row * n + col];
                Color color = Color.clear;
                if (ink == '*')
                    color = hot;
                else if (ink == 'b')
                    color = bright;
                else if (ink == 'h')
                    color = red;
                else if (ink == 'd')
                    color = deep;
                else if (ink == 'r')
                    color = rim;
                tex.SetPixel(col, n - 1 - row, color);
            }
        }
        tex.Apply();
        tex.hideFlags = HideFlags.HideAndDontSave;
        coreSprite = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), 1f);
        coreSprite.hideFlags = HideFlags.HideAndDontSave;
        return coreSprite;
    }

    static void EnsurePixel()
    {
        if (pixel != null)
            return;
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        tex.hideFlags = HideFlags.HideAndDontSave;
        pixel = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        pixel.hideFlags = HideFlags.HideAndDontSave;
    }

    static RectTransform NewRect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.localScale = Vector3.one;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    static Image AddImage(RectTransform rect, Sprite sprite, Color color)
    {
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        return image;
    }

    static void Stretch(RectTransform rect, float left, float bottom, float right, float top)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }
}

public class MiniMapWindow : MonoBehaviour, IPointerClickHandler
{
    public MiniMapHud hud;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hud != null)
            hud.ToggleAtlas();
    }
}
