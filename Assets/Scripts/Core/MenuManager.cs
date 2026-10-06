using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelMain;
    public GameObject panelSettings;
    public GameObject panelTutorial;
    public GameObject panelDevInfo;
    public GameObject panelQuitConfirm;

    void Start()
    {
        ShowOnly(panelMain);
    }

    // ===== Navigasi antar panel =====

    public void ShowMain()
    {
        ShowOnly(panelMain);
    }

    public void ShowSettings()
    {
        ShowOnly(panelSettings);
    }

    public void ShowTutorial()
    {
        ShowOnly(panelTutorial);
    }

    public void ShowDevInfo()
    {
        ShowOnly(panelDevInfo);
    }

    public void ShowQuitConfirm()
    {
        ShowOnly(panelQuitConfirm);
    }

    void ShowOnly(GameObject panelToShow)
    {
        panelMain.SetActive(panelToShow == panelMain);
        panelSettings.SetActive(panelToShow == panelSettings);
        panelTutorial.SetActive(panelToShow == panelTutorial);
        panelDevInfo.SetActive(panelToShow == panelDevInfo);
        panelQuitConfirm.SetActive(panelToShow == panelQuitConfirm);
    }

    // ===== Aksi utama =====

    public void PlayGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void QuitGame()
    {
        Debug.Log("Keluar Game");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}