using System.Collections;
using UnityEngine;

public class ColoredPiece : Piece
{
    [SerializeField] private ColoredPieceData pieceData;

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


    protected override void Awake()
    {
        base.Awake();
        SetStatus(ColoredPieceStatus.Normal);
    }

    public override bool OnBreak()
    {
        return false;
    }

    public override bool OnBreakPowerUp()
    {
        Status = ColoredPieceStatus.Exploding;
        return true;
    }

    public override void OnTap()
    {
        GameBoard.Instance.ResolveColoredPieceMatch(this);
    }

    private void SetStatus(ColoredPieceStatus status)
    {
        if (sr == null || pieceData == null) return;

        switch (status)
        {
            case ColoredPieceStatus.Normal:
                sr.sprite = pieceData.baseSprite;
                break;
            case ColoredPieceStatus.Rocketable:
                sr.sprite = pieceData.rocketSprite;
                break;
            case ColoredPieceStatus.Exploding:
                StartCoroutine(Explode());
                break;
        }
    }

    protected override IEnumerator Explode()
    {
        yield return null;
        Destroy(gameObject);
        // Additional explosion effects will be added here
    }
}
