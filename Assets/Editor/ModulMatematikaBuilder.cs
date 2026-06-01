#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Pasar Ajaib – Modul Matematika Popup Builder
/// Menu: PasarAjaib > Build Modul Matematika
///
/// Membuat popup viewer 13 halaman modul.
/// Muncul saat ModulIcon di LapakHitung diklik.
/// </summary>
public static class ModulMatematikaBuilder
{
    const string MODUL_PATH = "Assets/AssetFigma/ModulMatematika/";
    const string FONT       = "Assets/Fonts (Poppins)/Poppins-SemiBold SDF.asset";
    const int    TOTAL_PAGES = 13;

    static readonly Color OVERLAY_COL = new Color(0f, 0f, 0f, 0.85f);
    static readonly Color HEADER_COL  = new Color(0.36f, 0.64f, 0.83f, 1f);
    static readonly Color NAV_COL     = new Color(0.36f, 0.64f, 0.83f, 1f);
    static readonly Color NAV_DIM_COL = new Color(0.5f, 0.5f, 0.5f, 0.6f);
    static readonly Color WHITE       = Color.white;
    static readonly Color DARK        = new Color(0.1f, 0.25f, 0.4f, 1f);

    [MenuItem("PasarAjaib/Build Modul Matematika")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();

        Canvas cvs = null;
        foreach (var r in scene.GetRootGameObjects())
        { cvs = r.GetComponent<Canvas>(); if (cvs) break; }
        if (!cvs) { Debug.LogError("[ModulMatematika] Canvas tidak ditemukan!"); return; }

        // ── Fix import semua halaman modul sebagai Sprite ─────────────────────
        FixModulImports();

        // ── Hapus popup lama kalau ada ────────────────────────────────────────
        var oldTF = cvs.transform.Find("ModulMatematikaPopup");
        if (oldTF) Undo.DestroyObjectImmediate(oldTF.gameObject);

        // ── Buat popup root ───────────────────────────────────────────────────
        var popupGO = new GameObject("ModulMatematikaPopup");
        Undo.RegisterCreatedObjectUndo(popupGO, "Build Modul Matematika");
        popupGO.transform.SetParent(cvs.transform, false);
        popupGO.layer = 5;
        var popRT = popupGO.AddComponent<RectTransform>();
        popRT.anchorMin = Vector2.zero; popRT.anchorMax = Vector2.one;
        popRT.offsetMin = popRT.offsetMax = Vector2.zero;
        popupGO.SetActive(false);

        // ── Overlay ───────────────────────────────────────────────────────────
        var ov = Child(popupGO, "Overlay");
        ov.AddComponent<Image>().color = OVERLAY_COL;
        Stretch(ov);

        // ── Container (fullscreen) ─────────────────────────────────────────────
        var container = Child(popupGO, "Container");
        Stretch(container);

        // ── Header: Title + Close ──────────────────────────────────────────────
        var header = Child(container, "Header");
        header.AddComponent<Image>().color = HEADER_COL;
        var hRT = RT(header);
        hRT.anchorMin = new Vector2(0,1); hRT.anchorMax = new Vector2(1,1);
        hRT.pivot     = new Vector2(0.5f,1f);
        hRT.anchoredPosition = Vector2.zero;
        hRT.sizeDelta = new Vector2(0, 140);

        var titleGO = TMP(header, "Title", "Modul Matematika", 44, FontStyles.Bold, WHITE);
        var ttRT = RT(titleGO);
        ttRT.anchorMin = Vector2.zero; ttRT.anchorMax = Vector2.one;
        ttRT.offsetMin = new Vector2(0, 0); ttRT.offsetMax = new Vector2(-160, 0);
        titleGO.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        // Tombol Close [×]
        var closeGO  = Child(header, "BtnClose");
        var closeImg = closeGO.AddComponent<Image>();
        var closeSp  = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/AssetFigma/close-button.png");
        if (closeSp) { closeImg.sprite = closeSp; closeImg.preserveAspect = true; }
        closeImg.color = WHITE;
        var closeBtn = closeGO.AddComponent<Button>(); closeBtn.targetGraphic = closeImg;
        closeGO.AddComponent<UIButtonAnimator>();
        var cbRT = RT(closeGO);
        cbRT.anchorMin = cbRT.anchorMax = new Vector2(1f, 0.5f);
        cbRT.pivot     = new Vector2(1f, 0.5f);
        cbRT.anchoredPosition = new Vector2(-30, 0);
        cbRT.sizeDelta = new Vector2(60, 60);

        // ── Page Image (landscape, centered) ─────────────────────────────────
        // Halaman 2667×1500 → aspect 1.778
        // Di layar 1080 wide → tinggi = 1080/1.778 = 607px
        // Posisi: bawah header, di atas navigasi
        var pageGO  = Child(container, "PageImage");
        var pageImg = pageGO.AddComponent<Image>();
        pageImg.color           = WHITE;
        pageImg.preserveAspect  = true;
        var pgRT = RT(pageGO);
        pgRT.anchorMin = pgRT.anchorMax = new Vector2(0.5f, 0.5f);
        pgRT.pivot     = new Vector2(0.5f, 0.5f);
        pgRT.anchoredPosition = new Vector2(0, 30); // sedikit ke atas dari center
        pgRT.sizeDelta = new Vector2(1040, 585);    // 1040×585 ~ aspek 1.78:1

        // ── Navigation bar (bawah) ────────────────────────────────────────────
        var navBar = Child(container, "NavBar");
        navBar.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
        var nRT = RT(navBar);
        nRT.anchorMin = new Vector2(0,0); nRT.anchorMax = new Vector2(1,0);
        nRT.pivot     = new Vector2(0.5f,0f);
        nRT.anchoredPosition = Vector2.zero;
        nRT.sizeDelta = new Vector2(0, 130);

        // Tombol Prev
        var prevGO  = Child(navBar, "BtnPrev");
        var prevImg = prevGO.AddComponent<Image>();
        prevImg.color = NAV_COL;
        var prevBtn = prevGO.AddComponent<Button>(); prevBtn.targetGraphic = prevImg;
        prevGO.AddComponent<UIButtonAnimator>();
        var pvRT = RT(prevGO);
        pvRT.anchorMin = pvRT.anchorMax = new Vector2(0f, 0.5f);
        pvRT.pivot     = new Vector2(0f, 0.5f);
        pvRT.anchoredPosition = new Vector2(30, 0);
        pvRT.sizeDelta = new Vector2(240, 80);
        var prevLabel = TMP(prevGO, "Label", "◀  Sebelumnya", 26, FontStyles.Bold, WHITE);
        prevLabel.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        Stretch(prevLabel);

        // Page counter
        var counter = TMP(navBar, "PageCounter", "1 / 13", 32, FontStyles.Bold, WHITE);
        var pcRT    = RT(counter);
        pcRT.anchorMin = pcRT.anchorMax = new Vector2(0.5f, 0.5f);
        pcRT.pivot     = new Vector2(0.5f, 0.5f);
        pcRT.anchoredPosition = Vector2.zero;
        pcRT.sizeDelta = new Vector2(200, 60);
        counter.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        // Tombol Next
        var nextGO  = Child(navBar, "BtnNext");
        var nextImg = nextGO.AddComponent<Image>();
        nextImg.color = NAV_COL;
        var nextBtn = nextGO.AddComponent<Button>(); nextBtn.targetGraphic = nextImg;
        nextGO.AddComponent<UIButtonAnimator>();
        var nxRT = RT(nextGO);
        nxRT.anchorMin = nxRT.anchorMax = new Vector2(1f, 0.5f);
        nxRT.pivot     = new Vector2(1f, 0.5f);
        nxRT.anchoredPosition = new Vector2(-30, 0);
        nxRT.sizeDelta = new Vector2(240, 80);
        var nextLabel = TMP(nextGO, "Label", "Selanjutnya  ▶", 26, FontStyles.Bold, WHITE);
        nextLabel.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        Stretch(nextLabel);

        // ── ModulMatematikaManager ─────────────────────────────────────────────
        var mgr = popupGO.AddComponent<ModulMatematikaManager>();
        mgr.popupRoot   = popupGO;
        mgr.pageImage   = pageImg;
        mgr.pageCounter = counter.GetComponent<TMP_Text>();
        mgr.btnPrev     = prevBtn;
        mgr.btnNext     = nextBtn;
        mgr.btnClose    = closeBtn;

        // Load semua sprite halaman (urut 01-13)
        var sprites = new System.Collections.Generic.List<Sprite>();
        for (int i = 1; i <= TOTAL_PAGES; i++)
        {
            string path = $"{MODUL_PATH}Modul Matematika SD Interaktif-{i:D2}.png";
            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sp != null) sprites.Add(sp);
            else Debug.LogWarning($"[ModulMatematika] Sprite tidak ditemukan: {path}");
        }
        mgr.pages = sprites.ToArray();
        Debug.Log($"[ModulMatematika] {sprites.Count} halaman dimuat.");

        // Halaman pertama langsung tampil di editor
        if (sprites.Count > 0) pageImg.sprite = sprites[0];

        // ── Wire ModulIcon Hitung → OpenModul ─────────────────────────────────
        var modIconTF = cvs.transform.Find("LapakHitung/ModulIcon");
        if (modIconTF != null)
        {
            var modBtn = modIconTF.GetComponent<Button>();
            if (modBtn != null)
            {
                modBtn.onClick.RemoveAllListeners();
                // Hapus persistent listeners
                var so2 = new UnityEditor.SerializedObject(modBtn);
                var calls = so2.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
                if (calls != null) { calls.ClearArray(); so2.ApplyModifiedProperties(); }
                UnityEditor.Events.UnityEventTools.AddPersistentListener(modBtn.onClick, mgr.OpenModul);
                Debug.Log("[ModulMatematika] ModulIcon Hitung → OpenModul ✅");
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[PasarAjaib] ✅ Modul Matematika popup selesai!");
    }

    static void FixModulImports()
    {
        for (int i = 1; i <= TOTAL_PAGES; i++)
        {
            string path = $"{MODUL_PATH}Modul Matematika SD Interaktif-{i:D2}.png";
            var ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) continue;
            bool changed = false;
            if (ti.textureType != TextureImporterType.Sprite)
            { ti.textureType = TextureImporterType.Sprite; changed = true; }
            if (ti.spriteImportMode != SpriteImportMode.Single)
            { ti.spriteImportMode = SpriteImportMode.Single; changed = true; }
            if (!ti.alphaIsTransparency)
            { ti.alphaIsTransparency = true; changed = true; }
            if (ti.mipmapEnabled)
            { ti.mipmapEnabled = false; changed = true; }
            // Maksimalkan kualitas untuk halaman modul
            if (ti.maxTextureSize < 4096)
            { ti.maxTextureSize = 4096; changed = true; }
            if (changed) ti.SaveAndReimport();
        }
        Debug.Log("[ModulMatematika] Import settings fixed.");
    }

    static GameObject Child(GameObject p, string n)
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
}
#endif
