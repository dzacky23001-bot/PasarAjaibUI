using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LiterasiUIManager : MonoBehaviour
{
    public Image star1;
    public Image star2;
    public Image star3;

    public Sprite starOutline;
    public Sprite starFilled;

    void Start()
    {
        int progress = PlayerPrefs.GetInt("LiterasiProgress", 0);

        star1.sprite = starOutline;
        star2.sprite = starOutline;
        star3.sprite = starOutline;

        if (progress >= 1) star1.sprite = starFilled;
        if (progress >= 2) star2.sprite = starFilled;
        if (progress >= 3) star3.sprite = starFilled;
    }

    public void GoToScanLiterasi()
    {
        SceneManager.LoadScene("ScanLiterasiScene");
    }

    public void BackToLearning()
    {
        SceneManager.LoadScene("LearningScene");
    }
}