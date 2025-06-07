using UnityEngine;

public class PlayOnDeathBigEnemy : MonoBehaviour
{
    private void OnDestroy()
    {
        if (!GameManager.instance) return;
        if(GameManager.instance.AudioPlayerManager) GameManager.instance.AudioPlayerManager.PlayBigMonsterDeathSound();
    }
}
