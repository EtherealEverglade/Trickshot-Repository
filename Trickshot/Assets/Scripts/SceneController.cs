using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject howTo;
    [SerializeField] private GameObject winLose;

    [SerializeField] private GameObject pauseMenu;

    public void Start()
    {
        controlsScreen.SetActive(true);
        howTo.SetActive(false);
        winLose.SetActive(false);

        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pauseMenu.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void Controls()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Credits()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ControlsScreen()
    {
        controlsScreen.SetActive(true);
        howTo.SetActive(false);
        winLose.SetActive(false);
    }

    public void HowTo()
    {
        controlsScreen.SetActive(false);
        howTo.SetActive(true);
        winLose.SetActive(false);
    }

    public void WinLose()
    {
        controlsScreen.SetActive(false);
        howTo.SetActive(false);
        winLose.SetActive(true);
    }
}
