using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMotion : MonoBehaviour
{
    public bool skills;
    Coroutine play;
    Transform pulse;
    bool introDone;
    List<RectTransform> liveRoads;
    Image skillWash;
    RectTransform[] embers;
    Vector2[] emberRest;
    float[] emberPhase;
    RectTransform panel;
    Image[] skillPlates;
    float panelX;
    float panelRest;
    float panelHidden;
    bool drivePanel;
    bool closing;

    public void Begin()
    {
        if (!isActiveAndEnabled || closing)
            return;
        if (play != null)
            StopCoroutine(play);
        introDone = false;
        play = StartCoroutine(Run());
    }

    void OnEnable()
    {
        Begin();
    }

    void OnDisable()
    {
        if (play != null)
            StopCoroutine(play);
        play = null;
        introDone = false;
        pulse = null;
        liveRoads = null;
        skillWash = null;
        embers = null;
        drivePanel = false;
        closing = false;
        skillPlates = null;
    }

    void LateUpdate()
    {
        if (skills)
        {
            MuteSkillWash();
            HoldSkillPlates();
        }
        if (drivePanel && panel != null)
        {
            Vector2 position = panel.anchoredPosition;
            position.x = panelX;
            panel.anchoredPosition = position;
        }
        if (liveRoads != null)
        {
            for (int i = 0; i < liveRoads.Count; i++)
            {
                if (liveRoads[i] != null)
                    liveRoads[i].localScale = Vector3.one;
            }
        }
        if (!skills || !introDone)
            return;
        if (embers != null)
        {
            float time = Time.unscaledTime;
            for (int i = 0; i < embers.Length; i++)
            {
                if (embers[i] == null)
                    continue;
                float bob = Mathf.Sin(time * 1.35f + emberPhase[i]) * 5f;
                embers[i].anchoredPosition = emberRest[i] + new Vector2(0f, bob);
            }
        }
        if (pulse == null)
            return;
        float wave = 1f + Mathf.Sin(Time.unscaledTime * 3.1f) * 0.045f;
        pulse.localScale = new Vector3(wave, wave, 1f);
    }

    IEnumerator Run()
    {
        if (skills)
        {
            MeasurePanel();
            panelX = panelHidden;
            PrimeSkillEntrance();
        }
        yield return null;
        if (skills)
        {
            MenuAppearance.AlignSkills(transform);
            yield return AnimateSkills();
        }
        else
        {
            MenuAppearance.AlignInventory(transform);
            yield return AnimateInventory();
        }
        play = null;
    }

    IEnumerator AnimateInventory()
    {
        RectTransform side = MenuAppearance.FindRect(transform, "Inv");
        RectTransform sheet = MenuAppearance.FindRect(transform, "All");
        Vector2 sideRest = side != null ? side.anchoredPosition : Vector2.zero;
        CanvasGroup sideFade = Group(side);
        CanvasGroup sheetFade = Group(sheet);
        if (sideFade != null)
            sideFade.alpha = 0f;
        if (sheetFade != null)
            sheetFade.alpha = 0f;
        if (side != null)
            side.anchoredPosition = sideRest + new Vector2(56f, 0f);

        yield return new WaitForSecondsRealtime(0.05f);

        List<Transform> slots = new List<Transform>();
        Item[] items = GetComponentsInChildren<Item>(true);
        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].gameObject.activeInHierarchy)
                continue;
            items[i].transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            slots.Add(items[i].transform);
        }

        float duration = 0.36f;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float k = EaseOut(Mathf.Clamp01(time / duration));
            if (side != null)
                side.anchoredPosition = Vector2.Lerp(sideRest + new Vector2(56f, 0f), sideRest, k);
            if (sideFade != null)
                sideFade.alpha = k;
            if (sheetFade != null)
                sheetFade.alpha = k;
            yield return null;
        }

        if (side != null)
            side.anchoredPosition = sideRest;
        if (sideFade != null)
            sideFade.alpha = 1f;
        if (sheetFade != null)
            sheetFade.alpha = 1f;

        for (int i = 0; i < slots.Count; i++)
            StartCoroutine(Pop(slots[i], 0.7f, 1f, i * 0.028f, 0.26f));
    }

    IEnumerator AnimateSkills()
    {
        Image backdrop = null;
        Transform cover = transform.Find("SkillBackdrop");
        if (cover != null)
            backdrop = cover.GetComponent<Image>();
        CanvasGroup stageGroup = Group(transform.Find("SkillStage"));
        CanvasGroup vignetteGroup = Group(transform.Find("SkillVignette"));
        CanvasGroup panelGroup = Group(MenuAppearance.FindRect(transform, "DarkBG"));
        Transform stage = transform.Find("SkillStage");
        SetCoverAlpha(backdrop, 0f);
        if (stageGroup != null)
            stageGroup.alpha = 0f;
        if (vignetteGroup != null)
            vignetteGroup.alpha = 0f;
        if (panelGroup != null)
            panelGroup.alpha = 0f;
        if (stage != null)
            stage.localScale = new Vector3(0.94f, 0.94f, 1f);
        MeasurePanel();
        panelX = panelHidden;

        const float fadeDuration = 0.52f;
        float fadeTime = 0f;
        float guard = 2.4f;
        bool ready = false;
        while (guard > 0f && (!ready || fadeTime < fadeDuration))
        {
            float dt = Time.unscaledDeltaTime;
            fadeTime += dt;
            float coverK = EaseOut(Mathf.Clamp01(fadeTime / 0.4f));
            float stageK = EaseOut(Mathf.Clamp01((fadeTime - 0.06f) / 0.48f));
            float panelK = EaseOut(Mathf.Clamp01((fadeTime - 0.04f) / 0.46f));
            SetCoverAlpha(backdrop, coverK);
            if (vignetteGroup != null)
                vignetteGroup.alpha = coverK;
            panelX = Mathf.Lerp(panelHidden, panelRest, panelK);
            if (panelGroup != null)
                panelGroup.alpha = EaseOut(Mathf.Clamp01((fadeTime - 0.04f) / 0.38f));
            if (stageGroup != null)
                stageGroup.alpha = stageK;
            if (stage != null)
            {
                float scale = Mathf.Lerp(0.94f, 1f, stageK);
                stage.localScale = new Vector3(scale, scale, 1f);
            }
            ConcealTree();
            if (!ready)
            {
                RectTransform probe = MenuAppearance.FindRect(transform, "Skill0");
                ready = probe != null && probe.gameObject.activeInHierarchy && probe.localScale.x > 0.92f;
            }
            guard -= dt;
            yield return null;
        }
        SetCoverAlpha(backdrop, 1f);
        if (vignetteGroup != null)
            vignetteGroup.alpha = 1f;
        if (panelGroup != null)
            panelGroup.alpha = 1f;
        panelX = panelRest;
        if (stageGroup != null)
            stageGroup.alpha = 1f;
        if (stage != null)
            stage.localScale = Vector3.one;

        MenuAppearance.AlignSkills(transform);

        string[] names = { "Skill-1", "Skill0", "Skill1", "Skill2", "Skill3" };
        float[] delayByName = { 0f, 0.46f, 0.36f, 0.26f, 0.16f };
        int[] roadByName = { -1, 0, 1, 3, 2 };
        List<RectTransform> nodes = new List<RectTransform>();
        List<Vector2> targets = new List<Vector2>();
        List<Transform> icons = new List<Transform>();
        List<CanvasGroup> groups = new List<CanvasGroup>();
        List<float> delays = new List<float>();
        List<int> roadLinks = new List<int>();
        for (int i = 0; i < names.Length; i++)
        {
            RectTransform skill = MenuAppearance.FindRect(transform, names[i]);
            if (skill == null)
                continue;
            nodes.Add(skill);
            targets.Add(skill.anchoredPosition);
            groups.Add(Group(skill));
            delays.Add(delayByName[i]);
            roadLinks.Add(roadByName[i]);
            Transform icon = skill.Find("SkillIcon");
            icons.Add(icon);
            skill.anchoredPosition = Vector2.zero;
            if (icon != null)
                icon.localScale = new Vector3(0.2f, 0.2f, 1f);
            groups[groups.Count - 1].alpha = 0f;
        }

        string[] roadNames = { "Road1", "Road2", "Road3", "Road4" };
        List<RectTransform> roads = new List<RectTransform>();
        List<float> roadLengths = new List<float>();
        for (int i = 0; i < roadNames.Length; i++)
        {
            RectTransform road = MenuAppearance.FindRect(transform, roadNames[i]);
            if (road == null)
                continue;
            roads.Add(road);
            roadLengths.Add(road.sizeDelta.y);
            road.sizeDelta = new Vector2(road.sizeDelta.x, 0f);
            road.localScale = Vector3.one;
        }
        liveRoads = roads;

        const float nodeDuration = 0.44f;
        float total = 0.46f + nodeDuration;
        float time = 0f;
        while (time < total)
        {
            time += Time.unscaledDeltaTime;
            for (int i = 0; i < nodes.Count; i++)
            {
                float local = time - delays[i];
                if (local <= 0f)
                    continue;
                float travel = EaseOutBack(Mathf.Clamp01(local / nodeDuration));
                float fade = EaseOut(Mathf.Clamp01(local / (nodeDuration * 0.4f)));
                nodes[i].anchoredPosition = Vector2.LerpUnclamped(Vector2.zero, targets[i], travel);
                groups[i].alpha = fade;
                if (icons[i] != null)
                {
                    float iconScale = Mathf.Lerp(0.2f, 1f, travel);
                    icons[i].localScale = new Vector3(iconScale, iconScale, 1f);
                }
                int roadIndex = roadLinks[i];
                if (roadIndex >= 0 && roadIndex < roads.Count)
                {
                    float drawn = EaseOut(Mathf.Clamp01(local / nodeDuration));
                    roads[roadIndex].sizeDelta = new Vector2(roads[roadIndex].sizeDelta.x, Mathf.Lerp(0f, roadLengths[roadIndex], drawn));
                }
            }
            yield return null;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].anchoredPosition = targets[i];
            groups[i].alpha = 1f;
            if (icons[i] != null)
                icons[i].localScale = Vector3.one;
        }
        for (int i = 0; i < roads.Count; i++)
        {
            roads[i].localScale = Vector3.one;
            roads[i].sizeDelta = new Vector2(roads[i].sizeDelta.x, roadLengths[i]);
        }
        liveRoads = null;
        pulse = SelectedIcon();
        CollectEmbers();
        introDone = true;
    }

    void PrimeSkillEntrance()
    {
        Image backdrop = null;
        Transform cover = transform.Find("SkillBackdrop");
        if (cover != null)
            backdrop = cover.GetComponent<Image>();
        SetCoverAlpha(backdrop, 0f);
        CanvasGroup stageGroup = Group(transform.Find("SkillStage"));
        CanvasGroup vignetteGroup = Group(transform.Find("SkillVignette"));
        CanvasGroup panelGroup = Group(MenuAppearance.FindRect(transform, "DarkBG"));
        if (stageGroup != null)
            stageGroup.alpha = 0f;
        if (vignetteGroup != null)
            vignetteGroup.alpha = 0f;
        if (panelGroup != null)
            panelGroup.alpha = 0f;
        Transform stage = transform.Find("SkillStage");
        if (stage != null)
            stage.localScale = new Vector3(0.94f, 0.94f, 1f);
    }

    void CollectEmbers()
    {
        Transform stage = transform.Find("SkillStage");
        if (stage == null)
            return;
        List<RectTransform> found = new List<RectTransform>();
        for (int i = 0; i < stage.childCount; i++)
        {
            Transform child = stage.GetChild(i);
            if (!child.name.StartsWith("SkillSpark"))
                continue;
            RectTransform rect = child as RectTransform;
            if (rect != null)
                found.Add(rect);
        }
        embers = found.ToArray();
        emberRest = new Vector2[embers.Length];
        emberPhase = new float[embers.Length];
        for (int i = 0; i < embers.Length; i++)
        {
            emberRest[i] = embers[i].anchoredPosition;
            emberPhase[i] = i * 1.35f;
        }
    }

    void MuteSkillWash()
    {
        if (skillWash == null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name != "Scroll View")
                    continue;
                skillWash = child.GetComponent<Image>();
                break;
            }
        }
        if (skillWash == null)
            return;
        Color color = skillWash.color;
        color.a = 0f;
        skillWash.color = color;
    }

    static void SetCoverAlpha(Image image, float alpha)
    {
        if (image == null)
            return;
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public void PlayClose()
    {
        if (!skills || !isActiveAndEnabled || closing)
            return;
        closing = true;
        introDone = false;
        embers = null;
        if (play != null)
            StopCoroutine(play);
        play = StartCoroutine(AnimateClose());
    }

    IEnumerator AnimateClose()
    {
        MeasurePanel();
        Image backdrop = null;
        Transform cover = transform.Find("SkillBackdrop");
        if (cover != null)
            backdrop = cover.GetComponent<Image>();
        CanvasGroup stageGroup = Group(transform.Find("SkillStage"));
        CanvasGroup vignetteGroup = Group(transform.Find("SkillVignette"));
        CanvasGroup panelGroup = Group(panel);
        Transform stage = transform.Find("SkillStage");
        float stageFrom = stage != null ? stage.localScale.x : 1f;
        float panelFrom = panelX;

        string[] names = { "Skill-1", "Skill0", "Skill1", "Skill2", "Skill3" };
        float[] delay = { 0.14f, 0f, 0.04f, 0.08f, 0.1f };
        int[] roadForNode = { -1, 0, 1, 3, 2 };
        List<RectTransform> nodes = new List<RectTransform>();
        List<Vector2> origins = new List<Vector2>();
        List<CanvasGroup> groups = new List<CanvasGroup>();
        List<float> delays = new List<float>();
        List<int> roadLinks = new List<int>();
        for (int i = 0; i < names.Length; i++)
        {
            RectTransform skill = MenuAppearance.FindRect(transform, names[i]);
            if (skill == null)
                continue;
            nodes.Add(skill);
            origins.Add(skill.anchoredPosition);
            groups.Add(Group(skill));
            delays.Add(delay[i]);
            roadLinks.Add(roadForNode[i]);
        }

        string[] roadNames = { "Road1", "Road2", "Road3", "Road4" };
        List<RectTransform> roads = new List<RectTransform>();
        List<float> roadLengths = new List<float>();
        for (int i = 0; i < roadNames.Length; i++)
        {
            RectTransform road = MenuAppearance.FindRect(transform, roadNames[i]);
            if (road == null)
                continue;
            roads.Add(road);
            roadLengths.Add(road.sizeDelta.y);
            road.localScale = Vector3.one;
        }
        liveRoads = roads;

        const float nodeDuration = 0.34f;
        const float total = 0.62f;
        float time = 0f;
        while (time < total)
        {
            time += Time.unscaledDeltaTime;
            for (int i = 0; i < nodes.Count; i++)
            {
                float local = time - delays[i];
                if (local <= 0f)
                    continue;
                float travel = EaseOut(Mathf.Clamp01(local / nodeDuration));
                nodes[i].anchoredPosition = Vector2.Lerp(origins[i], Vector2.zero, travel);
                groups[i].alpha = 1f - travel;
                int roadIndex = roadLinks[i];
                if (roadIndex >= 0 && roadIndex < roads.Count)
                    roads[roadIndex].sizeDelta = new Vector2(roads[roadIndex].sizeDelta.x, Mathf.Lerp(roadLengths[roadIndex], 0f, travel));
            }

            float panelK = EaseOut(Mathf.Clamp01(time / 0.4f));
            panelX = Mathf.Lerp(panelFrom, panelHidden, panelK);
            if (panelGroup != null)
                panelGroup.alpha = 1f - panelK;

            float coverK = EaseOut(Mathf.Clamp01((time - 0.12f) / 0.42f));
            SetCoverAlpha(backdrop, 1f - coverK);
            if (vignetteGroup != null)
                vignetteGroup.alpha = 1f - coverK;
            if (stageGroup != null)
                stageGroup.alpha = 1f - coverK;
            if (stage != null)
            {
                float scale = Mathf.Lerp(stageFrom, 0.94f, coverK);
                stage.localScale = new Vector3(scale, scale, 1f);
            }
            yield return null;
        }

        for (int i = 0; i < groups.Count; i++)
            groups[i].alpha = 0f;
        panelX = panelHidden;
        if (panelGroup != null)
            panelGroup.alpha = 0f;
        SetCoverAlpha(backdrop, 0f);
        if (vignetteGroup != null)
            vignetteGroup.alpha = 0f;
        if (stageGroup != null)
            stageGroup.alpha = 0f;
        liveRoads = null;
        play = null;
    }

    void MeasurePanel()
    {
        panel = MenuAppearance.FindRect(transform, "DarkBG");
        if (panel == null)
            return;
        float width = panel.rect.width;
        if (width < 40f)
            width = 272f;
        panelRest = -width * 0.5f - 22f;
        panelHidden = width * 0.5f + 40f;
        drivePanel = true;
    }

    void HoldSkillPlates()
    {
        if (skillPlates == null)
        {
            string[] names = { "Skill-1", "Skill0", "Skill1", "Skill2", "Skill3" };
            List<Image> plates = new List<Image>();
            for (int i = 0; i < names.Length; i++)
            {
                Transform skill = MenuAppearance.FindRect(transform, names[i]);
                if (skill == null)
                    continue;
                Image image = skill.GetComponent<Image>();
                if (image != null)
                    plates.Add(image);
            }
            skillPlates = plates.ToArray();
        }
        for (int i = 0; i < skillPlates.Length; i++)
        {
            if (skillPlates[i] == null)
                continue;
            Color color = skillPlates[i].color;
            if (color.a >= 0.99f)
                continue;
            color.a = 1f;
            skillPlates[i].color = color;
        }
    }

    void ConcealTree()
    {
        string[] names = { "Skill-1", "Skill0", "Skill1", "Skill2", "Skill3" };
        for (int i = 0; i < names.Length; i++)
        {
            RectTransform skill = MenuAppearance.FindRect(transform, names[i]);
            CanvasGroup group = Group(skill);
            if (group != null)
                group.alpha = 0f;
        }
        string[] roadNames = { "Road1", "Road2", "Road3", "Road4" };
        for (int i = 0; i < roadNames.Length; i++)
        {
            RectTransform road = MenuAppearance.FindRect(transform, roadNames[i]);
            if (road == null)
                continue;
            road.sizeDelta = new Vector2(road.sizeDelta.x, 0f);
        }
    }

    Transform SelectedIcon()
    {
        int equipped = PlayerPrefs.GetInt("Skill");
        SelectSkillForButton[] nodes = GetComponentsInChildren<SelectSkillForButton>(true);
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].id != equipped)
                continue;
            return nodes[i].transform.Find("SkillIcon");
        }
        return null;
    }

    IEnumerator Pop(Transform target, float from, float to, float delay, float duration)
    {
        if (target == null)
            yield break;
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);
        float time = 0f;
        while (time < duration)
        {
            if (target == null)
                yield break;
            time += Time.unscaledDeltaTime;
            float k = EaseOutBack(Mathf.Clamp01(time / duration));
            float scale = Mathf.Lerp(from, to, k);
            target.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        if (target != null)
            target.localScale = new Vector3(to, to, 1f);
    }

    static CanvasGroup Group(Component target)
    {
        if (target == null)
            return null;
        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group == null)
            group = target.gameObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = true;
        group.interactable = true;
        return group;
    }

    static float EaseOut(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    static float EaseOutBack(float t)
    {
        float c1 = 1.4f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
