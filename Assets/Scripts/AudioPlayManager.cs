using UnityEngine;

public class AudioPlayerManager : MonoBehaviour
{
    [SerializeField] AudioSource humanDeathSound, monsterDeathSound, bigMonsterDeathSound, arrow1Sound, arrow2Sound, arrow3Sound, cannon1Sound,cannon2Sound;

    public void PlayHumanDeathSound()
    {
        humanDeathSound.PlayOneShot(humanDeathSound.clip);
    }
    public void PlayMonsterDeathSound()
    {
        monsterDeathSound.PlayOneShot(monsterDeathSound.clip);
    }
    public void PlayBigMonsterDeathSound()
    {
        bigMonsterDeathSound.PlayOneShot(bigMonsterDeathSound.clip);
    }
    public void PlayArrow1Sound()
    {
        arrow1Sound.PlayOneShot(arrow1Sound.clip);
    }
    public void PlayArrow2Sound()
    {
        arrow2Sound.PlayOneShot(arrow2Sound.clip);
    }
    public void PlayArrow3Sound()
    {
        arrow3Sound.PlayOneShot(arrow3Sound.clip);
    }
    public void PlayCannon1Sound()
    {
        cannon1Sound.PlayOneShot(cannon1Sound.clip);
    }
    public void PlayCannon2Sound()
    {
        cannon2Sound.PlayOneShot(cannon2Sound.clip);
    }
}
