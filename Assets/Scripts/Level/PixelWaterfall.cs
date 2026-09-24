using UnityEngine;

/// <summary>
/// Procedural PixelWaterfallLit — same idea as PixelFire (noise + pixel + soft edges).
/// Sprite is only a fall region mask. Light2D reacts via Sprite-Lit path.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class PixelWaterfall : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] Material waterfallMaterial;
    [SerializeField] Color waterColor = new Color(0.35f, 0.75f, 1f, 0.65f);
    [SerializeField] Color foamColor = new Color(0.9f, 0.97f, 1f, 0.95f);
    [SerializeField] Color deepColor = new Color(0.12f, 0.4f, 0.75f, 0.8f);
    [SerializeField] float pixelSize = 56f;
    [SerializeField] float noiseScale = 4.5f;
    [SerializeField] float flowSpeed = 1.8f;
    [SerializeField] float edgeErode = 0.18f;
    [SerializeField] float density = 0.95f;
    [SerializeField] int sortingOrder = 9;

    [Header("Pool Ripples")]
    [SerializeField] bool ripplePool = true;
    [SerializeField] float rippleInterval = 0.45f;
    [SerializeField] float rippleStrength = 0.55f;
    [SerializeField] PixelWaterBody targetPool;

    SpriteRenderer bodyRenderer;
    Material runtimeMat;
    float nextRippleTime;

    void Awake() => Setup();
    void OnEnable() => Setup();

    void Setup()
    {
        bodyRenderer = GetComponent<SpriteRenderer>();
        if (bodyRenderer == null)
            return;

        // Hide old particle experiments
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform c = transform.GetChild(i);
            if (c.name.StartsWith("Particle System") || c.name == "Stream" || c.name == "Mist" || c.name == "Impact" || c.name == "Drips" || c.name == "BaseSplash")
            {
                if (Application.isPlaying) Destroy(c.gameObject);
                else DestroyImmediate(c.gameObject);
            }
        }

        bodyRenderer.color = Color.white;
        bodyRenderer.sortingOrder = sortingOrder;

        if (waterfallMaterial == null)
            waterfallMaterial = Resources.Load<Material>("PixelWaterfall");
        if (waterfallMaterial == null)
            waterfallMaterial = bodyRenderer.sharedMaterial;

        if (Application.isPlaying)
        {
            if (runtimeMat == null && waterfallMaterial != null)
                runtimeMat = new Material(waterfallMaterial);
            if (runtimeMat != null)
                bodyRenderer.material = runtimeMat;
        }
        else if (waterfallMaterial != null)
        {
            bodyRenderer.sharedMaterial = waterfallMaterial;
        }

        Apply();
    }

    void Apply()
    {
        Material mat = Application.isPlaying ? runtimeMat : (bodyRenderer != null ? bodyRenderer.sharedMaterial : null);
        if (mat == null) return;
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", waterColor);
        if (mat.HasProperty("_FoamColor")) mat.SetColor("_FoamColor", foamColor);
        if (mat.HasProperty("_DeepColor")) mat.SetColor("_DeepColor", deepColor);
        if (mat.HasProperty("_PixelSize")) mat.SetFloat("_PixelSize", pixelSize);
        if (mat.HasProperty("_NoiseScale")) mat.SetFloat("_NoiseScale", noiseScale);
        if (mat.HasProperty("_FlowSpeed")) mat.SetFloat("_FlowSpeed", flowSpeed);
        if (mat.HasProperty("_EdgeErode")) mat.SetFloat("_EdgeErode", edgeErode);
        if (mat.HasProperty("_Density")) mat.SetFloat("_Density", density);
        if (bodyRenderer != null)
            bodyRenderer.sortingOrder = sortingOrder;
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

        if (ripplePool && Time.time >= nextRippleTime)
        {
            nextRippleTime = Time.time + rippleInterval;
            if (targetPool == null)
                targetPool = FindNearestPool();
            if (targetPool != null)
            {
                // Impact near bottom-centre of the fall curtain
                Bounds b = bodyRenderer.bounds;
                Vector2 hit = new Vector2(b.center.x, b.min.y + 0.5f);
                targetPool.AddRipple(hit, rippleStrength);
            }
        }
    }

    PixelWaterBody FindNearestPool()
    {
        PixelWaterBody[] pools = FindObjectsByType<PixelWaterBody>(FindObjectsSortMode.None);
        if (pools == null || pools.Length == 0)
            return null;
        Vector2 p = transform.position;
        PixelWaterBody best = null;
        float bestDist = float.MaxValue;
        for (int i = 0; i < pools.Length; i++)
        {
            if (pools[i] == null) continue;
            float d = ((Vector2)pools[i].transform.position - p).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = pools[i];
            }
        }
        return best;
    }

    void OnValidate()
    {
        if (bodyRenderer == null) bodyRenderer = GetComponent<SpriteRenderer>();
        Apply();
    }

    void OnDestroy()
    {
        if (runtimeMat == null) return;
        if (Application.isPlaying) Destroy(runtimeMat);
        else DestroyImmediate(runtimeMat);
    }
}
