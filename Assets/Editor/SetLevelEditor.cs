using UnityEditor;
using UnityEngine;

public class LevelInputWindow : EditorWindow
{
    private int level;

    [MenuItem("Set Level/Set Level")]
    public static void ShowWindow()
    {
        LevelInputWindow window = GetWindow<LevelInputWindow>("Set Level");
        window.level = PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    private void OnGUI()
    {

        GUILayout.Label("Set Level", EditorStyles.boldLabel);
        level = EditorGUILayout.IntField("Level:", level);

        if (GUILayout.Button("Save"))
        {
            PlayerPrefs.SetInt("CurrentLevel", level);
            PlayerPrefs.Save();
            Close();
        }
    }
}
