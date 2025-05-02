using System;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    [SerializeField,Range(1f, 20f)] private float timeToDestroy;

    private void FixedUpdate()
    {
        timeToDestroy-= Time.fixedDeltaTime;
        if(timeToDestroy<0) Destroy(this.gameObject);
    }
}
