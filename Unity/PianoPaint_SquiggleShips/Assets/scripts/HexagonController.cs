using System.Collections;
using UnityEngine;

public class HexagonController : MonoBehaviour
{
    public float growRate = 5f;
    public float rotationSpeed = 5f;

    [Header("Fade")]
    public float fadeDuration = 1.5f;

    // X = normalized time (0..1), Y = normalized blend (0..1)
    // Default is linear. Replace with ease-in/out, custom, etc.
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Time")]
    public bool useUnscaledTime = false;

    private SpriteRenderer sr;
    private Coroutine routine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        FadeOut();
    }

    public void FadeOut() => FadeTo(0f);
    public void FadeIn()  => FadeTo(1f);

    public void FadeTo(float targetAlpha)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = sr.color.a;
        float time = 0f;

        // Avoid weirdness if duration is 0
        float duration = Mathf.Max(0.0001f, fadeDuration);

        while (time < duration)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            time += dt;

            float t01 = Mathf.Clamp01(time / duration);

            // Evaluate curve: 0..1 -> 0..1 (or beyond if you design it that way)
            float curvedT = fadeCurve.Evaluate(t01);

            Color c = sr.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, curvedT);
            sr.color = c;

            yield return null;
        }

        Color final = sr.color;
        final.a = targetAlpha;
        sr.color = final;

        routine = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        transform.localScale *= 1 + (growRate * Time.deltaTime);

        Color c = sr.color;
        sr.color = c;

    }
}
