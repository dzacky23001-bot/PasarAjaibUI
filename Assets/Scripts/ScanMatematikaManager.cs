using UnityEngine;
using UnityEngine.SceneManagement;

public class ScanMatematikaManager : MonoBehaviour
{
    public void BackToMatematika()
    {
        SceneManager.LoadScene("LearningScene");
    }

    public void SetScore(int benar)
    {
        PlayerPrefs.SetInt("MatematikaScore", benar);
        PlayerPrefs.Save();
    }

    public void GoToCekHasil()
    {
        SetScore(5);
        SceneManager.LoadScene("CekHasilMatematikaScene");
    }
}