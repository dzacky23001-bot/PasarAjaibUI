using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to any Button for a smooth press/release scale animation.
/// </summary>
public class UIButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Range(0.7f, 1f)] public float pressedScale = 0.92f;
    public float animDuration = 0.08f;

    private Vector3 originalScale;
    private Coroutine currentAnim;

    void Awake() => originalScale = transform.localScale;

    public void OnPointerDown(PointerEventData _)
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(ScaleTo(originalScale * pressedScale, animDuration));
    }

    public void OnPointerUp(PointerEventData _)
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(ScaleWithBounce(originalScale, animDuration));
    }

    IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.localScale = target;
    }

    IEnumerator ScaleWithBounce(Vector3 target, float duration)
    {
        // Slight overshoot for satisfying pop
        Vector3 overshoot = target * 1.05f;
        yield return StartCoroutine(ScaleTo(overshoot, duration * 0.6f));
        yield return StartCoroutine(ScaleTo(target, duration * 0.4f));
    }
}
