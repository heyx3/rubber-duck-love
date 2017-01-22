﻿using UnityEngine;
using System.Collections;

public enum SoundEffectType
{
    kThrow,
    kSplash,
    kQuack,
    kSqueak,
    kBoat,
    kBoatHit,
    kMine,
    kMineExplode,
    kVictoryStab,
    kRockHit,
    kDefeat
}

public class AudioMgr : MonoBehaviour
{

    public static AudioMgr Instance;

    public AudioClip background;
    public AudioClip[] music;
    public AudioClip[] effects;

    public AudioClip[] quacks;
    public AudioClip[] splashes;

    public AudioSource ambient;
    public AudioSource bgm;
    public AudioSource sfx;
    public AudioSource thrower;

    public ThrowControl player;
    public bool throwing;

    void Awake()
    {
        Instance = this;
        Projectile.OnProjectileStageChange += Throw;
        ThrowControl.OnPlayerStateChange += Player;
    }

    private void Player(PlayerState oldState, PlayerState newState)
    {
        if (newState == PlayerState.Startup)
        {
            StartGame();
        }
        if (newState == PlayerState.Windup)
        {
            if (!throwing)
            {
                thrower.Play();
                throwing = true;
            }
        }
        if (newState != PlayerState.Windup)
        {
            if (throwing)
            {
                thrower.Stop();
                throwing = false;
            }
        }
        if (newState == PlayerState.Win)
        {
            Victory();
        }
        if (newState == PlayerState.Lose)
        {
            Defeat();
        }
    }

    private void Throw(ProjectileState oldState, ProjectileState newState, ProjectileType type)
    {
        if (newState == ProjectileState.Airborne)
        {
            PlaySFX(SoundEffectType.kThrow);
        }
        if (newState == ProjectileState.Landed)
        {
            PlaySFX(SoundEffectType.kSplash);
            int index = Random.Range(0, splashes.Length);
            effects[(int)SoundEffectType.kSplash] = splashes[index];
        }
    }

    public void StopAll()
    {
        ambient.Stop();
        bgm.Stop();
    }

    public void StartGame()
    {
        bgm.clip = music[0];
        ambient.Play();
        bgm.Play();
    }

    public void PlaySFX(SoundEffectType sfxType)
    {
        sfx.PlayOneShot(effects[(int)sfxType]);
    }

    void Update()
    {
        if (Input.GetKey("3"))
        {
            ambient.Stop();
            bgm.Stop();
        }
    }

    public void Victory()
    {
        PlaySFX(SoundEffectType.kVictoryStab);
        bgm.clip = music[1];
        ambient.Stop();
        bgm.Play();
    }

    public void Defeat()
    {
        bgm.Stop();
        PlaySFX(SoundEffectType.kDefeat);
    }


}
