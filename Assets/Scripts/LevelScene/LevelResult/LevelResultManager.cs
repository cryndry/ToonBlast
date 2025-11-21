using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelResultManager : LazySingleton<LevelResultManager>
{
    [SerializeField] private GameObject levelResultWinUIGO;
    [SerializeField] private GameObject levelResultLoseUIGO;


    private void OnEnable()
    {
        EventManager.OnLevelCompleted += HandleLevelCompleted;
        EventManager.OnCheckLevelCompletion += CheckLevelCompletion;
        EventManager.OnLevelResultUIFinished += HandleLevelResultUIFinished;
    }

    private void OnDisable()
    {
        EventManager.OnLevelCompleted -= HandleLevelCompleted;
        EventManager.OnCheckLevelCompletion -= CheckLevelCompletion;
        EventManager.OnLevelResultUIFinished -= HandleLevelResultUIFinished;
    }

    public void CheckLevelCompletion()
    {
        if (GoalManager.Instance.AreGoalsCompleted())
        {
            EventManager.InvokeLevelCompleted(true);
        }
        else if (!MoveCountManager.Instance.HasMovesLeft())
        {
            EventManager.InvokeLevelCompleted(false);
        }
    }

    private void HandleLevelCompleted(bool isWin)
    {
        if (isWin)
        {
            levelResultWinUIGO.SetActive(true);
            LevelResultWinUI.Instance.Show();
        }
        else
        {
            levelResultLoseUIGO.SetActive(true);
            LevelResultLoseUI.Instance.Show();
        }
    }

    private void HandleLevelResultUIFinished(bool isWin)
    {
        if (isWin)
        {
            int finishedLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
            PlayerPrefs.SetInt("CurrentLevel", finishedLevel + 1);
            SceneManager.LoadScene("MainScene");
        }
    }
}
