using System.Collections;
using UnityEngine;

public class Splash : MonoBehaviour
{
    [SerializeField] private float timeToScale, timeToFade,timeToShow,scaleFactor;
    [SerializeField] SpriteRenderer rend;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.Rotate(Vector3.forward,Random.Range(0,360));
        rend = rend? rend: GetComponentInChildren<SpriteRenderer>();
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
        Destroy(gameObject);
    }
}
