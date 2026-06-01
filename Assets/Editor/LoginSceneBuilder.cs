#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Pasar Ajaib - Login UI Builder
/// Menu: PasarAjaib > Build Login UI
///
/// Rebuilds the LoginScene Canvas to match the Figma design:
///   - Sky-blue background
///   - Dark-blue rounded header "Selamat Datang!"
///   - Pasar Ajaib AR logo
///   - Blue card with rabbit mascot, name input, class dropdown
///   - Orange "Masuk" button
///   - AudioManager + EventSystem
/// </summary>
public static class LoginSceneBuilder
{
    // ── Figma colour palette ──────────────────────────────────────────────────
    static readonly Color BG_COLOR     = Hex("#B8D9F0");
    static readonly Color HEADER_COLOR = Hex("#5BA3D0");
    static readonly Color CARD_COLOR   = Hex("#A2C4DC");
    static readonly Color INPUT_COLOR  = Color.white;
    static readonly Color LABEL_COLOR  = Hex("#1A4A6B");
    static readonly Color ERROR_COLOR  = Hex("#D9534F");

    // ── Asset paths ───────────────────────────────────────────────────────────
    const string LOGO_PATH    = "Assets/AssetFigma/logo.png";
    const string RABBIT_PATH  = "Assets/AssetFigma/emojione_rabbit.png";
    const string BTN_PATH     = "Assets/AssetFigma/tombol masuk.png";
    const string CARD_PATH    = "Assets/AssetFigma/input boxx.png";
    const string TITLE_PATH   = "Assets/AssetFigma/Selamat Datang!.png";
    const string GRADCAP_PATH = "Assets/AssetFigma/stash_graduation-cap-light.png";
    const string FONT_PATH    = "Assets/Fonts (Poppins)/Poppins-SemiBold SDF.asset";

    // ── Audio paths ───────────────────────────────────────────────────────────
    const string SFX_CLICK_PATH  = "Assets/Audio/click button/button1.wav";
    const string SFX_WRONG_PATH  = "Assets/Audio/Wrong/salah.mp3";
    const string BGM_PATH        = "Assets/Audio/background/background1.mp3";

    // ── Canvas reference resolution ───────────────────────────────────────────
    const float REF_W = 1080f;
    const float REF_H = 1920f;

    [MenuItem("PasarAjaib/Build Login UI")]
    public static void BuildLoginUI()
    {
        // 1 – Open LoginScene
        string scenePath = "Assets/Scenes/LoginScene.unity";
        if (EditorSceneManager.GetActiveScene().path != scenePath)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(scenePath);
        }

        Scene scene = EditorSceneManager.GetActiveScene();

        // 2 – Remove old Canvas & EventSystem (we rebuild cleanly)
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.GetComponent<Canvas>() != null ||
                root.GetComponent<UnityEngine.EventSystems.EventSystem>() != null)
            {
                Undo.DestroyObjectImmediate(root);
            }
        }

        // 3 – Create root Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Undo.RegisterCreatedObjectUndo(canvasGO, "Build Login UI");

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        canvasGO.layer = LayerMask.NameToLayer("UI");

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(REF_W, REF_H);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // 4 – Background
        GameObject bg = CreateImage(canvasGO, "Background", BG_COLOR);
        Stretch(bg);

        // 5 – Header bar
        GameObject header = CreateImage(canvasGO, "Header", HEADER_COLOR);
        RectTransform hRT = header.GetComponent<RectTransform>();
        hRT.anchorMin = new Vector2(0, 1);
        hRT.anchorMax = new Vector2(1, 1);
        hRT.pivot = new Vector2(0.5f, 1f);
        hRT.anchoredPosition = Vector2.zero;
        hRT.sizeDelta = new Vector2(0, 200);

        // Header logo asset (Selamat Datang! text sprite)
        Sprite titleSprite = LoadSprite(TITLE_PATH);
        if (titleSprite != null)
        {
            GameObject titleImg = CreateImageWithSprite(header, "HeaderTitle", titleSprite);
            RectTransform tRT = titleImg.GetComponent<RectTransform>();
            tRT.anchorMin = tRT.anchorMax = new Vector2(0.5f, 0.5f);
            tRT.pivot = new Vector2(0.5f, 0.5f);
            tRT.anchoredPosition = new Vector2(0, -10f);
            tRT.sizeDelta = new Vector2(520, 90);
            titleImg.GetComponent<Image>().preserveAspect = true;
        }
        else
        {
            // Fallback TMP text
            GameObject titleTMP = CreateTMP(header, "HeaderTitle", "Selamat Datang!",
                52, FontStyles.Bold, Color.white);
            RectTransform tRT = titleTMP.GetComponent<RectTransform>();
            tRT.anchorMin = tRT.anchorMax = new Vector2(0.5f, 0.5f);
            tRT.pivot = new Vector2(0.5f, 0.5f);
            tRT.anchoredPosition = new Vector2(0, -10);
            tRT.sizeDelta = new Vector2(800, 100);
        }

        // 6 – Logo (lebih besar)
        Sprite logoSprite = LoadSprite(LOGO_PATH);
        if (logoSprite != null)
        {
            GameObject logo = CreateImageWithSprite(canvasGO, "Logo", logoSprite);
            RectTransform lRT = logo.GetComponent<RectTransform>();
            lRT.anchorMin = lRT.anchorMax = new Vector2(0.5f, 1f);
            lRT.pivot = new Vector2(0.5f, 1f);
            lRT.anchoredPosition = new Vector2(0, -200);
            lRT.sizeDelta = new Vector2(560, 560);
            logo.GetComponent<Image>().preserveAspect = true;
        }

        // 7 – Form card
        Sprite cardSprite = LoadSprite(CARD_PATH);
        GameObject card = cardSprite != null
            ? CreateImageWithSprite(canvasGO, "FormCard", cardSprite)
            : CreateImage(canvasGO, "FormCard", CARD_COLOR);
        RectTransform cRT = card.GetComponent<RectTransform>();
        cRT.anchorMin = cRT.anchorMax = new Vector2(0.5f, 0.5f);
        cRT.pivot = new Vector2(0.5f, 0.5f);
        cRT.anchoredPosition = new Vector2(0, -110);
        cRT.sizeDelta = new Vector2(860, 520);
        Image cardImg = card.GetComponent<Image>();
        cardImg.type = Image.Type.Sliced;
        cardImg.preserveAspect = false;

        // 7a – Rabbit mascot
        Sprite rabbitSprite = LoadSprite(RABBIT_PATH);
        if (rabbitSprite != null)
        {
            GameObject rabbit = CreateImageWithSprite(card, "RabbitMascot", rabbitSprite);
            RectTransform rRT = rabbit.GetComponent<RectTransform>();
            rRT.anchorMin = rRT.anchorMax = new Vector2(0f, 1f);
            rRT.pivot = new Vector2(0f, 1f);
            rRT.anchoredPosition = new Vector2(30, -20);
            rRT.sizeDelta = new Vector2(110, 110);
            rabbit.GetComponent<Image>().preserveAspect = true;
        }

        // 7b – Greeting text
        GameObject greeting = CreateTMP(card, "GreetingText",
            "Halo Calon Pedagang Hebat!\nSiapa namamu?",
            30, FontStyles.Bold, LABEL_COLOR);
        RectTransform gRT = greeting.GetComponent<RectTransform>();
        gRT.anchorMin = gRT.anchorMax = new Vector2(0.5f, 1f);
        gRT.pivot = new Vector2(0.5f, 1f);
        gRT.anchoredPosition = new Vector2(40, -30);
        gRT.sizeDelta = new Vector2(660, 120);
        greeting.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;

        // 7c – Name input field
        GameObject namaInput = CreateInputField(card, "InputNama", "Ketik Nama");
        RectTransform niRT = namaInput.GetComponent<RectTransform>();
        niRT.anchorMin = niRT.anchorMax = new Vector2(0.5f, 1f);
        niRT.pivot = new Vector2(0.5f, 1f);
        niRT.anchoredPosition = new Vector2(0, -175);
        niRT.sizeDelta = new Vector2(760, 90);

        // 7d – Graduation cap icon + "Kelas" label row
        Sprite gradCap = LoadSprite(GRADCAP_PATH);
        if (gradCap != null)
        {
            GameObject capImg = CreateImageWithSprite(card, "GradCapIcon", gradCap);
            RectTransform capRT = capImg.GetComponent<RectTransform>();
            capRT.anchorMin = capRT.anchorMax = new Vector2(0f, 1f);
            capRT.pivot = new Vector2(0f, 1f);
            capRT.anchoredPosition = new Vector2(40, -290);
            capRT.sizeDelta = new Vector2(50, 50);
            capImg.GetComponent<Image>().color = LABEL_COLOR;
        }

        GameObject kelasLabel = CreateTMP(card, "KelasLabel", "Kelas",
            30, FontStyles.Bold, LABEL_COLOR);
        RectTransform klRT = kelasLabel.GetComponent<RectTransform>();
        klRT.anchorMin = klRT.anchorMax = new Vector2(0f, 1f);
        klRT.pivot = new Vector2(0f, 1f);
        klRT.anchoredPosition = new Vector2(100, -280);
        klRT.sizeDelta = new Vector2(200, 60);

        // 7e – Kelas dropdown
        GameObject kelasDD = CreateDropdown(card, "InputKelas");
        RectTransform ddRT = kelasDD.GetComponent<RectTransform>();
        ddRT.anchorMin = ddRT.anchorMax = new Vector2(0.5f, 1f);
        ddRT.pivot = new Vector2(0.5f, 1f);
        ddRT.anchoredPosition = new Vector2(0, -380);
        ddRT.sizeDelta = new Vector2(760, 90);

        // 8 – Error text
        GameObject errorObj = CreateTMP(canvasGO, "ErrorText",
            "Nama tidak boleh kosong!", 26, FontStyles.Normal, ERROR_COLOR);
        RectTransform eRT = errorObj.GetComponent<RectTransform>();
        eRT.anchorMin = eRT.anchorMax = new Vector2(0.5f, 0.5f);
        eRT.pivot = new Vector2(0.5f, 0.5f);
        eRT.anchoredPosition = new Vector2(0, -415);
        eRT.sizeDelta = new Vector2(800, 55);
        errorObj.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        errorObj.SetActive(false);

        // 9 – Masuk button
        Sprite btnSprite = LoadSprite(BTN_PATH);
        GameObject masukBtn = new GameObject("MasukButton");
        masukBtn.transform.SetParent(canvasGO.transform, false);
        Image btnImg = masukBtn.AddComponent<Image>();
        if (btnSprite != null)
        {
            btnImg.sprite = btnSprite;
            btnImg.type = Image.Type.Sliced;
            btnImg.preserveAspect = false;
        }
        else
        {
            btnImg.color = Hex("#F5A623");
        }
        Button btn = masukBtn.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        masukBtn.AddComponent<UIButtonAnimator>();

        RectTransform bRT = masukBtn.GetComponent<RectTransform>();
        bRT.anchorMin = bRT.anchorMax = new Vector2(0.5f, 0.5f);
        bRT.pivot = new Vector2(0.5f, 0.5f);
        bRT.anchoredPosition = new Vector2(0, -575);
        bRT.sizeDelta = new Vector2(760, 110);

        // Add "Masuk" text only if no sprite (sprite already has text baked in)
        if (btnSprite == null)
        {
            GameObject btnLabel = CreateTMP(masukBtn, "BtnLabel", "Masuk",
                44, FontStyles.Bold, Color.white);
            Stretch(btnLabel);
            btnLabel.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        }

        // 10 – Wire SceneLoader component
        SceneLoader loader = canvasGO.AddComponent<SceneLoader>();
        loader.InputNama = namaInput.GetComponent<TMP_InputField>();
        loader.InputKelas = kelasDD.GetComponent<TMP_Dropdown>();
        loader.ErrorText = errorObj.GetComponent<TMP_Text>();

        // Connect button onClick
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            btn.onClick,
            loader.LoadHome
        );

        // 11 – AudioManager GameObject
        SetupAudioManager(scene);

        // 12 – EventSystem
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // 13 – Save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[PasarAjaib] LoginScene UI rebuilt successfully!");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static void SetupAudioManager(Scene scene)
    {
        // Only create if none exists
        foreach (var root in scene.GetRootGameObjects())
            if (root.GetComponent<AudioManager>() != null) return;

        GameObject amGO = new GameObject("AudioManager");
        AudioManager am = amGO.AddComponent<AudioManager>();

        am.sfxButtonClick  = LoadClip(SFX_CLICK_PATH);
        am.sfxWrong        = LoadClip(SFX_WRONG_PATH);
        am.bgmMenu         = LoadClip(BGM_PATH);
    }

    static GameObject CreateImage(GameObject parent, string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    static GameObject CreateImageWithSprite(GameObject parent, string name, Sprite sprite)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        Image img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.color = Color.white;
        return go;
    }

    static TMP_FontAsset LoadPoppinsFont()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_PATH);
        if (font == null) Debug.LogWarning($"[PasarAjaib] Poppins font not found: {FONT_PATH}");
        return font;
    }

    static void ApplyFont(TMP_Text tmp)
    {
        var font = LoadPoppinsFont();
        if (font != null) tmp.font = font;
    }

    static GameObject CreateTMP(GameObject parent, string name, string text,
        float size, FontStyles style, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.color = color;
        ApplyFont(tmp);
#pragma warning disable 0618
        tmp.enableWordWrapping = true;
#pragma warning restore 0618
        return go;
    }

    static GameObject CreateInputField(GameObject parent, string name, string placeholder)
    {
        // Root
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent.transform, false);
        Image bg = root.AddComponent<Image>();
        bg.color = INPUT_COLOR;
        TMP_InputField field = root.AddComponent<TMP_InputField>();

        // Text area
        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(root.transform, false);
        RectTransform taRT = textArea.AddComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero;
        taRT.anchorMax = Vector2.one;
        taRT.offsetMin = new Vector2(16, 4);
        taRT.offsetMax = new Vector2(-16, -4);
        textArea.AddComponent<RectMask2D>();

        // Placeholder
        GameObject phGO = new GameObject("Placeholder");
        phGO.transform.SetParent(textArea.transform, false);
        TMP_Text ph = phGO.AddComponent<TextMeshProUGUI>();
        ph.text = placeholder;
        ph.fontSize = 30;
        ph.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        ph.fontStyle = FontStyles.Italic;
        ApplyFont(ph);
        Stretch(phGO);

        // Input text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(textArea.transform, false);
        TMP_Text inputText = textGO.AddComponent<TextMeshProUGUI>();
        inputText.text = "";
        inputText.fontSize = 30;
        inputText.color = LABEL_COLOR;
        ApplyFont(inputText);
        Stretch(textGO);

        field.textViewport = taRT;
        field.placeholder = ph;
        field.textComponent = inputText;

        return root;
    }

    static GameObject CreateDropdown(GameObject parent, string name)
    {
        // ── Root ─────────────────────────────────────────────────────────────
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent.transform, false);
        Image bg = root.AddComponent<Image>();
        bg.color = INPUT_COLOR;
        TMP_Dropdown dd = root.AddComponent<TMP_Dropdown>();
        root.AddComponent<UIButtonAnimator>();

        // ── Caption Label ────────────────────────────────────────────────────
        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(root.transform, false);
        TMP_Text label = labelGO.AddComponent<TextMeshProUGUI>();
        label.text = "Pilih Kelas";
        label.fontSize = 30;
        label.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        label.alignment = TextAlignmentOptions.MidlineLeft;
        ApplyFont(label);
        RectTransform lRT = labelGO.GetComponent<RectTransform>();
        lRT.anchorMin = Vector2.zero; lRT.anchorMax = Vector2.one;
        lRT.offsetMin = new Vector2(20, 4); lRT.offsetMax = new Vector2(-50, -4);

        // ── Arrow ────────────────────────────────────────────────────────────
        GameObject arrowGO = new GameObject("Arrow");
        arrowGO.transform.SetParent(root.transform, false);
        TMP_Text arrow = arrowGO.AddComponent<TextMeshProUGUI>();
        arrow.text = "▼";
        arrow.fontSize = 24;
        arrow.color = LABEL_COLOR;
        arrow.alignment = TextAlignmentOptions.Center;
        ApplyFont(arrow);
        RectTransform aRT = arrowGO.GetComponent<RectTransform>();
        aRT.anchorMin = new Vector2(1, 0); aRT.anchorMax = new Vector2(1, 1);
        aRT.pivot = new Vector2(1, 0.5f);
        aRT.anchoredPosition = new Vector2(-15, 0);
        aRT.sizeDelta = new Vector2(40, 0);

        // ── Template (dropdown popup) ─────────────────────────────────────────
        GameObject tmplGO = new GameObject("Template");
        tmplGO.transform.SetParent(root.transform, false);
        Image tmplImg = tmplGO.AddComponent<Image>();
        tmplImg.color = Color.white;
        ScrollRect scrollRect = tmplGO.AddComponent<ScrollRect>();
        RectTransform tmplRT = tmplGO.GetComponent<RectTransform>();
        tmplRT.anchorMin = new Vector2(0, 0); tmplRT.anchorMax = new Vector2(1, 0);
        tmplRT.pivot = new Vector2(0.5f, 1f);
        tmplRT.anchoredPosition = new Vector2(0, 2);
        tmplRT.sizeDelta = new Vector2(0, 300);

        // Viewport
        GameObject vpGO = new GameObject("Viewport");
        vpGO.transform.SetParent(tmplGO.transform, false);
        vpGO.AddComponent<Image>().color = new Color(1,1,1,0.01f);
        vpGO.AddComponent<Mask>().showMaskGraphic = false;
        RectTransform vpRT = vpGO.GetComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero; vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = vpRT.offsetMax = Vector2.zero;
        scrollRect.viewport = vpRT;

        // Content
        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(vpGO.transform, false);
        RectTransform contentRT = contentGO.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1); contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1f);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0, 0);
        scrollRect.content = contentRT;

        // Item template
        GameObject itemGO = new GameObject("Item");
        itemGO.transform.SetParent(contentGO.transform, false);
        Toggle itemToggle = itemGO.AddComponent<Toggle>();
        RectTransform itemRT = itemGO.GetComponent<RectTransform>();
        itemRT.anchorMin = new Vector2(0, 0.5f); itemRT.anchorMax = new Vector2(1, 0.5f);
        itemRT.sizeDelta = new Vector2(0, 80);

        // Item Background
        GameObject itemBgGO = new GameObject("Item Background");
        itemBgGO.transform.SetParent(itemGO.transform, false);
        Image itemBgImg = itemBgGO.AddComponent<Image>();
        itemBgImg.color = new Color(0.85f, 0.92f, 0.98f, 1f);
        Stretch(itemBgGO);

        // Item Checkmark (invisible placeholder)
        GameObject checkGO = new GameObject("Item Checkmark");
        checkGO.transform.SetParent(itemGO.transform, false);
        Image checkImg = checkGO.AddComponent<Image>();
        checkImg.color = new Color(0.2f, 0.6f, 1f, 1f);
        RectTransform checkRT = checkGO.GetComponent<RectTransform>();
        checkRT.anchorMin = new Vector2(0, 0.5f); checkRT.anchorMax = new Vector2(0, 0.5f);
        checkRT.sizeDelta = new Vector2(20, 20);
        checkRT.anchoredPosition = new Vector2(16, 0);

        // Item Label
        GameObject itemLabelGO = new GameObject("Item Label");
        itemLabelGO.transform.SetParent(itemGO.transform, false);
        TMP_Text itemLabel = itemLabelGO.AddComponent<TextMeshProUGUI>();
        itemLabel.text = "Option";
        itemLabel.fontSize = 30;
        itemLabel.color = LABEL_COLOR;
        itemLabel.alignment = TextAlignmentOptions.MidlineLeft;
        ApplyFont(itemLabel);
        RectTransform ilRT = itemLabelGO.GetComponent<RectTransform>();
        ilRT.anchorMin = Vector2.zero; ilRT.anchorMax = Vector2.one;
        ilRT.offsetMin = new Vector2(30, 4); ilRT.offsetMax = new Vector2(-10, -4);

        // Wire toggle
        itemToggle.targetGraphic = itemBgImg;
        itemToggle.graphic = checkImg;
        itemToggle.isOn = true;

        dd.itemText = itemLabel;
        dd.template = tmplRT;

        // Disable template (it activates at runtime when opened)
        tmplGO.SetActive(false);

        // ── Options ───────────────────────────────────────────────────────────
        dd.captionText = label;
        dd.options.Clear();
        dd.options.Add(new TMP_Dropdown.OptionData("Pilih Kelas"));
        dd.options.Add(new TMP_Dropdown.OptionData("Kelas 1"));
        dd.options.Add(new TMP_Dropdown.OptionData("Kelas 2"));
        dd.options.Add(new TMP_Dropdown.OptionData("Kelas 3"));
        dd.options.Add(new TMP_Dropdown.OptionData("Kelas 4"));
        dd.options.Add(new TMP_Dropdown.OptionData("Kelas 5"));
        dd.options.Add(new TMP_Dropdown.OptionData("Kelas 6"));
        dd.value = 0;
        dd.RefreshShownValue();

        return root;
    }

    static void Stretch(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static Sprite LoadSprite(string path)
    {
        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (s == null)
        {
            // Try loading as texture and converting
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex != null)
                s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        if (s == null) Debug.LogWarning($"[PasarAjaib] Sprite not found: {path}");
        return s;
    }

    static AudioClip LoadClip(string path)
    {
        AudioClip c = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        if (c == null) Debug.LogWarning($"[PasarAjaib] AudioClip not found: {path}");
        return c;
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }
}
#endif
