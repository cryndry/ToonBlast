using System.Collections.Generic;
using UnityEngine;

public class GoalManager : LazySingleton<GoalManager>
{
    [SerializeField] private List<GoalData> goalDataList;

    private Dictionary<GoalType, GoalData> goalDataDict;
    private Dictionary<GoalType, int> currentGoals;

    private void Awake()
    {
        currentGoals = new Dictionary<GoalType, int>();

        goalDataDict = new Dictionary<GoalType, GoalData>();
        foreach (GoalData goalData in goalDataList)
        {
            goalDataDict.Add(goalData.goalType, goalData);
        }
    }

    public void InitializeGoals(Dictionary<GoalType, int> goals)
    {
        currentGoals = new Dictionary<GoalType, int>(goals);
        EventManager.InvokeGoalsUpdated(currentGoals);
    }

    public void IncreaseGoalOfType(GoalType type)
    {
        currentGoals[type] = currentGoals.GetValueOrDefault(type, 0) + 1;
        EventManager.InvokeGoalsUpdated(currentGoals);
    }

    public void DecreaseGoalOfType(GoalType type)
    {
        if (currentGoals.ContainsKey(type))
        {
            currentGoals[type]--;
            EventManager.InvokeGoalsUpdated(currentGoals);
        }
    }

    public bool AreGoalsCompleted()
    {
        foreach (var goal in currentGoals)
        {
            if (goal.Value > 0){
                return false;
            }
        }

        return true;
    }
}