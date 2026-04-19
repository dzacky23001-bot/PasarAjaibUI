using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject settingsPopup;

    private bool isSoundOn = true;

    public void OpenSettings()
    {
        settingsPopup.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPopup.SetActive(false);
    }

    public void Logout()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("LoginScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Keluar aplikasi");
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        Debug.Log("Sound: " + (isSoundOn ? "ON" : "OFF"));
    }

    public void GoToLearning()
    {
        SceneManager.LoadScene("LearningScene");
    }
}