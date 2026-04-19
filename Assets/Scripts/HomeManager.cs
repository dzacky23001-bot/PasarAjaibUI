using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public TMP_Text greetingText;
    public Image profileImage;
    public Sprite[] profileSprites;

    void Start()
    {
        // SET NAMA
        string nama = PlayerPrefs.GetString("Nama", "Player");
        greetingText.text = "Selamat Datang, " + nama;

        // RANDOM PROFILE
        int index = Random.Range(0, profileSprites.Length);
        profileImage.sprite = profileSprites[index];
    }
}