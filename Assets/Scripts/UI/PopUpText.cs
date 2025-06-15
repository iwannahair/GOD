using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour
{
    [SerializeField,Range(0f,1f)] private float timeToDisappear = 0.3f;
    [SerializeField,Range(1f,3f)] private float scaleFactor = 1.7f;
    [SerializeField] private TMP_Text text, percentageText;

    private void OnEnable()
    { 
        StartCoroutine(JumpThenDisappear());
    }

    private IEnumerator JumpThenDisappear()
    {
        float timer = 0;
        while (timer < timeToDisappear)
        {
            timer+=Time.fixedDeltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one*scaleFactor,timer/timeToDisappear );
            yield return new WaitForFixedUpdate();
        }
 
        while (timer > 0)
        {
            timer-=Time.fixedDeltaTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, timer / timeToDisappear);
            percentageText.color = new Color(percentageText.color.r, percentageText.color.g, percentageText.color.b, timer / timeToDisappear);
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        percentageText.color = new Color(percentageText.color.r, percentageText.color.g, percentageText.color.b, 1);
        transform.localScale = Vector3.one;
    }
}
