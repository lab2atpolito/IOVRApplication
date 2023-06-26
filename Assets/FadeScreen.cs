using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public bool fadeOnStart = true; 
    public float fadeDuration;
    public Color fadeColor;
    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if( fadeOnStart )
        {
            FadeIn();
        }
    }

    public void FadeIn()
    {
        Fade(1, 0);
    }

    public void FadeOut()
    {
        Fade(0, 1);
    }

    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while (timer <= fadeDuration)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);
            _renderer.material.color = newColor;
            timer += Time.deltaTime;
            yield return null; 
        }

        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        _renderer.material.color = newColor2;
    }
}
