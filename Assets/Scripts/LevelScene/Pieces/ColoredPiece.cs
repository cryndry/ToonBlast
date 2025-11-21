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

    private const int maxParticleCount = 8;
    private int finishedParticleCount = 0;


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
        sr.sprite = null;

        Vector3 spread = transform.localScale * 0.5f;
        Vector3 particleScale = transform.localScale * 0.3f;

        for (int i = 0; i < maxParticleCount; i++)
        {
            PieceGenerator.Instance.GeneratePiece("particle", transform)
                .GetComponent<Particle>()
                .Initialize(
                    particleScale,
                    pieceData.explosionSprite
                ).Animate(
                    Random.insideUnitCircle.normalized * spread,
                    0.5f,
                    () => finishedParticleCount++
                );
        }

        while (finishedParticleCount < maxParticleCount)
            yield return null;

        Destroy(gameObject);
    }
}
