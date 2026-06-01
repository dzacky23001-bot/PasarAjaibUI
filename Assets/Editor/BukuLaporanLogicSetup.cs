#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Wire logic BukuLaporanManager ke elemen UI yang sudah ada.
/// TIDAK mengubah posisi/ukuran apapun — hanya tambah script & komponen.
/// Menu: PasarAjaib > Wire Buku Laporan Logic
/// </summary>
public static class BukuLaporanLogicSetup
{
    const string FONT = "Assets/Fonts (Poppins)/Poppins-SemiBold SDF.asset";

    [MenuItem("PasarAjaib/Wire Buku Laporan Logic")]
    public static void Setup()
    {
        var scene = EditorSceneManager.GetActiveScene();

        // ── Cari PialaPopUp ────────────────────────────────────────────────────
        Canvas cvs = null;
        foreach (var r in scene.GetRootGameObjects())
        { cvs = r.GetComponent<Canvas>(); if (cvs) break; }
        if (!cvs) { Debug.LogError("[BLSetup] Canvas tidak ditemukan."); return; }

        var popupTF = cvs.transform.Find("PialaPopUp");
        if (!popupTF) { Debug.LogError("[BLSetup] PialaPopUp tidak ditemukan."); return; }

        var cardTF    = popupTF.Find("Card");
        if (!cardTF)  { Debug.LogError("[BLSetup] PialaPopUp/Card tidak ditemukan."); return; }

        // ── Hapus BukuLaporanManager lama kalau ada ────────────────────────────
        var existingMgr = popupTF.GetComponent<BukuLaporanManager>();
        if (existingMgr) Undo.DestroyObjectImmediate(existingMgr);

        var mgr = Undo.AddComponent<BukuLaporanManager>(popupTF.gameObject);

        // ── 1. PersenText → buat child TMP overlay ────────────────────────────
        var persenTF = cardTF.Find("PersenText");
        if (persenTF)
        {
            // Sembunyikan Image statis
            var img = persenTF.GetComponent<Image>();
            if (img) img.enabled = false;

            // Cari atau buat child TMP
            var dynTF = persenTF.Find("PersenDynamic");
            GameObject dynGO;
            if (dynTF) dynGO = dynTF.gameObject;
            else
            {
                dynGO = new GameObject("PersenDynamic");
                Undo.RegisterCreatedObjectUndo(dynGO, "Add PersenDynamic");
                dynGO.transform.SetParent(persenTF, false);
                dynGO.layer = 5;
                var dRT = dynGO.AddComponent<RectTransform>();
                dRT.anchorMin = Vector2.zero; dRT.anchorMax = Vector2.one;
                dRT.offsetMin = dRT.offsetMax = Vector2.zero;
            }
            var tmt = dynGO.GetComponent<TextMeshProUGUI>() ??
                      dynGO.AddComponent<TextMeshProUGUI>();
            tmt.text      = "0%";
            tmt.fontSize  = 42;
            tmt.fontStyle = FontStyles.Bold;
            tmt.color     = new Color(0.30f, 0.60f, 0.85f, 1f);
            tmt.alignment = TextAlignmentOptions.Center;
            ApplyFont(tmt);
            mgr.persenText = tmt;
        }
        else Debug.LogWarning("[BLSetup] PersenText tidak ditemukan.");

        // ── 2. ProgressBarFill → tambah fill child ─────────────────────────────
        var trackTF = cardTF.Find("ProgressBarTrack");
        if (trackTF)
        {
            // Hapus ProgressFill lama (mungkin tidak punya RectTransform) & buat fresh
            var fillTF = trackTF.Find("ProgressFill");
            if (fillTF) Undo.DestroyObjectImmediate(fillTF.gameObject);

            // Buat pakai ObjectFactory agar dapat RectTransform otomatis
            var fillGO = ObjectFactory.CreateGameObject("ProgressFill",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Undo.RegisterCreatedObjectUndo(fillGO, "Add ProgressFill");
            fillGO.layer = 5;
            fillGO.transform.SetParent(trackTF, false);

            var fillRT = fillGO.GetComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = Vector2.one;
            fillRT.offsetMin = fillRT.offsetMax = Vector2.zero;

            var fillImg = fillGO.GetComponent<Image>() ?? fillGO.AddComponent<Image>();
            fillImg.color      = new Color(1f, 0.85f, 0.10f, 1f); // kuning
            fillImg.type       = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillOrigin = 0; // dari kiri
            fillImg.fillAmount = 0f;
            fillImg.enabled    = false; // mulai tersembunyi

            mgr.progressBarFill = fillImg;
        }
        else Debug.LogWarning("[BLSetup] ProgressBarTrack tidak ditemukan.");

        // ── 3. HitungCard ──────────────────────────────────────────────────────
        var hitungTF = cardTF.Find("HitungCard");
        if (hitungTF)
        {
            mgr.hitungBars = CollectBars(hitungTF);

            var levelTF = hitungTF.Find("LevelText");
            if (levelTF)
            {
                var img = levelTF.GetComponent<Image>();
                if (img) img.enabled = false;
                var tmt = MakeLevelTMP(levelTF, "LevelDynamic");
                mgr.hitungLevelText = tmt;
            }

            var sertifTF = hitungTF.Find("BtnSertifikat");
            if (sertifTF) mgr.sertifHitung = sertifTF.GetComponent<Button>();
        }
        else Debug.LogWarning("[BLSetup] HitungCard tidak ditemukan.");

        // ── 4. CeritaCard ──────────────────────────────────────────────────────
        var ceritaTF = cardTF.Find("CeritaCard");
        if (ceritaTF)
        {
            mgr.ceritaBars = CollectBars(ceritaTF);

            var levelTF = ceritaTF.Find("LevelText");
            if (levelTF)
            {
                var img = levelTF.GetComponent<Image>();
                if (img) img.enabled = false;
                var tmt = MakeLevelTMP(levelTF, "LevelDynamic");
                mgr.ceritaLevelText = tmt;
            }

            var sertifTF = ceritaTF.Find("BtnSertifikat");
            if (sertifTF) mgr.sertifCerita = sertifTF.GetComponent<Button>();
        }
        else Debug.LogWarning("[BLSetup] CeritaCard tidak ditemukan.");

        // ── Simpan ─────────────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[PasarAjaib] ✅ Buku Laporan logic terpasang! Prog1/2/3 = kuning saat selesai.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static Image[] CollectBars(Transform parent)
    {
        var list = new System.Collections.Generic.List<Image>();
        for (int i = 1; i <= 3; i++)
        {
            var tf = parent.Find($"Prog{i}");
            if (tf)
            {
                var img = tf.GetComponent<Image>();
                if (img) list.Add(img);
            }
        }
        return list.ToArray();
    }

    static TMP_Text MakeLevelTMP(Transform parent, string childName)
    {
        var tf = parent.Find(childName);
        GameObject go;
        if (tf) go = tf.gameObject;
        else
        {
            go = new GameObject(childName);
            Undo.RegisterCreatedObjectUndo(go, "Add LevelDynamic");
            go.transform.SetParent(parent, false);
            go.layer = 5;
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }
        var tmt = go.GetComponent<TextMeshProUGUI>() ?? go.AddComponent<TextMeshProUGUI>();
        tmt.text      = "0 dari 3 level selesai";
        tmt.fontSize  = 22;
        tmt.fontStyle = FontStyles.Normal;
        tmt.color     = new Color(0.35f, 0.35f, 0.35f, 1f);
        tmt.alignment = TextAlignmentOptions.MidlineLeft;
        ApplyFont(tmt);
        return tmt;
    }

    static void ApplyFont(TMP_Text tmp)
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT);
        if (font) tmp.font = font;
    }
}
#endif
