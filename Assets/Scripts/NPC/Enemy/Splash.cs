using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Splash : MonoBehaviour
{
    [SerializeField] private float timeToScale, timeToFade,timeToShow,scaleFactor;
    [SerializeField] SpriteRenderer rend;

    public float ScaleFactor { set => scaleFactor = value; }

    private void Awake()
    {
        rend = rend? rend: GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        transform.Rotate(Vector3.forward,Random.Range(0,360));
        rend.color = Color.white;
        StartCoroutine(ScaleUp());
    }

    private IEnumerator ScaleUp()
    {
        float timer=0;
        while (timer < timeToScale)
        {
            timer += Time.fixedDeltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one * scaleFactor,timer / timeToScale);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(timeToShow);
        timer = 0;
        while (timer < timeToFade)
        {
            timer += Time.fixedDeltaTime;
            rend.color = Color.Lerp(rend.color,Color.clear,timer / timeToFade);
            yield return new WaitForFixedUpdate();
        }

        if (scaleFactor != 1f) scaleFactor = 1f;
        GameManager.instance.ReturnSplash(gameObject);
    }
}
