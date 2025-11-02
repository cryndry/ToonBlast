using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }

    public abstract void OnTap();
    public abstract bool OnBreak();
    public virtual bool IsFallable() => true;
}