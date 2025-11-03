using UnityEngine;

public class ColoredPiece : Piece
{
    [SerializeField] private ColoredPieceData pieceData;
    [SerializeField] private SpriteRenderer sr;

    public ColoredPieceColor Color => pieceData.color;
    private ColoredPieceStatus _status = ColoredPieceStatus.Normal;
    public ColoredPieceStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            SetStatus(value);
        }
    }


    void Awake()
    {
        SetStatus(ColoredPieceStatus.Normal);
    }

    public override bool OnBreak()
    {
        return false;
    }

    public override void OnTap()
    {
        GameBoard.Instance.ResolveMatch(GridPosition);
    }

    private void SetStatus(ColoredPieceStatus status)
    {
        switch (status)
        {
            case ColoredPieceStatus.Normal:
                sr.sprite = pieceData.baseSprite;
                break;
            case ColoredPieceStatus.Rocketable:
                sr.sprite = pieceData.rocketSprite;
                break;
            case ColoredPieceStatus.Exploding:
                Explode();
                break;
        }
    }

    private void Explode()
    {
        Destroy(this.gameObject);
        // Additional explosion effects will be added here
    }
}
