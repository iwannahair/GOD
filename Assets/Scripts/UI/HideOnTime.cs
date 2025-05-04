using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class HideOnTime : MonoBehaviour
{
    [SerializeField] private float _timeToFadeIn;
    [SerializeField] private float _timeToShow;
    [SerializeField] private float _timeToHide;

    [SerializeField] TMP_Text _text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _text = _text ? _text : GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _text.color = new Color(1, 0, 0, 0);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float timer = 0;
        while (timer < _timeToFadeIn)
        {
            timer += 0.02f;
            _text.color = Color.Lerp( Color.clear, Color.red, timer / _timeToFadeIn);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        yield return new WaitForSecondsRealtime(_timeToShow);
        StartCoroutine(FadeOut());
    }
    
    IEnumerator FadeOut()
    {
        float timer = 0;
        while (timer < _timeToHide)
        {
            timer += 0.02f;
            _text.color = Color.Lerp(Color.red, Color.clear, timer / _timeToHide);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        gameObject.SetActive(false);
    }
}
