using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject settingsPopup;
    public GameObject pialaPopup;

    void Start()
    {
        // Pastikan popup tertutup saat scene load
        if (settingsPopup != null) settingsPopup.SetActive(false);
        if (pialaPopup    != null) pialaPopup.SetActive(false);

        // Sinkronkan tampilan tombol sound dengan state saat ini
        RefreshSoundButton();
    }

    // ── Settings ──────────────────────────────────────────────────────────────
    public void OpenSettings()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsPopup != null) settingsPopup.SetActive(true);
        RefreshSoundButton();
    }

    public void CloseSettings()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsPopup != null) settingsPopup.SetActive(false);
    }

    // ── Sound Toggle ──────────────────────────────────────────────────────────
    public void ToggleSound()
    {
        bool currentlyOn = AudioManager.Instance != null
            ? AudioManager.Instance.IsSoundEnabled()
            : AudioListener.volume > 0;

        bool newState = !currentlyOn;

        // Matikan/hidupkan audio
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSoundEnabled(newState);
        else
            AudioListener.volume = newState ? 1f : 0f;

        AudioManager.Instance?.PlayButtonClick();
        RefreshSoundButton();
        Debug.Log($"[UIManager] Sound: {(newState ? "ON" : "OFF")}");
    }

    void RefreshSoundButton()
    {
        if (settingsPopup == null) return;

        bool isOn = AudioManager.Instance != null
            ? AudioManager.Instance.IsSoundEnabled()
            : AudioListener.volume > 0;

        // Cari SoundButton di dalam popup
        var soundBtn = settingsPopup.transform.Find("Card/SoundRow/SoundButton");
        if (soundBtn == null) return;

        // Update label teks
        var label = soundBtn.Find("Label")?.GetComponent<TMP_Text>();
        if (label != null) label.text = isOn ? "ON" : "OFF";

        // Update warna tombol
        var img = soundBtn.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
            img.color = isOn
                ? new Color(0.27f, 0.70f, 0.41f, 1f)  // hijau
                : new Color(0.65f, 0.65f, 0.65f, 1f);  // abu
    }

    // ── Piala ─────────────────────────────────────────────────────────────────
    public void OpenPiala()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (pialaPopup != null) pialaPopup.SetActive(true);
    }

    public void ClosePiala()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (pialaPopup != null) pialaPopup.SetActive(false);
    }

    // ── Navigasi ──────────────────────────────────────────────────────────────
    public void GoToLearning()
    {
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene("LearningScene");
    }

    // ── Logout & Exit ─────────────────────────────────────────────────────────
    public void Logout()
    {
        AudioManager.Instance?.PlayButtonClick();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoginScene");
    }

    public void ExitGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        Debug.Log("[UIManager] Keluar Aplikasi");
        Application.Quit();
    }
}
