using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static event Action<Collider2D> OnTapEvent;

    public static void InvokeTap(Collider2D tappedCollider)
    {
        OnTapEvent?.Invoke(tappedCollider);
    }

    public static event Action<int> OnMoveCountChanged;

    public static void InvokeMoveCountChanged(int newMoveCount)
    {
        OnMoveCountChanged?.Invoke(newMoveCount);
    }

    public static event Action<Dictionary<GoalType, int>> OnGoalsUpdated;

    public static void InvokeGoalsUpdated(Dictionary<GoalType, int> currentGoals)
    {
        OnGoalsUpdated?.Invoke(currentGoals);
    }

    public static event Action<bool> OnLevelCompleted;

    public static void InvokeLevelCompleted(bool success)
    {
        OnLevelCompleted?.Invoke(success);
    }
}