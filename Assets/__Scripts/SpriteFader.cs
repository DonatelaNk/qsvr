using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpriteFader : MonoBehaviour
{
    public float fadeSpeed = 2.0f;
    public float fadeDelay = 0;
    public bool fadeOnLoad = false;
    public bool fadeOutLoad = false;

    IEnumerator FadeInCoroutine;
    IEnumerator FadeOutCoroutine;

    private void Start()
    {
        if (fadeOnLoad)
        {
            FadeInSprite();
        }
        if (fadeOutLoad)
        {
            FadeOutSprite();
        }
    }

    public void FadeInSprite()
    {
        FadeInCoroutine = FadeIn(GetComponent<SpriteRenderer>()); // create an IEnumerator object
        StartCoroutine(FadeInCoroutine);
    }
    public void FadeOutSprite()
    {
        FadeOutCoroutine = FadeOut(GetComponent<SpriteRenderer>()); // create an IEnumerator object
        StartCoroutine(FadeOutCoroutine);
    }

    IEnumerator FadeOut( SpriteRenderer _sprite)
    {
       yield return new WaitForSeconds(fadeDelay);
       Color tmpColor = _sprite.color;
       while (tmpColor.a >0)
        {
            tmpColor.a -= Time.deltaTime / fadeSpeed;
            _sprite.color = tmpColor;
            if (tmpColor.a <=0)
            {
                tmpColor.a = 0f;
                StopCoroutine(FadeOutCoroutine);
            }
            yield return null;
        }
        _sprite.color = tmpColor;
    }
    IEnumerator FadeIn(SpriteRenderer _sprite)
    {
        yield return new WaitForSeconds(fadeDelay);
        Color tmpColor = _sprite.color;
        while (tmpColor.a < 1.0f)
        {
            tmpColor.a += Time.deltaTime / fadeSpeed;
            _sprite.color = tmpColor;
            if (tmpColor.a >= 1.0f)
            {
                tmpColor.a = 1.0f;
                StopCoroutine(FadeInCoroutine);
            }
            yield return null;
        }
        _sprite.color = tmpColor;
    }
}