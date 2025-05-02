using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
