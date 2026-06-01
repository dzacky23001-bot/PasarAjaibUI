using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Mengelola popup Modul Literasi.
/// Menampilkan halaman modul literasi sebagai slideshow.
/// Dipanggil saat tombol Modul Literasi diklik.
/// </summary>
public class ModulLiterasiManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupRoot;    // Root popup Literasi (aktif/nonaktif)
    public Image pageImage;     // Image untuk tampilkan halaman
    public TMP_Text pageCounter;   // Teks counter halaman (misal: "1 / 10")
    public Button btnPrev;
    public Button btnNext;
    public Button btnClose;

    [Header("Halaman Modul Literasi")]
    public Sprite[] pages;          // Di-assign dari Inspector

    int _current = 0;

    void Awake()
    {
        // Menyambungkan tombol secara otomatis lewat script
        if (btnPrev) btnPrev.onClick.AddListener(PrevPage);
        if (btnNext) btnNext.onClick.AddListener(NextPage);
        if (btnClose) btnClose.onClick.AddListener(CloseModul);

        // Pastikan popup mati saat game baru mulai
        if (popupRoot) popupRoot.SetActive(false);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void OpenModul()
    {
        _current = 0;
        if (popupRoot) popupRoot.SetActive(true);
        AudioManager.Instance?.PlayButtonClick();
        RefreshPage();
    }

    public void CloseModul()
    {
        if (popupRoot) popupRoot.SetActive(false);
        AudioManager.Instance?.PlayButtonClick();
    }

    public void NextPage()
    {
        if (pages == null || pages.Length == 0) return;
        _current = Mathf.Min(_current + 1, pages.Length - 1);
        AudioManager.Instance?.PlayButtonClick();
        RefreshPage();
    }

    public void PrevPage()
    {
        if (pages == null || pages.Length == 0) return;
        _current = Mathf.Max(_current - 1, 0);
        AudioManager.Instance?.PlayButtonClick();
        RefreshPage();
    }

    // ── Internal ──────────────────────────────────────────────────────────────

    void RefreshPage()
    {
        if (pages == null || pages.Length == 0) return;

        // Tampilkan halaman sesuai urutan saat ini
        if (pageImage != null && _current < pages.Length)
            pageImage.sprite = pages[_current];

        // Perbarui teks counter
        if (pageCounter != null)
            pageCounter.text = $"{_current + 1} / {pages.Length}";

        // Nyalakan/matikan tombol navigasi sesuai halaman
        if (btnPrev) btnPrev.interactable = _current > 0;
        if (btnNext) btnNext.interactable = _current < pages.Length - 1;
    }
}