using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;

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
    public AudioClip enemyGrowlSound;
	
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayClickSFX() { PlaySFX(clickSound); }
    public void PlayPausedSFX() { PlaySFX(pausedSound); }
    public void PlayGameOverSFX() { PlaySFX(gameOverSound); }
    public void PlayEnemyGrowlSFX() { PlaySFX(enemyGrowlSound); }
    public void PlayJumpSFX() { PlaySFX(jumpSound); }
    public void PlayWalkingSFX() { PlayLoopingSFX(walkingSound); }
    public void PlayEnemyWalkSFX() { PlayLoopingSFX(enemyWalkSound); }
	
	void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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

	
	public void StopSFX()
	{
		SFXSource.Stop();
	}
	
    public void PlayLoopingSFX(AudioClip clip)
    {
        if (SFXSource.clip == clip && SFXSource.isPlaying) return;
        SFXSource.loop = true;
        SFXSource.clip = clip;
        SFXSource.Play();
    }


	public void StopLoopingSFX()
	{
		SFXSource.loop = false;
		SFXSource.Stop();
	}
}