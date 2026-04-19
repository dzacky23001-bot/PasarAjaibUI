using UnityEngine;
using UnityEngine.SceneManagement;

public class ScanLiterasiManager : MonoBehaviour
{
    public void BackToLiterasi()
    {
        SceneManager.LoadScene("LapakLiterasiScene");
    }

    public void SetScore(int benar)
    {
        PlayerPrefs.SetInt("LiterasiScore", benar);
        PlayerPrefs.Save();
    }

    public void GoToCekHasil()
    {
        SetScore(3);
        SceneManager.LoadScene("CekHasilLiterasiScene");
    }
}