using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalsUI : MonoBehaviour
{
    [SerializeField] private Transform goalGrid;
    [SerializeField] private GridLayoutGroup goalGridLayoutGroup;
    [SerializeField] private Transform successImage;
    [SerializeField] private GoalItemUI goalItemUIPrefab;

    private Dictionary<GoalType, GoalItemUI> goalItemUIs;


    private void Awake()
    {
        goalItemUIs = new Dictionary<GoalType, GoalItemUI>();
        successImage.gameObject.SetActive(false);

        EventManager.OnGoalsUpdated += UpdateGoalsUI;
        EventManager.OnLevelCompleted += ShowSuccessImage;
    }

    private void OnDestroy()
    {
        EventManager.OnGoalsUpdated -= UpdateGoalsUI;
        EventManager.OnLevelCompleted -= ShowSuccessImage;
    }

    private void UpdateGoalsUI(Dictionary<GoalType, int> currentGoals)
    {
        UpdateGoalItemUIs(currentGoals);
        SetGridItemSizeAndSpacing(currentGoals.Count);
    }

    private void UpdateGoalItemUIs(Dictionary<GoalType, int> currentGoals)
    {
        
        // Clean up old UI items that are no longer goals
        List<GoalType> goalItemUITypes = new List<GoalType>(goalItemUIs.Keys);
        foreach (GoalType uiGoalType in goalItemUITypes)
        {
            if (!currentGoals.ContainsKey(uiGoalType))
            {

                if (goalItemUIs.TryGetValue(uiGoalType, out GoalItemUI itemToDestroy))
                {
                    Destroy(itemToDestroy.gameObject);
                }
                goalItemUIs.Remove(uiGoalType);
            }
        }

        // Update existing UI items or create new ones if needed
        foreach (var goal in currentGoals)
        {
            GoalType type = goal.Key;
            int count = goal.Value;

            if (goalItemUIs.ContainsKey(type))
            {
                goalItemUIs[type].UpdateGoalCount(count);
            }
            else
            {
                GoalItemUI newItemUI = Instantiate(goalItemUIPrefab, goalGrid);
                GoalData goalData = GoalManager.Instance.GetGoalDataOfType(type);
                newItemUI.Initialize(goalData, count);

                goalItemUIs.Add(type, newItemUI);
            }
        }
    }

    private void SetGridItemSizeAndSpacing(int itemCount)
    {
        if (itemCount <= 0) return;

        if (itemCount == 1)
        {
            goalGridLayoutGroup.cellSize = new Vector2(100, 100);
            goalGridLayoutGroup.spacing = new Vector2(0, 0);
        }
        else if (itemCount == 2)
        {
            goalGridLayoutGroup.cellSize = new Vector2(72, 72);
            goalGridLayoutGroup.spacing = new Vector2(24, 24);
        }
        else
        {
            goalGridLayoutGroup.cellSize = new Vector2(64, 64);
            goalGridLayoutGroup.spacing = new Vector2(36, 24);
        }
    }

    private void ShowSuccessImage(bool success)
    {
        successImage.gameObject.SetActive(success);
    }
}