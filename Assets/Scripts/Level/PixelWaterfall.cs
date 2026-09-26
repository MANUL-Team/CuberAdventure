using UnityEngine;

/// <summary>
/// Side-view waterfall from a cliff: dense soft stream falls down (and a bit right),
/// collides with rocks, splashes foam on the pool. Sprite sheet stays hidden.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class PixelWaterfall : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] bool hideSpriteSheet = true;
    [SerializeField] Color waterColor = new Color(0.5f, 0.85f, 1f, 0.8f);
    [SerializeField] Color foamColor = new Color(0.92f, 0.98f, 1f, 0.95f);
    [SerializeField] int sortingOrder = 18;

    [Header("Stream (dense / soft)")]
    [Tooltip("Spawn a bit off the cliff lip so drops are not born inside rock colliders.")]
    [SerializeField] Vector2 emitOffset = new Vector2(2.8f, -1.4f);
    [SerializeField] Vector2 emitBox = new Vector2(4.2f, 1.6f);
    [SerializeField] float emitRate = 340f;
    [Tooltip("World push: soft cascade down the cliff, slight right.")]
    [SerializeField] Vector2 fallVelocity = new Vector2(2.0f, -3.8f);
    [SerializeField] float velocityJitter = 1.1f;
    [SerializeField] float gravityModifier = 1.05f;
    [SerializeField] float dropSize = 1.35f;
    [SerializeField] float lifetime = 5.5f;

    [Header("Mist")]
    [SerializeField] float mistRate = 70f;
    [SerializeField] float mistSize = 2.6f;

    [Header("Rock Collision")]
    [SerializeField] bool collideWithRocks = true;
    [SerializeField] float rockBounce = 0.18f;
    [SerializeField] float rockDampen = 0.55f;
    [Tooltip("Layers particles collide with (exclude Water trigger layer).")]
    [SerializeField] LayerMask rockMask = ~0;

    [Header("Pool Impact")]
    [SerializeField] bool splashOnPool = true;
    [SerializeField] float bounce = 0.28f;
    [SerializeField] float sidewaysKick = 3.5f;
    [SerializeField] float foamBurst = 8f;
    [SerializeField] PixelWaterBody targetPool;

    [Header("Pool Splash Cue")]
    [SerializeField] bool notifyPool = true;
    [SerializeField] float poolSplashInterval = 0.4f;
    [SerializeField] float poolSplashStrength = 0.35f;

    SpriteRenderer bodyRenderer;
    ParticleSystem fall;
    ParticleSystem mist;
    ParticleSystem foam;
    ParticleSystem.Particle[] particleBuf;
    float nextPoolSplash;
    float lastBounceX;
    int bounceCount;

    void Awake() => Setup();
    void OnEnable() => Setup();

    void Setup()
    {
        bodyRenderer = GetComponent<SpriteRenderer>();
        if (bodyRenderer == null)
            return;

        if (hideSpriteSheet)
        {
            bodyRenderer.enabled = false;
            bodyRenderer.color = new Color(1f, 1f, 1f, 0f);
        }

        bodyRenderer.sortingOrder = sortingOrder;

        if (!Application.isPlaying)
            return;

        // Rebuild so old "laser" configs don't stick around
        RebuildChild("Fall");
        RebuildChild("Mist");
        RebuildChild("Foam");

        EnsureSystems();
        if (targetPool == null)
            targetPool = FindNearestPool();
    }

    void RebuildChild(string name)
    {
        Transform t = transform.Find(name);
        if (t == null)
            return;
        if (Application.isPlaying)
            Destroy(t.gameObject);
        else
            DestroyImmediate(t.gameObject);
    }

    void EnsureSystems()
    {
        fall = EnsureChildPS("Fall", ConfigureFall);
        mist = EnsureChildPS("Mist", ConfigureMist);
        foam = EnsureChildPS("Foam", ConfigureFoam);
        particleBuf = new ParticleSystem.Particle[Mathf.Max(512, fall.main.maxParticles)];

        if (!fall.isPlaying) fall.Play(true);
        if (!mist.isPlaying) mist.Play(true);
    }

    ParticleSystem EnsureChildPS(string name, System.Action<ParticleSystem> configure)
    {
        Transform t = transform.Find(name);
        ParticleSystem ps;
        if (t == null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            ps = go.AddComponent<ParticleSystem>();
        }
        else
        {
            ps = t.GetComponent<ParticleSystem>();
            if (ps == null)
                ps = t.gameObject.AddComponent<ParticleSystem>();
        }

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        configure(ps);
        return ps;
    }

    void ConfigureFall(ParticleSystem ps)
    {
        var main = ps.main;
        main.loop = true;
        main.playOnAwake = true;
        main.duration = 1f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * 0.75f, lifetime);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(dropSize * 0.55f, dropSize * 1.25f);
        main.startColor = waterColor;
        main.gravityModifier = gravityModifier;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 1400;
        main.scalingMode = ParticleSystemScalingMode.Hierarchy;

        var emission = ps.emission;
        emission.rateOverTime = emitRate;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(emitBox.x, emitBox.y, 0.2f);
        shape.position = new Vector3(emitOffset.x, emitOffset.y, 0f);
        shape.randomDirectionAmount = 0.08f;

        // Absolute VOL fights Collision2D every frame and tunnels through rocks.
        var vel = ps.velocityOverLifetime;
        vel.enabled = false;

        // Soft continuous push — weak so Collision2D can deflect off rocks.
        var force = ps.forceOverLifetime;
        force.enabled = true;
        force.space = ParticleSystemSimulationSpace.World;
        force.x = ConstCurve(fallVelocity.x * 0.28f, velocityJitter * 0.15f);
        force.y = ConstCurve(fallVelocity.y * 0.18f, velocityJitter * 0.12f);
        force.z = ConstCurve(0f, 0f);

        // Cap speed so drops stay thick cascade, not a laser jet.
        var limit = ps.limitVelocityOverLifetime;
        limit.enabled = true;
        limit.limit = 9.5f;
        limit.dampen = 0.35f;
        limit.separateAxes = false;

        if (collideWithRocks)
            EnableWorldCollision(ps);

        var color = ps.colorOverLifetime;
        color.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(waterColor, 0.35f),
                new GradientColorKey(new Color(0.55f, 0.8f, 1f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.55f, 0f),
                new GradientAlphaKey(0.9f, 0.08f),
                new GradientAlphaKey(0.85f, 0.8f),
                new GradientAlphaKey(0f, 1f)
            });
        color.color = g;

        var size = ps.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 1.35f));

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sortingOrder;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.35f;
        noise.frequency = 0.4f;
        noise.scrollSpeed = 0.25f;
        noise.octaveCount = 1;
        noise.quality = ParticleSystemNoiseQuality.Medium;
    }

    void EnableWorldCollision(ParticleSystem ps)
    {
        var col = ps.collision;
        col.enabled = true;
        col.type = ParticleSystemCollisionType.World;
        col.mode = ParticleSystemCollisionMode.Collision2D;
        col.dampen = rockDampen;
        col.bounce = rockBounce;
        col.lifetimeLoss = 0f;
        col.minKillSpeed = 0f;
        col.maxKillSpeed = 10000f;
        col.radiusScale = 0.85f;
        col.collidesWith = rockMask;
        // Water trigger volume must not eat / stick the cascade.
        int waterBit = LayerMask.NameToLayer("Water");
        if (waterBit >= 0)
            col.collidesWith &= ~(1 << waterBit);
        col.quality = ParticleSystemCollisionQuality.High;
        col.maxCollisionShapes = 512;
        col.enableDynamicColliders = true;
        col.colliderForce = 0f;
        col.sendCollisionMessages = false;
    }

    static ParticleSystem.MinMaxCurve ConstCurve(float center, float jitter)
    {
        var c = new ParticleSystem.MinMaxCurve(center - jitter, center + jitter);
        c.mode = ParticleSystemCurveMode.TwoConstants;
        return c;
    }

    void ConfigureMist(ParticleSystem ps)
    {
        var main = ps.main;
        main.loop = true;
        main.playOnAwake = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.6f, 3f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(mistSize * 0.7f, mistSize * 1.5f);
        main.startColor = new Color(foamColor.r, foamColor.g, foamColor.b, 0.3f);
        main.gravityModifier = 0.45f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 220;

        var emission = ps.emission;
        emission.rateOverTime = mistRate;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(emitBox.x * 1.4f, emitBox.y * 2.2f, 0.2f);
        shape.position = new Vector3(emitOffset.x, emitOffset.y - 0.5f, 0f);

        var vel = ps.velocityOverLifetime;
        vel.enabled = false;

        var force = ps.forceOverLifetime;
        force.enabled = true;
        force.space = ParticleSystemSimulationSpace.World;
        force.x = ConstCurve(fallVelocity.x * 0.2f, 0.5f);
        force.y = ConstCurve(fallVelocity.y * 0.12f, 0.4f);

        // Mist should soft-overlap rocks, not vanish against the lip.
        var col = ps.collision;
        col.enabled = false;

        var color = ps.colorOverLifetime;
        color.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(foamColor, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.3f, 0.3f), new GradientAlphaKey(0f, 1f) });
        color.color = g;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sortingOrder - 1;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void ConfigureFoam(ParticleSystem ps)
    {
        var main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1.1f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 7f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.35f, 0.9f);
        main.startColor = foamColor;
        main.gravityModifier = 0.9f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 260;

        var emission = ps.emission;
        emission.rateOverTime = 0f;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.5f;
        shape.rotation = new Vector3(-90f, 0f, 0f);

        var color = ps.colorOverLifetime;
        color.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(foamColor, 1f) },
            new[] { new GradientAlphaKey(0.95f, 0f), new GradientAlphaKey(0f, 1f) });
        color.color = g;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sortingOrder + 2;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        if (bodyRenderer == null)
        {
            Setup();
            return;
        }

        if (hideSpriteSheet && bodyRenderer.enabled)
            bodyRenderer.enabled = false;

        bodyRenderer.sortingOrder = sortingOrder;

        if (!Application.isPlaying)
            return;

        if (fall == null || mist == null || foam == null)
            EnsureSystems();

        if (particleBuf == null || particleBuf.Length < fall.main.maxParticles)
            particleBuf = new ParticleSystem.Particle[fall.main.maxParticles];

        SeedAndSplash();
    }

    float GetSurfaceY()
    {
        if (targetPool == null)
            targetPool = FindNearestPool();
        if (targetPool != null)
            return targetPool.GetSurfaceWorldY();
        return transform.position.y - 20f;
    }

    void SeedAndSplash()
    {
        if (fall == null)
            return;

        int count = fall.GetParticles(particleBuf);
        if (count <= 0)
            return;

        float surfaceY = GetSurfaceY();
        bool dirty = false;
        bounceCount = 0;
        lastBounceX = 0f;

        for (int i = 0; i < count; i++)
        {
            ParticleSystem.Particle p = particleBuf[i];
            float age = p.startLifetime - p.remainingLifetime;

            // Give newborn drops an initial soft push off the cliff (once)
            if (age < 0.04f && p.velocity.sqrMagnitude < 0.8f)
            {
                p.velocity = new Vector3(
                    fallVelocity.x + Random.Range(-velocityJitter, velocityJitter),
                    fallVelocity.y + Random.Range(-velocityJitter * 0.35f, velocityJitter * 0.2f),
                    0f);
                p.position = new Vector3(p.position.x, p.position.y, transform.position.z);
                particleBuf[i] = p;
                dirty = true;
            }

            // Keep Z flat for 2D
            if (Mathf.Abs(p.position.z - transform.position.z) > 0.01f)
            {
                p.position = new Vector3(p.position.x, p.position.y, transform.position.z);
                p.velocity = new Vector3(p.velocity.x, p.velocity.y, 0f);
                particleBuf[i] = p;
                dirty = true;
            }

            if (!splashOnPool)
                continue;

            // Only splash after a real fall — ignore while still near the emitter / cliff lip
            if (p.position.y > surfaceY + 0.35f)
                continue;
            if (p.position.y > transform.position.y + emitOffset.y - 4f)
                continue;
            if (p.velocity.y >= -0.5f)
                continue;

            // Soft landing on pool — foam, keep living (do NOT cull lifetime)
            p.position = new Vector3(p.position.x, surfaceY + 0.08f, transform.position.z);
            p.velocity = new Vector3(
                p.velocity.x * 0.7f + sidewaysKick * Random.Range(0.35f, 1f),
                -p.velocity.y * bounce + Random.Range(0.8f, 2.2f),
                0f);
            particleBuf[i] = p;
            dirty = true;

            if (foam != null && Random.value < 0.4f)
            {
                foam.Emit(new ParticleSystem.EmitParams
                {
                    position = p.position,
                    applyShapeToPosition = true
                }, Mathf.RoundToInt(Random.Range(foamBurst * 0.35f, foamBurst)));
            }

            lastBounceX += p.position.x;
            bounceCount++;
        }

        if (dirty)
            fall.SetParticles(particleBuf, count);

        if (notifyPool && bounceCount > 0 && Time.time >= nextPoolSplash)
        {
            nextPoolSplash = Time.time + poolSplashInterval;
            if (targetPool == null)
                targetPool = FindNearestPool();
            if (targetPool != null)
                targetPool.AddSplash(new Vector2(lastBounceX / bounceCount, surfaceY), poolSplashStrength);
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
        if (bodyRenderer == null)
            bodyRenderer = GetComponent<SpriteRenderer>();
        if (bodyRenderer != null)
        {
            bodyRenderer.sortingOrder = sortingOrder;
            if (hideSpriteSheet)
                bodyRenderer.enabled = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + new Vector3(emitOffset.x, emitOffset.y, 0f);
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.7f);
        Gizmos.DrawWireCube(origin, new Vector3(emitBox.x, emitBox.y, 0.1f));
        Vector3 dir = new Vector3(fallVelocity.x, fallVelocity.y, 0f).normalized * 10f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origin, dir);
    }
}
