using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }

    public abstract void OnTap();
    public abstract bool OnBreak();
    public virtual bool IsMatchable(Piece other) => false;
    public virtual bool IsFallable() => true;
}