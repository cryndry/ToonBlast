using System.Collections;
using UnityEngine;

public class Rocket : Piece
{
    [SerializeField] private RocketData rocketData;

    public bool isInteractable = true;


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
        sr.sprite = null;

        GameBoard.Instance.SetSlotPiece(GridPosition, null);

        Coroutine rocketHeadCoroutine = StartCoroutine(FlyRocketPart(true));
        Coroutine rocketTailCoroutine = StartCoroutine(FlyRocketPart(false));

        yield return rocketHeadCoroutine;
        yield return rocketTailCoroutine;

        GameBoard.Instance.UpdateGrid();
        GameBoard.Instance.ShowRocketHints();

        Destroy(gameObject);
    }

    private IEnumerator FlyRocketPart(bool isHead)
    {
        GameObject rocketPartGO = PieceGenerator.Instance.GeneratePiece(IsVertical() ? "vro" : "hro", transform.parent);
        rocketPartGO.transform.position = transform.position;
        rocketPartGO.transform.localScale = transform.localScale;

        Rocket rocketPart = rocketPartGO.GetComponent<Rocket>();
        rocketPart.isInteractable = false;
        rocketPart.sr.sprite = isHead ? rocketData.headSprite : rocketData.tailSprite;

        Vector2Int direction = isHead
            ? (rocketData.isVertical ? Vector2Int.up : Vector2Int.right)
            : (rocketData.isVertical ? Vector2Int.down : Vector2Int.left);

        Vector2Int gridSize = GameBoard.Instance.GetGridSize();
        int cellsToMove = isHead
            ? (rocketData.isVertical ? gridSize.y - GridPosition.y : gridSize.x - GridPosition.x)
            : (rocketData.isVertical ? GridPosition.y : GridPosition.x);

        Vector2Int targetGridPosition = GridPosition + direction * cellsToMove;
        Vector2 targetWorldPosition = GameBoard.Instance.GetPositionOfTile(targetGridPosition.x, targetGridPosition.y);

        rocketPart.MoveToPosition(targetWorldPosition, extraSpeedFactor: 3f, shouldMoveInGrid: false);

        yield return new WaitWhile(rocketPart.IsMoving);
        Destroy(rocketPartGO);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Rocket collided with " + collider.gameObject.name);
        if (collider.gameObject.TryGetComponent(out Piece piece))
        {
            piece.OnBreakPowerUp();
        }
    }
}
