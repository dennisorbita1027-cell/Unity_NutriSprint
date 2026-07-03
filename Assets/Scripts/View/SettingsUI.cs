using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle sfxToggle;
    public Toggle bgmToggle;

    [Header("Confirmation Panel UI")]
    public GameObject confirmationPopUpPanel;

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxToggle.isOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        bgmToggle.isOn = PlayerPrefs.GetInt("BGMEnabled", 1) == 1;

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        sfxToggle.onValueChanged.AddListener(OnSFXToggle);
        bgmToggle.onValueChanged.AddListener(OnBGMToggle);
    }

    void OnVolumeChanged(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
    }

    void OnSFXToggle(bool value)
    {
        AudioManager.Instance.SetSFXEnabled(value);
    }

    void OnBGMToggle(bool value)
    {
        AudioManager.Instance.SetBGMEnabled(value);
    }

    public void ClearHistory()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ClearAllPlayerProfileData();
        }
        else
        {
            Debug.LogError("SettingsUI: Cannot clear data because GameManager Instance is missing!");
        }
    }

    public void OpenConfirmationPopUp()
    {
        if (confirmationPopUpPanel != null)
        {
            confirmationPopUpPanel.SetActive(true);
        }
    }

    public void ConfirmClearHistory()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ClearAllPlayerProfileData();
        }

        CloseConfirmationPopUp();
    }

    public void CloseConfirmationPopUp()
    {
        if (confirmationPopUpPanel != null)
        {
            confirmationPopUpPanel.SetActive(false);
        }
    }
}
