#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Pasar Ajaib – Home Scene Builder
/// Menu: PasarAjaib > Build Home UI
///
/// Layout sesuai Figma "Page Home":
///   - Background biru muda
///   - Header card biru: avatar + "Selamat Datang, [Nama]" + trophy + 5 bintang
///   - MainCard: logo Pasar Ajaib AR (floating) + welcome text
///   - Tombol "Mulai Berdagang" oranye
///   - Tombol Settings (gear, pojok kanan bawah)
///   - SettingsPopup & PialaPopUp dipertahankan (tidak dihapus)
/// </summary>
public static class HomeSceneBuilder
{
    // ── Warna ─────────────────────────────────────────────────────────────────
    static readonly Color BG_COLOR     = Hex("#B8D9F0");
    static readonly Color HEADER_COLOR = Hex("#5BA3D0");
    static readonly Color CARD_COLOR   = Hex("#9BBFD8");
    static readonly Color TEXT_DARK    = Hex("#1A4A6B");
    static readonly Color TEXT_LIGHT   = Hex("#FFFFFF");
    static readonly Color TEXT_SUB     = Hex("#4A7FA5");
    static readonly Color STAR_EMPTY   = new Color(1f, 1f, 1f, 0.4f);

    // ── Asset paths ────────────────────────────────────────────────────────────
    const string LOGO_PATH    = "Assets/AssetFigma/logo.png";
    const string STAR_PATH    = "Assets/AssetFigma/bintang1.png";
    const string BTN_PLAY_PATH= "Assets/Assets/Home/BtnMulaiBerdagang.png";
    const string PIALA_PATH   = "Assets/Assets/Home/piala.png";
    const string FONT_PATH    = "Assets/Fonts (Poppins)/Poppins-SemiBold SDF.asset";

    static readonly string[] PROFILE_PATHS = {
        "Assets/Assets/Profile/1 1.png",
        "Assets/Assets/Profile/2 1.png",
        "Assets/Assets/Profile/3 1.png",
        "Assets/Assets/Profile/4 1.png",
        "Assets/Assets/Profile/5 1.png",
        "Assets/Assets/Profile/6 1.png",
        "Assets/Assets/Profile/7 1.png",
        "Assets/Assets/Profile/beruang.png",
        "Assets/Assets/Profile/jerapah.png",
        "Assets/Assets/Profile/panda.png",
        "Assets/Assets/Profile/sapi.png",
    };

    const float REF_W = 1080f;
    const float REF_H = 1920f;

    [MenuItem("PasarAjaib/Build Home UI")]
    public static void BuildHomeUI()
    {
        // 1 ─ Buka HomeScene
        string scenePath = "Assets/Scenes/HomeScene.unity";
        if (EditorSceneManager.GetActiveScene().path != scenePath)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(scenePath);
        }

        Scene scene = EditorSceneManager.GetActiveScene();

        // 2 ─ Simpan referensi popup sebelum hapus Canvas
        GameObject savedSettings = null, savedPiala = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            var canvas = root.GetComponent<Canvas>();
            if (canvas == null) continue;

            // Lepas popup dari Canvas agar tidak ikut terhapus
            foreach (Transform t in root.transform)
            {
                if (t.name == "SettingsPopup") { savedSettings = t.gameObject; t.SetParent(null); }
                if (t.name == "PialaPopUp")    { savedPiala    = t.gameObject; t.SetParent(null); }
            }
            Undo.DestroyObjectImmediate(root);
            break;
        }

        // Hapus EventSystem lama
        foreach (var root in scene.GetRootGameObjects())
            if (root.GetComponent<UnityEngine.EventSystems.EventSystem>() != null)
                Undo.DestroyObjectImmediate(root);

        // 3 ─ Buat Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Undo.RegisterCreatedObjectUndo(canvasGO, "Build Home UI");
        canvasGO.layer = LayerMask.NameToLayer("UI");

        Canvas canvas2 = canvasGO.AddComponent<Canvas>();
        canvas2.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(REF_W, REF_H);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // 4 ─ Background
        var bg = MakeImage(canvasGO, "Background", BG_COLOR);
        Stretch(bg);

        // 5 ─ Header Card
        var header = MakeImage(canvasGO, "TopBar", HEADER_COLOR);
        var hRT = header.GetComponent<RectTransform>();
        hRT.anchorMin = new Vector2(0, 1); hRT.anchorMax = new Vector2(1, 1);
        hRT.pivot = new Vector2(0.5f, 1f);
        hRT.anchoredPosition = Vector2.zero;
        hRT.sizeDelta = new Vector2(0, 340);

        // 5a ─ Avatar circle — buat circle sprite dan simpan ke project
        var circleSprite = GetOrCreateCircleSprite();

        var avatarBg = MakeImage(header, "ProfileBg", Color.white);
        var abRT = avatarBg.GetComponent<RectTransform>();
        abRT.anchorMin = abRT.anchorMax = new Vector2(0f, 1f);
        abRT.pivot = new Vector2(0f, 1f);
        abRT.anchoredPosition = new Vector2(40, -40);
        abRT.sizeDelta = new Vector2(160, 160);

        // Set circle sprite → Mask akan clip sesuai bentuk lingkaran
        if (circleSprite != null) avatarBg.GetComponent<Image>().sprite = circleSprite;
        avatarBg.GetComponent<Image>().color = Color.white;

        var avatarMask = avatarBg.AddComponent<Mask>();
        avatarMask.showMaskGraphic = false; // sembunyikan lingkaran putih, hanya foto yang tampil

        var profileImg = MakeImage(avatarBg, "ProfileImage", Color.white);
        profileImg.GetComponent<Image>().preserveAspect = false;
        // Fill penuh ke parent, di-crop oleh Mask lingkaran
        var piRT = profileImg.GetComponent<RectTransform>();
        piRT.anchorMin = Vector2.zero;
        piRT.anchorMax = Vector2.one;
        piRT.offsetMin = Vector2.zero;
        piRT.offsetMax = Vector2.zero;
        // AspectRatioFitter EnvelopeParent = scale to fill sambil jaga rasio
        var arf = profileImg.AddComponent<AspectRatioFitter>();
        arf.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

        // 5b ─ Teks sambutan (ramah anak)
        var greeting = MakeTMP(header, "Greeting", "Halo, Pedagang Cilik!", 30, FontStyles.Normal, TEXT_LIGHT);
        var gRT = greeting.GetComponent<RectTransform>();
        gRT.anchorMin = gRT.anchorMax = new Vector2(0f, 1f);
        gRT.pivot = new Vector2(0f, 1f);
        gRT.anchoredPosition = new Vector2(220, -50);
        gRT.sizeDelta = new Vector2(600, 50);

        var nameObj = MakeTMP(header, "Nama", "Nama", 48, FontStyles.Bold, TEXT_LIGHT);
        var nRT = nameObj.GetComponent<RectTransform>();
        nRT.anchorMin = nRT.anchorMax = new Vector2(0f, 1f);
        nRT.pivot = new Vector2(0f, 1f);
        nRT.anchoredPosition = new Vector2(220, -100);
        nRT.sizeDelta = new Vector2(560, 70);

        // 5c ─ Tombol Piala — langsung pakai piala.png tanpa background tambahan
        var pialaBtn = new GameObject("Piala");
        pialaBtn.transform.SetParent(header.transform, false);
        pialaBtn.layer = LayerMask.NameToLayer("UI");

        var pialaSprite = LoadSprite(PIALA_PATH);
        var pialaBtnImg = pialaBtn.AddComponent<Image>();
        if (pialaSprite != null)
        {
            pialaBtnImg.sprite = pialaSprite;
            pialaBtnImg.preserveAspect = true;
            pialaBtnImg.color = Color.white;
        }
        else
        {
            pialaBtnImg.color = new Color(0, 0, 0, 0); // transparan kalau sprite tidak ada
        }

        var pialaBtnComp = pialaBtn.AddComponent<Button>();
        pialaBtnComp.targetGraphic = pialaBtnImg;
        // Hilangkan highlight default agar tidak ada perubahan warna saat hover
        var colors = pialaBtnComp.colors;
        colors.normalColor      = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor     = new Color(0.75f, 0.75f, 0.75f, 1f);
        pialaBtnComp.colors     = colors;
        pialaBtn.AddComponent<UIButtonAnimator>();

        var pRT = pialaBtn.GetComponent<RectTransform>();
        pRT.anchorMin = pRT.anchorMax = new Vector2(1f, 1f);
        pRT.pivot = new Vector2(1f, 1f);
        pRT.anchoredPosition = new Vector2(-30, -40);
        pRT.sizeDelta = new Vector2(120, 120);

        // 5d ─ Bintang (5 bintang di bawah header)
        var starsRow = new GameObject("StarsRow");
        starsRow.transform.SetParent(header.transform, false);
        starsRow.layer = LayerMask.NameToLayer("UI");
        starsRow.AddComponent<RectTransform>();
        var srRT = starsRow.GetComponent<RectTransform>();
        srRT.anchorMin = srRT.anchorMax = new Vector2(0.5f, 0f);
        srRT.pivot = new Vector2(0.5f, 0f);
        srRT.anchoredPosition = new Vector2(0, 20);
        srRT.sizeDelta = new Vector2(400, 60);
        var hlg = starsRow.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 15;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = false; hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false; hlg.childForceExpandHeight = false;

        // 6 bintang = 3 level Literasi + 3 level Matematika
        var starSprite = LoadSprite(STAR_PATH);
        var starImgList = new System.Collections.Generic.List<Image>();
        for (int i = 0; i < 6; i++)
        {
            var s = starSprite != null
                ? MakeImageWithSprite(starsRow, $"Star{i + 1}", starSprite)
                : MakeImage(starsRow, $"Star{i + 1}", STAR_EMPTY);
            var sImg = s.GetComponent<Image>();
            sImg.color = STAR_EMPTY; // awalnya mati semua
            starImgList.Add(sImg);
            var sRT = s.GetComponent<RectTransform>();
            sRT.sizeDelta = new Vector2(50, 50);
        }

        // 6 ─ MainCard (welcome card) — pakai input boxx.png untuk rounded corners
        const string CARD_SPRITE_PATH = "Assets/AssetFigma/input boxx.png";
        var cardSprite2 = LoadSprite(CARD_SPRITE_PATH);
        var card = cardSprite2 != null
            ? MakeImageWithSprite(canvasGO, "MainCard", cardSprite2)
            : MakeImage(canvasGO, "MainCard", CARD_COLOR);
        var cRT = card.GetComponent<RectTransform>();
        cRT.anchorMin = cRT.anchorMax = new Vector2(0.5f, 0.5f);
        cRT.pivot = new Vector2(0.5f, 0.5f);
        cRT.anchoredPosition = new Vector2(0, -50);
        cRT.sizeDelta = new Vector2(860, 500);
        // Simple type agar rounded corners tidak distort
        card.GetComponent<Image>().type   = Image.Type.Simple;
        card.GetComponent<Image>().preserveAspect = false;

        // 6a ─ Logo (floating di atas card, overlap ~200px dari tepi atas card)
        // Card top edge = -50 + 500/2 = 200 dari center canvas
        // Logo bawah harus ada di ~top edge card: anchoredPosition.y = 200 + some overlap
        var logoSprite = LoadSprite(LOGO_PATH);
        if (logoSprite != null)
        {
            var logo = MakeImageWithSprite(canvasGO, "Logo", logoSprite);
            logo.transform.SetSiblingIndex(card.transform.GetSiblingIndex() + 1);
            var lRT = logo.GetComponent<RectTransform>();
            lRT.anchorMin = lRT.anchorMax = new Vector2(0.5f, 0.5f);
            lRT.pivot = new Vector2(0.5f, 0f);     // pivot di bawah logo
            lRT.anchoredPosition = new Vector2(0, 130); // bottom logo = 130, top = 130+460=590
            lRT.sizeDelta = new Vector2(460, 460);
            logo.GetComponent<Image>().preserveAspect = true;
        }

        // 6b ─ Judul welcome (posisi di bagian bawah card, setelah logo overlap atas)
        var title = MakeTMP(card, "WelcomeTitle",
            "Selamat Datang di\nPasar Ajaib AR!!!",
            40, FontStyles.Bold, TEXT_DARK);
        var tRT = title.GetComponent<RectTransform>();
        tRT.anchorMin = tRT.anchorMax = new Vector2(0.5f, 0.5f);
        tRT.pivot = new Vector2(0.5f, 0.5f);
        tRT.anchoredPosition = new Vector2(0, 10);
        tRT.sizeDelta = new Vector2(750, 140);
        title.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        // 6c ─ Subjudul
        var sub = MakeTMP(card, "WelcomeSub",
            "Yuk belajar menjadi pedagang\nyang cerdas dan pintar!",
            26, FontStyles.Normal, TEXT_SUB);
        var subRT = sub.GetComponent<RectTransform>();
        subRT.anchorMin = subRT.anchorMax = new Vector2(0.5f, 0.5f);
        subRT.pivot = new Vector2(0.5f, 0.5f);
        subRT.anchoredPosition = new Vector2(0, -90);
        subRT.sizeDelta = new Vector2(700, 100);
        sub.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        // 7 ─ Tombol Mulai Berdagang (center, di bawah form card)
        var btnSprite = LoadSprite(BTN_PLAY_PATH);
        var playBtn = new GameObject("BtnMulaiDagang");
        playBtn.transform.SetParent(canvasGO.transform, false);
        playBtn.layer = LayerMask.NameToLayer("UI");
        var pbImg = playBtn.AddComponent<Image>();
        if (btnSprite != null) { pbImg.sprite = btnSprite; pbImg.color = Color.white; pbImg.preserveAspect = false; }
        else pbImg.color = Hex("#F5A623");
        var pbComp = playBtn.AddComponent<Button>();
        pbComp.targetGraphic = pbImg;
        playBtn.AddComponent<UIButtonAnimator>();
        var pbRT = playBtn.GetComponent<RectTransform>();
        pbRT.anchorMin = pbRT.anchorMax = new Vector2(0.5f, 0.5f);
        pbRT.pivot = new Vector2(0.5f, 0.5f);
        pbRT.anchoredPosition = new Vector2(0, -360);
        pbRT.sizeDelta = new Vector2(700, 110); // rasio asli ~6.4:1, tidak gepeng

        // 8 ─ Tombol Pengaturan (center, di bawah Mulai Berdagang)
        var settingBtn = new GameObject("BtnSetting");
        settingBtn.transform.SetParent(canvasGO.transform, false);
        settingBtn.layer = LayerMask.NameToLayer("UI");
        var sbImg = settingBtn.AddComponent<Image>();
        var btnSettingSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Assets/Home/bookReport-button.png");
        if (btnSettingSprite == null) btnSettingSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Assets/Home/BtnSetting.png");
        if (btnSettingSprite != null) { sbImg.sprite = btnSettingSprite; sbImg.color = Color.white; sbImg.preserveAspect = false; }
        else sbImg.color = Hex("#6CB33F");
        var sbComp = settingBtn.AddComponent<Button>();
        sbComp.targetGraphic = sbImg;
        settingBtn.AddComponent<UIButtonAnimator>();
        var sbRT = settingBtn.GetComponent<RectTransform>();
        sbRT.anchorMin = sbRT.anchorMax = new Vector2(0.5f, 0.5f);
        sbRT.pivot = new Vector2(0.5f, 0.5f);
        sbRT.anchoredPosition = new Vector2(0, -490);
        sbRT.sizeDelta = new Vector2(680, 100);
        sbRT.sizeDelta = new Vector2(200, 100);
        sbRT.anchoredPosition = new Vector2(-30, 40);

        // 9 ─ Kembalikan popup ke Canvas
        if (savedSettings != null)
        {
            savedSettings.transform.SetParent(canvasGO.transform, false);
            savedSettings.SetActive(false);
        }
        if (savedPiala != null)
        {
            savedPiala.transform.SetParent(canvasGO.transform, false);
            savedPiala.SetActive(false);
        }

        // 10 ─ Wire HomeManager
        var hm = canvasGO.AddComponent<HomeManager>();
        hm.nameText     = nameObj.GetComponent<TMP_Text>();
        hm.profileImage = profileImg.GetComponent<Image>();

        // Load semua profile sprites
        var sprites = new System.Collections.Generic.List<Sprite>();
        foreach (var path in PROFILE_PATHS)
        {
            var sp = LoadSprite(path);
            if (sp != null) sprites.Add(sp);
        }
        hm.profileSprites = sprites.ToArray();

        // Wire bintang (starSprite = bintang1.png kuning)
        hm.starImages  = starImgList.ToArray();
        hm.starFilled  = starSprite;
        hm.starEmpty   = null; // null = pakai warna transparan

        // 11 ─ Wire UIManager
        var um = canvasGO.AddComponent<UIManager>();
        if (savedSettings != null) um.settingsPopup = savedSettings;
        if (savedPiala    != null) um.pialaPopup    = savedPiala;

        // Wire tombol
        UnityEditor.Events.UnityEventTools.AddPersistentListener(pbComp.onClick,    um.GoToLearning);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(sbComp.onClick,    um.OpenSettings);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(pialaBtnComp.onClick, um.OpenPiala);

        // 12 ─ AudioManager
        bool hasAudio = false;
        foreach (var r in scene.GetRootGameObjects())
            if (r.GetComponent<AudioManager>() != null) { hasAudio = true; break; }
        if (!hasAudio)
        {
            var amGO = new GameObject("AudioManager");
            var am = amGO.AddComponent<AudioManager>();
            am.sfxButtonClick = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/click button/button1.wav");
            am.bgmMenu        = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/background/background1.mp3");
        }

        // 13 ─ EventSystem
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // 14 ─ Simpan
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[PasarAjaib] HomeScene UI rebuilt successfully!");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static GameObject MakeImage(GameObject parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.layer = LayerMask.NameToLayer("UI");
        go.AddComponent<Image>().color = color;
        return go;
    }

    static GameObject MakeImageWithSprite(GameObject parent, string name, Sprite sprite)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.layer = LayerMask.NameToLayer("UI");
        var img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.color  = Color.white;
        return go;
    }

    static GameObject MakeTMP(GameObject parent, string name, string text,
        float size, FontStyles style, Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.layer = LayerMask.NameToLayer("UI");
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.fontStyle = style;
        tmp.color     = color;
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_PATH);
        if (font != null) tmp.font = font;
        return go;
    }

    static void Stretch(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// Load circle sprite dari Assets/Assets/Profile/circle_mask.png.
    /// File sudah dibuat sebelumnya via Python/script.
    /// </summary>
    static Sprite GetOrCreateCircleSprite()
    {
        const string circlePath = "Assets/Assets/Profile/circle_mask.png";
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(circlePath);
        if (sprite == null)
            Debug.LogWarning($"[PasarAjaib] Circle mask tidak ditemukan: {circlePath}");
        return sprite;
    }

    static Sprite LoadSprite(string path)
    {
        var s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (s == null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex != null)
                s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        if (s == null) Debug.LogWarning($"[PasarAjaib] Sprite tidak ditemukan: {path}");
        return s;
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }
}
#endif
