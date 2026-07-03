using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    public GameObject buttonGroup;
    public GameObject achievementsPanel;
    public GameObject tipsPanel;
    public GameObject settingsPanel;
    public GameObject helpPanel;
    public GameObject creatorsPanel;
    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMainMenuBGM();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenAchievements()
    {
        buttonGroup.SetActive(false);
        achievementsPanel.SetActive(true);

    }
    public void BackToMenu()
    {
        achievementsPanel.SetActive(false);
        buttonGroup.SetActive(true);
    }

    public void OpenSettings()
    {
        buttonGroup.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        buttonGroup.SetActive(true);
    }

    public void OpenHelp()
    {
        buttonGroup.SetActive(false);
        helpPanel.SetActive(true);
    }

    public void CloseHelp()
    {
        helpPanel.SetActive(false);
        buttonGroup.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void OpenTips()
    {
        achievementsPanel.SetActive(false);
        tipsPanel.SetActive(true);
    }

    public void CloseTips()
    {
        tipsPanel.SetActive(false);
        achievementsPanel.SetActive(true);
    }

    public void OpenCreators()
    {
        creatorsPanel.SetActive(true);
    }

    public void CloseCreators()
    {
        creatorsPanel.SetActive(false);
    }
}
