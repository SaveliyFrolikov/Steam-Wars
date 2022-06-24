using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] win;
    public AudioClip[] lose;
    public AudioClip[] missed;
    public AudioClip hit;
    public AudioClip destroy;

    [Space]

    public AudioClip spiderShoot;
    public AudioClip spiderHit;
    public AudioClip mortarShoot;
    public AudioClip mortarHit;
    public AudioClip tankShpot;

    public AudioSource musicSource;
    public AudioSource tractorSource;

    AudioSource sfxSource;

    public static SoundManager Instance;

    void Start()
    {
        Instance = this;
        sfxSource = GetComponent<AudioSource>();
        sfxSource.volume = PlayerPrefs.GetFloat("sfxVolume")/100;
        tractorSource.volume = PlayerPrefs.GetFloat("sfxVolume")/100 - 0.45f;
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume")/100;
    }

    private void Update()
    {
        if(GameManager.Instance.unit != null)
        {
            if(GameManager.Instance.unit.teamID == 0)
            {
                tractorSource.mute = false;
            }
            else
            {
                tractorSource.mute = true;
            }
        }
    }

    public void PlayWin()
    {
        sfxSource.clip = win[Random.Range(0, win.Length)];
        sfxSource.Play();
    }

    public void PlayLose()
    {
        sfxSource.clip = lose[Random.Range(0, lose.Length)];
        sfxSource.Play();
    }

    public void PlayMissed()
    {
        sfxSource.clip = missed[Random.Range(0, missed.Length)];
        sfxSource.Play();
    }

    public void PlayHit()
    {
        sfxSource.clip = hit;
        sfxSource.Play();
    }

    public void PlayDestroyed()
    {
        sfxSource.clip = destroy;
        sfxSource.Play();
    }

    public void SpiderShoot()
    {
        sfxSource.clip = spiderShoot;
        sfxSource.Play();
        Invoke("SpiderShoot2", 5f);
    }

    void SpiderShoot2()
    {
        sfxSource.clip = spiderHit;
        sfxSource.Play();
    }

    public void TankShoot()
    {
        sfxSource.clip = tankShpot;
        sfxSource.Play();
    }

    public void MortarShoot()
    {
        sfxSource.clip = mortarShoot;
        sfxSource.Play();
        Invoke("MortarShoot2", 1f);
    }

    void MortarShoot2()
    {
        sfxSource.clip = mortarHit;
        sfxSource.Play();
    }
}
