using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    private static int _countOfEnemies;
    public static int CountOfEnemies => _countOfEnemies;

    private static PlayerBrain _player;

    private static float _cutValue = 0.6f;
    private static float _cutValueStart = _cutValue;

    // range is between 0 and 100
    private static float _gameSoundValue = 100.0f;
    private static float _musicValue = 100.0f;
    // range is between 0 and 20
    private static float _sensitivity = 7.5f;

    public static float GameSoundValue => _gameSoundValue * 100.0f / _cutValue;
    public static float MusicValue => _musicValue * 100.0f / _cutValue;

    public static float Sensitivity => _sensitivity / _cutValue;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        ChangeGameVolume(100);
        ChangeMusicVolume(100);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 2)
        {
            _countOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
            _player = FindObjectOfType<PlayerBrain>();

            FindObjectOfType<PlayerUI>().UpdateSliders();

            UpdateAudioVolumes();
            UpdateSensitivity();
        }
        else if (level == 1)
        {
            Invoke("LoadMainLevel", 3.0f);
        }
        else
        {
            _cutValue = 0.6f;

            UpdateAudioVolumes();
            UpdateSensitivity();

            FindObjectOfType<PlayerMenu>().UpdateSliders();
        }
    }

    /// <summary>
    /// Called when one of the enemies has died
    /// </summary>
    public static void EnemyOut()
    {
        _countOfEnemies--;

        if (_countOfEnemies == 0)
        {
            FinishLevel();
        }
    }    

    public static void ChangeGameVolume(float value)
    {
        _gameSoundValue = value / 100.0f * _cutValue;
        UpdateAudioVolumes();
    }

    public static void ChangeMusicVolume(float value)
    {
        _musicValue = value / 100.0f * _cutValue / 3 * 2;
        UpdateAudioVolumes();
    }

    public static void ChangeSensitivity(float value)
    {
        _sensitivity = value;
        UpdateSensitivity();
    }

    public static void CutSound()
    {
        float soundValue = _gameSoundValue / _cutValue * 100.0f;
        float musicValue = _musicValue / _cutValue * 100.0f;

        _cutValue = _cutValueStart - _cutValueStart / 8;
        Debug.Log(_cutValue);
        ChangeGameVolume(soundValue);
        ChangeMusicVolume(musicValue);
    }

    public static void Pause()
    {
        Time.timeScale = 0;

        EnemyBrain[] enemies = FindObjectsOfType<EnemyBrain>();
        foreach (EnemyBrain enemy in enemies)
        {
            enemy.Pause();
        }

        Camera.main.GetComponent<AudioSource>().Pause();
    }

    public static void UnPause()
    {
        Time.timeScale = 1;

        EnemyBrain[] enemies = FindObjectsOfType<EnemyBrain>();
        foreach (EnemyBrain enemy in enemies)
        {
            enemy.UnPause();
        }

        Camera.main.GetComponent<AudioSource>().UnPause();
    }
    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        UnPause();
    }

    public static void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public static void Quit()
    {
        Application.Quit();
    }

    private static void UpdateAudioVolumes()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

            foreach (AudioSource audio in audioSources)
            {
                if (audio.tag == "MainCamera") audio.volume = _musicValue;
                else audio.volume = _gameSoundValue;
            }
        }
        else
        {
            Camera.main.GetComponent<AudioSource>().volume = _musicValue;
        }
    }

    private static void UpdateSensitivity()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Camera.main.GetComponent<CameraController>().SetSensitivity(_sensitivity);
        }
    }

    private static void FinishLevel()
    {
        if (_player != null)
        {
            _player.FinishLevel();
        }
    }

    private void LoadMainLevel()
    {
        SceneManager.LoadScene(2);
    }
}
