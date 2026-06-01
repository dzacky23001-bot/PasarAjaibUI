using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LearningManager : MonoBehaviour
{
    void Start()
    {
        // Wire BackButton saat runtime juga — pastikan selalu terhubung
        var canvas = GetComponent<Canvas>() ?? FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        var backTF = canvas.transform.Find("Header/BackButton");
        if (backTF != null)
        {
            var btn = backTF.GetComponent<Button>();
            if (btn != null)
            {
                // Hapus semua listener lama (avoid duplicate)
                btn.onClick.RemoveAllListeners();
                // Tambah runtime listener
                btn.onClick.AddListener(BackToHome);
                Debug.Log("[LearningManager] BackButton runtime listener added ✅");
            }
        }
    }

    public void GoToMatematika()
    {
        Debug.Log("[LearningManager] GoToMatematika");
        SceneManager.LoadScene("ScanMatematikaScene");
    }

    public void GoToLiterasi()
    {
        Debug.Log("[LearningManager] GoToLiterasi");
        SceneManager.LoadScene("ScanLiterasiScene");
    }

    public void BackToHome()
    {
        Debug.Log("[LearningManager] BackToHome ← dipanggil!");
        SceneManager.LoadScene("HomeScene");
    }
}
