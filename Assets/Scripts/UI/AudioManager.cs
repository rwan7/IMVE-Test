using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SaveManager;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] private AudioSource UISFXSource;
    [SerializeField] private AudioSource playerSFXSource;
    [SerializeField] private AudioSource enemySFXSource;

    [Header("BGM List")]
    public AudioClip BgmMainMenu;
    public AudioClip BgmStage;

    [Header("Sound Effects List")]
    public AudioClip walkingSound;
    public AudioClip jumpSound;
    public AudioClip enemyWalkSound;
    public AudioClip clickSound;
    public AudioClip pausedSound;
    public AudioClip gameOverSound;

    private SaveManager saveManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveManager = new SaveManager();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    // private void Start()
    // {
    //     LoadAudioSettings();
    // }

    public void SetSoundEnabled(bool enabled)
    {
        UISFXSource.mute = !enabled;
        playerSFXSource.mute = !enabled;
        enemySFXSource.mute = !enabled;
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicSource.mute = !enabled;
    }

    public void PlayUISFX(AudioClip clip)
    {
        UISFXSource.PlayOneShot(clip);
    }

    public void PlayPlayerSFX(AudioClip clip)
    {
        playerSFXSource.PlayOneShot(clip);
    }

    public void PlayEnemySFX(AudioClip clip)
    {
        enemySFXSource.PlayOneShot(clip);
    }

    public void PlayClickSFX() { PlayUISFX(clickSound); }
    public void PlayPausedSFX() { PlayUISFX(pausedSound); }
    public void PlayGameOverSFX() { PlayUISFX(gameOverSound); }
    public void PlayJumpSFX() { PlayPlayerSFX(jumpSound); }
    public void PlayWalkingSFX() { PlayLoopingSFX(playerSFXSource, walkingSound); }
    public void PlayEnemyWalkSFX() { PlayLoopingSFX(enemySFXSource, enemyWalkSound); }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllSounds();
        switch (scene.name)
        {
            case "Main Menu":
                PlayBGM(BgmMainMenu);
                break;
            case "Stage":
                PlayBGM(BgmStage);
                break;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayLoopingSFX(AudioSource source, AudioClip clip)
    {
        if (source.clip == clip && source.isPlaying) return;
        source.loop = true;
        source.clip = clip;
        source.Play();
    }

    public void StopLoopingSFX(AudioSource source = null)
    {
        if (source == null)
        {
            if (playerSFXSource.isPlaying && playerSFXSource.loop)
            {
                playerSFXSource.loop = false;
                playerSFXSource.Stop();
            }
            
            if (enemySFXSource.isPlaying && enemySFXSource.loop)
            {
                enemySFXSource.loop = false;
                enemySFXSource.Stop();
            }
        }
        else
        {
            if (source.isPlaying && source.loop)
            {
                source.loop = false;
                source.Stop();
            }
        }
    }

    public void StopAllSounds()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        if (UISFXSource.isPlaying)
        {
            UISFXSource.Stop();
        }

        if (playerSFXSource.isPlaying)
        {
            playerSFXSource.Stop();
        }

        if (enemySFXSource.isPlaying)
        {
            enemySFXSource.Stop();
        }
    }

    // public void OnSoundEffectsToggleChanged(bool isEnabled)
    // {
    //     SetSoundEnabled(isEnabled);
    // }

    // public void LoadAudioSettings()
    // {
    //     SaveData data = saveManager.LoadData(); 
    //     SetMusicEnabled(data.MusicEnabled);
    //     SetSoundEnabled(data.SoundEffectsEnabled);
    // }
}