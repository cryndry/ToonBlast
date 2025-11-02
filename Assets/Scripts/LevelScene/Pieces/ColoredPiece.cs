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
        Debug.Log("Green Piece Broken");
        return true;
    }

    public override void OnTap()
    {
        Debug.Log("Green Piece Tapped");
    }

    public void ShowRocketHint()
    {
        sr.sprite = pieceData.rocketSprite;
    }

    public void HideRocketHint()
    {
        sr.sprite = pieceData.baseSprite;
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
                sr.sprite = pieceData.explosionSprite;
                break;
        }
    }
}
