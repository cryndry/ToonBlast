using System.Collections;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected BoxCollider2D boxCollider;

    public abstract void OnTap();
    public abstract bool OnBreak();
    public abstract bool OnBreakPowerUp();
    public virtual bool IsFallable() => true;
    protected abstract IEnumerator Explode();

    public event PieceEventHandler OnGridPositionChanged;
    public delegate void PieceEventHandler(Vector2Int newGridPosition);

    protected float PieceSize => GameBoard.Instance.pieceSize;
    private const float fallSpeedFactor = 3f;

    private Vector3? targetPosition = null;
    private float extraSpeedFactor = 1f;

    void Start()
    {
        SetColliderSizeAndOffset();
        EventManager.OnTapEvent += HandleTap;
    }

    void Update()
    {
        Move();
    }

    protected virtual void OnDestroy()
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

    public void MoveToPosition(Vector3 targetPosition, float extraSpeedFactor = 1f)
    {
        this.targetPosition = targetPosition;
        this.extraSpeedFactor = Mathf.Clamp(extraSpeedFactor, 1f, 3f);
    }

    private void Move()
    {
        if (targetPosition != null)
        {
            float movement = PieceSize * fallSpeedFactor * extraSpeedFactor * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.Value, movement);

            Vector2Int calculatedGridPosition = GameBoard.Instance.GetGridIndexFromPosition(transform.position);
            
            if (calculatedGridPosition != GridPosition)
            {
                GridPosition = calculatedGridPosition;
                OnGridPositionChanged?.Invoke(calculatedGridPosition);
            }

            if (transform.position == targetPosition.Value)
            {
                targetPosition = null;
            }
        }
    }

    public bool IsMoving()
    {
        return targetPosition != null;
    }

    private void SetColliderSizeAndOffset()
    {
        // 1. Set the size
        boxCollider.size = Vector2.one * sr.sprite.bounds.size.x * 0.9f;

        // 2. Calculate the bottom position of the sprite in local space
        // bounds.min.y gives you the bottom edge relative to the pivot
        float spriteBottomY = sr.sprite.bounds.min.y;

        // 3. Calculate the new offset
        // We want the bottom of the collider to be at 'spriteBottomY'
        // So we place its center half its height above that point.
        float newOffsetY = spriteBottomY + (boxCollider.size.y / 2f);

        // 4. Apply the offset (keeping standard X alignment)
        boxCollider.offset = new Vector2(sr.sprite.bounds.center.x, newOffsetY);
    }
}