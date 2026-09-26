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

public class MiniMapHud : MonoBehaviour, IPointerClickHandler
{
    public static MiniMapHud Live { get; private set; }

    const float Border = 2f;
    const float MaxInnerW = 206f;
    const float MaxInnerH = 104f;

    [SerializeField] RawImage picture;
    [SerializeField] RectTransform dot;

    Transform player;
    Camera eye;
    RenderTexture rt;
    int texW;
    int texH;
    RectTransform panel;
    CanvasGroup group;
    readonly List<GameObject> covers = new List<GameObject>();
    bool built;
    bool framed;
    bool locked;
    int fits;
    float nextFit;

    public static bool TrySpawn()
    {
        if (Live != null && Live.built)
            return true;
        if (Camera.main == null)
            return false;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
            return false;
        MiniMapHud hud = playerObject.GetComponentInChildren<MiniMapHud>(true);
        if (hud == null)
            return false;
        Live = hud;
        hud.Wake(playerObject.transform);
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

    void Wake(Transform playerTransform)
    {
        if (built)
            return;
        player = playerTransform;
        panel = transform as RectTransform;
        if (picture == null)
            picture = GetComponentInChildren<RawImage>(true);
        if (dot == null)
        {
            Transform mark = transform.Find("View/You");
            if (mark != null)
                dot = mark as RectTransform;
        }
        group = GetComponent<CanvasGroup>();
        CacheCovers(playerTransform.gameObject);

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
        EnsureRt(352, 136);
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
        if (!show && eye != null)
            eye.enabled = false;
        if (group != null)
        {
            group.blocksRaycasts = show;
            group.interactable = show;
            if (!show)
                group.alpha = 0f;
            else if (framed && group.alpha < 1f)
                group.alpha = Mathf.MoveTowards(group.alpha, 1f, Time.unscaledDeltaTime * 3.2f);
        }
    }

    void LateUpdate()
    {
        if (!built || eye == null || player == null || panel == null)
            return;
        if (OtherMenuOpen() || AtlasOpen())
        {
            eye.enabled = false;
            return;
        }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleAtlas();
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
