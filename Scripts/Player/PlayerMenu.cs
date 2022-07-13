using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timeText;

    [SerializeField]
    private Slider _gameVolumeSlider;
    [SerializeField]
    private Slider _musicVolumeSlider;
    [SerializeField]
    private Slider _sensibilitySlider;

    private void Start()
    {
        UpdateScore();

        _gameVolumeSlider.value = 100.0f;
        _musicVolumeSlider.value = 100.0f;
        _sensibilitySlider.value = 7.5f;
    }

    private void Awake()
    {
        GetComponentInChildren<Animator>().SetTrigger("stopWalking");
    }

    public void Play()
    {
        GameManager.LoadGame();
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

    public void ResetScore()
    {
        PlayerPrefs.DeleteKey("TimeScore");
        UpdateScore();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateSliders()
    {
        _gameVolumeSlider.value = GameManager.GameSoundValue;
        _musicVolumeSlider.value = GameManager.MusicValue;

        _sensibilitySlider.value = GameManager.Sensitivity;
    }

    private void UpdateScore()
    {
        int seconds = PlayerPrefs.GetInt("TimeScore", -1);

        if (seconds != -1)
        {
            int hours = seconds / 3600;
            int minutes = seconds / 60 - hours * 60;

            seconds = seconds - hours * 3600 - minutes * 60;

            string resultLine = "quickest time\n";

            if (hours != 0)
                resultLine += hours.ToString() + " hours ";
            if (minutes != 0)
                resultLine += minutes.ToString() + " minutes ";
            if (seconds != 0)
                resultLine += seconds.ToString() + " seconds";

            _timeText.text = resultLine;
        }
        else
        {
            _timeText.text = "quickest time\nnone yet";
        }
    }
}
