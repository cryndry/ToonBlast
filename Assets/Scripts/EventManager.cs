using System;
using UnityEngine;

public static class EventManager
{
    public static event Action<Collider2D> OnTapEvent;

    public static void InvokeTap(Collider2D tappedCollider)
    {
        OnTapEvent?.Invoke(tappedCollider);
    }
}