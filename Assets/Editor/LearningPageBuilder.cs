#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Pasar Ajaib – Learning Page Builder (v3, compact & proper)
/// Menu: PasarAjaib > Build Learning Page
/// </summary>
public static class LearningPageBuilder
{
    const string BASE = "Assets/AssetFigma/LearningPage/";
    const string FONT = "Assets/Fonts (Poppins)/Poppins-SemiBold SDF.asset";

    static readonly Color BG_COL  = new Color(0.72f, 0.85f, 0.94f, 1f);
    static readonly Color HDR_COL = new Color(0.36f, 0.64f, 0.83f, 1f);
    static readonly Color WHITE   = Color.white;
    static readonly Color DARK    = new Color(0.12f, 0.22f, 0.32f, 1f);
    static readonly Color GRAY    = new Color(0.40f, 0.50f, 0.60f, 1f);

    // ── Ukuran card & layout (sesuai card yang sudah dirapikan user: 784×718) ──
    const float CARD_W   = 784f;
    const float CARD_H   = 718f;
    const float PAD      = 30f;    // padding kiri-kanan
    const float BTN_H    = 94f;    // tinggi tombol Mulai Belajar
    const float TAG_H    = 60f;    // tinggi tag
    const float TAG_W    = 330f;   // lebar tag (2 tag per baris dalam 784px)
    const float LOGO_SZ  = 80f;    // ukuran icon lapak
    const float MODUL_SZ = 64f;    // ukuran icon modul

    [MenuItem("PasarAjaib/Build Learning Page")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();

        Canvas cvs = null;
        foreach (var r in scene.GetRootGameObjects())
        { cvs = r.GetComponent<Canvas>(); if (cvs) break; }
        if (!cvs) { Debug.LogError("[LearningPage] Canvas tidak ditemukan!"); return; }

        while (cvs.transform.childCount > 0)
            Undo.DestroyObjectImmediate(cvs.transform.GetChild(0).gameObject);

        var scaler = cvs.GetComponent<CanvasScaler>() ?? cvs.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 0.5f;
        if (!cvs.GetComponent<GraphicRaycaster>()) cvs.gameObject.AddComponent<GraphicRaycaster>();

        // ── BACKGROUND ────────────────────────────────────────────────────────
        var bg = Go(cvs.gameObject, "Background");
        bg.AddComponent<Image>().color = BG_COL;
        Stretch(bg);

        // ── HEADER ────────────────────────────────────────────────────────────
        var header = Go(cvs.gameObject, "Header");
        header.AddComponent<Image>().color = HDR_COL;
        var hRT = RT(header);
        hRT.anchorMin = new Vector2(0,1); hRT.anchorMax = new Vector2(1,1);
        hRT.pivot     = new Vector2(0.5f,1f);
        hRT.anchoredPosition = Vector2.zero;
        hRT.sizeDelta = new Vector2(0, 210);

        // Back button
        var backGO = Go(header, "BackButton");
        var bImg   = backGO.AddComponent<Image>();
        var bSp    = LoadSp("Assets/AssetFigma/back-button.png");
        if (bSp) { bImg.sprite = bSp; bImg.preserveAspect = true; }
        bImg.color = WHITE;
        var backBtn = backGO.AddComponent<Button>(); backBtn.targetGraphic = bImg;
        backGO.AddComponent<UIButtonAnimator>();
        var bkRT = RT(backGO);
        bkRT.anchorMin = bkRT.anchorMax = new Vector2(0f, 0.5f);
        bkRT.pivot = new Vector2(0f, 0.5f);
        bkRT.anchoredPosition = new Vector2(36, -6);
        bkRT.sizeDelta = new Vector2(50, 50);

        // Judul
        var tGO = TMP(header, "Title", "Pilih Lapak", 50, FontStyles.Bold, WHITE);
        var tRT2 = RT(tGO);
        tRT2.anchorMin = new Vector2(0,0); tRT2.anchorMax = new Vector2(1,1);
        tRT2.offsetMin = new Vector2(0, 52); tRT2.offsetMax = new Vector2(0,0);
        tGO.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        var sGO = TMP(header, "Subtitle", "Siap Belajar?", 28, FontStyles.Normal, new Color(1,1,1,0.85f));
        var sRT = RT(sGO);
        sRT.anchorMin = new Vector2(0,0); sRT.anchorMax = new Vector2(1,0);
        sRT.pivot = new Vector2(0.5f,0f);
        sRT.anchoredPosition = new Vector2(0, 24); sRT.sizeDelta = new Vector2(0, 40);
        sGO.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        // ── Fix import button sprites sebagai 9-slice ─────────────────────────
        SetSliceBorder(BASE + "start-lapak-button.png", 44);
        SetSliceBorder(BASE + "tombol mulai.png",        44);

        // Layout: Header(210) + gap(20) + Card1(460) + gap(20) + Card2(460) = 1170
        // Sisa bawah 1920-1170 = 750px → ada ruang
        var hitungCard = BuildCard(cvs.gameObject, "LapakHitung",
            BASE + "box matematika.png", BASE + "logo-lapak.png", BASE + "modul-button.png",
            "Lapak Hitung", "Matematika AR",
            "Belajar membandingkan angka, penjumlahan,\npengurangan, dan cerita matematika yang seru!",
            new[]{BASE+"tag-hitung-card-3.png", BASE+"tag-hitung-card-2.png",
                  BASE+"tag-hitung-card.png",   BASE+"tag-hitung-card-1.png"},
            BASE + "start-lapak-button.png", topY: -230f);

        var ceritaCard = BuildCard(cvs.gameObject, "LapakLiterasi",
            BASE + "box literasi.png", BASE + "logo-lapak-1.png", BASE + "modul-button-1.png",
            "Lapak Cerita", "Literasi AR",
            "Susun kalimat, jawab pertanyaan cerita, dan\nbantu tokoh mencapai tujuan mereka.",
            new[]{BASE+"tag-card.png",   BASE+"tag-card-1.png",
                  BASE+"tag-card-2.png", BASE+"tag-card-3.png"},
            BASE + "tombol mulai.png", topY: -968f); // 230 + 718 + 20

        // ── LearningManager ───────────────────────────────────────────────────
        var lm = cvs.GetComponent<LearningManager>() ?? cvs.gameObject.AddComponent<LearningManager>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(backBtn.onClick, lm.BackToHome);

        var hBtn = hitungCard.transform.Find("MulaiBelajar")?.GetComponent<Button>();
        var cBtn = ceritaCard.transform.Find("MulaiBelajar")?.GetComponent<Button>();
        if (hBtn) UnityEditor.Events.UnityEventTools.AddPersistentListener(hBtn.onClick, lm.GoToMatematika);
        if (cBtn) UnityEditor.Events.UnityEventTools.AddPersistentListener(cBtn.onClick, lm.GoToLiterasi);

        // ── EventSystem ───────────────────────────────────────────────────────
        bool hasES = false;
        foreach (var r in scene.GetRootGameObjects())
            if (r.GetComponent<UnityEngine.EventSystems.EventSystem>()) { hasES = true; break; }
        if (!hasES)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[PasarAjaib] ✅ Learning Page v3 selesai!");
    }

    // ── Build satu card lapak ──────────────────────────────────────────────────
    static GameObject BuildCard(GameObject parent, string name,
        string boxPath, string logoPath, string modulPath,
        string title, string subtitle, string desc,
        string[] tags, string btnPath, float topY)
    {
        // Container card (anchor top-center)
        var card = Go(parent, name);
        var cImg = card.AddComponent<Image>();
        var boxSp = LoadSp(boxPath);
        if (boxSp) { cImg.sprite = boxSp; cImg.type = Image.Type.Simple; cImg.color = WHITE; }
        else cImg.color = new Color(1f,1f,0.9f,1f);
        var cRT = RT(card);
        cRT.anchorMin = cRT.anchorMax = new Vector2(0.5f, 1f);
        cRT.pivot     = new Vector2(0.5f, 1f);
        cRT.anchoredPosition = new Vector2(0, topY);
        cRT.sizeDelta = new Vector2(CARD_W, CARD_H);

        // Layout vertikal untuk card 784×718
        // Total space = 718px, pad atas/bawah = 30px → available = 658px
        // Row1(logo 80) + gap(18) + desc(120) + gap(24) + tags(60+14+60) + gap(30) + btn(94) + pad_bawah(30) = 560
        // Extra space = 658-560 = 98px → distribusikan ke gaps

        float y = -30f; // top padding

        // ── Row 1: Logo | Title + Subtitle | Modul ───────────────────────────
        PlaceImg(card, "LogoIcon", logoPath,
            new Vector2(0f,1f), new Vector2(0f,1f), new Vector2(0f,1f),
            new Vector2(PAD, y), new Vector2(LOGO_SZ, LOGO_SZ));

        float titleX = PAD + LOGO_SZ + 16f;
        float titleW = CARD_W - titleX - MODUL_SZ - 16f - PAD;

        var titleGO = TMP(card, "CardTitle", title, 38, FontStyles.Bold, DARK);
        var ttRT    = RT(titleGO);
        ttRT.anchorMin = ttRT.anchorMax = new Vector2(0f,1f);
        ttRT.pivot = new Vector2(0f,1f);
        ttRT.anchoredPosition = new Vector2(titleX, y - 4f);
        ttRT.sizeDelta = new Vector2(titleW, 48);
        titleGO.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        var subGO = TMP(card, "CardSubtitle", subtitle, 24, FontStyles.Normal, GRAY);
        var stRT  = RT(subGO);
        stRT.anchorMin = stRT.anchorMax = new Vector2(0f,1f);
        stRT.pivot = new Vector2(0f,1f);
        stRT.anchoredPosition = new Vector2(titleX, y - 56f);
        stRT.sizeDelta = new Vector2(titleW, 34);
        subGO.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        PlaceImg(card, "ModulIcon", modulPath,
            new Vector2(1f,1f), new Vector2(1f,1f), new Vector2(1f,1f),
            new Vector2(-PAD, y), new Vector2(MODUL_SZ, MODUL_SZ));

        y -= LOGO_SZ + 26f; // -136

        // ── Divider tipis ─────────────────────────────────────────────────────
        var div = Go(card, "Divider");
        var dImg2 = div.AddComponent<Image>();
        dImg2.color = new Color(0.85f, 0.85f, 0.85f, 0.6f);
        var divRT = RT(div);
        divRT.anchorMin = new Vector2(0.04f, 1f); divRT.anchorMax = new Vector2(0.96f, 1f);
        divRT.pivot = new Vector2(0.5f, 1f);
        divRT.anchoredPosition = new Vector2(0, y); divRT.sizeDelta = new Vector2(0, 1.5f);
        y -= 16f;

        // ── Deskripsi ─────────────────────────────────────────────────────────
        var descGO = TMP(card, "CardDesc", desc, 26, FontStyles.Normal, DARK);
        var dRT    = RT(descGO);
        dRT.anchorMin = dRT.anchorMax = new Vector2(0f,1f);
        dRT.pivot = new Vector2(0f,1f);
        dRT.anchoredPosition = new Vector2(PAD, y);
        dRT.sizeDelta = new Vector2(CARD_W - PAD*2, 110);
        var dTMP = descGO.GetComponent<TMP_Text>();
        dTMP.alignment = TextAlignmentOptions.TopLeft;
        dTMP.lineSpacing = 8;
        y -= 124f;

        // ── Tags (2 × 2) ──────────────────────────────────────────────────────
        // TAG_W=330, gap=14px, 2 tags per row → 330+14+330=674 < 724(avail) ✓
        float tagGap = (CARD_W - PAD*2 - TAG_W*2) / 2f; // ~17px auto-gap
        float[] tagX = { PAD, PAD + TAG_W + tagGap, PAD, PAD + TAG_W + tagGap };
        float[] tagY = { y, y, y - TAG_H - 14f, y - TAG_H - 14f };
        for (int i = 0; i < Mathf.Min(tags.Length, 4); i++)
        {
            var tGO  = Go(card, $"Tag{i+1}");
            var tImg = tGO.AddComponent<Image>();
            var tSp  = LoadSp(tags[i]);
            if (tSp) { tImg.sprite = tSp; tImg.preserveAspect = true; tImg.color = WHITE; }
            else tImg.color = new Color(0.97f, 0.93f, 0.55f, 1f);
            var tRT  = RT(tGO);
            tRT.anchorMin = tRT.anchorMax = new Vector2(0f,1f);
            tRT.pivot = new Vector2(0f,1f);
            tRT.anchoredPosition = new Vector2(tagX[i], tagY[i]);
            tRT.sizeDelta = new Vector2(TAG_W, TAG_H);
        }
        y -= TAG_H * 2 + 14f + 30f;

        // ── Tombol Mulai Belajar ──────────────────────────────────────────────
        var btnGO  = Go(card, "MulaiBelajar");
        var btnImg = btnGO.AddComponent<Image>();
        var btnSp  = LoadSp(btnPath);
        if (btnSp) { btnImg.sprite = btnSp; btnImg.type = Image.Type.Sliced; btnImg.color = WHITE; }
        else btnImg.color = new Color(1f, 0.82f, 0f, 1f);
        var btn = btnGO.AddComponent<Button>(); btn.targetGraphic = btnImg;
        btnGO.AddComponent<UIButtonAnimator>();
        var bRT = RT(btnGO);
        bRT.anchorMin = bRT.anchorMax = new Vector2(0.5f,1f);
        bRT.pivot     = new Vector2(0.5f,1f);
        bRT.anchoredPosition = new Vector2(0, y);
        bRT.sizeDelta = new Vector2(CARD_W - PAD*2, BTN_H);

        return card;
    }

    // ── Set 9-slice border pada sprite button ─────────────────────────────────
    // left=border (sudut kiri pill), bottom=4, right=border (sudut kanan), top=4
    // Ini memastikan sudut pill tidak distort saat tombol dilebarkan
    static void SetSliceBorder(string path, float cornerRadius)
    {
        var ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null) return;
        bool changed = false;
        if (ti.textureType != TextureImporterType.Sprite)
        { ti.textureType = TextureImporterType.Sprite; changed = true; }
        // Pill button: border kiri & kanan = corner radius, atas & bawah minimal
        var newBorder = new Vector4(cornerRadius, 4f, cornerRadius, 4f);
        if (ti.spriteBorder != newBorder)
        { ti.spriteBorder = newBorder; changed = true; }
        if (changed) ti.SaveAndReimport();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    static void PlaceImg(GameObject p, string n, string path,
        Vector2 ancMin, Vector2 ancMax, Vector2 piv, Vector2 aPos, Vector2 sz)
    {
        var go = Go(p, n);
        var img = go.AddComponent<Image>();
        var sp  = LoadSp(path);
        if (sp) { img.sprite = sp; img.preserveAspect = true; img.color = WHITE; }
        else img.color = Color.clear;
        var rt = RT(go);
        rt.anchorMin = ancMin; rt.anchorMax = ancMax; rt.pivot = piv;
        rt.anchoredPosition = aPos; rt.sizeDelta = sz;
    }

    static GameObject Go(GameObject p, string n)
    {
        var go = new GameObject(n);
        go.transform.SetParent(p.transform, false);
        go.layer = 5;
        go.AddComponent<RectTransform>();
        return go;
    }

    static GameObject TMP(GameObject p, string n, string txt, float sz, FontStyles fs, Color col)
    {
        var go = new GameObject(n);
        go.transform.SetParent(p.transform, false);
        go.layer = 5;
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = txt; t.fontSize = sz; t.fontStyle = fs; t.color = col;
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT);
        if (font) t.font = font;
        return go;
    }

    static RectTransform RT(GameObject go) =>
        go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();

    static void Stretch(GameObject go)
    {
        var rt = RT(go);
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static Sprite LoadSp(string path)
    {
        var s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (!s) { var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex) s = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(0.5f,0.5f)); }
        return s;
    }
}
#endif
