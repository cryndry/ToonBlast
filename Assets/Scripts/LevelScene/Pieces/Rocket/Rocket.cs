using System.Collections;
using UnityEngine;

public class Rocket : Piece
{
    [SerializeField] private RocketData rocketData;

    public bool isActivated = false;
    public bool isInteractable = true;


    private void Awake()
    {
        OnGridPositionChanged += OnGridPositionChangedCallback;
    }

    public override bool OnBreak()
    {
        return false;
    }

    public override bool OnBreakPowerUp()
    {
        if (!isInteractable) return false;

        StartCoroutine(ActivateRocket());
        return true;
    }

    public override void OnTap()
    {
        if (!isInteractable) return;

        StartCoroutine(ActivateRocket());
    }

    protected override IEnumerator Explode()
    {
        yield return null;
    }

    public bool IsVertical()
    {
        return rocketData.isVertical;
    }

    private IEnumerator ActivateRocket()
    {
        isInteractable = false;
        boxCollider.enabled = false;
        isActivated = true;
        sr.sprite = null;
        OnGridPositionChanged -= OnGridPositionChangedCallback;

        GameBoard.Instance.AddFallCount(GridPosition.x);
        GameBoard.Instance.ClearSlotPiece(GridPosition);
        GameBoard.Instance.activeRocketCount++;

        Coroutine rocketHeadCoroutine = StartCoroutine(FlyRocketPart(true));
        Coroutine rocketTailCoroutine = StartCoroutine(FlyRocketPart(false));

        yield return rocketHeadCoroutine;
        yield return rocketTailCoroutine;

        if (--GameBoard.Instance.activeRocketCount == 0)
        {
            GameBoard.Instance.UpdateGrid();
            GameBoard.Instance.ShowRocketHints();
        }

        Destroy(gameObject);
    }

    private IEnumerator FlyRocketPart(bool isHead)
    {
        GameObject rocketPartGO = PieceGenerator.Instance.GeneratePiece(IsVertical() ? "vro" : "hro", transform.parent);
        rocketPartGO.transform.position = transform.position;
        rocketPartGO.transform.localScale = transform.localScale;

        Rocket rocketPart = rocketPartGO.GetComponent<Rocket>();
        rocketPart.isInteractable = false;
        rocketPart.isActivated = true;
        rocketPart.sr.sprite = isHead ? rocketData.headSprite : rocketData.tailSprite;
        rocketPart.OnGridPositionChanged -= rocketPart.OnGridPositionChangedCallback;
        rocketPart.OnGridPositionChanged += rocketPart.OnGridPositionChangedCallbackForRocketPart;

        Vector2Int direction = isHead
            ? (rocketData.isVertical ? Vector2Int.up : Vector2Int.right)
            : (rocketData.isVertical ? Vector2Int.down : Vector2Int.left);

        Vector2Int gridSize = GameBoard.Instance.GetGridSize();
        int cellsToMove = 2 + (isHead
            ? (rocketData.isVertical ? gridSize.y - GridPosition.y : gridSize.x - GridPosition.x)
            : (rocketData.isVertical ? GridPosition.y : GridPosition.x));

        Vector2Int targetGridPosition = GridPosition + direction * cellsToMove;
        Vector2 targetWorldPosition = GameBoard.Instance.GetPositionOfTile(targetGridPosition.x, targetGridPosition.y);

        rocketPart.MoveToPosition(targetWorldPosition, extraSpeedFactor: 3f);

        yield return new WaitWhile(rocketPart.IsMoving);

        rocketPart.OnGridPositionChanged -= rocketPart.OnGridPositionChangedCallbackForRocketPart;
        Destroy(rocketPartGO);
    }

    private void OnGridPositionChangedCallback(Vector2Int newGridPosition)
    {
        GameBoard.Instance.SetSlotPiece(newGridPosition, this);
    }

    private void OnGridPositionChangedCallbackForRocketPart(Vector2Int newGridPosition)
    {
        Debug.Log("Rocket part affecting position: " + newGridPosition);
        var piece = GameBoard.Instance.GetSlotPiece(newGridPosition);
        if (piece != null)
        {
            bool isBroken = piece.OnBreakPowerUp();
            if (isBroken)
            {
                GameBoard.Instance.HandlePieceBroken(piece);
            }
        }
    }
}
