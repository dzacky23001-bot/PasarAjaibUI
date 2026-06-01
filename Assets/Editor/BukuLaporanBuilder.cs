#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Pasar Ajaib – Buku Laporan Popup Builder
/// Menu: PasarAjaib > Build Buku Laporan
/// Muncul saat tombol Piala ditekan.
/// Dibuat persis sesuai Figma Hi-Fi.
/// </summary>
public static class BukuLaporanBuilder
{
    // ── Base path ─────────────────────────────────────────────────────────────
    const string BASE = "Assets/AssetFigma/Buku Laporan/";
    const string FONT = "Assets/Fonts (Poppins)/Poppins-SemiBold SDF.asset";

    [MenuItem("PasarAjaib/Build Buku Laporan")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();

        // ── Cari Canvas ───────────────────────────────────────────────────────
        Canvas cvs = null;
        foreach (var r in scene.GetRootGameObjects())
        { cvs = r.GetComponent<Canvas>(); if (cvs) break; }
        if (!cvs) { Debug.LogError("[BukuLaporan] Canvas tidak ditemukan!"); return; }

        // ── Hapus PialaPopUp lama & buat baru (fresh) ────────────────────────
        var popTF = cvs.transform.Find("PialaPopUp");
        if (popTF) Undo.DestroyObjectImmediate(popTF.gameObject);

        var pop = new GameObject("PialaPopUp");
        Undo.RegisterCreatedObjectUndo(pop, "Build Buku Laporan");
        pop.transform.SetParent(cvs.transform, false);
        pop.layer = 5;

        // Tambah RectTransform fresh
        var popRT = pop.AddComponent<RectTransform>();
        popRT.anchorMin = Vector2.zero;
        popRT.anchorMax = Vector2.one;
        popRT.offsetMin = popRT.offsetMax = Vector2.zero;

        pop.SetActive(false);

        // ── 1. Overlay ────────────────────────────────────────────────────────
        var ov    = Go(pop, "Overlay");
        var ovImg = ov.AddComponent<Image>();
        ovImg.color = new Color(0f, 0f, 0f, 0.55f);
        SetStretch(ov);
        var ovBtn = ov.AddComponent<Button>();
        var oc = ovBtn.colors;
        oc.normalColor = oc.highlightedColor = oc.selectedColor = new Color(0,0,0,0.55f);
        oc.pressedColor = new Color(0,0,0,0.72f);
        ovBtn.colors = oc; ovBtn.targetGraphic = ovImg;

        // ── 2. Card utama ─────────────────────────────────────────────────────
        var card    = Go(pop, "Card");
        var cardImg = card.AddComponent<Image>();
        var cardSp  = Sp(BASE + "box buku laporan.png");
        if (cardSp) { cardImg.sprite = cardSp; cardImg.type = Image.Type.Simple; cardImg.color = Color.white; }
        else cardImg.color = new Color(0.88f, 0.95f, 1f, 1f);
        var cRT = RT(card);
        cRT.anchorMin = cRT.anchorMax = new Vector2(0.5f, 0.5f);
        cRT.pivot     = new Vector2(0.5f, 0.5f);
        cRT.anchoredPosition = new Vector2(0, 0);
        cRT.sizeDelta = new Vector2(820, 780);

        // ── 2a. Close [X] pojok kanan atas ────────────────────────────────────
        var closeBtn = Go(card, "BtnClose");
        var closeImg = closeBtn.AddComponent<Image>();
        var closeSp  = Sp(BASE + "mdi_close-outline.png");
        if (closeSp) { closeImg.sprite = closeSp; closeImg.preserveAspect = true; }
        closeImg.color = Color.white;
        var closeBtnC = closeBtn.AddComponent<Button>();
        closeBtnC.targetGraphic = closeImg;
        closeBtn.AddComponent<UIButtonAnimator>();
        var cbRT = RT(closeBtn);
        cbRT.anchorMin = cbRT.anchorMax = new Vector2(1f, 1f);
        cbRT.pivot     = new Vector2(1f, 1f);
        cbRT.anchoredPosition = new Vector2(-18, -18);
        cbRT.sizeDelta = new Vector2(44, 44);

        // Layout vertikal — dihitung dari atas card
        // Menggunakan anchor top agar konsisten
        float y = -52f;

        // ── 2b. Title "Buku Laporan" ───────────────────────────────────────────
        PlaceImg(card, "TitleBukuLaporan", BASE + "Buku Laporan.png",
            new Vector2(0, y), new Vector2(360, 52));
        y -= 68f;

        // ── 2c. Trophy icon ────────────────────────────────────────────────────
        PlaceImg(card, "TrophyIcon", BASE + "piala.png",
            new Vector2(0, y), new Vector2(88, 88));
        y -= 100f;

        // ── 2d. "Progres Belajar" ──────────────────────────────────────────────
        PlaceImg(card, "ProgresBelajarText", BASE + "Progres Belajar.png",
            new Vector2(0, y), new Vector2(260, 34));
        y -= 44f;

        // ── 2e. "0%" ──────────────────────────────────────────────────────────
        PlaceImg(card, "PersenText", BASE + "0%.png",
            new Vector2(0, y), new Vector2(68, 42));
        y -= 52f;

        // ── 2f. Progress bar (overall) ────────────────────────────────────────
        PlaceImg(card, "ProgressBarTrack", BASE + "progress-bar.png",
            new Vector2(0, y), new Vector2(640, 14));
        y -= 34f;

        // ── 3. LAPAK HITUNG card ───────────────────────────────────────────────
        BuildLapakCard(card, "HitungCard",
            BASE + "box lapak cerita.png",          // orange box
            BASE + "fxemoji_barchart.png",
            BASE + "Lapak Hitung (Matematika).png",
            BASE + "0 dari 3 level selesai.png",
            BASE + "progress1.png",
            BASE + "progress2.png",
            BASE + "progress3.png",
            BASE + "lihat sertifikat.png",
            topY: y);
        y -= 168f;

        // ── 4. LAPAK CERITA card ──────────────────────────────────────────────
        BuildLapakCard(card, "CeritaCard",
            BASE + "box lapak hitung.png",          // green box
            BASE + "noto_books.png",
            BASE + "Lapak Cerita (Literasi).png",
            BASE + "0 dari 3 level selesai-1.png",
            BASE + "progress1-1.png",
            BASE + "progress2-1.png",
            BASE + "progress3-1.png",
            BASE + "lihat sertifikat-1.png",
            topY: y);

        // ── 5. Wire UIManager ─────────────────────────────────────────────────
        var um = cvs.GetComponent<UIManager>() ?? cvs.gameObject.AddComponent<UIManager>();
        um.pialaPopup = pop;

        // Overlay close
        UnityEditor.Events.UnityEventTools.AddPersistentListener(ovBtn.onClick,      um.ClosePiala);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(closeBtnC.onClick,  um.ClosePiala);

        // Wire Piala button di TopBar
        var pialaTopBar = cvs.transform.Find("TopBar/Piala")?.GetComponent<Button>();
        if (pialaTopBar)
        {
            pialaTopBar.onClick.RemoveAllListeners();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(pialaTopBar.onClick, um.OpenPiala);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[PasarAjaib] ✅ Buku Laporan popup selesai dibangun!");
    }

    // ── Bangun satu card lapak ────────────────────────────────────────────────
    static void BuildLapakCard(GameObject parent, string name,
        string boxPath, string iconPath, string titlePath, string levelPath,
        string prog1, string prog2, string prog3, string sertifPath, float topY)
    {
        const float CARD_W = 740f;
        const float CARD_H = 155f;

        // Container card (anchor = top-center of parent)
        var lCard    = Go(parent, name);
        var lCardImg = lCard.AddComponent<Image>();
        var boxSp    = Sp(boxPath);
        if (boxSp) { lCardImg.sprite = boxSp; lCardImg.type = Image.Type.Simple; lCardImg.color = Color.white; }
        else lCardImg.color = new Color(1f, 0.95f, 0.85f, 1f);
        var lcRT = RT(lCard);
        lcRT.anchorMin = new Vector2(0.5f, 1f); lcRT.anchorMax = new Vector2(0.5f, 1f);
        lcRT.pivot     = new Vector2(0.5f, 1f);
        lcRT.anchoredPosition = new Vector2(0, topY);
        lcRT.sizeDelta = new Vector2(CARD_W, CARD_H);

        // Icon (kiri, vertikal center)
        var iconGO  = Go(lCard, "Icon");
        var iconImg = iconGO.AddComponent<Image>();
        var iconSp  = Sp(iconPath);
        if (iconSp) { iconImg.sprite = iconSp; iconImg.preserveAspect = true; }
        iconImg.color = Color.white;
        var iRT = RT(iconGO);
        iRT.anchorMin = iRT.anchorMax = new Vector2(0f, 0.5f);
        iRT.pivot     = new Vector2(0f, 0.5f);
        iRT.anchoredPosition = new Vector2(16, 0);
        iRT.sizeDelta = new Vector2(46, 46);

        // Title (top-left, kanan icon) — preserveAspect, jangan stretch
        var titleGO  = Go(lCard, "TitleLapak");
        var titleImg = titleGO.AddComponent<Image>();
        var titleSp  = Sp(titlePath);
        if (titleSp) { titleImg.sprite = titleSp; titleImg.preserveAspect = true; }
        titleImg.color = Color.white;
        var tRT = RT(titleGO);
        tRT.anchorMin = new Vector2(0f, 1f); tRT.anchorMax = new Vector2(0f, 1f);
        tRT.pivot     = new Vector2(0f, 1f);
        tRT.anchoredPosition = new Vector2(72, -14);
        tRT.sizeDelta = new Vector2(360, 36);

        // Progress bar label (kiri, di bawah title)
        var lvlGO  = Go(lCard, "LevelText");
        var lvlImg = lvlGO.AddComponent<Image>();
        var lvlSp  = Sp(levelPath);
        if (lvlSp) { lvlImg.sprite = lvlSp; lvlImg.preserveAspect = true; }
        lvlImg.color = Color.white;
        var lvRT = RT(lvlGO);
        lvRT.anchorMin = new Vector2(0f, 0f); lvRT.anchorMax = new Vector2(0f, 0f);
        lvRT.pivot     = new Vector2(0f, 0f);
        lvRT.anchoredPosition = new Vector2(72, 22);
        lvRT.sizeDelta = new Vector2(260, 26);

        // 3 progress bars (kiri, tengah vertikal)
        float[] pxArr = { 72f, 72f + 130f, 72f + 260f };
        string[] paths = { prog1, prog2, prog3 };
        for (int i = 0; i < 3; i++)
        {
            var pGO  = Go(lCard, $"Prog{i+1}");
            var pImg = pGO.AddComponent<Image>();
            var pSp  = Sp(paths[i]);
            if (pSp) { pImg.sprite = pSp; pImg.preserveAspect = false; }
            pImg.color = Color.white;
            var pRT  = RT(pGO);
            pRT.anchorMin = pRT.anchorMax = new Vector2(0f, 0.5f);
            pRT.pivot     = new Vector2(0f, 0.5f);
            pRT.anchoredPosition = new Vector2(pxArr[i], 0);
            pRT.sizeDelta = new Vector2(118, 12);
        }

        // Sertifikat button (kanan bawah, tidak stretch)
        var sertifGO  = Go(lCard, "BtnSertifikat");
        var sertifImg = sertifGO.AddComponent<Image>();
        var sertifSp  = Sp(sertifPath);
        if (sertifSp) { sertifImg.sprite = sertifSp; sertifImg.preserveAspect = false; sertifImg.color = Color.white; }
        else sertifImg.color = new Color(0.99f, 0.65f, 0.35f, 1f);
        var sBtn = sertifGO.AddComponent<Button>();
        sBtn.targetGraphic = sertifImg;
        sertifGO.AddComponent<UIButtonAnimator>();
        var sRT = RT(sertifGO);
        sRT.anchorMin = sRT.anchorMax = new Vector2(1f, 0.5f);
        sRT.pivot     = new Vector2(1f, 0.5f);
        sRT.anchoredPosition = new Vector2(-14, 0);
        sRT.sizeDelta = new Vector2(150, 50);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// PlaceImg: anchor top-center, pivot top-center
    static void PlaceImg(GameObject parent, string name, string path,
        Vector2 aPos, Vector2 size)
    {
        var go  = Go(parent, name);
        var img = go.AddComponent<Image>();
        var sp  = Sp(path);
        if (sp) { img.sprite = sp; img.preserveAspect = true; img.color = Color.white; }
        else img.color = Color.clear;
        var rt  = RT(go);
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.anchoredPosition = aPos;
        rt.sizeDelta = size;
    }

    static GameObject Go(GameObject p, string n)
    {
        var go = new GameObject(n);
        go.transform.SetParent(p.transform, false);
        go.layer = 5;
        go.AddComponent<RectTransform>();
        return go;
    }

    static RectTransform RT(GameObject go) =>
        go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();

    static void SetStretch(GameObject go)
    {
        var rt = RT(go);
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static Sprite Sp(string path)
    {
        var s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (!s)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex) s = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(0.5f,0.5f));
        }
        return s;
    }

    static void ClearChildren(GameObject go)
    {
        while (go.transform.childCount > 0)
            Undo.DestroyObjectImmediate(go.transform.GetChild(0).gameObject);
    }
}
#endif
