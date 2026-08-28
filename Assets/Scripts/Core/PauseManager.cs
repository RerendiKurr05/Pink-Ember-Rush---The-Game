using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject pauseCanvas;
    public GameObject pauseMainPanel;
    public GameObject settingsPanel;
    public GameObject quitConfirmPanel;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        BackToPauseMain();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
    }
    public void OpenSettings()
    {
        pauseMainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenQuitConfirm()
    {
        pauseMainPanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    public void BackToPauseMain()
    {
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        pauseMainPanel.SetActive(true);
    }

    public void QuitToMainMenuYes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void SetVolumeBGM(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeSFX(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}