using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Procedural PixelWaterLit: body look from noise, free-surface waves/foam at
/// BuoyancyEffector2D surface, ripples from contacts / AddRipple.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class PixelWaterBody : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] Material waterMaterial;
    [SerializeField] Color shallowColor = new Color(0.25f, 0.65f, 0.95f, 0.55f);
    [SerializeField] Color deepColor = new Color(0.05f, 0.25f, 0.5f, 0.75f);
    [SerializeField] Color foamColor = new Color(0.85f, 0.95f, 1f, 0.9f);
    [SerializeField] float pixelSize = 48f;
    [SerializeField] float noiseScale = 3.5f;

    [Header("Free Surface Waves")]
    [Tooltip("If true, surface UV follows BuoyancyEffector2D.surfaceLevel.")]
    [SerializeField] bool useBuoyancySurface = true;
    [SerializeField] [Range(0f, 1f)] float surfaceUV = 0.75f;
    [SerializeField] float waveSpeed = 0.7f;
    [SerializeField] float waveAmp = 0.022f;
    [SerializeField] float waveFreq = 0.55f;
    [SerializeField] float surfaceSoft = 0.018f;
    [SerializeField] float foamAmount = 0.85f;
    [SerializeField] float foamSoft = 2.4f;
    [SerializeField] float foamGurgle = 0.75f;

    [Header("Interaction")]
    [SerializeField] bool interact = true;
    [SerializeField] float splashCooldown = 0.18f;
    [SerializeField] float minSplashSpeed = 1.1f;
    [SerializeField] float rippleStrength = 0.9f;
    [SerializeField] float shoreRippleInterval = 0.55f;
    [SerializeField] LayerMask shoreMask = ~0;

    [Tooltip("Must be below player sorting (~15+) so the character draws on top of the pool.")]
    [SerializeField] int sortingOrder = 8;

    SpriteRenderer bodyRenderer;
    Material runtimeMat;
    BuoyancyEffector2D buoyancy;
    Collider2D waterCollider;

    readonly Vector4[] ripples = new Vector4[3];
    int rippleIndex;
    float lastSplashTime = -999f;
    float lastShoreTime = -999f;
    readonly List<Collider2D> overlapBuf = new List<Collider2D>(24);
    ContactFilter2D shoreFilter;

    void Awake() => Setup();
    void OnEnable() => Setup();

    void Setup()
    {
        bodyRenderer = GetComponent<SpriteRenderer>();
        if (bodyRenderer == null)
            return;

        buoyancy = GetComponent<BuoyancyEffector2D>();
        waterCollider = GetComponent<Collider2D>();
        CleanupLegacyKids();

        bodyRenderer.color = Color.white;
        bodyRenderer.sortingOrder = sortingOrder;

        if (waterMaterial == null)
            waterMaterial = Resources.Load<Material>("PixelWater");
        if (waterMaterial == null)
            waterMaterial = bodyRenderer.sharedMaterial;

        if (Application.isPlaying)
        {
            if (runtimeMat == null && waterMaterial != null)
                runtimeMat = new Material(waterMaterial);
            if (runtimeMat != null)
                bodyRenderer.material = runtimeMat;

            for (int i = 0; i < ripples.Length; i++)
                ripples[i] = new Vector4(0f, 0f, -999f, 0f);

            shoreFilter = new ContactFilter2D
            {
                useTriggers = false,
                useLayerMask = true,
                layerMask = shoreMask,
                useDepth = false
            };
        }
        else if (waterMaterial != null)
        {
            bodyRenderer.sharedMaterial = waterMaterial;
        }

        SyncSurfaceFromBuoyancy();
        Apply();
    }

    void CleanupLegacyKids()
    {
        string[] junk = { "SurfaceWaves", "SurfaceFoam", "Bubbles", "Splash" };
        for (int i = 0; i < junk.Length; i++)
        {
            Transform t = transform.Find(junk[i]);
            if (t == null) continue;
            if (Application.isPlaying) Destroy(t.gameObject);
            else DestroyImmediate(t.gameObject);
        }
    }

    void SyncSurfaceFromBuoyancy()
    {
        if (!useBuoyancySurface || buoyancy == null)
            return;

        // Unity: surfaceLevel 0 = collider centre, 1 = top.
        var box = waterCollider as BoxCollider2D;
        if (box != null)
        {
            float localY = buoyancy.surfaceLevel * box.size.y * 0.5f + box.offset.y;
            surfaceUV = localY / box.size.y + 0.5f;
        }
        else if (waterCollider != null)
        {
            Bounds b = waterCollider.bounds;
            float surfaceWorldY = transform.position.y + buoyancy.surfaceLevel * b.extents.y;
            float localY = transform.InverseTransformPoint(new Vector3(transform.position.x, surfaceWorldY, 0f)).y;
            // Approximate UV for centred sprite
            float half = b.extents.y / Mathf.Max(Mathf.Abs(transform.lossyScale.y), 0.0001f);
            surfaceUV = half > 0.0001f ? localY / (half * 2f) + 0.5f : 0.75f;
        }

        surfaceUV = Mathf.Clamp01(surfaceUV);
    }

    void Apply()
    {
        Material mat = Application.isPlaying ? runtimeMat : (bodyRenderer != null ? bodyRenderer.sharedMaterial : null);
        if (mat == null) return;

        SyncSurfaceFromBuoyancy();

        if (mat.HasProperty("_Color")) mat.SetColor("_Color", shallowColor);
        if (mat.HasProperty("_DeepColor")) mat.SetColor("_DeepColor", deepColor);
        if (mat.HasProperty("_FoamColor")) mat.SetColor("_FoamColor", foamColor);
        if (mat.HasProperty("_PixelSize")) mat.SetFloat("_PixelSize", pixelSize);
        if (mat.HasProperty("_NoiseScale")) mat.SetFloat("_NoiseScale", noiseScale);
        if (mat.HasProperty("_WaveSpeed")) mat.SetFloat("_WaveSpeed", waveSpeed);
        if (mat.HasProperty("_WaveAmp")) mat.SetFloat("_WaveAmp", waveAmp);
        if (mat.HasProperty("_WaveFreq")) mat.SetFloat("_WaveFreq", waveFreq);
        if (mat.HasProperty("_SurfaceUV")) mat.SetFloat("_SurfaceUV", surfaceUV);
        if (mat.HasProperty("_SurfaceSoft")) mat.SetFloat("_SurfaceSoft", surfaceSoft);
        if (mat.HasProperty("_FoamAmount")) mat.SetFloat("_FoamAmount", foamAmount);
        if (mat.HasProperty("_FoamSoft")) mat.SetFloat("_FoamSoft", foamSoft);
        if (mat.HasProperty("_FoamGurgle")) mat.SetFloat("_FoamGurgle", foamGurgle);

        if (Application.isPlaying)
        {
            if (mat.HasProperty("_Ripple1")) mat.SetVector("_Ripple1", ripples[0]);
            if (mat.HasProperty("_Ripple2")) mat.SetVector("_Ripple2", ripples[1]);
            if (mat.HasProperty("_Ripple3")) mat.SetVector("_Ripple3", ripples[2]);
        }

        if (bodyRenderer != null)
            bodyRenderer.sortingOrder = sortingOrder;
    }

    /// <summary>Spawn a surface ripple at a world position (e.g. waterfall impact).</summary>
    public void AddRipple(Vector2 worldPos, float strength = 1f)
    {
        if (!Application.isPlaying)
            return;
        if (runtimeMat == null)
            Setup();

        float surfaceY = GetSurfaceWorldY();
        worldPos.y = surfaceY;
        ripples[rippleIndex] = new Vector4(worldPos.x, worldPos.y, Time.time, Mathf.Clamp01(strength) * rippleStrength);
        rippleIndex = (rippleIndex + 1) % ripples.Length;
        Apply();
    }

    public float GetSurfaceWorldY()
    {
        if (buoyancy != null && waterCollider != null)
            return transform.position.y + buoyancy.surfaceLevel * waterCollider.bounds.extents.y;

        // Fallback from UV
        float half = bodyRenderer != null ? bodyRenderer.bounds.extents.y : 1f;
        return transform.position.y + (surfaceUV - 0.5f) * 2f * half;
    }

    void Update()
    {
        if (bodyRenderer == null)
        {
            Setup();
            return;
        }

        if (!Application.isPlaying)
        {
            Apply();
            return;
        }

        Apply();
        if (interact)
            TickShoreContacts();
    }

    void TickShoreContacts()
    {
        if (waterCollider == null || Time.time - lastShoreTime < shoreRippleInterval)
            return;

        overlapBuf.Clear();
        int count = waterCollider.Overlap(shoreFilter, overlapBuf);
        if (count <= 0)
            return;

        float surfaceY = GetSurfaceWorldY();
        for (int i = 0; i < count; i++)
        {
            Collider2D c = overlapBuf[i];
            if (c == null || c.transform.IsChildOf(transform))
                continue;

            // Static / kinematic near free surface → small shore foam ripple
            Rigidbody2D rb = c.attachedRigidbody;
            Bounds b = c.bounds;
            bool nearSurface = b.min.y <= surfaceY + 1.5f && b.max.y >= surfaceY - 2.5f;
            if (!nearSurface)
                continue;

            bool isStatic = rb == null || rb.bodyType == RigidbodyType2D.Static;
            if (!isStatic)
                continue;

            float x = Mathf.Clamp(b.center.x, waterCollider.bounds.min.x, waterCollider.bounds.max.x);
            if (b.center.x < waterCollider.bounds.center.x)
                x = Mathf.Min(b.max.x, waterCollider.bounds.max.x);
            else
                x = Mathf.Max(b.min.x, waterCollider.bounds.min.x);

            lastShoreTime = Time.time;
            AddRipple(new Vector2(x, surfaceY), 0.35f);
            break;
        }
    }

    void OnTriggerEnter2D(Collider2D other) => TrySplash(other, true);
    void OnTriggerStay2D(Collider2D other) => TrySplash(other, false);

    void TrySplash(Collider2D other, bool entered)
    {
        if (!interact || !Application.isPlaying || other == null)
            return;
        if (Time.time - lastSplashTime < splashCooldown)
            return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null)
            return;

        float speed = rb.linearVelocity.magnitude;
        if (!entered && speed < minSplashSpeed)
            return;
        if (entered && speed < minSplashSpeed * 0.35f)
            return;

        float surfaceY = GetSurfaceWorldY();
        Bounds b = other.bounds;
        // Only splash near free surface, not deep underwater
        if (b.min.y > surfaceY + 2f || b.max.y < surfaceY - 3f)
            return;

        float strength = entered
            ? Mathf.Clamp01(0.4f + speed / 8f)
            : Mathf.Clamp01(speed / 10f);

        lastSplashTime = Time.time;
        AddRipple(new Vector2(b.center.x, surfaceY), strength);
    }

    void OnValidate()
    {
        if (bodyRenderer == null) bodyRenderer = GetComponent<SpriteRenderer>();
        if (buoyancy == null) buoyancy = GetComponent<BuoyancyEffector2D>();
        if (waterCollider == null) waterCollider = GetComponent<Collider2D>();
        Apply();
    }

    void OnDestroy()
    {
        if (runtimeMat == null) return;
        if (Application.isPlaying) Destroy(runtimeMat);
        else DestroyImmediate(runtimeMat);
    }
}
