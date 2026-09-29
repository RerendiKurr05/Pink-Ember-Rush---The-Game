using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;
    public GameObject quitConfirmPanel;
    public GameObject devInfoPanel;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;

    void Start()
    {
        BackToMainMenu();

        {
            settingsPanel.SetActive(false);
            tutorialPanel.SetActive(false);
            quitConfirmPanel.SetActive(false);
            devInfoPanel.SetActive(false);
            mainPanel.SetActive(true);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenTutorial()
    {
        mainPanel.SetActive(false);
        tutorialPanel.SetActive(true);
    }

    public void OpenQuitConfirm()
    {
        mainPanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        settingsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);

        mainPanel.SetActive(true);
    }

    public void QuitGameYes()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
    public void SetVolumeBGM(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeSFX(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void OpenDevInfo()
    {
        mainPanel.SetActive(false);
        devInfoPanel.SetActive(true);
    }

}