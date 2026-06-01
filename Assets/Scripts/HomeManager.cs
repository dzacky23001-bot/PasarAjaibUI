using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [Header("Profile")]
    public TMP_Text nameText;
    public Image profileImage;
    public Sprite[] profileSprites;

    [Header("Stars (6 total = 3 Literasi + 3 Matematika)")]
    public Image[] starImages;       // 6 elemen
    public Sprite starFilled;
    public Sprite starEmpty;

    void Start()
    {
        // Nama pemain
        string nama = PlayerPrefs.GetString("Nama", "Pejuang Cilik");
        if (nameText != null) nameText.text = nama;

        // Random avatar
        if (profileSprites != null && profileSprites.Length > 0)
        {
            int idx = Random.Range(0, profileSprites.Length);
            if (profileImage != null) profileImage.sprite = profileSprites[idx];
        }

        // Bintang — total progress dari kedua modul
        UpdateStars();
    }

    public void UpdateStars()
    {
        if (starImages == null || starImages.Length == 0) return;

        // Hitung total level selesai (masing-masing 0-3)
        int literasiDone   = PlayerPrefs.GetInt("LiterasiProgress",  0);
        int matematikaDone = PlayerPrefs.GetInt("MatematikaProgress", 0);
        int totalDone      = Mathf.Clamp(literasiDone + matematikaDone, 0, starImages.Length);

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null) continue;
            bool lit = i < totalDone;
            if (starFilled != null && starEmpty != null)
                starImages[i].sprite = lit ? starFilled : starEmpty;
            else
                starImages[i].color = lit
                    ? new Color(1f, 0.85f, 0f, 1f)   // kuning terang
                    : new Color(1f, 1f, 1f, 0.35f);  // putih transparan
        }
    }
}
