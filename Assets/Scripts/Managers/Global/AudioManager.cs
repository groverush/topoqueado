using System;
using UnityEngine;

[ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{
    // === Audio sources ===
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    // === Music ===
    [SerializeField] private MusicList[] musicList;
    [HideInInspector] public enum MusicType { MainMenu, InGame, GameOver }

    // === Sfx ===
    [SerializeField] private SfxList[] sfxList;
    [HideInInspector] public enum SfxType { Shared, Hammer, Mole, PowerUp };

    // === Volumes ===
    [Header("Music Volumes")]
    [SerializeField, Range(0, 1)] private float mainMenuSongVolume;
    [SerializeField, Range(0, 1)] private float mainThemeSongVolume;
    [SerializeField, Range(0, 1)] private float gameOverSongVolume;

    [Header("SFX Volumes")]
    [SerializeField, Range(0, 1)] private float selectionVolume;
    [SerializeField, Range(0, 1)] private float hammerHitVolume;
    [SerializeField, Range(0, 1)] private float hammerCloneHitVolume;
    [SerializeField, Range(0, 1)] private float moleHitVolume;
    [SerializeField, Range(0, 1)] private float moleLaughVolume;
    [SerializeField, Range(0, 1)] private float powerUpVolume;
    [SerializeField, Range(0, 1)] private float xRayEffectVolume;

    // === Properties ===
    public float MainMenuSongVolume => mainMenuSongVolume;
    public float MainThemeSongVolume => mainThemeSongVolume;
    public float GameOverSongVolume => gameOverSongVolume;
    public float SelectionVolume => selectionVolume;
    public float HammerHitVolume => hammerHitVolume;
    public float HammerCloneHitVolume => hammerCloneHitVolume;
    public float PowerUpVolume => powerUpVolume;
    public float XRayEffectVolume => xRayEffectVolume;

    // === Singleton ===
    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RegisterCollisionManager(CollisionManager collisionManager)
    {
        collisionManager.OnHitSuccess += (_) => PlaySFX(SfxType.Mole, 0, moleHitVolume);
        collisionManager.OnHitMiss += (_) => PlaySFX(SfxType.Mole, 1, moleLaughVolume);
    }

    public void PlayMusic(MusicType music, int musicIndex, float volume = 1)
    {
        AudioClip[] songs = musicList[(int)music].Songs;
        musicSource.clip = songs[musicIndex];
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void PlaySFX(SfxType sfx, int sfxIndex, float volume = 1)
    {
        AudioClip[] clips = sfxList[(int)sfx].Sfx;
        sfxSource.PlayOneShot(clips[sfxIndex], volume);
    }

    // 
#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] musicNames = Enum.GetNames(typeof(MusicType));
        Array.Resize(ref musicList, musicNames.Length);

        for (int i = 0; i < musicList.Length; i++)
        {
            musicList[i].name = musicNames[i];
        }

        string[] sfxNames = Enum.GetNames(typeof(SfxType));
        Array.Resize(ref sfxList, sfxNames.Length);

        for (int i = 0; i < sfxList.Length; i++)
        {
            sfxList[i].name = sfxNames[i];
        }
    }
#endif
}

//
[Serializable]
public struct SfxList
{
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sfx;
    public AudioClip[] Sfx => sfx;
}

[Serializable]
public struct MusicList
{
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] songs;
    public AudioClip[] Songs => songs;
}