using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Procedural PixelWaterLit. Ambient surface waves + immersion splash that
/// sends real surface waves left/right (not circular ripples).
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class PixelWaterBody : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] Material waterMaterial;
    [SerializeField] Color shallowColor = new Color(0.22f, 0.58f, 0.92f, 0.5f);
    [SerializeField] Color deepColor = new Color(0.04f, 0.2f, 0.45f, 0.72f);
    [SerializeField] Color foamColor = new Color(0.92f, 0.98f, 1f, 1f);
    [SerializeField] float pixelSize = 48f;
    [SerializeField] float noiseScale = 3.5f;

    [Header("Top Edge Foam / Waves")]
    [SerializeField] [Range(0.85f, 1f)] float surfaceUV = 1f;
    [SerializeField] float waveSpeed = 1.85f;
    [Tooltip("Crest height in world units — independent of the water rectangle size.")]
    [SerializeField] float waveAmp = 0.45f;
    [Tooltip("Crest density: about this many waves on a ~70-unit-wide pool. Larger water gets more crests automatically.")]
    [SerializeField] float waveFreq = 14f;
    [Tooltip("Softness of the wavy lip, in world units.")]
    [SerializeField] float edgeSoft = 0.35f;
    [Tooltip("Foam band thickness along the surface, in world units.")]
    [SerializeField] float foamWidth = 0.7f;
    [SerializeField] float foamAmount = 1f;
    [SerializeField] float foamGurgle = 0.8f;

    [Header("Physics Match")]
    [SerializeField] bool syncBuoyancyToTop = true;
    [SerializeField] [Range(0.85f, 1f)] float buoyancySurfaceLevel = 1f;

    [Header("Splash / Immersion")]
    [SerializeField] bool interact = true;
    [SerializeField] float splashCooldown = 0.22f;
    [SerializeField] float minSplashSpeed = 1.0f;
    [SerializeField] float splashStrength = 1.15f;
    [SerializeField] bool splashParticles = true;
    [SerializeField] float shoreSplashInterval = 0.7f;
    [SerializeField] LayerMask shoreMask = ~0;

    [Tooltip("Above player (~15–17), below shore rocks (~19) so water tints the character.")]
    [SerializeField] int sortingOrder = 18;

    SpriteRenderer bodyRenderer;
    Material runtimeMat;
    BuoyancyEffector2D buoyancy;
    Collider2D waterCollider;
    ParticleSystem splashFx;

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

            shoreFilter = new ContactFilter2D
            {
                useTriggers = false,
                useLayerMask = true,
                layerMask = shoreMask,
                useDepth = false
            };

            if (splashParticles)
                EnsureSplashParticles();
        }
        else if (waterMaterial != null)
        {
            bodyRenderer.sharedMaterial = waterMaterial;
        }

        SyncBuoyancy();
        Apply();
    }

    void CleanupLegacyKids()
    {
        string[] junk = { "SurfaceWaves", "SurfaceFoam", "Bubbles", "Splash", "SplashFX" };
        // Keep SplashFX if we own it — only wipe truly legacy names
        string[] legacy = { "SurfaceWaves", "SurfaceFoam", "Bubbles", "Splash" };
        for (int i = 0; i < legacy.Length; i++)
        {
            Transform t = transform.Find(legacy[i]);
            if (t == null) continue;
            if (Application.isPlaying) Destroy(t.gameObject);
            else DestroyImmediate(t.gameObject);
        }
    }

    void EnsureSplashParticles()
    {
        Transform existing = transform.Find("SplashFX");
        if (existing != null)
        {
            splashFx = existing.GetComponent<ParticleSystem>();
            if (splashFx != null)
                return;
        }

        var go = new GameObject("SplashFX");
        go.transform.SetParent(transform, false);
        splashFx = go.AddComponent<ParticleSystem>();
        splashFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = splashFx.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = 0.35f;
        main.startLifetime = 0.45f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 12f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.7f);
        main.startColor = new Color(0.85f, 0.95f, 1f, 0.9f);
        main.gravityModifier = 1.4f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 48;

        var emission = splashFx.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 18, 28) });

        var shape = splashFx.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.4f;
        shape.rotation = new Vector3(-90f, 0f, 0f);

        var colorOverLife = splashFx.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.6f, 0.85f, 1f), 1f) },
            new[] { new GradientAlphaKey(0.95f, 0f), new GradientAlphaKey(0f, 1f) });
        colorOverLife.color = g;

        var sizeOverLife = splashFx.sizeOverLifetime;
        sizeOverLife.enabled = true;
        sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.15f));

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sortingOrder + 2;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void SyncBuoyancy()
    {
        if (!syncBuoyancyToTop || buoyancy == null)
            return;
        buoyancy.surfaceLevel = buoyancySurfaceLevel;
    }

    void Apply()
    {
        Material mat = Application.isPlaying ? runtimeMat : (bodyRenderer != null ? bodyRenderer.sharedMaterial : null);
        if (mat == null) return;

        SyncBuoyancy();

        if (mat.HasProperty("_Color")) mat.SetColor("_Color", shallowColor);
        if (mat.HasProperty("_DeepColor")) mat.SetColor("_DeepColor", deepColor);
        if (mat.HasProperty("_FoamColor")) mat.SetColor("_FoamColor", foamColor);
        if (mat.HasProperty("_PixelSize")) mat.SetFloat("_PixelSize", pixelSize);
        if (mat.HasProperty("_NoiseScale")) mat.SetFloat("_NoiseScale", noiseScale);
        if (mat.HasProperty("_WaveSpeed")) mat.SetFloat("_WaveSpeed", waveSpeed);
        if (mat.HasProperty("_WaveAmp")) mat.SetFloat("_WaveAmp", waveAmp);
        if (mat.HasProperty("_WaveFreq")) mat.SetFloat("_WaveFreq", waveFreq);
        if (mat.HasProperty("_SurfaceUV")) mat.SetFloat("_SurfaceUV", surfaceUV);
        if (mat.HasProperty("_EdgeSoft")) mat.SetFloat("_EdgeSoft", edgeSoft);
        if (mat.HasProperty("_FoamWidth")) mat.SetFloat("_FoamWidth", foamWidth);
        if (mat.HasProperty("_FoamAmount")) mat.SetFloat("_FoamAmount", foamAmount);
        if (mat.HasProperty("_FoamGurgle")) mat.SetFloat("_FoamGurgle", foamGurgle);

        if (mat.HasProperty("_RectSize") && bodyRenderer != null)
        {
            Vector3 s = bodyRenderer.bounds.size;
            mat.SetVector("_RectSize", new Vector4(Mathf.Max(s.x, 0.01f), Mathf.Max(s.y, 0.01f), 0f, 0f));
        }

        if (bodyRenderer != null)
            bodyRenderer.sortingOrder = sortingOrder;
    }

    /// <summary>Immersion splash particles only (no running surface waves).</summary>
    public void AddSplash(Vector2 worldPos, float strength = 1f)
    {
        if (!Application.isPlaying)
            return;
        if (runtimeMat == null)
            Setup();

        float s = Mathf.Clamp01(strength) * splashStrength;

        if (splashParticles)
        {
            if (splashFx == null)
                EnsureSplashParticles();
            if (splashFx != null)
            {
                splashFx.transform.position = new Vector3(worldPos.x, GetSurfaceWorldY(), transform.position.z);
                var main = splashFx.main;
                main.startSpeed = new ParticleSystem.MinMaxCurve(3f + s * 6f, 8f + s * 10f);
                int count = Mathf.RoundToInt(Mathf.Lerp(14f, 32f, s));
                splashFx.Emit(count);
            }
        }
    }

    /// <summary>Kept for waterfall / callers; particle splash only.</summary>
    public void AddRipple(Vector2 worldPos, float strength = 1f) => AddSplash(worldPos, strength);

    public float GetSurfaceWorldY()
    {
        if (bodyRenderer == null)
            bodyRenderer = GetComponent<SpriteRenderer>();
        if (bodyRenderer == null)
            return transform.position.y;

        Bounds b = bodyRenderer.bounds;
        return Mathf.Lerp(b.min.y, b.max.y, surfaceUV);
    }

    void Update()
    {
        if (bodyRenderer == null)
        {
            Setup();
            return;
        }

        Apply();
        if (Application.isPlaying && interact)
            TickShoreContacts();
    }

    void TickShoreContacts()
    {
        if (waterCollider == null || Time.time - lastShoreTime < shoreSplashInterval)
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

            Rigidbody2D rb = c.attachedRigidbody;
            Bounds b = c.bounds;
            bool nearSurface = b.min.y <= surfaceY + 1.5f && b.max.y >= surfaceY - 2.5f;
            if (!nearSurface)
                continue;

            bool isStatic = rb == null || rb.bodyType == RigidbodyType2D.Static;
            if (!isStatic)
                continue;

            float x = b.center.x < waterCollider.bounds.center.x
                ? Mathf.Min(b.max.x, waterCollider.bounds.max.x)
                : Mathf.Max(b.min.x, waterCollider.bounds.min.x);

            lastShoreTime = Time.time;
            AddSplash(new Vector2(x, surfaceY), 0.3f);
            break;
        }
    }

    void OnTriggerEnter2D(Collider2D other) => TryFallInSplash(other);

    void TryFallInSplash(Collider2D other)
    {
        if (!interact || !Application.isPlaying || other == null)
            return;
        if (Time.time - lastSplashTime < splashCooldown)
            return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null)
            return;

        // Only when falling into the water from above — not while swimming under the surface
        float vy = rb.linearVelocity.y;
        if (vy >= -minSplashSpeed * 0.35f)
            return;

        float surfaceY = GetSurfaceWorldY();
        Bounds b = other.bounds;
        // Must be crossing the free surface (body still overlapping the lip), not deep underwater
        if (b.min.y > surfaceY + 1.5f)
            return;
        if (b.max.y < surfaceY - 0.75f)
            return;

        float speed = rb.linearVelocity.magnitude;
        float dive = Mathf.Clamp01((-vy) / 10f);
        float strength = Mathf.Clamp01(0.4f + speed / 7f + dive * 0.55f);

        lastSplashTime = Time.time;
        AddSplash(new Vector2(b.center.x, surfaceY), strength);
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
