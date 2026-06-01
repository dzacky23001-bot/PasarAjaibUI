using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Mengurus logic tampilan Buku Laporan / Piala Popup.
/// Dipanggil setiap kali popup dibuka (OnEnable).
///
/// Data dari PlayerPrefs:
///   MatematikaProgress (0-3) = level Hitung selesai
///   LiterasiProgress   (0-3) = level Cerita selesai
/// </summary>
public class BukuLaporanManager : MonoBehaviour
{
    [Header("Overall Progress")]
    [Tooltip("TMP text untuk menampilkan persentase (0%..96%)")]
    public TMP_Text persenText;

    [Tooltip("Image dengan fillMethod Horizontal untuk progress bar overall")]
    public Image progressBarFill;

    [Header("Lapak Hitung (Matematika)")]
    public Image[] hitungBars;          // Prog1, Prog2, Prog3 di HitungCard
    public TMP_Text hitungLevelText;    // "X dari 3 level selesai"
    public Button   sertifHitung;       // Tombol Sertifikat Hitung

    [Header("Lapak Cerita (Literasi)")]
    public Image[] ceritaBars;          // Prog1, Prog2, Prog3 di CeritaCard
    public TMP_Text ceritaLevelText;    // "X dari 3 level selesai"
    public Button   sertifCerita;       // Tombol Sertifikat Cerita

    // Warna progress bar
    static readonly Color COL_BAR_DONE  = new Color(1f, 0.85f, 0.10f, 1f); // kuning
    static readonly Color COL_BAR_EMPTY = new Color(0.80f, 0.80f, 0.80f, 1f); // abu

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void OnEnable()  => RefreshUI();
    void Start()     => RefreshUI();

    // ── Main Refresh ──────────────────────────────────────────────────────────

    public void RefreshUI()
    {
        int hitProg  = Mathf.Clamp(PlayerPrefs.GetInt("MatematikaProgress", 0), 0, 3);
        int cerProg  = Mathf.Clamp(PlayerPrefs.GetInt("LiterasiProgress",   0), 0, 3);
        int total    = hitProg + cerProg; // 0-6

        // ── Overall percentage (kelipatan 16) ─────────────────────────────────
        int persen = total * 16; // 0, 16, 32, 48, 64, 80, 96
        if (persenText != null)
            persenText.text = persen + "%";

        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = persen / 100f;
            progressBarFill.enabled    = persen > 0;
        }

        // ── Hitung bars ───────────────────────────────────────────────────────
        RefreshBars(hitungBars, hitProg);

        if (hitungLevelText != null)
            hitungLevelText.text = $"{hitProg} dari 3 level selesai";

        SetSertifikat(sertifHitung, hitProg >= 3);

        // ── Cerita bars ───────────────────────────────────────────────────────
        RefreshBars(ceritaBars, cerProg);

        if (ceritaLevelText != null)
            ceritaLevelText.text = $"{cerProg} dari 3 level selesai";

        SetSertifikat(sertifCerita, cerProg >= 3);
    }

    void RefreshBars(Image[] bars, int progress)
    {
        if (bars == null) return;
        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] == null) continue;
            bars[i].color = i < progress ? COL_BAR_DONE : COL_BAR_EMPTY;
        }
    }

    void SetSertifikat(Button btn, bool unlocked)
    {
        if (btn == null) return;
        btn.interactable = unlocked;
        var img = btn.GetComponent<Image>();
        if (img) img.color = unlocked ? Color.white : new Color(0.55f, 0.55f, 0.55f, 0.65f);
    }
}
