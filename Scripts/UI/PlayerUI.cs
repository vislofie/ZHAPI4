using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private Slider _hpSlider;
    [SerializeField]
    private GameObject _menu;
    [SerializeField]
    private GameObject _gameOverPanel;
    [SerializeField]
    private GameObject _levelFinishedPanel;
    [SerializeField]
    private Text _timeText;

    [SerializeField]
    private Slider _gameVolumeSlider;
    [SerializeField]
    private Slider _musicVolumeSlider;
    [SerializeField]
    private Slider _sensibilitySlider;

    private PlayerBrain _brain;

    private float _maxHP;
    private bool _updateHealthWasCalled;

    private void Awake()
    {
        _brain = GetComponent<PlayerBrain>();
    }

    private void Start()
    {
        _updateHealthWasCalled = false;
    }

    public void UpdateHealth(float _hp)
    {
        if (!_updateHealthWasCalled)
        {
            _maxHP = _hp;
            _updateHealthWasCalled = true;
        }

        _hpSlider.value = _hp / _maxHP;
    }

    public void ActivateUI()
    {
        _canvas.gameObject.SetActive(true);
    }

    public void UpdateTimer(float seconds)
    {
        int flooredSeconds = Mathf.FloorToInt(seconds);
        int hours = flooredSeconds / 3600;
        int minutes = flooredSeconds / 60 - hours * 60;

        flooredSeconds = flooredSeconds - hours * 3600 - minutes * 60;

        _timeText.text = (hours < 10 ? '0' + hours.ToString() : hours.ToString()) + ":" + (minutes < 10 ? '0' + minutes.ToString() : minutes.ToString()) + ":" + (flooredSeconds < 10 ? '0' + flooredSeconds.ToString() : flooredSeconds.ToString());
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void OpenMenu()
    {
        _menu.SetActive(true);

        UnlockCursor();

        _brain.BlockMovement();
        _brain.BlockCameraMovement();
    }

    public void CloseMenu()
    {
        LockCursor();

        _brain.AllowMovement();
        _brain.AllowCameraMovement();

        _brain.UnPause();
    }

    public void Restart()
    {
        GameManager.RestartLevel();
    }

    public void MainMenu()
    {
        GameManager.LoadMainMenu();
        Time.timeScale = 1;
    }

    public void Quit()
    {
        GameManager.Quit();
    }

    public void GameOver()
    {
        _gameOverPanel.SetActive(true);
    }

    public void LevelFinish()
    {
        _levelFinishedPanel.SetActive(true);
    }

    public void Resume()
    {
        _brain.Resume();
    }

    public void OnChangeGameVolume()
    {
        GameManager.ChangeGameVolume(_gameVolumeSlider.value);
    }

    public void OnChangeMusicVolume()
    {
        GameManager.ChangeMusicVolume(_musicVolumeSlider.value);
    }

    public void OnChangeSensitivity()
    {
        GameManager.ChangeSensitivity(_sensibilitySlider.value);
    }

    public void UpdateSliders()
    {
        _gameVolumeSlider.value = GameManager.GameSoundValue;
        _musicVolumeSlider.value = GameManager.MusicValue;

        _sensibilitySlider.value = GameManager.Sensitivity;
    }
}
