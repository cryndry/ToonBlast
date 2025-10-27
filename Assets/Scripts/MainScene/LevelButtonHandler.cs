using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LevelButtonHandler : MonoBehaviour
{
    [SerializeField] private Button levelButton;
    [SerializeField] private TextMeshProUGUI levelButtonText;

    readonly string levelsFolderPath = Path.Combine(Application.dataPath, "Levels");

    void Start()
    {
        bool isLevelLeft = GetLatestLevel();
        if (isLevelLeft)
        {
            levelButton.onClick.AddListener(LoadLevelScene);
        }
    }

    private bool GetLatestLevel()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);

        string levelFileName = $"level_{level:D2}.json";
        string levelFilePath = Path.Combine(levelsFolderPath, levelFileName);

        if (File.Exists(levelFilePath))
        {
            levelButtonText.text = "Level " + level;
            return true;
        }
        else
        {
            levelButtonText.text = "Finished";
            return false;
        }
    }

    public void LoadLevelScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
    }
}
