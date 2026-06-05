using System.Collections;
using UnityEngine;
using TMPro;

public class AlignmentUI : MonoBehaviour
{
    [SerializeField] TMP_Text pointsText;
    [SerializeField] TMP_Text comboText;
    [SerializeField] TMP_Text ratingText;

    [Header("Animation")]
    [SerializeField] float punchScale = 1.4f;
    [SerializeField] float punchDuration = 0.12f;
    [SerializeField] float fadeDelay = 1f;
    [SerializeField] float fadeDuration = 0.4f;

    Coroutine ratingCoroutine;
    Coroutine comboCoroutine;

    static readonly Color ColorPerfect   = new Color(1f,   0.84f, 0f);
    static readonly Color ColorExcellent = new Color(0.2f, 0.9f,  0.2f);
    static readonly Color ColorGood      = Color.white;
    static readonly Color ColorOkay      = new Color(1f,   0.85f, 0.2f);
    static readonly Color ColorBad       = new Color(1f,   0.45f, 0.1f);
    static readonly Color ColorTerrible  = new Color(0.8f, 0.1f,  0.1f);

    public void UpdateUI(string rating, int pointsEarned, float multiplier, int totalPoints, int streak)
    {
        if (pointsText != null)
            pointsText.text = $"Score: {totalPoints}";

        if (ratingText != null)
        {
            ratingText.text = $"{rating}\n+{pointsEarned}";
            ratingText.color = GetRatingColor(rating);
            if (ratingCoroutine != null) StopCoroutine(ratingCoroutine);
            ratingCoroutine = StartCoroutine(PunchAndFade(ratingText));
        }

        if (comboText != null)
        {
            if (streak >= 2)
            {
                string tier = multiplier >= 1.5f ? "PERFECT!!!" : "Excellent!";
                comboText.text = $"{streak}x {tier} Combo!";
                comboText.color = GetRatingColor(rating);
                if (comboCoroutine != null) StopCoroutine(comboCoroutine);
                comboCoroutine = StartCoroutine(PunchAndFade(comboText));
            }
            else
            {
                comboText.text = "";
            }
        }
    }

    Color GetRatingColor(string rating)
    {
        switch (rating)
        {
            case "PERFECT!!!": return ColorPerfect;
            case "Excellent!": return ColorExcellent;
            case "Good":       return ColorGood;
            case "okay":       return ColorOkay;
            case "bad.":       return ColorBad;
            default:           return ColorTerrible;
        }
    }

    IEnumerator PunchAndFade(TMP_Text label)
    {
        Vector3 originalScale = Vector3.one;
        label.transform.localScale = originalScale;

        float t = 0f;
        while (t < punchDuration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, punchScale, t / punchDuration);
            label.transform.localScale = Vector3.one * s;
            yield return null;
        }

        t = 0f;
        while (t < punchDuration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(punchScale, 1f, t / punchDuration);
            label.transform.localScale = Vector3.one * s;
            yield return null;
        }

        label.transform.localScale = originalScale;

        yield return new WaitForSeconds(fadeDelay);

        Color baseColor = label.color;
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            label.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        label.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        label.text = "";
    }
}
