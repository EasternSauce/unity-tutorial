using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTint : MonoBehaviour
{
    [SerializeField] Image screenCover;

    [SerializeField] Color unTintedColor;
    [SerializeField] Color tintedColor;

    float f;
    [SerializeField] float speed = 0.5f;

    [ContextMenu("Start Tint")]
    public void Tint()
    {
        StopAllCoroutines();
        f = 0f;
        StartCoroutine(TintScreen());
    }

    [ContextMenu("Start UnTint")]
    public void UnTint()
    {
        StopAllCoroutines();
        f = 0f;
        StartCoroutine(UnTintScreen());
    }

    IEnumerator TintScreen()
    {
        while (f < 1f)
        {
            f += Time.deltaTime * speed;
            f = Mathf.Clamp01(f);

            Color c = screenCover.color;
            c = Color.Lerp(unTintedColor, tintedColor, f);
            screenCover.color = c;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator UnTintScreen()
    {
        while (f < 1f)
        {
            f += Time.deltaTime * speed;
            f = Mathf.Clamp01(f);

            Color c = screenCover.color;
            c = Color.Lerp(tintedColor, unTintedColor, f);
            screenCover.color = c;

            yield return new WaitForEndOfFrame();
        }
    }
}
