using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

[CreateAssetMenu(menuName = "Fate/Sound/Sound Player")]
public class SoundPlayer : ScriptableObject
{
    public void PlayOneShot(SoundEntity sound)
    {
        GameManager.Instance.PlaySoundOneShot(sound);
    }
    public void PlayOneShotIgnorePause(SoundEntity sound)
    {
        GameManager.Instance.PlaySound(sound, ignoreListenerPause: true);
    }
}
