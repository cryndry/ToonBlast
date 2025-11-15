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
        float targetAreaBottom = sr.sprite.bounds.min.y;
        float targetAreaTop = sr.sprite.bounds.size.x + targetAreaBottom;

        float targetCenterY = (targetAreaTop + targetAreaBottom) / 2f;
        float targetCenterX = sr.sprite.bounds.center.x;

        boxCollider.size = sr.sprite.bounds.size.x * 0.9f * Vector2.one;
        boxCollider.offset = new Vector2(targetCenterX, targetCenterY);
    }
}