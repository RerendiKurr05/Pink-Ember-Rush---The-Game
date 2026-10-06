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

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;

    private bool isPaused = false;

    void Start()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(false);

        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(true);
    }

    void Update()
    {
        // Jangan bisa pause kalau game sudah over
        if (gameManager != null && gameManager.isGameOver)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);

        BackToPauseMain();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
    }

    public void OpenSettings()
    {
        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void OpenQuitConfirm()
    {
        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(false);

        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(true);
    }

    public void BackToPauseMain()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(false);

        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenuYes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void SetVolumeBGM(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
    }

    public void SetVolumeSFX(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
    }
}