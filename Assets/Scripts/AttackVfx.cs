using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AttackVfx : MonoBehaviour
{
    struct Piece
    {
        public Transform transform;
        public SpriteRenderer renderer;
        public float offset;
        public float distance;
        public int kind;
    }

    static Sprite shardSprite;
    static Sprite sparkSprite;
    static Sprite boltSprite;
    static Sprite beamSprite;
    static Sprite crescentSprite;
    static Sprite ringSprite;
    static Sprite discSprite;
    static Material unlit;

    Piece[] pieces;
    float duration;
    float time;
    float facing;
    float reach;
    int mode;
    bool echo;
    Color tint;
    float power = 1f;
    float arc = 1f;
    float fit = 1f;
    Light2D glow;

    public static void HideCircle(GameObject weapon)
    {
        if (weapon == null)
            return;
        SpriteRenderer[] renderers = weapon.GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = false;
        Light2D[] lights = weapon.GetComponentsInChildren<Light2D>(true);
        for (int i = 0; i < lights.Length; i++)
            lights[i].enabled = false;
    }

    public static void Slash(Vector3 origin, float facingDegrees, bool heavy, float radius)
    {
        if (!heavy)
        {
            Wave(origin, facingDegrees, new Color(0.15f, 0.96f, 1f, 1f), false, 1f, -1f, radius);
            return;
        }
        Scatter(origin, facingDegrees, radius);
    }

    static void Scatter(Vector3 origin, float facingDegrees, float radius)
    {
        Color side = new Color(0.78f, 0.38f, 1f, 1f);
        Color core = new Color(0.78f, 1f, 0.9f, 1f);
        Wave(origin, facingDegrees - 36f, side, false, 1f, 0f, radius);
        Wave(origin, facingDegrees, core, false, 1f, 0.06f, radius);
        Wave(origin, facingDegrees + 36f, side, false, 1f, 0.12f, radius);
    }

    public static void DoubleSlash(Vector3 origin, float facingDegrees, float radius)
    {
        Wave(origin, facingDegrees, new Color(0.2f, 0.95f, 1f, 1f), false, 1f, -1f, radius);
        Wave(origin, facingDegrees, new Color(1f, 0.74f, 0.22f, 1f), true, 1f, -1f, radius);
    }

    static void Wave(Vector3 origin, float facingDegrees, Color color, bool echo, float power, float delay, float radius)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = 4;
        swing.facing = facingDegrees;
        swing.echo = echo;
        swing.tint = color;
        swing.power = power;
        swing.fit = WaveFit(radius, echo);
        swing.duration = 0.34f;
        swing.time = delay >= 0f ? -delay : (echo ? -0.11f : 0f);
        swing.reach = (echo ? 2.35f : 2.05f) * power;
        swing.AttachGlow(color, radius * power);

        swing.pieces = new Piece[9];
        swing.pieces[0] = swing.Make(Crescent(), color, 0f, 0f, 0);
        swing.pieces[0].transform.localScale = new Vector3(1.22f, echo ? -1.22f : 1.22f, 1f);
        swing.pieces[1] = swing.Make(Crescent(), Color.white, 0f, 0f, 1);
        swing.pieces[2] = swing.Make(Crescent(), color, 0f, 0f, 2);
        swing.pieces[3] = swing.Make(Ring(), new Color(1f, 1f, 1f, 1f), 0f, 0f, 3);
        swing.pieces[4] = swing.Make(Disc(), color, 0f, 0f, 4);
        for (int i = 0; i < 4; i++)
            swing.pieces[5 + i] = swing.Make(Disc(), i % 2 == 0 ? Color.white : color, (i - 1.5f) * 16f, 0.2f, 5);
        for (int i = 0; i < swing.pieces.Length; i++)
        {
            Color hidden = swing.pieces[i].renderer.color;
            hidden.a = 0f;
            swing.pieces[i].renderer.color = hidden;
        }
    }

    public static void Thrust(Vector3 origin, float facingDegrees, float radius)
    {
        FocusWave(origin, facingDegrees, new Color(1f, 0.58f, 0.22f, 1f), 1f, 0f, radius);
        FocusWave(origin, facingDegrees, new Color(1f, 0.96f, 0.82f, 1f), 0.62f, 0.045f, radius);
    }

    static void FocusWave(Vector3 origin, float facingDegrees, Color color, float power, float delay, float radius)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = 4;
        swing.facing = facingDegrees;
        swing.tint = color;
        swing.power = power;
        swing.arc = 0.4f;
        swing.fit = WaveFit(radius, false);
        swing.duration = 0.3f;
        swing.time = -delay;
        swing.reach = 1.05f * power;
        swing.AttachGlow(color, radius * power);
        swing.pieces = new Piece[3];
        swing.pieces[0] = swing.Make(Crescent(), color, 0f, 0f, 0);
        swing.pieces[1] = swing.Make(Crescent(), Color.white, 0f, 0f, 1);
        swing.pieces[2] = swing.Make(Crescent(), color, 0f, 0f, 2);
        swing.HidePieces();
    }

    public static void Muzzle(Vector3 origin, float facingDegrees)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = 7;
        swing.facing = facingDegrees;
        swing.duration = 0.2f;
        swing.power = 0.72f;
        swing.reach = 0.2f;
        swing.tint = new Color(0.55f, 1f, 0.96f, 1f);
        swing.AttachGlow(swing.tint, 1.35f);
        swing.pieces = new Piece[3];
        swing.pieces[0] = swing.Make(Disc(), swing.tint, 0f, 0f, 0);
        swing.pieces[1] = swing.Make(Disc(), Color.white, 0f, 0f, 1);
        swing.pieces[2] = swing.Make(Ring(), Color.white, 0f, 0f, 3);
        swing.HidePieces();
    }

    static void Lance(Vector3 origin, float facingDegrees, Color color, float power, float duration)
    {
        FocusWave(origin, facingDegrees, color, power, 0f, 0.35f);
    }

    static float WaveFit(float radius, bool echo)
    {
        float peak = echo ? 1.28f : 1.1f;
        float designed = (150f / 80f) * 1.34f * peak;
        return Mathf.Max(0.05f, radius) / designed;
    }

    public static void Impact(Vector3 origin)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = 7;
        swing.duration = 0.26f;
        swing.power = 0.9f;
        swing.reach = 0.85f;
        swing.tint = new Color(0.45f, 1f, 0.95f, 1f);
        swing.AttachGlow(swing.tint, 2.2f);
        swing.pieces = new Piece[4];
        swing.pieces[0] = swing.Make(Disc(), swing.tint, 0f, 0f, 0);
        swing.pieces[1] = swing.Make(Disc(), Color.white, 0f, 0f, 1);
        swing.pieces[2] = swing.Make(Ring(), Color.white, 0f, 0f, 3);
        swing.pieces[3] = swing.Make(Disc(), new Color(1f, 0.95f, 0.8f, 1f), 0f, 0f, 4);
        swing.HidePieces();
    }

    public static void Spark(Vector3 origin)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = 7;
        swing.duration = 0.22f;
        swing.power = 0.62f;
        swing.reach = 0.02f;
        swing.tint = new Color(0.55f, 1f, 0.95f, 1f);
        swing.pieces = new Piece[1];
        swing.pieces[0] = swing.Make(Disc(), swing.tint, 0f, 0f, 4);
        swing.HidePieces();
    }

    public static void DressBolt(SpriteRenderer renderer)
    {
        if (renderer == null)
            return;
        renderer.sprite = Bolt();
        renderer.color = Color.white;
        renderer.sortingOrder = 80;
        Material mat = Unlit();
        if (mat != null)
            renderer.sharedMaterial = mat;
        renderer.transform.localScale = new Vector3(1f, 1f, 1f);
        Light2D light = renderer.GetComponentInChildren<Light2D>(true);
        if (light == null)
            return;
        light.lightType = Light2D.LightType.Point;
        light.blendStyleIndex = 1;
        light.color = new Color(0.4f, 1f, 0.95f, 1f);
        light.intensity = 0.85f;
        light.pointLightInnerRadius = 0.15f;
        light.pointLightOuterRadius = 1.05f;
        light.falloffIntensity = 0.5f;
        light.shadowsEnabled = false;
    }

    static AttackVfx Spawn(Vector3 origin)
    {
        GameObject go = new GameObject("AttackVfx");
        go.transform.position = origin;
        return go.AddComponent<AttackVfx>();
    }

    void AttachGlow(Color color, float radius)
    {
        glow = gameObject.AddComponent<Light2D>();
        glow.lightType = Light2D.LightType.Point;
        glow.blendStyleIndex = 1;
        glow.color = color;
        glow.intensity = 0f;
        glow.pointLightInnerRadius = radius * 0.18f;
        glow.pointLightOuterRadius = radius;
        glow.falloffIntensity = 0.5f;
        glow.shadowsEnabled = false;
    }

    void HidePieces()
    {
        if (pieces == null)
            return;
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i].renderer == null)
                continue;
            Color hidden = pieces[i].renderer.color;
            hidden.a = 0f;
            pieces[i].renderer.color = hidden;
        }
    }

    Piece Make(Sprite sprite, Color color, float offset, float distance, int kind = 0)
    {
        GameObject go = new GameObject("Shard");
        go.transform.SetParent(transform, false);
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = 80;
        Material mat = Unlit();
        if (mat != null)
            renderer.sharedMaterial = mat;
        Piece piece = new Piece();
        piece.transform = go.transform;
        piece.renderer = renderer;
        piece.offset = offset;
        piece.distance = distance;
        piece.kind = kind;
        return piece;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (mode == 4)
        {
            AnimateWave();
            return;
        }
        if (mode == 6)
        {
            AnimateLance();
            return;
        }
        if (mode == 7)
        {
            AnimateBurst();
            return;
        }
        float t = duration <= 0f ? 1f : Mathf.Clamp01(time / duration);
        float fade = t < 0.18f ? t / 0.18f : 1f - Mathf.InverseLerp(0.45f, 1f, t);
        for (int i = 0; i < pieces.Length; i++)
        {
            Piece piece = pieces[i];
            if (piece.renderer == null)
                continue;
            float distance = piece.distance;
            float angle = facing + piece.offset;
            Vector3 scale = piece.transform.localScale;
            if (mode == 0)
            {
                angle = facing + Mathf.Lerp(piece.offset - 34f, piece.offset + 18f, Ease(t));
                distance = Mathf.Lerp(0.35f, reach, Ease(t));
                scale = new Vector3(Mathf.Lerp(0.15f, 0.42f, Ease(t)), 0.07f, 1f);
            }
            else if (mode == 1)
            {
                distance = Mathf.Lerp(0.25f, reach, Ease(t));
                scale = new Vector3(Mathf.Lerp(0.2f, 0.7f, Ease(t)), 0.08f, 1f);
            }
            else if (mode == 2)
            {
                distance = Mathf.Lerp(piece.distance, reach, Ease(t));
                if (i == 0)
                    scale = new Vector3(Mathf.Lerp(0.15f, 0.7f, Ease(t)), 0.1f, 1f);
            }
            else
            {
                float end = piece.distance <= 0.001f ? piece.distance : reach;
                distance = Mathf.Lerp(piece.distance, end, Ease(t));
            }
            float rad = angle * Mathf.Deg2Rad;
            piece.transform.localPosition = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * distance;
            piece.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            piece.transform.localScale = scale;
            Color color = piece.renderer.color;
            color.a = Mathf.Clamp01(fade);
            piece.renderer.color = color;
        }
        if (t >= 1f)
            Destroy(gameObject);
    }

    static float Ease(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    static Sprite Shard()
    {
        if (shardSprite == null)
            shardSprite = Bar(32, 8, true);
        return shardSprite;
    }

    static Sprite Spark()
    {
        if (sparkSprite == null)
            sparkSprite = Bar(12, 12, false);
        return sparkSprite;
    }

    static Sprite Bolt()
    {
        if (boltSprite == null)
            boltSprite = BakeBolt();
        return boltSprite;
    }

    static Sprite Beam()
    {
        if (beamSprite == null)
            beamSprite = BakeBeam();
        return beamSprite;
    }

    static Sprite Bar(int width, int height, bool tapered, bool upright = false)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[width * height];
        float cx = (width - 1) * 0.5f;
        float cy = (height - 1) * 0.5f;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float along = upright ? y / (float)(height - 1) : x / (float)(width - 1);
                float across = upright
                    ? Mathf.Abs(x - cx) / Mathf.Max(0.01f, cx)
                    : Mathf.Abs(y - cy) / Mathf.Max(0.01f, cy);
                float edge = tapered ? Mathf.Lerp(0.25f, 1f, along) : 1f;
                byte alpha = !tapered
                    ? (byte)(across <= 1f && Mathf.Abs(upright ? y - cy : x - cx) <= (upright ? cy : cx) ? 255 : 0)
                    : (byte)(across <= edge ? 255 : 0);
                if (!tapered)
                {
                    float dx = Mathf.Abs(x - cx) / Mathf.Max(0.01f, cx);
                    float dy = Mathf.Abs(y - cy) / Mathf.Max(0.01f, cy);
                    alpha = dx <= 1f && dy <= 1f ? (byte)255 : (byte)0;
                }
                byte shade = (byte)Mathf.Lerp(255f, 190f, 1f - along);
                pixels[y * width + x] = new Color32(255, shade, (byte)Mathf.Lerp(shade, 255f, upright ? 0.45f : 0f), alpha);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 32f);
    }

    void AnimateWave()
    {
        if (pieces == null)
            return;
        if (time < 0f)
        {
            if (glow != null)
                glow.intensity = 0f;
            return;
        }
        float t = duration <= 0f ? 1f : Mathf.Clamp01(time / duration);
        float eased = Ease(t);
        float fade = t < 0.08f ? t / 0.08f : 1f - Mathf.SmoothStep(0.4f, 1f, t);
        float sweepStart = (echo ? 36f : -42f) * arc;
        float sweepEnd = (echo ? -24f : 32f) * arc;
        float sweep = Mathf.Lerp(sweepStart, sweepEnd, eased);
        float lagT = Ease(Mathf.Clamp01((t - 0.1f) / 0.9f));
        float lag = Mathf.Lerp(sweepStart, sweepEnd, lagT);
        float punch = t < 0.16f
            ? Mathf.Lerp(0.38f, echo ? 1.18f : 1.05f, t / 0.16f)
            : Mathf.Lerp(echo ? 1.18f : 1.05f, echo ? 1.28f : 1.1f, (t - 0.16f) / 0.84f);
        punch *= power;
        float ySign = echo ? -1f : 1f;
        float face = facing * Mathf.Deg2Rad;
        Vector3 forward = new Vector3(Mathf.Cos(face), Mathf.Sin(face), 0f);
        for (int i = 0; i < pieces.Length; i++)
        {
            Piece piece = pieces[i];
            if (piece.renderer == null)
                continue;
            float alpha = fade;
            float angle = facing + sweep;
            Vector3 scale = Vector3.one;
            Vector3 pos = Vector3.zero;
            if (piece.kind == 0)
            {
                scale = new Vector3(1.34f * punch, 1.34f * punch * ySign, 1f);
            }
            else if (piece.kind == 1)
            {
                scale = new Vector3(0.9f * punch, 0.9f * punch * ySign, 1f);
            }
            else if (piece.kind == 2)
            {
                angle = facing + lag;
                scale = new Vector3(1.12f * punch, 1.12f * punch * ySign, 1f);
                alpha = fade * 0.42f;
            }
            else if (piece.kind == 3)
            {
                float ring = Mathf.Clamp01(t / 0.55f);
                scale = Vector3.one * Mathf.Lerp(0.28f, echo ? 1.7f : 1.45f, Ease(ring)) * power;
                pos = forward * 1.15f;
                alpha = (1f - ring) * fade;
                angle = facing;
            }
            else if (piece.kind == 4)
            {
                scale = Vector3.one * Mathf.Lerp(0.85f, 0.2f, eased) * power;
                pos = forward * 0.45f;
                alpha = fade * (1f - t);
                angle = 0f;
            }
            else
            {
                angle = facing + sweep * 0.55f + piece.offset;
                float dist = Mathf.Lerp(0.35f, reach, eased);
                float spark = angle * Mathf.Deg2Rad;
                pos = new Vector3(Mathf.Cos(spark), Mathf.Sin(spark), 0f) * dist;
                float size = Mathf.Lerp(0.42f, 0.08f, eased) * power;
                scale = new Vector3(size, size, 1f);
                alpha = fade * (1f - eased * 0.35f);
                angle += 45f;
            }
            piece.transform.localPosition = pos * fit;
            piece.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            piece.transform.localScale = scale * fit;
            Color color = piece.renderer.color;
            color.a = Mathf.Clamp01(alpha);
            piece.renderer.color = color;
        }
        if (glow != null)
        {
            glow.intensity = fade * (echo ? 1.65f : 1.35f);
            glow.color = tint;
        }
        if (time >= duration)
            Destroy(gameObject);
    }

    void AnimateLance()
    {
        if (pieces == null)
            return;
        float t = duration <= 0f ? 1f : Mathf.Clamp01(time / duration);
        float form = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / 0.62f));
        float fadeIn = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / 0.22f));
        float fadeOut = 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((t - 0.48f) / 0.52f));
        float fade = fadeIn * fadeOut;
        float length = Mathf.Lerp(0.55f, 1f, form);
        float swell = 0.9f + 0.18f * Mathf.Sin(t * Mathf.PI);
        for (int i = 0; i < pieces.Length; i++)
        {
            Piece piece = pieces[i];
            if (piece.renderer == null)
                continue;
            float alpha = fade;
            Vector3 scale = Vector3.one;
            if (piece.kind == 0)
            {
                scale = new Vector3(length * power, swell * power, 1f);
                alpha = fade * 0.8f;
            }
            else if (piece.kind == 1)
            {
                scale = new Vector3(length * power * 0.78f, swell * power * 0.5f, 1f);
                alpha = fade * 0.55f;
            }
            else
            {
                scale = Vector3.one * swell * 0.55f * power;
                alpha = fade * 0.45f;
            }
            piece.transform.localPosition = Vector3.zero;
            piece.transform.localRotation = Quaternion.Euler(0f, 0f, facing);
            piece.transform.localScale = scale;
            Color color = piece.renderer.color;
            color.a = Mathf.Clamp01(alpha);
            piece.renderer.color = color;
        }
        if (glow != null)
        {
            glow.intensity = fade * 0.9f * power;
            glow.color = tint;
        }
        if (time >= duration)
            Destroy(gameObject);
    }

    void AnimateBurst()
    {
        if (pieces == null)
            return;
        float t = duration <= 0f ? 1f : Mathf.Clamp01(time / duration);
        float eased = Ease(t);
        float fade = t < 0.08f ? t / 0.08f : 1f - Mathf.SmoothStep(0.32f, 1f, t);
        for (int i = 0; i < pieces.Length; i++)
        {
            Piece piece = pieces[i];
            if (piece.renderer == null)
                continue;
            float alpha = fade;
            float angle = piece.offset;
            Vector3 pos = Vector3.zero;
            Vector3 scale;
            if (piece.kind == 0)
                scale = Vector3.one * Mathf.Lerp(0.35f, 1.45f, eased) * power;
            else if (piece.kind == 1)
                scale = Vector3.one * Mathf.Lerp(0.2f, 0.62f, eased) * power;
            else if (piece.kind == 3)
            {
                float ring = Mathf.Clamp01(t / 0.7f);
                scale = Vector3.one * Mathf.Lerp(0.25f, 1.85f, Ease(ring)) * power;
                alpha = (1f - ring) * fade;
            }
            else if (piece.kind == 4)
            {
                scale = Vector3.one * Mathf.Lerp(0.34f, 0.08f, eased) * power;
                alpha = fade * 0.7f;
            }
            else
            {
                float spark = (facing + piece.offset) * Mathf.Deg2Rad;
                pos = new Vector3(Mathf.Cos(spark), Mathf.Sin(spark), 0f) * Mathf.Lerp(0.05f, reach, eased);
                float size = Mathf.Lerp(0.38f, 0.1f, eased) * power;
                scale = new Vector3(size, size, 1f);
                angle = piece.offset;
            }
            piece.transform.localPosition = pos;
            piece.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            piece.transform.localScale = scale;
            Color color = piece.renderer.color;
            color.a = Mathf.Clamp01(alpha);
            piece.renderer.color = color;
        }
        if (glow != null)
        {
            glow.intensity = fade * 1.7f;
            glow.color = tint;
        }
        if (time >= duration)
            Destroy(gameObject);
    }

    static Sprite Crescent()
    {
        if (crescentSprite == null)
            crescentSprite = BakeCrescent();
        return crescentSprite;
    }

    static Sprite Ring()
    {
        if (ringSprite == null)
            ringSprite = BakeRing();
        return ringSprite;
    }

    static Sprite Disc()
    {
        if (discSprite == null)
            discSprite = BakeDisc();
        return discSprite;
    }

    static Material Unlit()
    {
        if (unlit != null)
            return unlit;
        unlit = Resources.Load<Material>("AttackUnlit");
        if (unlit == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");
            if (shader != null)
                unlit = new Material(shader);
        }
        return unlit;
    }

    static Sprite BakeCrescent()
    {
        const int size = 256;
        const float center = 36f;
        const float radius = 150f;
        const float start = -1.15f;
        const float end = 1.2f;
        float mid = (start + end) * 0.5f;
        float span = (end - start) * 0.5f;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - 128f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float ang = Mathf.Atan2(dy, dx);
                float alpha = 0f;
                if (ang >= start && ang <= end)
                {
                    float along = Mathf.Clamp01(1f - Mathf.Abs(ang - mid) / span);
                    along = Mathf.Pow(along, 0.65f);
                    float thick = 7f + along * 34f;
                    float soft = 10f;
                    float radial = Mathf.Abs(dist - radius);
                    if (radial <= thick * 0.5f + soft)
                    {
                        alpha = radial <= thick * 0.5f
                            ? Mathf.Lerp(0.75f, 1f, Mathf.Clamp01((thick * 0.5f - radial) / 8f))
                            : 0.75f * (1f - (radial - thick * 0.5f) / soft);
                        alpha *= Mathf.Clamp01(along * 1.4f);
                    }
                }
                byte a = (byte)(Mathf.Clamp01(alpha) * 255f);
                pixels[y * size + x] = new Color32(255, 255, 255, a);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(center / size, 0.5f), 80f);
    }

    static Sprite BakeRing()
    {
        const int size = 96;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[size * size];
        float c = (size - 1) * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / c;
                float band = 1f - Mathf.Clamp01(Mathf.Abs(d - 0.74f) / 0.2f);
                byte a = (byte)(Mathf.Clamp01(band) * 255f);
                pixels[y * size + x] = new Color32(255, 255, 255, a);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 48f);
    }

    static Sprite BakeDisc()
    {
        const int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[size * size];
        float c = (size - 1) * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / c;
                float alpha = 1f - Mathf.SmoothStep(0.15f, 1f, d);
                byte a = (byte)(Mathf.Clamp01(alpha) * 255f);
                pixels[y * size + x] = new Color32(255, 255, 255, a);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 64f);
    }

    static Sprite BakeBeam()
    {
        const int width = 160;
        const int height = 48;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[width * height];
        float cy = (height - 1) * 0.5f;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float along = x / (float)(width - 1);
                float across = Mathf.Abs(y - cy) / cy;
                float half = 0.58f + 0.42f * Mathf.Sin(along * Mathf.PI);
                float alpha = 0f;
                if (across <= half + 0.35f)
                {
                    alpha = across <= half
                        ? Mathf.Lerp(0.55f, 0.95f, Mathf.Clamp01((half - across) / 0.4f))
                        : 0.55f * (1f - (across - half) / 0.35f);
                    alpha *= Mathf.SmoothStep(0f, 0.12f, along) * Mathf.SmoothStep(1f, 0.82f, along);
                }
                byte a = (byte)(Mathf.Clamp01(alpha) * 255f);
                pixels[y * width + x] = new Color32(255, 255, 255, a);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0f, 0f, width, height), new Vector2(0.08f, 0.5f), 64f);
    }

    static Sprite BakeBolt()
    {
        const int width = 72;
        const int height = 180;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[width * height];
        float headX = (width - 1) * 0.5f;
        float headY = 132f;
        float headRadius = 24f;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = x - headX;
                float head = Mathf.Sqrt(dx * dx + (y - headY) * (y - headY));
                float along = Mathf.Clamp01((headY - y) / headY);
                float tailHalf = Mathf.Lerp(headRadius, 3f, Mathf.SmoothStep(0f, 1f, along));
                float tail = y <= headY ? Mathf.Abs(dx) / Mathf.Max(1f, tailHalf) : 2f;
                float alpha = 0f;
                if (head <= headRadius + 10f)
                {
                    alpha = head <= headRadius
                        ? Mathf.Lerp(0.75f, 1f, Mathf.Clamp01((headRadius - head) / 10f))
                        : 0.7f * (1f - (head - headRadius) / 10f);
                }
                if (y < headY && tail <= 1.35f)
                {
                    float tailAlpha = tail <= 1f
                        ? Mathf.Lerp(0.15f, 0.7f, 1f - along) * (1f - tail * 0.35f)
                        : 0.25f * (1.35f - tail) / 0.35f;
                    alpha = Mathf.Max(alpha, tailAlpha);
                }
                byte a = (byte)(Mathf.Clamp01(alpha) * 255f);
                float hot = head <= headRadius * 0.55f ? 1f : 0f;
                byte r = (byte)Mathf.Lerp(150f, 255f, Mathf.Max(hot, 1f - along * 0.8f));
                byte g = 255;
                byte b = 255;
                pixels[y * width + x] = new Color32(r, g, b, a);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0f, 0f, width, height), new Vector2(0.5f, headY / height), 90f);
    }
}
