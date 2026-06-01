#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// Wire BackButton onclick ke BackToHome + perbesar touch target.
public static class QuickWireLearning
{
    [MenuItem("PasarAjaib/Wire Learning Buttons")]
    public static void Wire()
    {
        var scene = EditorSceneManager.GetActiveScene();

        Canvas cvs = null;
        foreach (var r in scene.GetRootGameObjects())
        { cvs = r.GetComponent<Canvas>(); if (cvs) break; }
        if (!cvs) { Debug.LogError("[Wire] Canvas tidak ditemukan!"); return; }

        var lm = cvs.GetComponent<LearningManager>() ?? cvs.gameObject.AddComponent<LearningManager>();

        // ── BackButton ─────────────────────────────────────────────────────────
        var backTF = cvs.transform.Find("Header/BackButton");
        if (backTF)
        {
            // Perbesar touch target supaya lebih mudah diklik
            var rt = backTF.GetComponent<RectTransform>();
            if (rt) rt.sizeDelta = new Vector2(80, 80);

            // Pastikan Image ada
            var img = backTF.GetComponent<Image>();
            if (img) img.raycastTarget = true;

            // Hapus semua persistent listener lama & tambah baru
            var btn = backTF.GetComponent<Button>();
            if (!btn) btn = backTF.gameObject.AddComponent<Button>();
            btn.interactable   = true;
            btn.targetGraphic  = img;
            btn.onClick.RemoveAllListeners();

            // Hapus persistent listeners lama
            var so = new UnityEditor.SerializedObject(btn);
            var onClickProp = so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
            if (onClickProp != null) { onClickProp.ClearArray(); so.ApplyModifiedProperties(); }

            // Tambah persistent listener baru
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, lm.BackToHome);

            // Animator
            if (!backTF.GetComponent<UIButtonAnimator>())
                backTF.gameObject.AddComponent<UIButtonAnimator>();

            Debug.Log($"[Wire] BackButton → BackToHome WIRED (listeners: {btn.onClick.GetPersistentEventCount()}) ✅");
        }

        // ── ModulIcon Hitung ────────────────────────────────────────────────────
        var modH = cvs.transform.Find("LapakHitung/ModulIcon");
        if (modH)
        {
            EnsureButton(modH.gameObject);
            Debug.Log("[Wire] ModulIcon Hitung ✅");
        }

        // ── ModulIcon Cerita ────────────────────────────────────────────────────
        var modC = cvs.transform.Find("LapakLiterasi/ModulIcon");
        if (modC)
        {
            EnsureButton(modC.gameObject);
            Debug.Log("[Wire] ModulIcon Cerita ✅");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[PasarAjaib] ✅ Wire selesai!");
    }

    static void EnsureButton(GameObject go)
    {
        var img = go.GetComponent<Image>();
        if (img) img.raycastTarget = true;
        var btn = go.GetComponent<Button>() ?? go.AddComponent<Button>();
        btn.interactable = true;
        if (img) btn.targetGraphic = img;
        if (!go.GetComponent<UIButtonAnimator>()) go.AddComponent<UIButtonAnimator>();
    }
}
#endif
