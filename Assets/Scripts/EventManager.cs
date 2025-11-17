using System;
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

    public static event Action OnMovesExhausted;

    public static void InvokeMovesExhausted()
    {
        OnMovesExhausted?.Invoke();
    }
}