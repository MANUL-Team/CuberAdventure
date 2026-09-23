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
    }

    static Sprite shardSprite;
    static Sprite sparkSprite;
    static Sprite boltSprite;

    Piece[] pieces;
    float duration;
    float time;
    float facing;
    float reach;
    int mode;

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

    public static void Slash(Vector3 origin, float facingDegrees, bool heavy)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = heavy ? 1 : 0;
        swing.facing = facingDegrees;
        swing.duration = heavy ? 0.34f : 0.22f;
        swing.reach = heavy ? 2.15f : 1.05f;
        int count = heavy ? 8 : 5;
        float arc = heavy ? 100f : 72f;
        Color color = heavy ? new Color(0.45f, 0.95f, 0.92f, 1f) : new Color(1f, 0.62f, 0.28f, 1f);
        swing.pieces = new Piece[count];
        for (int i = 0; i < count; i++)
        {
            float along = count == 1 ? 0f : i / (float)(count - 1);
            swing.pieces[i] = swing.Make(Shard(), color, (along - 0.5f) * arc, 0.55f);
        }
    }

    public static void Thrust(Vector3 origin, float facingDegrees)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = 2;
        swing.facing = facingDegrees;
        swing.duration = 0.16f;
        swing.reach = 0.85f;
        swing.pieces = new Piece[4];
        swing.pieces[0] = swing.Make(Shard(), new Color(1f, 0.95f, 0.82f, 1f), 0f, 0.35f);
        swing.pieces[0].transform.localScale = new Vector3(0.55f, 0.12f, 1f);
        for (int i = 1; i < 4; i++)
            swing.pieces[i] = swing.Make(Spark(), new Color(1f, 0.72f, 0.32f, 1f), (i - 2) * 18f, 0.2f);
    }

    public static void Muzzle(Vector3 origin, float facingDegrees)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = 2;
        swing.facing = facingDegrees;
        swing.duration = 0.12f;
        swing.reach = 0.45f;
        swing.pieces = new Piece[3];
        swing.pieces[0] = swing.Make(Shard(), new Color(0.75f, 1f, 0.98f, 1f), 0f, 0.12f);
        swing.pieces[0].transform.localScale = new Vector3(0.42f, 0.1f, 1f);
        swing.pieces[1] = swing.Make(Spark(), Color.white, 16f, 0.08f);
        swing.pieces[2] = swing.Make(Spark(), new Color(0.4f, 0.95f, 0.9f, 1f), -16f, 0.08f);
    }

    public static void Impact(Vector3 origin)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = 3;
        swing.duration = 0.24f;
        swing.reach = 0.55f;
        swing.pieces = new Piece[7];
        swing.pieces[0] = swing.Make(Spark(), Color.white, 0f, 0f);
        swing.pieces[0].transform.localScale = new Vector3(0.28f, 0.28f, 1f);
        for (int i = 1; i < 7; i++)
        {
            float angle = i * 60f;
            Color color = i % 2 == 0
                ? new Color(0.45f, 0.95f, 0.92f, 1f)
                : new Color(1f, 0.86f, 0.55f, 1f);
            swing.pieces[i] = swing.Make(Shard(), color, angle, 0.05f);
            swing.pieces[i].transform.localScale = new Vector3(0.22f, 0.07f, 1f);
        }
    }

    public static void Spark(Vector3 origin)
    {
        AttackVfx swing = Spawn(origin);
        swing.mode = 3;
        swing.duration = 0.12f;
        swing.reach = 0.08f;
        swing.pieces = new Piece[1];
        swing.pieces[0] = swing.Make(Spark(), new Color(0.55f, 1f, 0.95f, 0.85f), 0f, 0f);
        swing.pieces[0].transform.localScale = new Vector3(0.12f, 0.12f, 1f);
    }

    public static void DressBolt(SpriteRenderer renderer)
    {
        if (renderer == null)
            return;
        renderer.sprite = Bolt();
        renderer.color = Color.white;
    }

    static AttackVfx Spawn(Vector3 origin)
    {
        GameObject go = new GameObject("AttackVfx");
        go.transform.position = origin;
        return go.AddComponent<AttackVfx>();
    }

    Piece Make(Sprite sprite, Color color, float offset, float distance)
    {
        GameObject go = new GameObject("Shard");
        go.transform.SetParent(transform, false);
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = 30;
        Piece piece = new Piece();
        piece.transform = go.transform;
        piece.renderer = renderer;
        piece.offset = offset;
        piece.distance = distance;
        return piece;
    }

    void Update()
    {
        time += Time.deltaTime;
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
            boltSprite = Bar(8, 36, true, true);
        return boltSprite;
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
}
