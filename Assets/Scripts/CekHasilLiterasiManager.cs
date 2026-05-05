using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CekHasilLiterasiManager : MonoBehaviour
{
    public GameObject[] koinObjects;

    public TMP_Text hasilText;
    public TMP_Text skorText;

    public GameObject btnCobaLagi;
    public GameObject btnNextLevel;

    void Start()
    {
        int score = PlayerPrefs.GetInt("LiterasiScore", 0);

        // Reset koin
        for (int i = 0; i < koinObjects.Length; i++)
        {
            koinObjects[i].SetActive(false);
        }

        // Aktifkan sesuai score
        for (int i = 0; i < score; i++)
        {
            koinObjects[i].SetActive(true);
        }

        skorText.text = score + " / 5";

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
        SceneManager.LoadScene("ScanLiterasiScene");
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("ScanLiterasiScene");
    }
}