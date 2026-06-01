using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField InputNama;
    public TMP_Dropdown InputKelas;

    [Header("Feedback")]
    public TMP_Text ErrorText;

    void Start()
    {
        if (ErrorText != null)
            ErrorText.gameObject.SetActive(false);
    }

    public void LoadHome()
    {
        string nama = InputNama.text.Trim();
        int kelasIndex = InputKelas.value;

        if (nama == "")
        {
            ShowError("Nama tidak boleh kosong!");
            ShakeObject(InputNama.gameObject);
            return;
        }

        if (kelasIndex == 0)
        {
            ShowError("Kelas belum dipilih!");
            ShakeObject(InputKelas.gameObject);
            return;
        }

        AudioManager.Instance?.PlayButtonClick();

        PlayerPrefs.SetString("Nama", nama);
        PlayerPrefs.SetInt("Kelas", kelasIndex);
        PlayerPrefs.Save();

        StartCoroutine(LoadWithFade());
    }

    void ShowError(string msg)
    {
        AudioManager.Instance?.PlayWrong();
        if (ErrorText == null) return;
        ErrorText.text = msg;
        ErrorText.gameObject.SetActive(true);
        StopCoroutine("HideErrorAfterDelay");
        StartCoroutine(HideErrorAfterDelay(3f));
    }

    IEnumerator HideErrorAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (ErrorText != null)
            ErrorText.gameObject.SetActive(false);
    }

    void ShakeObject(GameObject target)
    {
        StartCoroutine(ShakeCoroutine(target));
    }

    IEnumerator ShakeCoroutine(GameObject target)
    {
        if (target == null) yield break;
        Vector3 origin = target.transform.localPosition;
        float duration = 0.4f;
        float magnitude = 8f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = origin.x + Random.Range(-1f, 1f) * magnitude * (1f - elapsed / duration);
            target.transform.localPosition = new Vector3(x, origin.y, origin.z);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        target.transform.localPosition = origin;
    }

    IEnumerator LoadWithFade()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        SceneManager.LoadScene("HomeScene");
    }
}
