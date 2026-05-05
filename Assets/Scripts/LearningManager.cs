using UnityEngine;
using UnityEngine.SceneManagement;

public class LearningManager : MonoBehaviour
{
    public void GoToMatematika()
    {
        SceneManager.LoadScene("ScanMatematikaScene");
    }

    public void GoToLiterasi()
    {
        SceneManager.LoadScene("ScanLiterasiScene");
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }
}