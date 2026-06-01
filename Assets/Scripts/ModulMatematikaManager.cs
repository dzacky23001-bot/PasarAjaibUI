using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Mengelola popup Modul Matematika.
/// Menampilkan 13 halaman modul sebagai slideshow.
/// Dipanggil dari LearningManager saat tombol ModulIcon diklik.
/// </summary>
public class ModulMatematikaManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupRoot;    // Root popup (aktif/nonaktif)
    public Image     pageImage;     // Image untuk tampilkan halaman
    public TMP_Text  pageCounter;   // "1 / 13"
    public Button    btnPrev;
    public Button    btnNext;
    public Button    btnClose;

    [Header("Halaman Modul (urut 01-13)")]
    public Sprite[] pages;          // Di-assign dari Inspector atau ResourceLoader

    int _current = 0;

    void Awake()
    {
        // Wire tombol
        if (btnPrev)  btnPrev.onClick.AddListener(PrevPage);
        if (btnNext)  btnNext.onClick.AddListener(NextPage);
        if (btnClose) btnClose.onClick.AddListener(CloseModul);
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

        // Tampilkan halaman
        if (pageImage != null && _current < pages.Length)
            pageImage.sprite = pages[_current];

        // Counter
        if (pageCounter != null)
            pageCounter.text = $"{_current + 1} / {pages.Length}";

        // Aktif/nonaktif tombol navigasi
        if (btnPrev) btnPrev.interactable = _current > 0;
        if (btnNext) btnNext.interactable = _current < pages.Length - 1;
    }
}
