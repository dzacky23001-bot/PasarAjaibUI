using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CekHasilMatematikaManager : MonoBehaviour
{
    public GameObject[] koinObjects; // Coin1–Coin5

    public TMP_Text hasilText;
    public TMP_Text skorText;

    public GameObject btnCobaLagi;
    public GameObject btnNextLevel;

    void Start()
    {
        int score = PlayerPrefs.GetInt("MatematikaScore", 0);

        // Matikan semua koin
        for (int i = 0; i < koinObjects.Length; i++)
        {
            koinObjects[i].SetActive(false);
        }

        // Aktifkan sesuai score
        for (int i = 0; i < score; i++)
        {
            koinObjects[i].SetActive(true);
        }

        // Set skor text
        skorText.text = score + " / 5";

        // Logic hasil + tombol
        if (score < 4)
        {
            hasilText.text = "Hampir Berhasil!";
            btnCobaLagi.SetActive(true);
            btnNextLevel.SetActive(false);
        }
        else
        {
            hasilText.text = "Kamu Berhasil!";
            btnCobaLagi.SetActive(false);
            btnNextLevel.SetActive(true);
        }
    }

    public void CobaLagi()
    {
        SceneManager.LoadScene("ScanMatematikaScene");
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("LapakMatematikaScene");
    }
}