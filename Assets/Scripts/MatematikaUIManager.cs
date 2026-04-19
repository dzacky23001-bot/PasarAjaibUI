using UnityEngine;
using UnityEngine.SceneManagement;

public class MatematikaUIManager : MonoBehaviour
{
    public void GoToScanMatematika()
    {
        SceneManager.LoadScene("ScanMatematikaScene");
    }

    public void BackToLearning()
    {
        SceneManager.LoadScene("LearningScene");
    }
}