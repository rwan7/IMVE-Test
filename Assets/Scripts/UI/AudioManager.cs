using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

	[Header("BGM List")]
	public AudioClip BgmMainMenu;
    public AudioClip BgmStage;
	
    [Header("Sound Effects")]
    public AudioClip walkingSound;
    public AudioClip jumpSound;
    public AudioClip enemyWalkSound;
    public AudioClip clickSound;
    public AudioClip pausedSound;
    public AudioClip gameOverSound;
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); 
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    IEnumerator DelayWinSFX(AudioClip clip) {
        SFXSource.PlayOneShot(clip);
        musicSource.Pause();
        yield return new WaitForSeconds(5f);
        musicSource.Play();
    }

    public void PlayClickSFX() { PlaySFX(clickSound); }
	
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
        musicSource.loop = true; 
    }

	
	public void StopSFX()
	{
		SFXSource.Stop();
	}
	
	public void PlayLoopingSFX(AudioClip clip)
	{
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