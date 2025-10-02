using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public GameObject gameClearPanel;
    public GameObject gameOverPanel;
    public GameObject gameFoundPanel;

    public Button firstButtonA;
    public Button firstButtonB;

    public GameObject pauseMenu; // UIのPauseMenu Canvas
    public static bool isPaused { get; private set; } = false;
    public void GameClear()
    {
        gameClearPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); // 一旦クリア（バグ対策）
        EventSystem.current.SetSelectedGameObject(firstButtonA.gameObject);

        Time.timeScale = 0f; // ゲーム停止
    }

    public void GrenadeGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void FoundGameOver()
    {
        gameFoundPanel.SetActive(true);
    }

void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        EventSystem.current.SetSelectedGameObject(null); // 一旦クリア（バグ対策）
        EventSystem.current.SetSelectedGameObject(firstButtonB.gameObject);

        Cursor.lockState = CursorLockMode.None; // マウスを自由に動かせるように
        Cursor.visible = true;                  // マウスカーソルを表示

    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);


        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked; // マウスを画面中央に固定
        Cursor.visible = false;                   // カーソルを非表示
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("title");
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}

