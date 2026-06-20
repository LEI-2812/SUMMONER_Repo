using UnityEngine;

[DisallowMultipleComponent]
// 역할: 소환수 공격, 버프, 디버프 효과음을 재생한다.
public class SummonSoundView : MonoBehaviour
{
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource debuffSound;
    [SerializeField] private AudioSource buffSound;

    public void AudioSourcesSet(AudioSource attack, AudioSource debuff, AudioSource buff)
    {
        if (attackSound == null)
        {
            attackSound = attack;
        }

        if (debuffSound == null)
        {
            debuffSound = debuff;
        }

        if (buffSound == null)
        {
            buffSound = buff;
        }
    }

    public void AttackSoundPlay()
    {
        SoundPlay(attackSound);
    }

    public void DebuffSoundPlay()
    {
        SoundPlay(debuffSound);
    }

    public void BuffSoundPlay()
    {
        SoundPlay(buffSound);
    }

    private void SoundPlay(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
