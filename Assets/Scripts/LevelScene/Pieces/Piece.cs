using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }

    public abstract void OnTap();
    public abstract bool OnBreak();
    public virtual bool IsFallable() => true;

    void Start()
    {
        EventManager.OnTapEvent += HandleTap;
    }
    void OnDestroy()
    {
        EventManager.OnTapEvent -= HandleTap;
    }

    private void HandleTap(Collider2D tappedCollider)
    {
        if (tappedCollider != null && tappedCollider.gameObject == this.gameObject)
        {
            OnTap();
        }
    }
}