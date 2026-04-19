using UnityEngine;
using UnityEngine.SceneManagement;

public class LearningManager : MonoBehaviour
{
    public void GoToMatematika()
    {
        SceneManager.LoadScene("LapakMatematikaScene");
    }

    public void GoToLiterasi()
    {
        SceneManager.LoadScene("LapakLiterasiScene");
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }
}