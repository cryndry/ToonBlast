using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }

    public abstract void OnTap();
    public abstract bool OnBreak();
    public virtual bool IsFallable() => true;

    private float PieceSize => GameBoard.Instance.pieceSize;
    private const float fallSpeedFactor = 3f;

    private Vector3? targetPosition = null;

    void Start()
    {
        EventManager.OnTapEvent += HandleTap;
    }

    void Update()
    {
        if (targetPosition != null)
        {
            float movement = PieceSize * fallSpeedFactor * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.Value, movement);

            Vector2Int calculatedGridPosition = GameBoard.Instance.GetGridIndexFromPosition(transform.position);
            if (calculatedGridPosition != GridPosition)
            {
                GameBoard.Instance.SetSlotPiece(calculatedGridPosition, this);
            }

            if (transform.position == targetPosition.Value)
            {
                targetPosition = null;
            }
        }
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

    public void MoveToPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}