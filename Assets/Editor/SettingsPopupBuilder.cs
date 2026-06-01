#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Pasar Ajaib – Settings Popup (Advanced Child-Friendly UI)
/// Card compact, warna-warni ceria, tombol besar, layout rapi.
/// </summary>
public static class SettingsPopupBuilder
{
    // ── Palet warna ───────────────────────────────────────────────────────────
    static Color C(string hex) { ColorUtility.TryParseHtmlString(hex, out var c); return c; }

    static readonly Color COL_OVERLAY    = new Color(0f, 0f, 0f, 0.55f);
    static readonly Color COL_HEADER_TOP = new Color(0.31f, 0.62f, 0.87f, 1f); // #4F9EDE
    static readonly Color COL_CARD       = new Color(0.96f, 0.98f, 1.00f, 1f); // hampir putih
    static readonly Color COL_SHADOW     = new Color(0f, 0f, 0f, 0.18f);
    static readonly Color COL_DIVIDER    = new Color(0.87f, 0.92f, 0.96f, 1f);
    static readonly Color COL_TXT_DARK   = new Color(0.11f, 0.27f, 0.42f, 1f);
    static readonly Color COL_TXT_SUB    = new Color(0.45f, 0.58f, 0.70f, 1f);
    static readonly Color COL_WHITE      = Color.white;
    // Tombol
    static readonly Color COL_SOUND_ON   = new Color(0.20f, 0.75f, 0.40f, 1f); // hijau
    static readonly Color COL_SOUND_OFF  = new Color(0.65f, 0.65f, 0.65f, 1f); // abu
    static readonly Color COL_LOGOUT     = new Color(1.00f, 0.60f, 0.10f, 1f); // oranye
    static readonly Color COL_EXIT       = new Color(0.92f, 0.22f, 0.22f, 1f); // merah

    // ── Asset paths ───────────────────────────────────────────────────────────
    const string FONT       = "Assets/Fonts (Poppins)/Poppins-SemiBold SDF.asset";
    const string ICO_CLOSE  = "Assets/AssetFigma/close-button.png";
    const string ICO_SOUND  = "Assets/Assets/Settings/audiofix.png";
    const string ICO_LOGOUT = "Assets/Assets/Settings/exit_logo.png";
    const string ICO_EXIT   = "Assets/Assets/Settings/back.png";

    // ── Dimensi ───────────────────────────────────────────────────────────────
    const float W = 640f;   // lebar card
    const float H = 520f;   // tinggi card

    [MenuItem("PasarAjaib/Build Settings Popup")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();

        // Cari Canvas
        Canvas cvs = null;
        foreach (var r in scene.GetRootGameObjects())
        { cvs = r.GetComponent<Canvas>(); if (cvs) break; }
        if (!cvs) { Debug.LogError("[Settings] Canvas tidak ditemukan."); return; }

        // Cari / buat SettingsPopup
        var popTF = cvs.transform.Find("SettingsPopup");
        GameObject pop;
        if (popTF) { pop = popTF.gameObject; ClearChildren(pop); }
        else
        {
            pop = new GameObject("SettingsPopup");
            Undo.RegisterCreatedObjectUndo(pop, "Build Settings Popup");
            pop.transform.SetParent(cvs.transform, false);
        }
        pop.layer = 5;
        SetStretch(pop);
        pop.SetActive(false);

        // ── 1. OVERLAY gelap ──────────────────────────────────────────────────
        var ov = Child(pop, "Overlay");
        var ovImg = ov.AddComponent<Image>(); ovImg.color = COL_OVERLAY;
        SetStretch(ov);
        var ovBtn = ov.AddComponent<Button>();
        var oc = ovBtn.colors;
        oc.normalColor = oc.highlightedColor = oc.selectedColor = COL_OVERLAY;
        oc.pressedColor = new Color(0,0,0,0.72f);
        ovBtn.colors = oc; ovBtn.targetGraphic = ovImg;

        // ── 2. SHADOW card (sedikit lebih besar, gelap transparan) ────────────
        var shadow = Child(pop, "CardShadow");
        var shImg  = shadow.AddComponent<Image>(); shImg.color = COL_SHADOW;
        var shRT   = RT(shadow);
        shRT.anchorMin = shRT.anchorMax = new Vector2(0.5f, 0.5f);
        shRT.pivot     = new Vector2(0.5f, 0.5f);
        shRT.anchoredPosition = new Vector2(6, -8);   // offset ke kanan-bawah
        shRT.sizeDelta = new Vector2(W + 12, H + 12);
        // Rounded corners pakai sprite cream
        var creamSp = Load<Sprite>("Assets/AssetFigma/box pop up coba lagi.png");
        if (creamSp) { shImg.sprite = creamSp; shImg.type = Image.Type.Simple; }

        // ── 3. CARD utama ─────────────────────────────────────────────────────
        var card = Child(pop, "Card");
        var cardImg = card.AddComponent<Image>();
        cardImg.color = COL_CARD;
        if (creamSp) { cardImg.sprite = creamSp; cardImg.type = Image.Type.Simple; }
        var cRT = RT(card);
        cRT.anchorMin = cRT.anchorMax = new Vector2(0.5f, 0.5f);
        cRT.pivot     = new Vector2(0.5f, 0.5f);
        cRT.anchoredPosition = Vector2.zero;
        cRT.sizeDelta = new Vector2(W, H);

        // ── 3a. HEADER ─────────────────────────────────────────────────────────
        var hdr    = Child(card, "Header");
        var hdrImg = hdr.AddComponent<Image>(); hdrImg.color = COL_HEADER_TOP;
        var hRT    = RT(hdr);
        hRT.anchorMin = new Vector2(0,1); hRT.anchorMax = new Vector2(1,1);
        hRT.pivot     = new Vector2(0.5f,1f);
        hRT.anchoredPosition = Vector2.zero; hRT.sizeDelta = new Vector2(0, 90);

        // Icon gear PNG di header
        var hIcon    = Child(hdr, "HdrIcon");
        var hIconImg = hIcon.AddComponent<Image>();
        var gearSp   = Load<Sprite>("Assets/Assets/Settings/gear_icon.png");
        if (gearSp) { hIconImg.sprite = gearSp; hIconImg.preserveAspect = true; }
        hIconImg.color = COL_WHITE;
        var hiRT  = RT(hIcon);
        hiRT.anchorMin = new Vector2(0, 0.5f); hiRT.anchorMax = new Vector2(0, 0.5f);
        hiRT.pivot     = new Vector2(0, 0.5f);
        hiRT.anchoredPosition = new Vector2(22, 0);
        hiRT.sizeDelta = new Vector2(42, 42);

        // Judul
        var hTitle = TMP(hdr, "HdrTitle", "Pengaturan", 36, FontStyles.Bold, COL_WHITE);
        var htRT   = RT(hTitle);
        htRT.anchorMin = Vector2.zero; htRT.anchorMax = Vector2.one;
        htRT.offsetMin = new Vector2(76,0); htRT.offsetMax = new Vector2(-80,0);
        hTitle.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        // Tombol [X]
        var xBtn  = MakeIconBtn(hdr, "BtnClose", ICO_CLOSE, COL_WHITE, 38, 38);
        var xRT   = RT(xBtn);
        xRT.anchorMin = xRT.anchorMax = new Vector2(1f,0.5f);
        xRT.pivot     = new Vector2(1f,0.5f);
        xRT.anchoredPosition = new Vector2(-20,0);

        // ── 3b. CONTENT area ──────────────────────────────────────────────────
        float y = -90f - 14f;  // tepat di bawah header + padding atas

        // -- SOUND ROW --
        var sndRow = SectionCard(card, "SoundRow", y, 80);

        // Icon speaker
        AddIcon(sndRow, ICO_SOUND, new Vector2(0,0.5f), new Vector2(0,0.5f),
                new Vector2(22, 0), new Vector2(46,46));

        // Label
        var sndLbl = TMP(sndRow, "SndLabel", "Backsound / Musik",
                         28, FontStyles.Bold, COL_TXT_DARK);
        var slRT = RT(sndLbl);
        slRT.anchorMin = Vector2.zero; slRT.anchorMax = Vector2.one;
        slRT.offsetMin = new Vector2(80,0); slRT.offsetMax = new Vector2(-160,0);
        sndLbl.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        // Toggle pill (ON/OFF)
        var sndBtn  = Child(sndRow, "SoundButton");
        var sndImg  = sndBtn.AddComponent<Image>(); sndImg.color = COL_SOUND_ON;
        var sndBtnC = sndBtn.AddComponent<Button>();
        sndBtnC.targetGraphic = sndImg;
        sndBtn.AddComponent<UIButtonAnimator>();
        var sbRT = RT(sndBtn);
        sbRT.anchorMin = sbRT.anchorMax = new Vector2(1f,0.5f);
        sbRT.pivot     = new Vector2(1f,0.5f);
        sbRT.anchoredPosition = new Vector2(-20, 0);
        sbRT.sizeDelta = new Vector2(100, 46);
        var sndLblBtn = TMP(sndBtn, "Label", "ON", 22, FontStyles.Bold, COL_WHITE);
        sndLblBtn.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        SetStretch(sndLblBtn);

        y -= 80f + 12f;

        // Divider tipis
        MakeDivider(card, y + 6); y -= 0;

        y -= 14f;

        // -- LOGOUT BUTTON (full-width, oranye) --
        var logoutBtn = BigButton(card, "LogoutButton", y, "Logout",
                                  ICO_LOGOUT, COL_LOGOUT);
        y -= 90f + 14f;

        // -- EXIT BUTTON (full-width, merah) --
        var exitBtn = BigButton(card, "ExitButton", y, "Keluar Aplikasi",
                                ICO_EXIT, COL_EXIT);

        // ── 4. WIRE UIManager ─────────────────────────────────────────────────
        var um = cvs.GetComponent<UIManager>() ?? cvs.gameObject.AddComponent<UIManager>();
        um.settingsPopup = pop;

        Link(ovBtn.onClick,                              um.CloseSettings);
        Link(xBtn.GetComponent<Button>().onClick,        um.CloseSettings);
        Link(sndBtnC.onClick,                            um.ToggleSound);
        Link(logoutBtn.GetComponent<Button>().onClick,   um.Logout);
        Link(exitBtn.GetComponent<Button>().onClick,     um.ExitGame);

        var bsSetting = cvs.transform.Find("BtnSetting")?.GetComponent<Button>();
        if (bsSetting)
        {
            bsSetting.onClick.RemoveAllListeners();
            Link(bsSetting.onClick, um.OpenSettings);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[PasarAjaib] ✅ Settings Popup (Advanced) selesai!");
    }

    // ── Section card (background putih tipis) ──────────────────────────────────
    static GameObject SectionCard(GameObject parent, string name, float topY, float h)
    {
        var go  = Child(parent, name);
        var img = go.AddComponent<Image>();
        img.color = new Color(1f,1f,1f,0.7f);
        var rt  = RT(go);
        rt.anchorMin = new Vector2(0.05f, 1f); rt.anchorMax = new Vector2(0.95f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, topY);
        rt.sizeDelta = new Vector2(0, h);
        return go;
    }

    // ── Big action button (logout/exit) ───────────────────────────────────────
    static GameObject BigButton(GameObject parent, string name, float topY,
                                string label, string iconPath, Color col)
    {
        var go  = Child(parent, name);
        var img = go.AddComponent<Image>(); img.color = col;
        var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
        go.AddComponent<UIButtonAnimator>();

        var rt  = RT(go);
        rt.anchorMin = new Vector2(0.05f, 1f); rt.anchorMax = new Vector2(0.95f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, topY);
        rt.sizeDelta = new Vector2(0, 82);

        // Icon kiri
        AddIcon(go, iconPath,
                new Vector2(0,0.5f), new Vector2(0,0.5f),
                new Vector2(22,0), new Vector2(44,44));

        // Label center (offset sedikit agar seimbang dengan icon)
        var lbl = TMP(go, "Label", label, 28, FontStyles.Bold, Color.white);
        var lRT = RT(lbl);
        lRT.anchorMin = Vector2.zero; lRT.anchorMax = Vector2.one;
        lRT.offsetMin = new Vector2(0, 0); lRT.offsetMax = new Vector2(0, 0);
        lbl.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        return go;
    }

    // ── Icon button helper ────────────────────────────────────────────────────
    static GameObject MakeIconBtn(GameObject parent, string name, string iconPath,
                                  Color col, float w, float h)
    {
        var go  = Child(parent, name);
        var img = go.AddComponent<Image>();
        var sp  = Load<Sprite>(iconPath);
        if (sp) { img.sprite = sp; img.preserveAspect = true; }
        img.color = col;
        var btn = go.AddComponent<Button>(); btn.targetGraphic = img;
        go.AddComponent<UIButtonAnimator>();
        var rt = RT(go); rt.sizeDelta = new Vector2(w, h);
        return go;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    static void AddIcon(GameObject parent, string path,
                        Vector2 anchorMin, Vector2 anchorMax,
                        Vector2 aPos, Vector2 size)
    {
        var go  = Child(parent, "Icon");
        var img = go.AddComponent<Image>();
        var sp  = Load<Sprite>(path);
        if (sp) { img.sprite = sp; img.preserveAspect = true; }
        img.color = Color.white;
        var rt  = RT(go);
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot     = anchorMin;
        rt.anchoredPosition = aPos;
        rt.sizeDelta = size;
    }

    static void MakeDivider(GameObject parent, float topY)
    {
        var d   = Child(parent, "Divider");
        d.AddComponent<Image>().color = COL_DIVIDER;
        var rt  = RT(d);
        rt.anchorMin = new Vector2(0.05f,1f); rt.anchorMax = new Vector2(0.95f,1f);
        rt.pivot     = new Vector2(0.5f,1f);
        rt.anchoredPosition = new Vector2(0, topY);
        rt.sizeDelta = new Vector2(0, 1.5f);
    }

    static GameObject Child(GameObject p, string n)
    {
        var go = new GameObject(n);
        go.transform.SetParent(p.transform, false);
        go.layer = 5;
        go.AddComponent<RectTransform>();
        return go;
    }

    static GameObject TMP(GameObject p, string n, string txt,
                          float sz, FontStyles style, Color col)
    {
        var go  = new GameObject(n);
        go.transform.SetParent(p.transform, false);
        go.layer = 5;
        var t   = go.AddComponent<TextMeshProUGUI>();
        t.text = txt; t.fontSize = sz; t.fontStyle = style; t.color = col;
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT);
        if (font) t.font = font;
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

    static T Load<T>(string path) where T : Object =>
        AssetDatabase.LoadAssetAtPath<T>(path);

    static void Link(UnityEngine.Events.UnityEvent evt,
                     UnityEngine.Events.UnityAction action) =>
        UnityEditor.Events.UnityEventTools.AddPersistentListener(evt, action);

    static void ClearChildren(GameObject go)
    {
        while (go.transform.childCount > 0)
            Undo.DestroyObjectImmediate(go.transform.GetChild(0).gameObject);
    }
}
#endif
