using UnityEngine;
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
        }
    }

    void Start()
    {
        ambient.clip = background;
        StartGame();
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
        if (Input.GetMouseButtonDown(0))
        {
            Defeat();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Victory();
        }
        if (player.currState == PlayerState.Windup)
        {
            if (!throwing)
            {
                thrower.Play();
                throwing = true;
            }
        }
        if (player.currState != PlayerState.Windup)
        {
            if (throwing)
            {
                thrower.Stop();
                throwing = false;
            }
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
