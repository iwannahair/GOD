using UnityEngine;

public class PlayOnDeathEnemy : MonoBehaviour
{
    private void OnDestroy()
    {
        if (!GameManager.instance) return;
        if(GameManager.instance.AudioPlayerManager) GameManager.instance.AudioPlayerManager.PlayMonsterDeathSound();
    }
}
